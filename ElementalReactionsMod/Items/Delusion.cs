using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Orbs;
using HG;
using R2API;
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
        public static ItemTag delusionItemTag;
        public static Dictionary<ItemDef, ElementDef> delusionToElement = new Dictionary<ItemDef, ElementDef>();

        private static Material delusionMat;

        public static void Initialize()
        {
            delusionItemTag = ItemAPI.AddItemTag("ElementalReactionsDelusion");
            delusionMat = Assets.CreateVisionMaterial(delusionLogo, new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDefault_png), 0.75f);
            AssetAsyncReferenceManager<GameObject>.LoadAsset(delusionPickupModel).Completed += x =>
            {
                x.Result.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = delusionMat;
                AddModelPanelParameters(x.Result);
            };
            delusion = AddNewItem("Delusion", "DELUSION", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(delusionItemIcon).WaitForCompletion(), delusionPickupModel, InitializeItemDisplays(), ItemTag.Damage, ItemTag.AllowedForUseAsCraftingIngredient);
            ItemAPI.ApplyTagToItem(delusionItemTag, delusion);
        }
        public static ItemDef CreateNewDelusion(ElementDef element)
        {
            ItemDef delusion = AddNewItem($"Delusion{element.cachedName}", $"DELUSION_{element.cachedName.ToUpper()}", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(delusionItemIcon).WaitForCompletion(), delusionPickupModel, null, ItemTag.Damage, ItemTag.WorldUnique, ItemTag.AllowedForUseAsCraftingIngredient);
            ItemAPI.ApplyTagToItem(delusionItemTag, delusion);
            delusionToElement.Add(delusion, element);
            return delusion;
        }
        public static List<ElementDef> GetDelusions(this Inventory inventory, out int count)
        {
            List<ElementDef> list = new();
            count = 0;
            foreach (var element in delusionToElement.Values)
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
            foreach (var delusion in delusionToElement.Keys)
            {
                count += inventory.GetItemCountEffective(delusion);
            }
            return count;
        }
        public static ItemDisplayRuleDict InitializeItemDisplays()
        {
            GameObject displayPrefab = AssetAsyncReferenceManager<GameObject>.LoadAsset(delusionDisplayModel).WaitForCompletion();
            displayPrefab.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = delusionMat;
            CreateItemDisplay(displayPrefab, CreateItemRendererInfo(displayPrefab, 0, visionHolderMaterial), CreateItemRendererInfo(displayPrefab, 1, delusionMat));
            ItemDisplayRuleDict itemDisplays = new ItemDisplayRuleDict();
            /*itemDisplays.Add("CommandoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "LowerArmL",
                localPos = new Vector3(0.03206F, 0.23382F, -0.05406F),
                localAngles = new Vector3(5.56553F, 0.75563F, 0.95463F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("HuntressBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.00073F, 0.20592F, 0.162F),
                localAngles = new Vector3(331.3469F, 1.57845F, 1.41202F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("Bandit2Body", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Hat",
                localPos = new Vector3(-0.00262F, 0.11559F, 0.08425F),
                localAngles = new Vector3(317.5828F, 353.8432F, 5.02905F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F)
            });
            itemDisplays.Add("ToolbotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("EngiBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("MageBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("MercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("TreebotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });*/
            itemDisplays.Add("LoaderBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "MechHandR",
                localPos = new Vector3(0.07702F, 0.14753F, 0.14142F),
                localAngles = new Vector3(4.85349F, 34.75354F, 181.8131F),
                localScale = new Vector3(1F, 1F, 1F)
            });/*
            itemDisplays.Add("CrocoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("CaptainBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("RailgunnerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });*/
            itemDisplays.Add("VoidSurvivorBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "CannonEnd",
                localPos = new Vector3(-0.01739F, -0.20445F, -0.09182F),
                localAngles = new Vector3(355.2102F, 51.1422F, 184.9114F),
                localScale = new Vector3(1F, 1F, 1F)
            });/*
            itemDisplays.Add("SeekerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Pack",
                localPos = new Vector3(-0.17127F, -0.10742F, -0.35991F),
                localAngles = new Vector3(332.9016F, 29.06501F, 28.89558F),
                localScale = new Vector3(1F, 1F, 1F)

            });
            itemDisplays.Add("FalseSonBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("ChefBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("DroneTechBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });*/
            itemDisplays.Add("DrifterBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "BagBulgeLeft",
                localPos = new Vector3(0.07217F, 0.12617F, 0.23026F),
                localAngles = new Vector3(345.8675F, 297.6887F, 270.2614F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("BrotherBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject("de62e89840152434cb622e434bf9bb62"),
                childName = "chest",
                localPos = new Vector3(-0.22503F, 0.31914F, 0.08435F),
                localAngles = new Vector3(7.70729F, 20.07879F, 317.6756F),
                localScale = new Vector3(0.11F, 0.11F, 0.11F)
            });
            return itemDisplays;
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
                    if (DelusionManager.delusionToElement.TryGetValue(ItemCatalog.GetItemDef(results.givenItem.itemIndex), out var element))
                    {
                        GenericElementEffectComponent.SpawnActivatedEffect(gameObject.transform, element.index, 1.5f, true);
                    };
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
            List<ItemDef> possibleDelusions = DelusionManager.delusionToElement.Keys.ToList();
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
            if (body.skillLocator && body.skillLocator.special && body.skillLocator.special == skill)
            {
                body.ClearTimedBuffs(Buffs.delusionActiveBuff);
                body.AddTimedBuffTimer(Buffs.delusionActiveBuff, StaticValues.delusionDuration);
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
                        damage = (StaticValues.delusionHealthPercentCost * body.inventory.GetItemCountEffective(element.delusion)) * body.healthComponent.fullHealth,
                        damageType = DamageType.BypassArmor | DamageType.NonLethal,
                        position = body.corePosition,
                        crit = false,
                        procCoefficient = 0f,
                        inflictedHurtbox = body.mainHurtBox,
                        damageColorIndex = DamageColorIndex.Item
                    });
                    DelusionOrb.FireDelusionOrb(body, target, body.inventory.GetItemCountEffective(element.delusion), body.RollCrit(), element.index);
                    GenericElementEffectComponent.SpawnActivatedEffect(gameObject.transform, element.index, true);
                    yield return new WaitForSeconds((1 / StaticValues.delusionAttacksPerSecond) / DelusionManager.delusionToElement.Keys.Count);
                }
            }
        }
        private void FixedUpdate()
        {
            if (body.HasBuff(Buffs.delusionActiveBuff)) attackCooldown = Mathf.Max(attackCooldown - Time.fixedDeltaTime, 0);
        }

        private void OnDisable()
        {
            if (body)
            {
                body.onSkillActivatedServer -= OnSkillActivated;
                if (body.HasBuff(Buffs.delusionActiveBuff)) body.ClearTimedBuffs(Buffs.delusionActiveBuff);
            }
        } 
    }
}
