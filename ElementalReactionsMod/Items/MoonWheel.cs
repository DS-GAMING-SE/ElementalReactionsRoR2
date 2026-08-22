using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using HG;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Items;
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
        public static ItemDef hiddenMoonWheel;
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
            moonWheel.unlockableDef = Achievements.Unlockables.elementalMasteryUnlockableDef;
            hiddenMoonWheel = AddNewItem("HiddenMoonWheel", "MOON_WHEEL", false, null,
                null, moonWheelPickupModel, null, ItemTag.CannotSteal, ItemTag.CannotCopy, ItemTag.CannotDuplicate, ItemTag.WorldUnique, ItemTag.IgnoreForDropList);
        }
        public static ItemDisplayRuleDict InitializeItemDisplays()
        {
            GameObject displayPrefab = AssetAsyncReferenceManager<GameObject>.LoadAsset(moonWheelDisplayModel).WaitForCompletion();
            displayPrefab.AddComponent<MoonWheelDisplay>();
            displayPrefab.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = moonWheelMat;
            CreateItemDisplay(displayPrefab, CreateItemRendererInfo(displayPrefab, 0, Assets.visionMaterial), CreateItemRendererInfo(displayPrefab, 1, moonWheelMat));
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
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
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
            itemDisplays.Add("EngiBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.45F, 0.48687F, -0.3F),
                localAngles = new Vector3(0F, 87F, 0F),
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
            });
            itemDisplays.Add("EngiTurretBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Head",
                localPos = new Vector3(-0.61612F, 0.59652F, -1.31641F),
                localAngles = new Vector3(0F, 39.04362F, 0F),
                localScale = new Vector3(2.5F, 2.5F, 2.5F)
            });
            itemDisplays.Add("EngiWalkerTurretBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Neck",
                localPos = new Vector3(-0.7483F, 0.65663F, 0F),
                localAngles = new Vector3(0F, 90F, 0F),
                localScale = new Vector3(2.5F, 2.5F, 2.5F)
            });
            itemDisplays.Add("MageBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.21887F, 0.2151F, -0.1934F),
                localAngles = new Vector3(355F, 90F, 9F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("MercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "LowerArmR",
                localPos = new Vector3(0.00603F, 0.16364F, -0.10984F),
                localAngles = new Vector3(4.82814F, 0.91842F, 173.788F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F)
            });
            itemDisplays.Add("TreebotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "HeadBase",
                localPos = new Vector3(-0.19751F, -0.42146F, 0.72772F),
                localAngles = new Vector3(39.14164F, 349.3906F, 172.4279F),
                localScale = new Vector3(1.3F, 1.3F, 1.3F)
            });
            itemDisplays.Add("LoaderBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.2F, 0.28F, 0.28F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
            });
            itemDisplays.Add("CrocoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "SpineStomach1",
                localPos = new Vector3(1.14943F, 0.2214F, 1.60011F),
                localAngles = new Vector3(313.6886F, 121.8416F, 262.7798F),
                localScale = new Vector3(5.5F, 5.5F, 5.5F)

            });
            itemDisplays.Add("CaptainBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Head",
                localPos = new Vector3(0.00498F, 0.27717F, 0.14714F),
                localAngles = new Vector3(357.0426F, 359.8333F, 359.709F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F)
            });
            itemDisplays.Add("RailgunnerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Backpack",
                localPos = new Vector3(-0.24508F, 0.12649F, -0.08851F),
                localAngles = new Vector3(0F, 88F, 0F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F)
            });
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

            });
            itemDisplays.Add("FalseSonBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.26695F, 0.18735F, 0.28028F),
                localAngles = new Vector3(333.2034F, 20.23438F, 335.0917F),
                localScale = new Vector3(1.2F, 1.2F, 1.2F)
            });
            itemDisplays.Add("ChefBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.0173F, -0.48905F, 0.33534F),
                localAngles = new Vector3(58.64297F, 2.619F, 83.89237F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("DroneTechBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Backpack",
                localPos = new Vector3(-0.23746F, 0.30876F, -0.22503F),
                localAngles = new Vector3(1.73688F, 8.40125F, 356.0705F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F)
            });
            itemDisplays.Add("DrifterBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "BagBulgeRight",
                localPos = new Vector3(0.02959F, -0.19342F, 0.30996F),
                localAngles = new Vector3(331.3999F, 108.6055F, 68.72771F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("BrotherBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject("b06477748e168314abd4c861c4cbcd76"),
                childName = "Head",
                localPos = new Vector3(0.01749F, 0.07662F, -0.09656F),
                localAngles = new Vector3(17.45016F, 252.3006F, 324.7871F),
                localScale = new Vector3(0.08F, 0.08F, 0.08F)
            });
            itemDisplays.Add("ScavBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Backpack",
                localPos = new Vector3(-8.30025F, 4.75887F, -0.11149F),
                localAngles = new Vector3(21.86793F, 78.82794F, 354.8484F),
                localScale = new Vector3(10F, 10F, 10F)
            });
            itemDisplays.Add("NemCommandoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.71209F, 0.88104F, -1.73658F),
                localAngles = new Vector3(358.8832F, 353.9355F, 0.15805F),
                localScale = new Vector3(3F, 3F, 3F)
            });
            itemDisplays.Add("NemMercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.15075F, 0.08836F, 0.13905F),
                localAngles = new Vector3(1.75716F, 34.06154F, 1.90495F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F)
            });
            itemDisplays.Add("Executioner2Body", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.13895F, 0.1785F, 0.158F),
                localAngles = new Vector3(351.9363F, 342.002F, 349.7007F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F)
            });
            itemDisplays.Add("ChirrBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(moonWheelDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.49649F, 0.61212F, 0.24153F),
                localAngles = new Vector3(16.17279F, 282.3415F, 358.2969F),
                localScale = new Vector3(1.2F, 1.2F, 1.2F)
            });
            return itemDisplays;
        }

        public class MoonWheelBehaviour : RerollableItem
        {
            [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = true)]
            private static ItemDef GetItemDef()
            {
                return moonWheel;
            }
            public GameObject moonWheelDisplay;
            public override ItemDef item => moonWheel;
            public override ReadOnlyList<PickupIndex> availableRerolls => Run.instance.availableTier3DropList;
            public override ElementDef coreElement => DefaultElementDefs.hydroElement;
            public override ElementDef[] reactingElements => [DefaultElementDefs.electroElement, DefaultElementDefs.geoElement, DefaultElementDefs.dendroElement];
        }

        public class MoonWheelDisplay : MonoBehaviour
        {
            public MoonWheelBehaviour moonWheel;
            private void Start()
            {
                CharacterModel characterModel = base.GetComponentInParent<CharacterModel>();
                if (characterModel && characterModel.body && characterModel.body.TryGetComponent<MoonWheelBehaviour>(out moonWheel))
                {
                    moonWheel.moonWheelDisplay = gameObject;
                }
            }
        }
    }
}
