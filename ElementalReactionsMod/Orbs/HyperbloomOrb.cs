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
using UnityEngine.UIElements;

namespace ElementalReactionsMod.Orbs
{
    public class HyperbloomOrb : GenericDamageOrb
    {
        public static void FireHyperbloomOrb(GameObject attacker, TeamIndex attackerTeam, Vector3 origin, float damage)
        {
            HurtBox target = null;

            BullseyeSearch search = new BullseyeSearch();
            search.searchOrigin = origin;
            search.searchDirection = Vector3.zero;
            search.teamMaskFilter = TeamMask.GetEnemyTeams(attackerTeam);
            search.filterByLoS = false;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = StaticValues.hyperBloomRadius;
            search.RefreshCandidates();
            target = search.GetResults().FirstOrDefault();
            if (!target) return;

            HyperbloomOrb hyperbloomOrb = new HyperbloomOrb
            {
                attacker = attacker.gameObject,
                origin = origin,
                damageValue = target.teamIndex == TeamIndex.Player ? damage * (Config.PlayerBloomResistance().Value / 100f) : damage,
                isCrit = false,
                target = target,
                damageType = new DamageTypeCombo(),
                procCoefficient = 0.5f,
                teamIndex = attackerTeam,
                speed = 90f,
                damageColorIndex = DamageColorIndex.Item
            };
            hyperbloomOrb.damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
            OrbManager.instance.AddOrb(hyperbloomOrb);
        }
        public override void Begin()
        {
            base.duration = (base.distanceToTarget / this.speed) + 0.1f;
            if (this.GetOrbEffect())
            {
                EffectData effectData = new EffectData
                {
                    scale = this.scale,
                    origin = this.origin,
                    genericFloat = base.duration
                };
                effectData.SetHurtBoxReference(this.target);
                EffectManager.SpawnEffect(this.GetOrbEffect(), effectData, true);
            }
        }
        public override GameObject GetOrbEffect()
        {
            return ElementalReactionManager.hyperbloomOrb.WaitForCompletion();
        }
    }
}
