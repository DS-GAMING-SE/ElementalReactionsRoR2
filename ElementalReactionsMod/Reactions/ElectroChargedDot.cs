using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Orbs;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using static RoR2.DotController;

namespace ElementalReactionsMod.Reactions
{
    public static class ElectroChargedDot
    {
        public static DotController.DotIndex electroChargeDot;
        
        public static void Initialize()
        {
            electroChargeDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                damageCoefficient = StaticValues.electroChargeDamageCoefficient,
                interval = 1f,
                associatedBuff = Buffs.electroChargeBuff,
                resetTimerOnAdd = true,
                damageColorIndex = DamageColorIndex.Default
            }, null, null, ElectroChargedFireOrb);
        }
        public static void ElectroChargedFireOrb(DotController self, DotController.PendingDamage damage)
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
                damageInfo.damageColorIndex = DamageColorIndex.Default;
                damageInfo.damageType = DamageType.DoT;
                damageInfo.damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageInfo.inflictedHurtbox = damage.hitHurtBox;
                self.victimHealthComponent.TakeDamage(damageInfo);

                SphereSearch search = new SphereSearch();
                search.radius = StaticValues.genericReactionExplosionRadius;
                search.origin = damage.hitHurtBox.transform.position;
                search.mask = LayerIndex.entityPrecise.mask;
                search.RefreshCandidates();
                search.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(TeamComponent.GetObjectTeam(damage.attackerObject)));
                search.FilterCandidatesByDistinctHurtBoxEntities();
                HurtBox spreadTarget = search.GetHurtBoxes().FirstOrDefault(x => x.healthComponent && x.healthComponent != self.victimHealthComponent && x.healthComponent.alive &&
                    x.healthComponent.body.HasBuff(DefaultElementDefs.hydroElement.buff));
                if (spreadTarget) ElectroChargedOrb.CreateOrb(damage.hitHurtBox.transform.position, spreadTarget, damage.attackerObject, damage.totalDamage);
            }
        }
    }
}
