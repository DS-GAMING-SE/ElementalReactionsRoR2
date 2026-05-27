using ElementalReactionsMod.Elements;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        public ElementDef[] reactingElements;
        public delegate void ElementalReactionDelegate(ElementDef firstElement, ElementDef secondElement, CharacterBody victim, ref DamageInfo damageInfo);
        public event ElementalReactionDelegate onElementalReactionTriggered;
        public bool showInLoadoutMenu;
        public ReactionIndex index
        {
            get
            {
                return (ReactionIndex)Array.IndexOf(ElementalReactionCatalog.elementalReactionCatalog, this);
            }
        }
        public void TriggerReaction(ElementDef firstElement, ElementDef secondElement, CharacterBody victim, ref DamageInfo damageInfo)
        {
            onElementalReactionTriggered?.Invoke(firstElement, secondElement, victim, ref damageInfo);
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
        public static ElementalReactionDef vaporize;
        public static ElementalReactionDef overload;
        public static ElementalReactionDef melt;
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
            vaporize = ElementalReactionDef.CreateElementalReactionDef("Vaporize", $"{ElementalReactionsPlugin.PREFIX}REACTION_VAPORIZE", pyroElement, hydroElement);
            vaporize.onElementalReactionTriggered += (element1, element2, victim, ref damage) => 
            { 
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_StunChanceOnHit.ImpactStunGrenade_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                damage.damage *= element2 == pyroElement ? vaporizeMultiplierPyroTrigger : vaporizeMultiplierHydroTrigger;
                damage.damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
            };

            overload = ElementalReactionDef.CreateElementalReactionDef("Overload", $"{ElementalReactionsPlugin.PREFIX}REACTION_OVERLOAD", pyroElement, electroElement);
            overload.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_SolusAmalgamator.SolusAmalgamatorTrackingBombExplosion_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                Util.CreateBlastAttack(damage, overloadDamageCoefficient, genericReactionExplosionRadius, 0f, pyroElement.index, true, 1000f).Fire();
            };

            melt = ElementalReactionDef.CreateElementalReactionDef("Melt", $"{ElementalReactionsPlugin.PREFIX}REACTION_MELT", pyroElement, cryoElement);
            melt.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_StunChanceOnHit.ImpactStunGrenade_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                damage.damage *= element2 == pyroElement ? vaporizeMultiplierPyroTrigger : vaporizeMultiplierHydroTrigger;
                damage.damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
            };

            electroCharge = ElementalReactionDef.CreateElementalReactionDef("ElectroCharge", $"{ElementalReactionsPlugin.PREFIX}REACTION_ELECTRO_CHARGE", electroElement, hydroElement);
            electroCharge.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                if (victim)
                {
                    Orbs.ElectroChargedOrb.CreateOrb(victim.mainHurtBox, damage.attacker, electroChargeDamageCoefficient, damage.attacker ? damage.attacker.GetComponent<CharacterBody>() : null);
                }
            };

            frozen = ElementalReactionDef.CreateElementalReactionDef("Frozen", $"{ElementalReactionsPlugin.PREFIX}REACTION_FROZEN", cryoElement, hydroElement);
            frozen.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                if (victim && victim.healthComponent && victim.healthComponent.alive && victim.TryGetComponent<SetStateOnHurt>(out var freeze))
                {
                    freeze.SetFrozen(1f);
                }
            };

            superconduct = ElementalReactionDef.CreateElementalReactionDef("Superconduct", $"{ElementalReactionsPlugin.PREFIX}REACTION_SUPERCONDUCT", cryoElement, electroElement);
            superconduct.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.ChefSecondaryIceBoxBoostedVFXShort_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                DamageTypeCombo damageType = default;
                damageType.SetElement(cryoElement.index);
                damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageType.AddModdedDamageType(DamageTypes.superconductDamageType);
                damageType |= DamageType.AOE;
                Util.CreateBlastAttack(damage, superconductDamageCoefficient, genericReactionExplosionRadius, 0f, damageType, 0f).Fire();
            };

            swirl = ElementalReactionDef.CreateElementalReactionDef("Swirl", $"{ElementalReactionsPlugin.PREFIX}REACTION_SWIRL", anemoElement, [pyroElement, hydroElement, electroElement, cryoElement]);
            swirl.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                ElementDef swirledElement = element1 == anemoElement ? element2 : element1;
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.ChefIceBoxExplosionVFX_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                DamageTypeCombo damageType = default;
                damageType.SetElement(swirledElement.index);
                damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageType |= DamageType.AOE;
                Util.CreateBlastAttack(damage, swirlDamageCoefficient, genericReactionExplosionRadius, 0f, damageType, 0f).Fire();
            };

            crystallize = ElementalReactionDef.CreateElementalReactionDef("Crystallize", $"{ElementalReactionsPlugin.PREFIX}REACTION_CRYSTALLIZE", geoElement, [pyroElement, hydroElement, electroElement, cryoElement]);
            crystallize.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                if (damage.attacker && damage.attacker.TryGetComponent<CharacterBody>(out var characterBody))
                {
                    Util.GetRandomNode(damage.position, out var pos, 0.5f, 15f);
                    //ElementalReactionPooledObject crystallize = ElementalReactionManager.CreatePooledDeployable(ElementalReactionManager.crystallizePool, characterBody, ElementalReactionManager.crystallizeDeployableSlot, pos);
                    GameObject crystallize = GameObject.Instantiate(Assets.crystallizePickup, pos, Quaternion.identity);
                    if (characterBody.master) characterBody.master.AddDeployable(crystallize.GetComponent<Deployable>(), ElementalReactionManager.crystallizeDeployableSlot);
                    if (crystallize) crystallize.GetComponent<TeamFilter>().teamIndex = characterBody.teamComponent.teamIndex;
                }
            };

            burning = ElementalReactionDef.CreateElementalReactionDef("Burning", $"{ElementalReactionsPlugin.PREFIX}REACTION_BURNING", dendroElement, pyroElement);
            burning.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                damage.damageType |= DamageType.IgniteOnHit;
            };

            quicken = ElementalReactionDef.CreateElementalReactionDef("Quicken", $"{ElementalReactionsPlugin.PREFIX}REACTION_QUICKEN", dendroElement, electroElement);
            quicken.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                victim.AddTimedBuff(Buffs.quickenBuff, 5f);
            };

            bloom = ElementalReactionDef.CreateElementalReactionDef("Bloom", $"{ElementalReactionsPlugin.PREFIX}REACTION_BLOOM", dendroElement, hydroElement);
            bloom.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                if (damage.attacker && damage.attacker.TryGetComponent<CharacterBody>(out var characterBody))
                {
                    Util.GetRandomNode(damage.position, out var pos, 0.5f, 15f);
                    //ElementalReactionPooledObject bloom = ElementalReactionManager.CreatePooledDeployable(ElementalReactionManager.bloomPool, characterBody, ElementalReactionManager.bloomDeployableSlot, pos);
                    GameObject bloom = GameObject.Instantiate(Assets.bloomDendroCore, pos, Quaternion.identity);
                    if (characterBody.master) characterBody.master.AddDeployable(bloom.GetComponent<Deployable>(), ElementalReactionManager.bloomDeployableSlot);
                    //if (bloom) bloom.teamFilter.teamIndex = characterBody.teamComponent.teamIndex;
                }
            };


            ElementalReactionCatalog.AddElementalReactionDefs([vaporize, overload, melt, electroCharge, frozen, superconduct, swirl, crystallize, burning, quicken, bloom]);
        }
    }
}
