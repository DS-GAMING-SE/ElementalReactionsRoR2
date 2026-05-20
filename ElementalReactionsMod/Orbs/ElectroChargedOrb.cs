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
    // make a new stupid copy of lightning orb because lightning orb sucks. Make electro charge only jump to enemies with hydro buff on them. Electro charge is a dot?
    public static class ElectroChargedOrb
    {
        public static LightningOrb CreateOrb(HurtBox target, GameObject attacker, float damage, CharacterBody attackerBody = null)
        {
            DamageTypeCombo damageTypeCombo = default;
            //damageTypeCombo.SetElement(DefaultElementDefs.electroElement.index);
            damageTypeCombo.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
            LightningOrb orb = new LightningOrb
            {
                origin = target.transform.position,
                lightningType = LightningOrb.LightningType.Tesla,
                attacker = attacker,
                inflictor = attacker,
                teamIndex = attackerBody ? attackerBody.teamComponent.teamIndex : TeamIndex.None,
                isCrit = false,
                damageCoefficientPerBounce = attackerBody ? attackerBody.damage * damage : damage,
                damageType = damageTypeCombo,
                canBounceOnSameTarget = false,
                procCoefficient = 0,
                bouncesRemaining = 2,
                bouncedObjects = [target.healthComponent],
                targetsToFindPerBounce = 2
            };
            orb.target = orb.PickNextTarget(target.transform.position);
            OrbManager.instance.AddOrb(orb);
            return orb;
        }
    }
}
