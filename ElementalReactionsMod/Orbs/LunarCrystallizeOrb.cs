using ElementalReactionsMod.Elements;
using R2API;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ElementalReactionsMod.Orbs
{
    public class LunarCrystallizeOrb : GenericDamageOrb
    {
        public static void FireLunarCrystallizeOrbs(CharacterBody attacker, Vector3 origin)
        {
            int fireCount = StaticValues.lunarCrystallizeTriggersToAttack;
            if (ElementalReactionsPlugin.qualityModExists && attacker.inventory)
            {
                float chance = attacker.inventory.GetMoonWheelQualityChance();
                for (int i = 0; i < StaticValues.lunarCrystallizeTriggersToAttack; i++)
                {
                    if (RoR2.Util.CheckRoll(chance, attacker.master))
                    {
                        fireCount++;
                    }
                }
            }
            HurtBox target = null;

            BullseyeSearch search = new BullseyeSearch();
            search.searchOrigin = origin;
            search.searchDirection = Vector3.zero;
            search.teamMaskFilter = TeamMask.GetEnemyTeams(attacker.teamComponent.teamIndex);
            search.filterByLoS = false;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = StaticValues.lunarCrystallizeRadius;
            search.RefreshCandidates();
            HurtBox[] targets = search.GetResults().ToArray();
            if (targets.Count() > 0)
            {
                for (int i = 0; i < fireCount; i++)
                {
                    target = targets[i % targets.Count()];
                    if (!target) return;

                    FireLunarCrystallizeOrb(attacker, origin, target, attacker.RollCrit());
                }
            }
        }
        public static void FireLunarCrystallizeOrb(CharacterBody attacker, Vector3 origin, HurtBox target, bool crit)
        {
            LunarCrystallizeOrb lunarCrystallizeOrb = new LunarCrystallizeOrb
            {
                attacker = attacker.gameObject,
                origin = origin,
                damageValue = StaticValues.lunarCrystallizeDamageCoefficient * attacker.damage * (target.teamIndex == TeamIndex.Player ? StaticValues.lunarCrystallizePlayerResistMultiplier : 1f),
                isCrit = crit,
                target = target,
                damageType = new DamageTypeCombo(),
                damageColorIndex = DamageColorIndex.Item,
                procCoefficient = 1,
                teamIndex = attacker.teamComponent.teamIndex,
                speed = 100 + UnityEngine.Random.Range(0f, 10f)
            };
            lunarCrystallizeOrb.damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
            lunarCrystallizeOrb.damageType.AddModdedDamageType(DamageTypes.lunarDamageType);
            OrbManager.instance.AddOrb(lunarCrystallizeOrb);
        }
        public override void Begin()
        {
            duration = (distanceToTarget / speed) + 0.1f;
            if (this.GetOrbEffect())
            {
                EffectData effectData = new EffectData
                {
                    scale = this.scale,
                    origin = this.origin,
                    genericFloat = base.duration,
                    genericUInt = (uint)DefaultElementDefs.geoElement.index
                };
                effectData.SetHurtBoxReference(this.target);
                EffectManager.SpawnEffect(this.GetOrbEffect(), effectData, true);
            }
        }
        public override GameObject GetOrbEffect()
        {
            return ElementalReactionManager.genericElementOrbEffect.WaitForCompletion();
        }
    }
}
