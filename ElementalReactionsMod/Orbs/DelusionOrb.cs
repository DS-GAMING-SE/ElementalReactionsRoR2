using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ElementalReactionsMod.Elements;
using R2API;

namespace ElementalReactionsMod.Orbs
{
    // Blast attack aoe damage?
    public class DelusionOrb : GenericDamageOrb
    {
        public static void FireDelusionOrb(CharacterBody attacker, HurtBox target, int stacks, bool crit, ElementIndex element)
        {
            DelusionOrb delusionOrb = new DelusionOrb
            {
                attacker = attacker.gameObject,
                origin = attacker.corePosition,
                damageValue = (StaticValues.delusionDamageCoefficient + (StaticValues.delusionStackDamageCoefficient * (stacks - 1))) * attacker.damage,
                isCrit = crit,
                target = target,
                damageType = new DamageTypeCombo(),
                procCoefficient = 1,
                teamIndex = attacker.teamComponent.teamIndex
            };
            delusionOrb.damageType.SetElement(element);
            delusionOrb.damageType.AddModdedDamageType(DamageTypes.delusionDamageType);
            OrbManager.instance.AddOrb(delusionOrb);
        }
        public override GameObject GetOrbEffect()
        {
            return Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Items_ShockDamageAura.ShockDamageAuraOrbEffect_prefab).WaitForCompletion();
        }
    }
}
