using RoR2;
using RoR2.Projectile;
using Sandswept.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ElementalReactionsMod.Reactions
{
    [RequireComponent(typeof(ProjectileController))]
    [RequireComponent(typeof(ProjectileDamage))]
    [RequireComponent(typeof(ProjectileExplosion))]
    public class StellarSwirlProjectile : NetworkBehaviour
    {
        public ProjectileController projectileController;
        public ProjectileDamage projectileDamage;
        public ProjectileExplosion projectileExplosion;
        public float stacks;
        public bool upgraded;
        private bool upgradedDirty;
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
            upgraded = true;
            upgradedDirty = true;
            projectileExplosion.SetExplosionRadius(StaticValues.stellarSwirlMaxRadius);
            if (projectileController.ghost && projectileController.ghost.TryGetComponent<StellarSwirlProjectileGhost>(out var ghost))
            {
                ghost.Upgrade();
            }
            projectileExplosion.explosionEffect = ElementalReactionManager.stellarSwirlExplosion2Effect.WaitForCompletion();
        }
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (upgradedDirty)
            {
                writer.Write(upgraded);
                upgradedDirty = false;
                return true;
            }
            return false;
        }
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            upgraded = reader.ReadBoolean();
            if (upgraded)
            {
                Upgrade();
            }
        }
    }
    [RequireComponent(typeof(EffectManagerHelper))]
    public class StellarSwirlProjectileGhost : MonoBehaviour
    {
        public ObjectScaleCurve starScaleCurve;
        public ObjectScaleCurve orbScaleCurve;
        public Renderer orbRenderer;
        public static Material orbMaterial;
        public static Material upgradedOrbMaterial;

        private void Awake()
        {
            GetComponent<EffectManagerHelper>().OnEffectActivated += Reset;
        }
        private void Reset()
        {
            orbRenderer.material = orbMaterial;
            orbScaleCurve.enabled = false;
            starScaleCurve.enabled = false;
        }
        public void Upgrade()
        {
            orbRenderer.material = upgradedOrbMaterial;
            starScaleCurve.enabled = true;
            orbScaleCurve.enabled = true;
        }
    }
}
