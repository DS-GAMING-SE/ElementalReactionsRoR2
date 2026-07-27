using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Orbs;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static ElementalReactionsMod.ElementalReactionManager;
using static ElementalReactionsMod.Elements.DefaultElementDefs;
using static ElementalReactionsMod.StaticValues;

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
        public static ElementalReactionDef lunarCharge;
        public static ElementalReactionDef lunarBloom;
        public static ElementalReactionDef lunarCrystallize;

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
                DamageTypeCombo damageType = new DamageTypeCombo(DamageType.AOE, DamageTypeExtended.FireNoIgnite, DamageSource.NoneSpecified);
                damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                Util.CreateBlastAttack(damage, overloadDamageCoefficient * damage.procCoefficient, genericReactionExplosionRadius, genericReactionProcCoefficient, damageType, 1000f).Fire();
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
                if (damage.procCoefficient > 0.15f && victim && victim.healthComponent && victim.healthComponent.alive && victim.TryGetComponent<SetStateOnHurt>(out var freeze))
                {
                    freeze.SetFrozen(Mathf.Max(1f * damage.procCoefficient, 0.4f));
                }
            };

            superconduct = ElementalReactionDef.CreateElementalReactionDef("Superconduct", $"{ElementalReactionsPlugin.PREFIX}REACTION_SUPERCONDUCT", cryoElement, electroElement);
            superconduct.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                EffectManager.SimpleEffect(superconductEffect.WaitForCompletion(), damage.position, Quaternion.identity, true);
                DamageTypeCombo damageType = DamageType.AOE;
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
                    /*GameObject crystallize = GameObject.Instantiate(crystallizePickup.WaitForCompletion(), pos + (2f * Vector3.up), Quaternion.identity);
                    if (characterBody.master) characterBody.master.AddDeployable(crystallize.GetComponent<Deployable>(), crystallizeDeployableSlot);
                    if (crystallize) crystallize.GetComponent<TeamFilter>().teamIndex = characterBody.teamComponent.teamIndex;
                    NetworkServer.Spawn(crystallize);*/
                    if (characterBody.master) CrystallizeSpawnOrb.SpawnCrystallize(characterBody.master, characterBody.teamComponent.teamIndex, pos + (2f * Vector3.up), damage.position);
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
                victim.AddTimedBuff(Buffs.quickenBuff, quickenDuration);
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

                    BloomSpawnOrb.SpawnBloom(damage.attacker, characterBody.damage, pos + (2 * Vector3.up), damage.position);

                    /*ProjectileManager.instance.FireProjectileServer(new FireProjectileInfo
                    {
                        projectilePrefab = bloomCore.WaitForCompletion(),
                        damage = characterBody.damage,
                        crit = false,
                        position = pos + (2f * Vector3.up),
                        rotation = Quaternion.identity,
                        owner = damage.attacker
                    });*/
                }
            };
            bloom.baseFirstReactionCoefficient = 0.5f;
            bloom.baseLastReactionCoefficient = 2f;

            #region Lunar Reactions
            lunarCharge = ElementalReactionDef.CreateElementalReactionDef("LunarCharge", $"{ElementalReactionsPlugin.PREFIX}REACTION_LUNAR_CHARGE", electroElement, hydroElement);
            lunarCharge.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                if (victim && victim.healthComponent && victim.healthComponent.alive)
                {
                    if (damage.attacker && !victim.healthComponent.body.HasBuff(Buffs.lunarChargeBuff))
                    {
                        GenericElementEffectComponent.SpawnActivatedEffect(damage.attacker.transform, ParentEffectToItemDisplay.ItemDisplayParent.MoonWheel, electroElement.index, 0.7f, true);
                    }
                    DotController.InflictDot(victim.gameObject, damage.attacker, damage.inflictedHurtbox, ElectroChargedDot.lunarChargeDot, lunarChargeDotDuration);
                }
            };
            lunarCharge.showInLoadoutMenu = false;

            lunarBloom = ElementalReactionDef.CreateElementalReactionDef("LunarBloom", $"{ElementalReactionsPlugin.PREFIX}REACTION_LUNAR_BLOOM", dendroElement, hydroElement);
            lunarBloom.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                bloom.TriggerReaction(element1, element2, victim, ref damage, ref addedDamage);
                if (damage.attacker && damage.attacker.TryGetComponent<CharacterBody>(out var attackerBody))
                {
                    if (ElementalReactionsPlugin.qualityModExists && attackerBody.inventory)
                    {
                        if (attackerBody.GetBuffCount(Buffs.lunarBloomBuff) < lunarBloomVerdantDewCap && RoR2.Util.CheckRoll(attackerBody.inventory.GetMoonWheelQualityChance(), attackerBody.master))
                        {
                            attackerBody.AddBuff(Buffs.lunarBloomBuff);
                            if (attackerBody.GetBuffCount(Buffs.lunarBloomBuff) >= lunarBloomVerdantDewCap)
                            {
                                GenericElementEffectComponent.SpawnActivatedEffect(attackerBody.transform, ParentEffectToItemDisplay.ItemDisplayParent.MoonWheel, dendroElement.index, 0.7f, true);
                            }
                        }
                    }
                    if (attackerBody.GetBuffCount(Buffs.lunarBloomBuff) < lunarBloomVerdantDewCap)
                    {
                        attackerBody.AddBuff(Buffs.lunarBloomBuff);
                        if (attackerBody.GetBuffCount(Buffs.lunarBloomBuff) >= lunarBloomVerdantDewCap)
                        {
                            GenericElementEffectComponent.SpawnActivatedEffect(attackerBody.transform, ParentEffectToItemDisplay.ItemDisplayParent.MoonWheel, dendroElement.index, 0.7f, true);
                        }
                    }
                }
            };
            lunarBloom.baseFirstReactionCoefficient = 0.5f;
            lunarBloom.baseLastReactionCoefficient = 2f;
            lunarBloom.showInLoadoutMenu = false;

            lunarCrystallize = ElementalReactionDef.CreateElementalReactionDef("LunarCrystallize", $"{ElementalReactionsPlugin.PREFIX}REACTION_LUNAR_CRYSTALLIZE", geoElement, hydroElement);
            lunarCrystallize.onElementalReactionTriggered += (element1, element2, victim, ref damage, ref addedDamage) =>
            {
                if (damage.attacker && damage.attacker.TryGetComponent<CharacterBody>(out var characterBody))
                {
                    characterBody.AddBuff(Buffs.lunarCrystallizeBuff);
                }
            };
            lunarCrystallize.baseFirstReactionCoefficient = 0.5f;
            lunarCrystallize.baseLastReactionCoefficient = 0.5f;
            lunarCrystallize.showInLoadoutMenu = false;
            #endregion

            ElementalReactionCatalog.AddElementalReactionDefs([vaporizeMelt, overload, electroCharge, frozen, superconduct, swirl, crystallize, burning, quicken, bloom, lunarCharge, lunarBloom, lunarCrystallize]);
        }
    }
}
