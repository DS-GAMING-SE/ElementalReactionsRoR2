using ElementalReactionsMod.Reactions;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ElementalReactionsMod.Assets.AssetReferences;
using static ElementalReactionsMod.Items.Items;

namespace ElementalReactionsMod.Items
{
    public static class MoonWheel
    {
        public static ItemDef moonWheel;
        private static Material moonWheelMat;

        public static void Initialize()
        {
            moonWheelMat = Assets.CreateVisionMaterial(moonWheelVisionIcon, moonWheelVisionRamp, 1.5f);
            AssetAsyncReferenceManager<GameObject>.LoadAsset(moonWheelPickupModel).Completed += x =>
            {
                x.Result.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = moonWheelMat;
                AddModelPanelParameters(x.Result);
            };
            moonWheel = AddNewItem("MoonWheel", "MOON_WHEEL", true,
                Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.Tier3Def_asset).WaitForCompletion(),
                moonWheelItemIcon.LoadAssetAsync<Sprite>().WaitForCompletion(), moonWheelPickupModel, InitializeItemDisplays(), ItemTag.Damage, ItemTag.CanBeTemporary, ItemTag.AllowedForUseAsCraftingIngredient, ItemTag.DevotionBlacklist);

            ElementalReactionManager.onPreElementalReactionTriggered += (ref reaction, element1, element2, victim, ref damage) =>
            {
                if (damage.attacker && damage.attacker.TryGetComponent<CharacterBody>(out var attackerBody) && attackerBody.inventory && attackerBody.inventory.GetItemCountWithQuality(Items.moonWheel) > 0)
                {
                    if (reaction == DefaultElementalReactions.electroCharge)
                    {
                        reaction = DefaultElementalReactions.lunarCharge;
                    }
                    else if (reaction == DefaultElementalReactions.bloom)
                    {
                        reaction = DefaultElementalReactions.lunarBloom;
                    }
                    else if (reaction == DefaultElementalReactions.crystallize)
                    {
                        reaction = DefaultElementalReactions.lunarCrystallize;
                    }
                }
            };
        }
        public static ItemDisplayRuleDict InitializeItemDisplays()
        {
            GameObject displayPrefab = AssetAsyncReferenceManager<GameObject>.LoadAsset(moonWheelDisplayModel).WaitForCompletion();
            displayPrefab.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = moonWheelMat;
            CreateItemDisplay(displayPrefab, CreateItemRendererInfo(displayPrefab, 0, visionHolderMaterial), CreateItemRendererInfo(displayPrefab, 1, moonWheelMat));
            ItemDisplayRuleDict itemDisplays = new ItemDisplayRuleDict();
            itemDisplays.Add("CommandoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "LowerArmL",
                localPos = new Vector3(0.03206F, 0.23382F, -0.05406F),
                localAngles = new Vector3(5.56553F, 0.75563F, 0.95463F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("HuntressBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.00073F, 0.20592F, 0.162F),
                localAngles = new Vector3(331.3469F, 1.57845F, 1.41202F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("Bandit2Body", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Hat",
                localPos = new Vector3(-0.00262F, 0.11559F, 0.08425F),
                localAngles = new Vector3(317.5828F, 353.8432F, 5.02905F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F)
            });
            itemDisplays.Add("ToolbotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(1.92758F, 1.8005F, 3.28254F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(5F, 5F, 5F)
            });
            /*
            itemDisplays.Add("EngiBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("MageBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("MercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("TreebotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });*/
            itemDisplays.Add("LoaderBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "MechHandL",
                localPos = new Vector3(-0.06373F, 0.10357F, 0.13916F),
                localAngles = new Vector3(10.07847F, 334.9965F, 177.9507F),
                localScale = new Vector3(1F, 1F, 1F)
            });/*
            itemDisplays.Add("CrocoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("CrocoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("CaptainBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("RailgunnerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });*/
            itemDisplays.Add("VoidSurvivorBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "CannonEnd",
                localPos = new Vector3(0.31781F, -0.10994F, 0.09012F),
                localAngles = new Vector3(347.5417F, 246.9279F, 174.3385F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("SeekerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Pack",
                localPos = new Vector3(-0.17127F, -0.10742F, -0.35991F),
                localAngles = new Vector3(332.9016F, 29.06501F, 28.89558F),
                localScale = new Vector3(1F, 1F, 1F)

            });/*
            itemDisplays.Add("FalseSonBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("ChefBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("DroneTechBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });*/
            itemDisplays.Add("DrifterBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "BagBulgeRight",
                localPos = new Vector3(0.02959F, -0.19342F, 0.30996F),
                localAngles = new Vector3(331.3999F, 108.6055F, 68.72771F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            return itemDisplays;
        }
    }
}
