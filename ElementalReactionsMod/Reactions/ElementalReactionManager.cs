using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using Grumpy;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;
using static UnityEngine.GridBrushBase;

namespace ElementalReactionsMod.Reactions
{
    public class ElementalReactionManager : MonoBehaviour
    {
        public static ElementalReactionManager instance;

        #region Assets
        public static AsyncOperationHandle<GameObject> genericElementActivatedEffect;
        public static AsyncOperationHandle<GameObject> genericElementOrbEffect;
        public static AsyncOperationHandle<GameObject> overloadEffect;
        public static AsyncOperationHandle<GameObject> electroChargedTempVisualEffect;
        public static AsyncOperationHandle<GameObject> superconductEffect;
        public static AsyncOperationHandle<GameObject> superconductTempVisualEffect;
        public static AsyncOperationHandle<GameObject> swirlEffect;
        public static AsyncOperationHandle<GameObject> crystallizeSpawnOrbEffect;
        public static AsyncOperationHandle<GameObject> crystallizePickup;
        public static DeployableSlot crystallizeDeployableSlot;
        //public static PrefabComponentPool<ElementalReactionPooledObject> crystallizePool;
        public static AsyncOperationHandle<GameObject> quickenTempVisualEffect;
        public static GameObject bloomSpawnOrb;
        public static AsyncOperationHandle<GameObject> bloomCore;
        public static AsyncOperationHandle<GameObject> bloomExplosion;
        public static AsyncOperationHandle<GameObject> burgeonExplosion;
        public static AsyncOperationHandle<GameObject> hyperbloomOrb;
        public static DeployableSlot bloomDeployableSlot;
        //public static PrefabComponentPool<ElementalReactionPooledObject> bloomPool;
        public static AsyncOperationHandle<GameObject> lunarChargedEffect;
        public static ComponentPoolManager lunarChargeEnemyPool;
        public static GameObject lunarChargeEnemyStrikePrefab;
        public static AsyncOperationHandle<GameObject> lunarBloomEffect;
        public static AsyncOperationHandle<GameObject> lunarCrystallizeController;
        public static AsyncOperationHandle<GameObject> lunarCrystallizeActivatedEffect;
        public static ComponentPoolManager lunarCrystallizePool;
        #endregion

        public delegate void PreElementalReactionDelegate(ref ElementalReactionDef reaction, ElementDef firstElement, ElementDef secondElement, CharacterBody victim, ref DamageInfo damageInfo);
        public static event PreElementalReactionDelegate onPreElementalReactionTriggered;

        public delegate void ElementalReactionDelegate(ElementalReactionDef reaction, ElementDef firstElement, ElementDef secondElement, CharacterBody victim, GameObject attacker);
        public static event ElementalReactionDelegate onElementalReactionTriggered;
        public void OnEnable()
        {
            SingletonHelper.Assign(ref instance, this);
            PreloadAssets();
            SpawnCard.onSpawnedServerGlobal += AddMoonWheelToLunarEnemies;
            //if (bloomPool == null) CreatePool(ref bloomPool, AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.bloomObject, AsyncReferenceHandleUnloadType.OnRunEnd).WaitForCompletion(), StaticValues.bloomCap);
            //if (crystallizePool == null) CreatePool(ref crystallizePool, AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.crystallizePickup, AsyncReferenceHandleUnloadType.OnRunEnd).WaitForCompletion(), StaticValues.crystallizeCap);
        }
        public void OnDisable()
        {
            //bloomPool.Kill();
            //crystallizePool.Kill();
            if (lunarChargeEnemyPool != null) lunarChargeEnemyPool.ResetPools();
            if (lunarCrystallizePool != null) lunarCrystallizePool.ResetPools();
            SpawnCard.onSpawnedServerGlobal -= AddMoonWheelToLunarEnemies;
            UnloadAssets();
            SingletonHelper.Unassign(ref instance, this);
        }
        public static void ApplyElement(ElementDef element, CharacterBody target, float procCoefficient = 1f, GameObject overrideAttacker = null, bool alwaysPersist = false, float overrideICD = -1)
        {
            if (element && target)
            {
                DamageInfo empty = new DamageInfo();
                empty.procCoefficient = procCoefficient;
                if (overrideAttacker) empty.attacker = overrideAttacker;
                ApplyElement(element, target, ref empty, out _, alwaysPersist, overrideICD);
            }
        }
        public static void ApplyElement(ElementDef element, CharacterBody target, ref DamageInfo damageInfo, out float addedDamage, bool alwaysPersist = false, float overrideICD = -1)
        {
            addedDamage = 0;
            if (element && element != DefaultElementDefs.physicalElement && !target.HasBuff(element.cooldownBuff) && damageInfo.procCoefficient != 0)
            {
                if (!TryTriggerReaction(element, target, ref damageInfo, ref addedDamage) && (element.canPersist || alwaysPersist))
                {
                    if (damageInfo.procCoefficient == float.MaxValue)
                    {
                        target.AddBuff(element.buff);
                    }
                    else
                    {
                        target.AddTimedBuff(element.buff, StaticValues.elementAppliedDuration * damageInfo.procCoefficient * StaticValues.elementAppliedTaxMultiplier);
                    }
                    target.AddTimedBuff(element.cooldownBuff, (overrideICD > 0 ? overrideICD : StaticValues.elementAppliedICD));
                }
            }
        }
        private static bool TryTriggerReaction(ElementDef element, CharacterBody target, ref DamageInfo damageInfo, ref float addedDamage)
        {
            ElementDef reacting = ElementalReactionCatalog.GetFirstReactableElement(element, target);
            if (reacting)
            {
                ElementalReactionDef reaction = ElementalReactionCatalog.GetElementalReaction(element, reacting);
                if (reaction)
                {
                    if (damageInfo.procCoefficient != float.MaxValue)
                    {
                        float remainingElementDuration = target.ReduceTimedBuffDuration(reacting.buff, StaticValues.elementAppliedDuration * damageInfo.procCoefficient * (reacting == reaction.baseElement ? reaction.baseFirstReactionCoefficient : reaction.baseLastReactionCoefficient));
                        if (remainingElementDuration == float.MaxValue)
                        {
                            target.RemoveBuff(reacting.buff);
                            target.AddTimedBuff(reacting.cooldownBuff, StaticValues.permanentElementICD);
                        }
                        if (remainingElementDuration == 0)
                        {
                            target.AddTimedBuff(reacting.cooldownBuff, StaticValues.elementRemovedICD);
                        }
                    }
                    else
                    {
                        target.ClearTimedBuffs(reacting.buff);
                        if (target.HasBuff(reacting.buff)) target.RemoveBuff(reacting.buff);
                        target.AddTimedBuff(reacting.cooldownBuff, StaticValues.elementRemovedICD);
                    }
                    target.AddTimedBuff(element.cooldownBuff, StaticValues.elementAppliedICD);

                    if (damageInfo.procCoefficient == float.MaxValue) damageInfo.procCoefficient = 1f;

                    onPreElementalReactionTriggered?.Invoke(ref reaction, reacting, element, target, ref damageInfo);
                    reaction.TriggerReaction(reacting, element, target, ref damageInfo, ref addedDamage);
                    onElementalReactionTriggered?.Invoke(reaction, reacting, element, target, damageInfo.attacker);
                    return true;
                }
            }
            return false;
        }
        // Lunar Wisp's secondary has damage falloff, so it doesn't always do enough damage to proc lunar bloom. Do something about this?
        private static void AddMoonWheelToLunarEnemies(SpawnCard.SpawnResult spawnResult)
        {
            if (!Config.LunarEnemyLunarReactions().Value) return;
            CharacterMaster characterMaster = spawnResult.spawnedInstance ? spawnResult.spawnedInstance.GetComponent<CharacterMaster>() : null;
            if (characterMaster && characterMaster.inventory && characterMaster.backupBodyIndex != BodyIndex.None && EnemyElementLoadouts.enemiesWithMoonWheel.Contains(characterMaster.backupBodyIndex) && characterMaster.inventory.GetItemCountPermanent(Items.MoonWheel.hiddenMoonWheel) == 0)
            {
                characterMaster.inventory.GiveItemPermanent(Items.MoonWheel.hiddenMoonWheel);
            }
        }

        public static void CreateEnemyLunarChargeLightningStrike(GameObject attacker, TeamIndex team, float damage, bool crit, Vector3 position)
        {
            if (lunarChargeEnemyPool == null)
            {
                lunarChargeEnemyPool = new ComponentPoolManager(1, 6, false, true);
            }
            EnemyLunarChargeInstance strike = (EnemyLunarChargeInstance)lunarChargeEnemyPool.GetPooledObject(lunarChargeEnemyStrikePrefab);
            strike.CreateLightningStrike(attacker, team, damage, crit, position);
        }

        public static void CreateLunarCrystallizeController(CharacterBody characterBody)
        {
            if (lunarCrystallizePool== null)
            {
                lunarCrystallizePool = new ComponentPoolManager(1, 3, false, true);
            }
            LunarCrystallizeController lunarCrystallize = (LunarCrystallizeController)lunarCrystallizePool.GetPooledObject(lunarCrystallizeController.WaitForCompletion());
            lunarCrystallize.CreateLunarCrystallizeController(characterBody);
        }

        private static void PreloadAssets()
        {
            genericElementActivatedEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.genericElementActivatedEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            genericElementOrbEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.genericElementOrbEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            overloadEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.overloadEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            electroChargedTempVisualEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.electroChargeTempVisualEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            superconductEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.superconductEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            superconductTempVisualEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.superconductTempVisualEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            swirlEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.swirlEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            crystallizeSpawnOrbEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.crystallizeSpawnOrbEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            crystallizePickup = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.crystallizePickup, AsyncReferenceHandleUnloadType.OnRunEnd);
            quickenTempVisualEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.quickenTempVisualEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            bloomCore = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.bloomObject, AsyncReferenceHandleUnloadType.OnRunEnd);
            bloomExplosion = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.bloomExplosion, AsyncReferenceHandleUnloadType.OnRunEnd);
            burgeonExplosion = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.burgeonExplosion, AsyncReferenceHandleUnloadType.OnRunEnd);
            hyperbloomOrb = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.hyperbloomOrb, AsyncReferenceHandleUnloadType.OnRunEnd);
            lunarChargedEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.lunarChargedLightningEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            lunarBloomEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.lunarBloomEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
            lunarCrystallizeController = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.lunarCrystallizeController, AsyncReferenceHandleUnloadType.OnRunEnd);
            lunarCrystallizeActivatedEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.lunarCrystallizeActivatedEffect, AsyncReferenceHandleUnloadType.OnRunEnd);
        }
        private static void UnloadAssets()
        {
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.genericElementActivatedEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.genericElementOrbEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.overloadEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.electroChargeTempVisualEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.superconductEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.superconductTempVisualEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.swirlEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.crystallizeSpawnOrbEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.crystallizePickup);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.quickenTempVisualEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.bloomObject);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.bloomExplosion);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.burgeonExplosion);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.hyperbloomOrb);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.lunarChargedLightningEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.lunarBloomEffect);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.lunarCrystallizeController);
            AssetAsyncReferenceManager<GameObject>.UnloadAsset(Assets.AssetReferences.lunarCrystallizeActivatedEffect);
        }
        #region Pooling Attempts
        public static void CreatePool(ref PrefabComponentPool<ElementalReactionPooledObject> pool, GameObject prefab, int baseCap)
        {
            pool = new PrefabComponentPool<ElementalReactionPooledObject>
            {
                PrefabObject = prefab,
                AddComponentIfMissing = true,
                AlwaysGrowable = true,
                DoNotCull = true
            };
            pool.Initialize(baseCap, baseCap);
        }
        public static ElementalReactionPooledObject CreatePooledDeployable(PrefabComponentPool<ElementalReactionPooledObject> pool, CharacterBody owner, DeployableSlot deployableSlot, Vector3 position)
        {
            if (owner && owner.master)
            {
                ElementalReactionPooledObject pooledObject = CreatePooledObject(pool, position);
                if (pooledObject)
                {
                    if (pooledObject.deployable)
                    {
                        owner.master.AddDeployable(pooledObject.deployable, deployableSlot);
                    }
                }
                return pooledObject;
            }
            return null;
        }
        public static ElementalReactionPooledObject CreatePooledObject(PrefabComponentPool<ElementalReactionPooledObject> pool, Vector3 position)
        {
            ElementalReactionPooledObject pooledObject = pool.GetObject();
            if (pooledObject)
            {
                pooledObject.gameObject.SetActive(true);
                pooledObject.pool = pool;
                pooledObject.transform.position = position;
                pooledObject.transform.rotation = Quaternion.identity;
                if (pooledObject.newlySpawned)
                {
                    pooledObject.newlySpawned = false;
                    NetworkServer.Spawn(pooledObject.gameObject);
                }
                else
                {
                    new NetworkPooledObjectSetActive(pooledObject.networkIdentity.netId, true).Send(R2API.Networking.NetworkDestination.Clients);
                }
                return pooledObject;
            }
            return null;
        }
        #endregion
    }
}
