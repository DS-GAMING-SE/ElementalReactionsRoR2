using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Orbs;
using HG;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static ElementalReactionsMod.Assets.AssetReferences;
using static ElementalReactionsMod.Items.Items;

namespace ElementalReactionsMod.Items
{
    public static class DelusionManager
    {
        public static List<ItemDef> elementalDelusions = new List<ItemDef>();
        public static List<ElementDef> delusionElements = new List<ElementDef>();

        public static void Initialize()
        {
            AssetAsyncReferenceManager<GameObject>.LoadAsset(delusionPickupModel).Completed += x =>
            {
                x.Result.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = Assets.CreateVisionMaterial(delusionLogo, new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDefault_png));
                AddModelPanelParameters(x.Result);
            };
            delusion = AddNewItem("Delusion", "DELUSION", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(delusionItemIcon).WaitForCompletion(), delusionPickupModel, ItemTag.Damage);
            CharacterBody.onBodyInventoryChangedGlobal += AddDelusionBehaviour;
        }
        public static ItemDef CreateNewDelusion(ElementDef element)
        {
            ItemDef delusion = AddNewItem($"Delusion{element.cachedName}", $"DELUSION_{element.cachedName.ToUpper()}", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(delusionItemIcon).WaitForCompletion(), delusionPickupModel, ItemTag.Damage, ItemTag.WorldUnique);
            elementalDelusions.Add(delusion);
            delusionElements.Add(element);
            return delusion;
        }
        public static void AddDelusionBehaviour(CharacterBody body)
        {
            List<ElementDef> list = body.inventory.GetDelusions(out int count);
            DelusionBehaviour behaviour = body.AddItemBehavior<DelusionBehaviour>(count);
            if (behaviour)
            {
                behaviour.delusionElements = list;
            }
        }
        public static List<ElementDef> GetDelusions(this Inventory inventory, out int count)
        {
            List<ElementDef> list = new();
            count = 0;
            foreach (var element in delusionElements)
            {
                if (inventory.GetItemCountEffective(element.delusion) > 0)
                {
                    list.Add(element);
                    count += inventory.GetItemCountEffective(element.delusion);
                }
            }
            return list;
        }
        public static int GetDelusionCount(this Inventory inventory)
        {
            int count = 0;
            foreach (var delusion in elementalDelusions)
            {
                count += inventory.GetItemCountEffective(delusion);
            }
            return count;
        }
    }
    public class NoElementDelusion : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusion;
        }

        private Run.FixedTimeStamp transformTimeStamp;
        private ElementLoadoutComponent elementLoadout;
        private void OnEnable()
        {
            transformTimeStamp = Run.FixedTimeStamp.now + 1f;
        }

        private void Start()
        {
            elementLoadout = GetComponent<ElementLoadoutComponent>();
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active && Run.FixedTimeStamp.now > transformTimeStamp && body.inventory)
            {
                if (new Inventory.ItemTransformation
                {
                    originalItemIndex = GetItemDef().itemIndex,
                    newItemIndex = DecideDelusionElement().itemIndex,
                    maxToTransform = 1,
                    transformationType = 0
                }.TryTransform(body.inventory, out var results))
                {
                    PickupIndex pickup = PickupCatalog.FindPickupIndex(results.givenItem.itemIndex);
                    if (pickup != PickupIndex.none)
                    {
                        if (body.master && body.master.playerCharacterMasterController && body.master.playerCharacterMasterController.networkUser != null && body.master.playerCharacterMasterController.networkUser.localUser != null)
                        {
                            body.master.playerCharacterMasterController.networkUser.localUser.userProfile.DiscoverPickup(pickup);
                        }
                    }
                }
                else
                {
                    transformTimeStamp += 1f;
                }
            }
        }

        private ItemDef DecideDelusionElement()
        {
            Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
            List<ItemDef> possibleDelusions = new List<ItemDef>();
            RoR2.Util.CopyList<ItemDef>(DelusionManager.elementalDelusions, possibleDelusions);
            List<ItemDef> loadoutElements = new List<ItemDef>();
            if (elementLoadout)
            {
                if (elementLoadout.primaryElement && elementLoadout.primaryElement.hasDelusion) loadoutElements.Add(elementLoadout.primaryElement.delusion);
                if (elementLoadout.secondaryElement && elementLoadout.secondaryElement.hasDelusion) loadoutElements.Add(elementLoadout.secondaryElement.delusion);
                if (elementLoadout.utilityElement && elementLoadout.utilityElement.hasDelusion) loadoutElements.Add(elementLoadout.utilityElement.delusion);
                if (elementLoadout.specialElement && elementLoadout.specialElement.hasDelusion) loadoutElements.Add(elementLoadout.specialElement.delusion);
            }
            List<ItemDef> ownedDelusions = new List<ItemDef>();
            for (int i = 0; i < possibleDelusions.Count; i++)
            {
                if (body.inventory.GetItemCountPermanent(possibleDelusions[i]) > 0)
                {
                    ownedDelusions.Add(possibleDelusions[i]);
                }
            }
            if (loadoutElements.Count + ownedDelusions.Count >= possibleDelusions.Count)
            {
                if (ownedDelusions.Count >= possibleDelusions.Count)
                {
                    return possibleDelusions[rng.RangeInt(0, possibleDelusions.Count)];
                }
                possibleDelusions = possibleDelusions.Except(ownedDelusions).ToList();
                return possibleDelusions[rng.RangeInt(0, possibleDelusions.Count)];

            }
            possibleDelusions = possibleDelusions.Except(ownedDelusions).Except(loadoutElements).ToList();
            return possibleDelusions[rng.RangeInt(0, possibleDelusions.Count)];
        }
    }
    public class DelusionBehaviour : CharacterBody.ItemBehavior, IOnDamageDealtServerReceiver
    {
        private bool wasActive = false;
        protected float attackCooldown;
        public List<ElementDef> delusionElements;

        private void Start()
        {
            if (body)
            {
                body.onSkillActivatedServer += OnSkillActivated;
            }
        }
        private void OnSkillActivated(GenericSkill skill)
        {
            if (body.HasBuff(Buffs.delusionReadyBuff) && body.skillLocator && body.skillLocator.special && body.skillLocator.special == skill)
            {
                Log.Message("Delusion activated");
                body.AddTimedBuffTimer(Buffs.delusionActiveBuff, StaticValues.delusionDuration);
                body.RemoveBuff(Buffs.delusionReadyBuff);
                attackCooldown = 0;
            }
        }
        public void OnDamageDealtServer(DamageReport damageReport)
        {
            if (body.HasBuff(Buffs.delusionActiveBuff) && damageReport.damageInfo.damageType.IsDamageSourceSkillBased && attackCooldown == 0 && damageReport.victimBody)
            {
                attackCooldown = 1 / StaticValues.delusionAttacksPerSecond;
                StartCoroutine(FireDelusionsOrbs(damageReport.victimBody.mainHurtBox));
            }
        }
        private IEnumerator FireDelusionsOrbs(HurtBox target)
        {
            RoR2.Util.ShuffleList(delusionElements);
            foreach (var element in delusionElements)
            {
                if (!target || !target.healthComponent || !target.healthComponent.alive || !body || !body.healthComponent || !body.inventory)
                {
                    break;
                }
                if (element.hasDelusion && body.inventory.GetItemCountEffective(element.delusion) > 0)
                {
                    body.healthComponent.TakeDamage(new DamageInfo
                    {
                        attacker = null,
                        inflictor = null,
                        damage = StaticValues.delusionHealthPercentCost * body.healthComponent.fullHealth,
                        damageType = DamageType.BypassArmor | DamageType.NonLethal,
                        position = body.corePosition,
                        crit = false,
                        procCoefficient = 0f,
                        inflictedHurtbox = body.mainHurtBox
                    });
                    DelusionOrb.FireDelusionOrb(body, target, body.inventory.GetItemCountEffective(element.delusion), body.RollCrit(), element.index);
                    yield return new WaitForSeconds((1 / StaticValues.delusionAttacksPerSecond) / DelusionManager.elementalDelusions.Count);
                }
            }
        }
        private void FixedUpdate()
        {
            bool cooldown = body.HasBuff(Buffs.delusionCooldownBuff);
            bool ready = body.HasBuff(Buffs.delusionReadyBuff);
            bool active = body.HasBuff(Buffs.delusionActiveBuff);
            if (!cooldown && !ready && !active)
            {
                if (wasActive)
                {
                    body.AddTimedBuffTimer(Buffs.delusionCooldownBuff, StaticValues.delusionCooldown);
                }
                else
                {
                    body.AddBuff(Buffs.delusionReadyBuff);
                }
            }
            if (ready && active)
            {
                body.RemoveBuff(Buffs.delusionReadyBuff);
            }
            if (active && cooldown)
            {
                body.RemoveBuff(Buffs.delusionActiveBuff);
            }
            wasActive = body.HasBuff(Buffs.delusionActiveBuff);
            if (active) attackCooldown = Mathf.Max(attackCooldown - Time.fixedDeltaTime, 0);
        }

        private void OnDisable()
        {
            if (body)
            {
                body.onSkillActivatedServer -= OnSkillActivated;
                if (body.HasBuff(Buffs.delusionCooldownBuff)) body.RemoveBuff(Buffs.delusionCooldownBuff);
                if (body.HasBuff(Buffs.delusionReadyBuff)) body.RemoveBuff(Buffs.delusionReadyBuff);
                if (body.HasBuff(Buffs.delusionActiveBuff)) body.RemoveBuff(Buffs.delusionActiveBuff);
            }
        } 
    }
}
