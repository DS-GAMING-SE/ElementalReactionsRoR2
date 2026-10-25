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
            RoR2.Util.PlaySound("Play_seeker_skill1_fire_orb", gameObject);
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
            RoR2.Util.PlaySound("Play_seeker_skill1_orb_explo", gameObject);
            upgraded = true;
            SetDirtyBit(1U);
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
            if (initialState)
            {
                writer.Write(upgraded);
                return true;
            }
            if (upgradedDirty)
            {
                writer.Write(true);
                writer.Write(upgraded);
                ClearAllDirtyBits();
                return true;
            }
            else
            {
                writer.Write(false);
                return false;
            }
        }
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                upgraded = reader.ReadBoolean();
                Log.Message("Initial Upgraded " + upgraded);
                if (upgraded)
                {
                    Upgrade();
                }
                return;
            }
            if (reader.ReadBoolean())
            {
                upgraded = reader.ReadBoolean();
                Log.Message("Upgraded " + upgraded);
                if (upgraded)
                {
                    Upgrade();
                }
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
            EffectManager.SimpleEffect(ElementalReactionManager.stellarSwirlSpawnEffect.WaitForCompletion(), transform.position, Quaternion.identity, false);
        }
        public void Upgrade()
        {
            orbRenderer.material = upgradedOrbMaterial;
            starScaleCurve.enabled = true;
            orbScaleCurve.enabled = true;
            EffectManager.SpawnEffect(ElementalReactionManager.stellarSwirlSpawnEffect.WaitForCompletion(), new EffectData { origin = transform.position, scale = 1.2f }, false);
        }
    }
}
