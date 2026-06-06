using ElementalReactionsMod.Elements;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static ElementalReactionsMod.Elements.DefaultElementDefs;
using static ElementalReactionsMod.StaticValues;
using static ElementalReactionsMod.Reactions.ElementalReactionManager;

namespace ElementalReactionsMod.Reactions
{
    public class ElementalReactionDef : ScriptableObject
    {
        public string cachedName { get { return _cachedName; } set { name = value; _cachedName = value; } }
        private string _cachedName;
        public string nameToken;
        public string descriptionToken;
        public ElementDef baseElement;
        public float baseFirstReactionCoefficient = 1f;
        public ElementDef[] reactingElements;
        public float baseLastReactionCoefficient = 1f;
        public delegate void ElementalReactionDelegate(ElementDef firstElement, ElementDef secondElement, CharacterBody victim, ref DamageInfo damageInfo, ref float addedDamage);
        public event ElementalReactionDelegate onElementalReactionTriggered;
        public bool showInLoadoutMenu;
        public ReactionIndex index
        {
            get
            {
                return (ReactionIndex)Array.IndexOf(ElementalReactionCatalog.elementalReactionCatalog, this);
            }
        }
        public void TriggerReaction(ElementDef firstElement, ElementDef secondElement, CharacterBody victim, ref DamageInfo damageInfo, ref float addedDamage)
        {
            if (victim.ReduceTimedBuffDuration(firstElement.buff, elementAppliedDuration * damageInfo.procCoefficient * (firstElement == baseElement ? baseFirstReactionCoefficient : baseLastReactionCoefficient)) == 0)
            {
                victim.AddTimedBuff(firstElement.cooldownBuff, elementRemovedICD);
            }
            victim.AddTimedBuff(secondElement.cooldownBuff, elementAppliedICD);
            onElementalReactionTriggered?.Invoke(firstElement, secondElement, victim, ref damageInfo, ref addedDamage);
        }
        public override string ToString()
        {
            return Language.GetString(this.nameToken, Language.currentLanguageName);
        }
        public static ElementalReactionDef CreateElementalReactionDef(string internalName, string token, ElementDef baseElement, ElementDef reactingElement, bool showInLoadoutMenu = true)
        {
            return CreateElementalReactionDef(internalName, token, baseElement, [reactingElement]);
        }

        public static ElementalReactionDef CreateElementalReactionDef(string internalName, string token, ElementDef baseElement, ElementDef[] reactingElements, bool showInLoadoutMenu = true)
        {
            ElementalReactionDef reactionDef = ScriptableObject.CreateInstance<ElementalReactionDef>();
            reactionDef.cachedName = internalName;
            reactionDef.nameToken = token + "_NAME";
            reactionDef.descriptionToken = token + "_DESCRIPTION";
            reactionDef.baseElement = baseElement;
            reactionDef.reactingElements = reactingElements;
            reactionDef.showInLoadoutMenu = showInLoadoutMenu;
            return reactionDef;
        }
    }

    public enum ReactionIndex
    {
        None = -1
    }

    public static class DefaultElementalReactions
    {
        public static ElementalReactionDef vaporizeMelt;
        public static ElementalReactionDef overload;
        public static ElementalReactionDef electroCharge;
        public static ElementalReactionDef frozen;
        public static ElementalReactionDef superconduct;
        public static ElementalReactionDef swirl;
        public static ElementalReactionDef crystallize;
        public static ElementalReactionDef burning;
        public static ElementalReactionDef quicken;
        public static ElementalReactionDef bloom;
        // bloom and burgeon are an IOnIncomingDamageServerReceiver on the bloom cores, so not using an ElementalReactionDef

        public static void Initialize()
        {
            vaporizeMelt = ElementalReactionDef.CreateElementalReactionDef("Vaporize", $"{ElementalReactionsPlugin.PREFIX}REACTION_VAPORIZE", pyroElement, [hydroElement, cryoElement]);
            vaporizeMelt.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) => 
            { 
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_StunChanceOnHit.ImpactStunGrenade_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                addedDamage += damage.damage * (element2 == pyroElement ? vaporizeMultiplierPyroTrigger - 1f : vaporizeMultiplierHydroTrigger - 1f);
            };
            vaporizeMelt.baseFirstReactionCoefficient = 0.5f;
            vaporizeMelt.baseLastReactionCoefficient = 2f;

            overload = ElementalReactionDef.CreateElementalReactionDef("Overload", $"{ElementalReactionsPlugin.PREFIX}REACTION_OVERLOAD", pyroElement, electroElement);
            overload.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                EffectManager.SimpleEffect(overloadEffect.WaitForCompletion(), damage.position, Quaternion.identity, true);
                Util.CreateBlastAttack(damage, overloadDamageCoefficient * damage.procCoefficient, genericReactionExplosionRadius, genericReactionProcCoefficient, pyroElement.index, true, 1000f).Fire();
            };

            electroCharge = ElementalReactionDef.CreateElementalReactionDef("ElectroCharge", $"{ElementalReactionsPlugin.PREFIX}REACTION_ELECTRO_CHARGE", electroElement, hydroElement);
            electroCharge.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                if (victim && victim.healthComponent && victim.healthComponent.alive)
                {
                    DotController.InflictDot(victim.gameObject, damage.attacker, damage.inflictedHurtbox, ElectroChargedDot.electroChargeDot, electroChargeDuration);
                }
            };

            frozen = ElementalReactionDef.CreateElementalReactionDef("Frozen", $"{ElementalReactionsPlugin.PREFIX}REACTION_FROZEN", cryoElement, hydroElement);
            frozen.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                if (victim && victim.healthComponent && victim.healthComponent.alive && victim.TryGetComponent<SetStateOnHurt>(out var freeze))
                {
                    freeze.SetFrozen(1f);
                }
            };

            superconduct = ElementalReactionDef.CreateElementalReactionDef("Superconduct", $"{ElementalReactionsPlugin.PREFIX}REACTION_SUPERCONDUCT", cryoElement, electroElement);
            superconduct.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.ChefIceBoxExplosionVFX_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                DamageTypeCombo damageType = DamageType.AOE;
                damageType.SetElement(cryoElement.index);
                damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageType.AddModdedDamageType(DamageTypes.superconductDamageType);
                Util.CreateBlastAttack(damage, superconductDamageCoefficient * damage.procCoefficient, genericReactionExplosionRadius, genericReactionProcCoefficient, damageType, 0f).Fire();
            };

            swirl = ElementalReactionDef.CreateElementalReactionDef("Swirl", $"{ElementalReactionsPlugin.PREFIX}REACTION_SWIRL", anemoElement, [pyroElement, hydroElement, electroElement, cryoElement]);
            swirl.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                ElementDef swirledElement = element1 == anemoElement ? element2 : element1;
                EffectManager.SpawnEffect(swirlEffect.WaitForCompletion(), new EffectData
                {
                    origin = damage.position,
                    rotation = Quaternion.identity,
                    genericUInt = (uint)swirledElement.index
                }, true);
                DamageTypeCombo damageType = DamageType.AOE;
                damageType.SetElement(swirledElement.index);
                damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                Util.CreateBlastAttack(damage, swirlDamageCoefficient * damage.procCoefficient, genericReactionExplosionRadius, genericReactionProcCoefficient, damageType, 200f).Fire();
            };
            swirl.baseFirstReactionCoefficient = 0.5f;
            swirl.baseLastReactionCoefficient = 0.5f;

            crystallize = ElementalReactionDef.CreateElementalReactionDef("Crystallize", $"{ElementalReactionsPlugin.PREFIX}REACTION_CRYSTALLIZE", geoElement, [pyroElement, hydroElement, electroElement, cryoElement]);
            crystallize.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                if (damage.attacker && damage.attacker.TryGetComponent<CharacterBody>(out var characterBody))
                {
                    Util.GetRandomNode(damage.position, out var pos, 0.5f, 15f);
                    //ElementalReactionPooledObject crystallize = ElementalReactionManager.CreatePooledDeployable(ElementalReactionManager.crystallizePool, characterBody, ElementalReactionManager.crystallizeDeployableSlot, pos);
                    GameObject crystallize = GameObject.Instantiate(crystallizePickup.WaitForCompletion(), pos + (1.5f * Vector3.up), Quaternion.identity);
                    if (characterBody.master) characterBody.master.AddDeployable(crystallize.GetComponent<Deployable>(), ElementalReactionManager.crystallizeDeployableSlot);
                    if (crystallize) crystallize.GetComponent<TeamFilter>().teamIndex = characterBody.teamComponent.teamIndex;
                    NetworkServer.Spawn(crystallize);
                }
            };
            crystallize.baseFirstReactionCoefficient = 0.5f;
            crystallize.baseLastReactionCoefficient = 0.5f;

            burning = ElementalReactionDef.CreateElementalReactionDef("Burning", $"{ElementalReactionsPlugin.PREFIX}REACTION_BURNING", dendroElement, pyroElement);
            burning.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                damage.damageType |= DamageType.IgniteOnHit;
            };

            quicken = ElementalReactionDef.CreateElementalReactionDef("Quicken", $"{ElementalReactionsPlugin.PREFIX}REACTION_QUICKEN", dendroElement, electroElement);
            quicken.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                victim.AddTimedBuff(Buffs.quickenBuff, 5f);
            };

            bloom = ElementalReactionDef.CreateElementalReactionDef("Bloom", $"{ElementalReactionsPlugin.PREFIX}REACTION_BLOOM", dendroElement, hydroElement);
            bloom.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                if (damage.attacker && damage.attacker.TryGetComponent<CharacterBody>(out var characterBody))
                {
                    Util.GetRandomNode(damage.position, out var pos, 0.5f, 15f);
                    //ElementalReactionPooledObject bloom = ElementalReactionManager.CreatePooledDeployable(ElementalReactionManager.bloomPool, characterBody, ElementalReactionManager.bloomDeployableSlot, pos);
                    /*GameObject bloom = GameObject.Instantiate(Assets.bloomDendroCore, pos + (1.5f * Vector3.up), Quaternion.identity);
                    if (characterBody.master) characterBody.master.AddDeployable(bloom.GetComponent<Deployable>(), ElementalReactionManager.bloomDeployableSlot);
                    bloom.GetComponent<TeamComponent>().teamIndex = characterBody.teamComponent.teamIndex;
                    NetworkServer.Spawn(bloom);*/

                    ProjectileManager.instance.FireProjectileServer(new FireProjectileInfo
                    {
                        projectilePrefab = bloomCore.WaitForCompletion(),
                        damage = characterBody.damage,
                        crit = false,
                        position = pos + (1.5f * Vector3.up),
                        rotation = Quaternion.identity,
                        owner = damage.attacker
                    });
                }
            };
            bloom.baseFirstReactionCoefficient = 0.5f;
            bloom.baseLastReactionCoefficient = 2f;


            ElementalReactionCatalog.AddElementalReactionDefs([vaporizeMelt, overload, electroCharge, frozen, superconduct, swirl, crystallize, burning, quicken, bloom]);
        }
    }
}
