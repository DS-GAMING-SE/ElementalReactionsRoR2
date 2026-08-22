using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod.Orbs
{
    public class BloomSpawnOrb : Orb
    {
        public float damage;
        public GameObject owner;
        public Vector3 targetPosition;
        public override void Begin()
        {
            base.duration = 0.2f;
            EffectData effectData = new EffectData
            {
                origin = this.origin,
                start = targetPosition,
                genericFloat = base.duration
            };
            EffectManager.SpawnEffect(ElementalReactionManager.bloomSpawnOrb, effectData, true);
        }
        public override void OnArrival()
        {
            base.OnArrival();
            ProjectileManager.instance.FireProjectileServer(new FireProjectileInfo
            {
                projectilePrefab = ElementalReactionManager.bloomCore.WaitForCompletion(),
                damage = damage,
                crit = false,
                position = targetPosition,
                rotation = Util.RandomForwardRotation(),
                owner = owner
            });
        }

        public static void SpawnBloom(GameObject owner, float damage, Vector3 targetPosition, Vector3 originPosition)
        {
            OrbManager.instance.AddOrb(new BloomSpawnOrb
            {
                owner = owner,
                damage = damage,
                targetPosition = targetPosition,
                origin = originPosition
            });
        }
    }
}
