using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Orbs;
using R2API;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using static RoR2.DotController;

namespace ElementalReactionsMod.Reactions
{
    public static class ElectroChargedDot
    {
        public static DotController.DotIndex electroChargeDot;
        public static DotController.DotIndex lunarChargeDot;

        public static void Initialize()
        {
            electroChargeDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                damageCoefficient = StaticValues.electroChargeDamageCoefficient,
                interval = 1f,
                associatedBuff = Buffs.electroChargeBuff,
                resetTimerOnAdd = false,
                damageColorIndex = DamageColorIndex.Default
            }, null, null, ElectroChargedFireOrb);
            lunarChargeDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                damageCoefficient = StaticValues.lunarChargeDamageCoefficient,
                interval = StaticValues.lunarChargeTimeBetweenAttacks,
                associatedBuff = Buffs.lunarChargeBuff,
                resetTimerOnAdd = false,
                damageColorIndex = DamageColorIndex.Default
            }, null, null, LunarChargedLightning);
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
                damageInfo.damageType = new DamageTypeCombo(DamageType.DoT, DamageTypeExtended.Electrical, DamageSource.NoneSpecified);
                damageInfo.damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageInfo.inflictedHurtbox = damage.hitHurtBox;
                self.victimHealthComponent.TakeDamage(damageInfo);

                SphereSearch search = new SphereSearch();
                search.radius = StaticValues.electroChargeRadius;
                search.origin = damage.hitHurtBox ? damage.hitHurtBox.transform.position : self.victimBody.corePosition;
                search.mask = LayerIndex.entityPrecise.mask;
                search.RefreshCandidates();
                search.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(TeamComponent.GetObjectTeam(damage.attackerObject)));
                search.FilterCandidatesByDistinctHurtBoxEntities();
                HurtBox spreadTarget = search.GetHurtBoxes().FirstOrDefault(x => x.healthComponent && x.healthComponent != self.victimHealthComponent && x.healthComponent.alive &&
                    x.healthComponent.body.HasBuff(DefaultElementDefs.hydroElement.buff));
                if (spreadTarget) ElectroChargedOrb.CreateOrb(search.origin, spreadTarget, damage.attackerObject, damage.totalDamage);
            }
        }
        public static void LunarChargedLightning(DotController self, PendingDamage damage)
        {
            if (self.victimHealthComponent.alive)
            {
                CharacterBody attackerBody = damage.attackerObject ? damage.attackerObject.GetComponent<CharacterBody>() : null;
                Vector3 damagePosition = damage.hitHurtBox ? damage.hitHurtBox.transform.position : self.victimBody.corePosition;
                TeamIndex team = TeamComponent.GetObjectTeam(damage.attackerObject);
                if (team == TeamIndex.Player)
                {
                    CreateLunarChargedLightning(damage.attackerObject, team, damage.totalDamage, attackerBody ? attackerBody.RollCrit() : false, damagePosition);

                    if (ElementalReactionsPlugin.qualityModExists && attackerBody && attackerBody.inventory && RoR2.Util.CheckRoll(attackerBody.inventory.GetMoonWheelQualityChance(), attackerBody.master))
                    {
                        self.victimBody.StartCoroutine(LunarChargedQualityStrikeTwice(damage.attackerObject, team, damage.totalDamage, attackerBody.RollCrit(), self.victimBody, damage.hitHurtBox));
                    }
                }
                else
                {
                    ElementalReactionManager.CreateEnemyLunarChargeLightningStrike(damage.attackerObject, team, damage.totalDamage, attackerBody ? attackerBody.RollCrit() : false, damagePosition);
                }
            }
        }
        public static void CreateLunarChargedLightning(GameObject attacker, TeamIndex team, float damage, bool crit, Vector3 position)
        {
            if (!Mathf.Approximately(damage, 0f))
            {
                DamageTypeCombo damageType = new DamageTypeCombo(DamageType.DoT, DamageTypeExtended.Electrical, DamageSource.NoneSpecified);
                damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageType.AddModdedDamageType(DamageTypes.lunarDamageType);
                EffectManager.SimpleEffect(ElementalReactionManager.lunarChargedEffect.WaitForCompletion(), position, Quaternion.identity, true);

                BlastAttack blast = Util.CreateBlastAttack(attacker, team, damage, crit, StaticValues.lunarChargeRadius, BlastAttack.FalloffModel.None, 1f, damageType, position, 0f);
                blast.bonusForce = new Vector3(0f, -500f, 0f);
                blast.Fire();
            }
        }
        public static IEnumerator LunarChargedQualityStrikeTwice(GameObject attacker, TeamIndex team, float damage, bool crit, CharacterBody target, HurtBox targetHurtBox)
        {
            yield return new WaitForSeconds(0.4f);

            if (target && target.healthComponent.alive)
            {
                // Doesn't work for the last hit, since the DOT controller disappears
                CreateLunarChargedLightning(attacker, team, damage, crit, targetHurtBox ? targetHurtBox.transform.position : target.corePosition);
            }
        }
    }
}
