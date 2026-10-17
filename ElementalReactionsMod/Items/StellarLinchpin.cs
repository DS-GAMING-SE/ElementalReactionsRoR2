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
    public static class StellarLinchpin
    {
        public static ItemDef stellarLinchpin;
        public static ItemDef hiddenStellarLinchpin;
        private static Material stellarLinchpinMat;

        public static void Initialize()
        {
            stellarLinchpinMat = Assets.CreateVisionMaterial(stellarLinchpinVisionIcon, stellarLinchpinVisionRamp, 1.7f, false);
            AssetAsyncReferenceManager<GameObject>.LoadAsset(stellarLinchpinPickupModel).Completed += x =>
            {
                x.Result.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = stellarLinchpinMat;
                AddModelPanelParameters(x.Result);
            };
            stellarLinchpin = AddNewItem("StellarLinchpin", "STELLAR_LINCHPIN", true,
                Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common.Tier3Def_asset).WaitForCompletion(),
                stellarLinchpinItemIcon, stellarLinchpinPickupModel, InitializeItemDisplays(), ItemTag.Damage, ItemTag.CanBeTemporary, ItemTag.AllowedForUseAsCraftingIngredient, ItemTag.DevotionBlacklist, ItemTag.CanBeRental);
            hiddenStellarLinchpin = AddNewItem("HiddenStellarLinchpin", "STELLAR_LINCHPIN", false, null,
                null, stellarLinchpinPickupModel, null, ItemTag.CannotSteal, ItemTag.CannotCopy, ItemTag.CannotDuplicate, ItemTag.WorldUnique, ItemTag.IgnoreForDropList);
        }
        public static ItemDisplayRuleDict InitializeItemDisplays()
        {
            GameObject displayPrefab = AssetAsyncReferenceManager<GameObject>.LoadAsset(stellarLinchpinDisplayModel).WaitForCompletion();
            displayPrefab.AddComponent<StellarLinchpinDisplay>();
            displayPrefab.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = stellarLinchpinMat;
            CreateItemDisplay(displayPrefab, CreateItemRendererInfo(displayPrefab, 0, Assets.visionMaterial), CreateItemRendererInfo(displayPrefab, 1, stellarLinchpinMat));
            ItemDisplayRuleDict itemDisplays = new ItemDisplayRuleDict();
            itemDisplays.Add("CommandoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.00197F, 0.36745F, -0.16818F),
                localAngles = new Vector3(328.7037F, 180.0638F, 359.7718F),
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
            });
            itemDisplays.Add("HuntressBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(0.15519F, -0.03767F, -0.0283F),
                localAngles = new Vector3(29.31321F, 117.586F, 199.8553F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F)
            });
            itemDisplays.Add("Bandit2Body", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.12552F, 0.27143F, 0.12847F),
                localAngles = new Vector3(335.6792F, 331.5812F, 358.2685F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F)
            });
            itemDisplays.Add("ToolbotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(2.43345F, 1.83926F, 2.6588F),
                localAngles = new Vector3(0F, 90F, 0F),
                localScale = new Vector3(5F, 5F, 5F)
            });
            itemDisplays.Add("EngiBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.08606F, 0.27935F, 0.25383F),
                localAngles = new Vector3(339.4455F, 8.59431F, 353.9696F),
                localScale = new Vector3(0.95F, 0.95F, 0.95F)
            });
            itemDisplays.Add("EngiTurretBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Head",
                localPos = new Vector3(-0.61612F, 0.535F, -1.31641F),
                localAngles = new Vector3(0F, 39.04362F, 0F),
                localScale = new Vector3(2.5F, 2.5F, 2.5F)
            });
            itemDisplays.Add("EngiWalkerTurretBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Head",
                localPos = new Vector3(0F, 1.25196F, 0.51917F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(2F, 2F, 2F)
            });
            itemDisplays.Add("MageBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.21887F, 0.2151F, -0.1934F),
                localAngles = new Vector3(355F, 90F, 9F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("MercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "LowerArmR",
                localPos = new Vector3(0.00603F, 0.1504F, -0.10984F),
                localAngles = new Vector3(4.82814F, 0.91842F, 173.788F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F)
            });
            itemDisplays.Add("TreebotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "PlatformBase",
                localPos = new Vector3(-0.01594F, 0.7192F, -0.84874F),
                localAngles = new Vector3(9.61085F, 67.62119F, 14.08854F),
                localScale = new Vector3(1.3F, 1.3F, 1.3F)
            });
            itemDisplays.Add("LoaderBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.2F, 0.28F, 0.28F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
            });
            itemDisplays.Add("CrocoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "LowerArmR",
                localPos = new Vector3(-0.89492F, 3.7274F, -0.77738F),
                localAngles = new Vector3(1.39027F, -313.4044F, -86.03119F),
                localScale = new Vector3(5.5F, 5.5F, 5.5F)

            });
            itemDisplays.Add("CaptainBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.00345F, 0.22107F, 0.21209F),
                localAngles = new Vector3(347.8674F, 5.93791F, 359.8032F),
                localScale = new Vector3(1F, 1F, 1F)

            });
            itemDisplays.Add("RailgunnerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Backpack",
                localPos = new Vector3(0.13122F, -0.27467F, -0.1178F),
                localAngles = new Vector3(359.8842F, 358.4265F, 359.2345F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F)
            });
            itemDisplays.Add("VoidSurvivorBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.12424F, 0.07869F, 0.18366F),
                localAngles = new Vector3(350.6365F, 342.6595F, 14.55495F),
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
            });
            itemDisplays.Add("SeekerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Pack",
                localPos = new Vector3(0.15904F, 0.01225F, -0.19406F),
                localAngles = new Vector3(40.70699F, 270.8703F, 10.12899F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            /*itemDisplays.Add("FalseSonBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.26695F, 0.18735F, 0.28028F),
                localAngles = new Vector3(333.2034F, 20.23438F, 335.0917F),
                localScale = new Vector3(1.2F, 1.2F, 1.2F)
            });
            itemDisplays.Add("ChefBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.0173F, -0.48905F, 0.33534F),
                localAngles = new Vector3(58.64297F, 2.619F, 83.89237F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("DroneTechBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Backpack",
                localPos = new Vector3(-0.23746F, 0.30876F, -0.22503F),
                localAngles = new Vector3(1.73688F, 8.40125F, 356.0705F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F)
            });
            itemDisplays.Add("DrifterBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "BagBulgeRight",
                localPos = new Vector3(0.02959F, -0.19342F, 0.30996F),
                localAngles = new Vector3(331.3999F, 108.6055F, 68.72771F),
                localScale = new Vector3(1F, 1F, 1F)
            });*/
            itemDisplays.Add("BrotherBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject("b06477748e168314abd4c861c4cbcd76"),
                childName = "ThighL",
                localPos = new Vector3(-0.08694F, 0.20859F, 0.10689F),
                localAngles = new Vector3(288.7786F, 131.1556F, 256.7206F),
                localScale = new Vector3(0.05F, 0.05F, 0.05F)
            });
            itemDisplays.Add("ScavBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Weapon",
                localPos = new Vector3(-0.06316F, 2.90545F, 3.78494F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(11F, 11F, 11F)
            });/*
            itemDisplays.Add("NemCommandoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.71209F, 0.88104F, -1.73658F),
                localAngles = new Vector3(358.8832F, 353.9355F, 0.15805F),
                localScale = new Vector3(3F, 3F, 3F)
            });
            itemDisplays.Add("NemMercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.15075F, 0.08836F, 0.13905F),
                localAngles = new Vector3(1.75716F, 34.06154F, 1.90495F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F)
            });
            itemDisplays.Add("Executioner2Body", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.13895F, 0.1785F, 0.158F),
                localAngles = new Vector3(351.9363F, 342.002F, 349.7007F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F)
            });
            itemDisplays.Add("ChirrBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(stellarLinchpinDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.49649F, 0.61212F, 0.24153F),
                localAngles = new Vector3(16.17279F, 282.3415F, 358.2969F),
                localScale = new Vector3(1.2F, 1.2F, 1.2F)
            });*/
            return itemDisplays;
        }

        public class StellarLinchpinBehaviour : RerollableItem
        {
            [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = true)]
            private static ItemDef GetItemDef()
            {
                return stellarLinchpin;
            }
            public GameObject stellarLinchpinDisplay;
            public override ItemDef item => stellarLinchpin;
            public override ReadOnlyList<PickupIndex> availableRerolls => Run.instance.availableTier3DropList;
            public override ElementDef coreElement => DefaultElementDefs.cryoElement;
            public override ElementDef[] reactingElements => [DefaultElementDefs.electroElement, DefaultElementDefs.anemoElement];
        }

        public class StellarLinchpinDisplay : MonoBehaviour
        {
            public StellarLinchpinBehaviour stellarLinchpin;
            private void Start()
            {
                CharacterModel characterModel = base.GetComponentInParent<CharacterModel>();
                if (characterModel && characterModel.body && characterModel.body.TryGetComponent<StellarLinchpinBehaviour>(out stellarLinchpin))
                {
                    stellarLinchpin.stellarLinchpinDisplay = gameObject;
                }
            }
        }
    }
}
