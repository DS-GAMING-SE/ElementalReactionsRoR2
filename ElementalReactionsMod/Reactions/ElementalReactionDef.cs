using ElementalReactionsMod.Elements;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ElementalReactionsMod.Elements.DefaultElementDefs;

namespace ElementalReactionsMod.Reactions
{
    public class ElementalReactionDef : ScriptableObject
    {
        public static DamageAPI.ModdedDamageType elementalReactionDamageType;
        
        public string cachedName { get { return _cachedName; } set { name = value; _cachedName = value; } }
        private string _cachedName;
        public string nameToken;
        public ElementDef baseElement;
        public ElementDef[] reactingElements;
        public delegate void ElementalReactionDelegate(ElementDef firstElement, ElementDef secondElement, HealthComponent victim, ref DamageInfo damageInfo);
        public event ElementalReactionDelegate onElementalReactionTriggered;
        public ReactionIndex index
        {
            get
            {
                return (ReactionIndex)Array.IndexOf(ElementalReactionCatalog.elementalReactionCatalog, this);
            }
        }
        public void TriggerReaction(ElementDef firstElement, ElementDef secondElement, HealthComponent victim, ref DamageInfo damageInfo)
        {
            onElementalReactionTriggered?.Invoke(firstElement, secondElement, victim, ref damageInfo);
        }
        public override string ToString()
        {
            return Language.GetString(this.nameToken, Language.currentLanguageName);
        }
        public static ElementalReactionDef CreateElementalReactionDef(string internalName, string nameToken, ElementDef baseElement, ElementDef reactingElement)
        {
            return CreateElementalReactionDef(internalName, nameToken, baseElement, [reactingElement]);
        }

        public static ElementalReactionDef CreateElementalReactionDef(string internalName, string nameToken, ElementDef baseElement, ElementDef[] reactingElements)
        {
            ElementalReactionDef reactionDef = ScriptableObject.CreateInstance<ElementalReactionDef>();
            reactionDef.cachedName = internalName;
            reactionDef.nameToken = nameToken;
            reactionDef.baseElement = baseElement;
            reactionDef.reactingElements = reactingElements;
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
        public const float vaporizeMultiplierPyroTrigger = 2f;
        public const float vaporizeMultiplierHydroTrigger = 1.5f;
        public static ElementalReactionDef overload;
        public const float overloadDamageCoefficient = 2f;
        public static ElementalReactionDef melt;
        public static ElementalReactionDef electroCharge;
        public static ElementalReactionDef frozen;
        public static ElementalReactionDef superConduct;
        public static ElementalReactionDef swirl;
        public static ElementalReactionDef crystallize;
        public static ElementalReactionDef burning;
        public static ElementalReactionDef bloom;
        // bloom and burgeon will be IOnIncomingDamageServerReceiver on the bloom cores, so not using an ElementalReactionDef
        public static ElementalReactionDef quicken;

        public static void Initialize()
        {
            ElementalReactionDef.elementalReactionDamageType = DamageAPI.ReserveDamageType();
            
            vaporize = ElementalReactionDef.CreateElementalReactionDef("Vaporize", $"{ElementalReactionsPlugin.PREFIX}REACTION_VAPORIZE", pyroElement, hydroElement);
            vaporize.onElementalReactionTriggered += (element1, element2, victim, ref damage) => 
            { 
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_StunChanceOnHit.ImpactStunGrenade_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                damage.damage *= element2 == pyroElement ? vaporizeMultiplierPyroTrigger : vaporizeMultiplierHydroTrigger;
            };

            overload = ElementalReactionDef.CreateElementalReactionDef("Overload", $"{ElementalReactionsPlugin.PREFIX}REACTION_OVERLOAD", pyroElement, electroElement);
            overload.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.ExplosionVFX_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                DamageTypeCombo overloadDamageType = default;
                //overloadDamageType.SetElement(pyroElement.index);
                overloadDamageType.AddModdedDamageType(ElementalReactionDef.elementalReactionDamageType);
                overloadDamageType |= DamageType.AOE;
                Util.CreateBlastAttack(damage, overloadDamageCoefficient, 5f, 0f, overloadDamageType, 1000f).Fire();
            };

            melt = ElementalReactionDef.CreateElementalReactionDef("Melt", $"{ElementalReactionsPlugin.PREFIX}REACTION_MELT", pyroElement, cryoElement);
            melt.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_StunChanceOnHit.ImpactStunGrenade_prefab).WaitForCompletion(), damage.position, Quaternion.identity, true);
                damage.damage *= element2 == pyroElement ? vaporizeMultiplierPyroTrigger : vaporizeMultiplierHydroTrigger;
            };

            frozen = ElementalReactionDef.CreateElementalReactionDef("Frozen", $"{ElementalReactionsPlugin.PREFIX}REACTION_FROZEN", cryoElement, hydroElement);
            frozen.onElementalReactionTriggered += (element1, element2, victim, ref damage) =>
            {
                if (victim && victim.alive && victim.TryGetComponent<SetStateOnHurt>(out var freeze))
                {
                    freeze.SetFrozen(1f);
                }
            };


            ElementalReactionCatalog.AddElementalReactionDefs([vaporize, overload, melt, frozen]);
        }
    }
}
