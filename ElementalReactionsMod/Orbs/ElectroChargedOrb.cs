using RoR2.Orbs;
using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using ElementalReactionsMod.Elements;
using R2API;
using ElementalReactionsMod.Reactions;

namespace ElementalReactionsMod.Orbs
{
    public static class ElectroChargedOrb
    {
        public static LightningOrb CreateOrb(Vector3 origin, HurtBox target, GameObject attacker, float damage)
        {
            DamageTypeCombo damageTypeCombo = DamageType.DoT;
            damageTypeCombo.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
            LightningOrb orb = new LightningOrb
            {
                origin = origin,
                target = target,
                lightningType = LightningOrb.LightningType.Tesla,
                attacker = attacker,
                inflictor = attacker,
                isCrit = false,
                damageValue = damage,
                damageType = damageTypeCombo,
                damageColorIndex = DamageColorIndex.Item,
                canBounceOnSameTarget = false,
                procCoefficient = 0,
                bouncesRemaining = 0,
                targetsToFindPerBounce = 0
            };
            OrbManager.instance.AddOrb(orb);
            return orb;
        }
    }
}
