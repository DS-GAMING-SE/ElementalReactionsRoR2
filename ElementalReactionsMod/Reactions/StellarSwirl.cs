using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace ElementalReactionsMod.Reactions
{
    [RequireComponent(typeof(ProjectileController))]
    [RequireComponent(typeof(ProjectileDamage))]
    [RequireComponent(typeof(ProjectileExplosion))]
    public class StellarSwirlProjectile : MonoBehaviour
    {
        public ProjectileController projectileController;
        public ProjectileDamage projectileDamage;
        public ProjectileExplosion projectileExplosion;
        public float stacks;
        public bool upgraded;
        public float timer;

        public void Awake()
        {
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();
            projectileExplosion = GetComponent<ProjectileExplosion>();
        }

        public void Start()
        {
            InstanceTracker.Add(this);
            timer = StaticValues.stellarSwirlMaxDuration;
            stacks = ((float)projectileController.combo) / 10f;
        }

        public void OnDestroy()
        {
            InstanceTracker.Remove(this);
        }

        public void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;
            if (NetworkServer.active && timer <= 0)
            {
                Detonate();
            }
        }

        public void AddStack(float proc, float damageStat)
        {
            stacks += 0.5f + (proc * 0.5f);
            if (stacks >= StaticValues.stellarSwirlUpgradeStacks && !upgraded)
            {
                upgraded = true;
                Upgrade();
            }
            float damage = damageStat * (upgraded ? StaticValues.stellarSwirlMaxDamage : StaticValues.stellarSwirlMinDamage);
            if (damage > projectileDamage.damage)
            {
                projectileDamage.damage = damage;
            }
            if (stacks >= StaticValues.stellarSwirlDetonateStacks)
            {
                Detonate();
            }
        }
        public void Detonate()
        {
            projectileExplosion.Detonate();
        }
        public void Upgrade()
        {
            projectileExplosion.SetExplosionRadius(StaticValues.stellarSwirlMaxRadius);
            if (projectileController.ghost)
            {
                projectileController.ghost.emh.ReturnToPool();
            }
            // projectileController.ghost = upgraded projectile
        }
    }
}
