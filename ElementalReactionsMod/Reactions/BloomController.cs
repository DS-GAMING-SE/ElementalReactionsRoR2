using ElementalReactionsMod;
using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Orbs;
using HG;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static ElementalReactionsMod.Util;
using static UnityEngine.UI.Image;

namespace ElementalReactionsMod.Reactions
{
    public class BloomController : MonoBehaviour, IOnIncomingDamageServerReceiver
    {
        public CharacterBody characterBody;
        public ProjectileController projectileController;
        public ProjectileDamage projectileDamage;
        public Deployable deployable;

        private bool triedExplode;
        
        ElementalReactionPooledObject pool;
        private float timer;
        public void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();
            deployable = GetComponent<Deployable>();
            deployable.onUndeploy.AddListener(Explode);
        }
        public void Start()
        {
            pool = GetComponent<ElementalReactionPooledObject>();
        }
        public void OnIncomingDamageServer(DamageInfo damageInfo)
        {
            ElementIndex element = damageInfo.damageType.GetElement();
            if (element == DefaultElementDefs.pyroElement.index)
            {
                Chat.AddMessage("burgeon");
                Burgeon(damageInfo.attacker);
            }
            else if (element == DefaultElementDefs.electroElement.index)
            {
                Chat.AddMessage("hyperbloom");
                Hyperbloom(damageInfo.attacker);
            }
            damageInfo.rejected = true;
        }
        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer > StaticValues.bloomDuration && NetworkServer.active)
            {
                Explode();
            }
        }
        public void Explode()
        {
            if (!NetworkServer.active) return;
            if (!triedExplode)
            {
                triedExplode = true;

                DamageTypeCombo damageType = DamageType.AOE;
                damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageType.SetElement(DefaultElementDefs.dendroElement.index);

                float damage = StaticValues.bloomDamageCoefficient * projectileDamage.damage;

                ManualBlastAttack(characterBody.corePosition, StaticValues.genericReactionExplosionRadius, projectileController.owner, projectileController.teamFilter.teamIndex,
                    damage, damage * (Config.PlayerBloomResistance().Value / 100f), false, damageType, true);

                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Treebot.OmniExplosionVFXTreebot_prefab).WaitForCompletion(), characterBody.corePosition, Quaternion.identity, true);
            }
            
            DestroyObject();
        }
        public void Burgeon(GameObject attacker)
        {
            if (!NetworkServer.active) return;
            if (!triedExplode)
            {
                triedExplode = true;

                DamageTypeCombo damageType = DamageType.AOE;
                damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageType.SetElement(DefaultElementDefs.dendroElement.index);

                float damage = StaticValues.burgeonDamageCoefficient;
                damage *= attacker.TryGetComponent<CharacterBody>(out var attackerBody) ? attackerBody.damage : projectileDamage.damage;

                ManualBlastAttack(characterBody.corePosition, StaticValues.burgeonRadius, projectileController.owner, projectileController.teamFilter.teamIndex,
                    damage, damage * (Config.PlayerBloomResistance().Value / 100f), false, damageType, true);

                EffectManager.SimpleEffect(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Treebot.OmniExplosionVFXTreebot_prefab).WaitForCompletion(), characterBody.corePosition, Quaternion.identity, true);
            }
            
            DestroyObject();
        }
        public void Hyperbloom(GameObject attacker)
        {
            if (!NetworkServer.active) return;
            if (!triedExplode)
            {
                triedExplode = true;
                float damage = StaticValues.hyperBloomDamageCoefficient;
                damage *= attacker.TryGetComponent<CharacterBody>(out var attackerBody) ? attackerBody.damage : projectileDamage.damage;
                TeamIndex team = TeamComponent.GetObjectTeam(attacker);
                HyperbloomOrb.FireHyperbloomOrb(attacker, team == TeamIndex.None ? team : projectileController.teamFilter.teamIndex, characterBody.corePosition, damage);
            }
            DestroyObject();
        }
        private void DestroyObject()
        {
            if (pool)
            {
                pool.ReturnObject();
            }
            else
            {
                GameObject.Destroy(base.gameObject);
            }
        }
    }
}
