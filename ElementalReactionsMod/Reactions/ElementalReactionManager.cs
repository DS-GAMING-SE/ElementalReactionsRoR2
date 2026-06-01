using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using Grumpy;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ElementalReactionsMod.Reactions
{
    public class ElementalReactionManager : MonoBehaviour
    {
        public static ElementalReactionManager instance;
        // how do you network pools?
        public static DeployableSlot bloomDeployableSlot;
        //public static PrefabComponentPool<ElementalReactionPooledObject> bloomPool;

        public static DeployableSlot crystallizeDeployableSlot;
        //public static PrefabComponentPool<ElementalReactionPooledObject> crystallizePool;
        public void OnEnable()
        {
            SingletonHelper.Assign(ref instance, this);
            //if (bloomPool == null) CreatePool(ref bloomPool, AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.bloomObject, AsyncReferenceHandleUnloadType.OnRunEnd).WaitForCompletion(), StaticValues.bloomCap);
            //if (crystallizePool == null) CreatePool(ref crystallizePool, AssetAsyncReferenceManager<GameObject>.LoadAsset(Assets.AssetReferences.crystallizePickup, AsyncReferenceHandleUnloadType.OnRunEnd).WaitForCompletion(), StaticValues.crystallizeCap);
        }
        public void OnDisable()
        {
            //bloomPool.Kill();
            //crystallizePool.Kill();
            SingletonHelper.Unassign(ref instance, this);
        }

        public static void ApplyElement(ElementDef element, CharacterBody target)
        {
            if (element && target)
            {
                DamageInfo empty = new DamageInfo();
                ApplyElement(element, target, ref empty, out _);
            }
        }
        public static void ApplyElement(ElementDef element, CharacterBody target, ref DamageInfo damageInfo, out float addedDamage)
        {
            addedDamage = 0;
            if (element && element != DefaultElementDefs.physicalElement && !target.HasBuff(element.cooldownBuff))
            {
                if (!TryTriggerReaction(element, target, ref damageInfo, ref addedDamage) && element.canPersist)
                {
                    target.AddTimedBuff(element.buff, 10);
                    target.AddTimedBuff(element.cooldownBuff, StaticValues.elementAppliedICD);
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
                    target.ClearTimedBuffs(reacting.buff.buffIndex);
                    target.AddTimedBuff(element.cooldownBuff, StaticValues.elementRemovedICD);
                    target.AddTimedBuff(reacting.cooldownBuff, StaticValues.elementRemovedICD);
                    reaction.TriggerReaction(reacting, element, target, ref damageInfo, ref addedDamage);
                    return true;
                }
            }
            return false;
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
