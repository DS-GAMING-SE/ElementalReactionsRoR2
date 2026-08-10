using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Orbs;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod.Reactions
{
    public static class BurningDot
    {
        public static DotController.DotIndex burningDot;
        public static DotController.DotIndex strongBurningDot;
        public static void Initialize()
        {
            burningDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                damageCoefficient = StaticValues.burningDamageCoefficient,
                interval = StaticValues.burningTimeBetweenAttacks,
                associatedBuff = Buffs.burningBuff,
                resetTimerOnAdd = false,
            }, null, null, BurningDamage);
            strongBurningDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                damageCoefficient = StaticValues.burningDamageCoefficient,
                interval = StaticValues.burningTimeBetweenAttacks,
                associatedBuff = Buffs.strongBurningBuff,
                resetTimerOnAdd = false,
            }, null, null, BurningDamage);
        }

        public static void BurningDamage(DotController self, DotController.PendingDamage damage)
        {
            if (!Mathf.Approximately(damage.totalDamage, 0f) && self.victimHealthComponent.alive)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.attacker = damage.attackerObject;
                damageInfo.crit = false;
                damageInfo.damage = damage.totalDamage;
                damageInfo.force = Vector3.zero;
                damageInfo.inflictor = self.gameObject;
                damageInfo.position = self.victimBody.corePosition;
                damageInfo.procCoefficient = 0f;
                damageInfo.damageColorIndex = DamageColorIndex.Item;
                damageInfo.damageType = new DamageTypeCombo(DamageType.DoT, DamageTypeExtended.FireNoIgnite, DamageSource.NoneSpecified);
                damageInfo.damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageInfo.inflictedHurtbox = damage.hitHurtBox;
                self.victimHealthComponent.TakeDamage(damageInfo);

                self.victimBody.ReduceTimedBuffDuration(DefaultElementDefs.dendroElement.buff, StaticValues.burningDendroDurationReduction);
                self.victimBody.ExtendTimedBuffIfPresent(DefaultElementDefs.pyroElement.buff, 0.3f, StaticValues.elementAppliedDuration * StaticValues.elementAppliedTaxMultiplier);
            }
        }
    }
}
