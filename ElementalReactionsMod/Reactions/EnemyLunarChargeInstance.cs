using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using Grumpy;
using UnityEngine.AddressableAssets;

namespace ElementalReactionsMod.Reactions
{
    public class EnemyLunarChargeInstance : ComponentPoolObject
    {
        private GameObject attacker;
        private TeamIndex team;
        private float damage;
        private bool crit;
        private Vector3 impactPosition;
        public static GameObject warningPrefab;

        private float stepTimer;
        private bool struck;
        private bool initialized;
        public void CreateLightningStrike(GameObject attacker, TeamIndex team, float damage, bool crit, Vector3 position)
        {
            impactPosition = position;
            this.team = team;
            this.damage = damage;
            this.crit = crit;
            this.attacker = attacker;

            struck = false;
            stepTimer = StaticValues.lunarChargeEnemyDelay;
            initialized = true;
            HandleWarning();
        }
        
        private void HandleWarning()
        {
            EffectManager.SpawnEffect(warningPrefab, new EffectData
            {
                origin = impactPosition,
                scale = StaticValues.lunarChargeRadius,
                genericFloat = StaticValues.lunarChargeEnemyDelay
            }, true);
        }
        private void HandleStrike()
        {
            struck = true;
            ElectroChargedDot.CreateLunarChargedLightning(attacker, team, damage, crit, impactPosition);
            stepTimer = 0.5f;
        }
        private void FixedUpdate()
        {
            if (!initialized) return;
            stepTimer -= Time.fixedDeltaTime;
            if (stepTimer <= 0)
            {
                if (!struck)
                {
                    HandleStrike();
                }
                else
                {
                    PreReturnToPool();
                    ReturnToPool();
                }
            }
        }
        public override void PreReturnToPool()
        {
            initialized = false;
        }
    }
}
