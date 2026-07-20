using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using ItemQualities;
using ItemQualities.Utilities.Extensions;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ElementalReactionsMod.Assets.AssetReferences;
using static ElementalReactionsMod.Items.Items;
using static ElementalReactionsMod.Util;

namespace ElementalReactionsMod.Items
{
    public static class InstructorsTeaCup
    {
        public static ItemDef instructorsTeaCup;
        public static void Initialize()
        {
            instructorsTeaCup = AddNewItem("InstructorsTeaCup", "INSTRUCTORS_TEA_CUP", true,
                Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.Tier1Def_asset).WaitForCompletion(),
                instructorsTeaCupItemIcon.LoadAssetAsync<Sprite>().WaitForCompletion(), instructorsTeaCupPickupModel, InitializeItemDisplays(), ItemTag.Damage, ItemTag.CanBeTemporary);
        }
        public static ItemDisplayRuleDict InitializeItemDisplays()
        {
            GameObject displayPrefab = AssetAsyncReferenceManager<GameObject>.LoadAsset(instructorsTeaCupDisplayModel).WaitForCompletion();
            CreateItemDisplay(displayPrefab, CreateItemRendererInfo(displayPrefab, 0, instructorsTeaCupMaterial));
            ItemDisplayRuleDict itemDisplays = new ItemDisplayRuleDict();
            itemDisplays.Add("CommandoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(0.19491F, -0.05331F, 0.07828F),
                localAngles = new Vector3(9.42617F, 149.9708F, 155.2179F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("HuntressBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(-0.13932F, -0.0301F, -0.09815F),
                localAngles = new Vector3(14.20561F, 335.1174F, 140.3133F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("Bandit2Body", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(0.20266F, 0.02811F, -0.13093F),
                localAngles = new Vector3(348.1317F, 229.6773F, 146.8423F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("ToolbotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Hip",
                localPos = new Vector3(0.84022F, 2.73873F, 0.6803F),
                localAngles = new Vector3(0.00002F, 0.00003F, 89.26896F),
                localScale = new Vector3(6.3F, 6.3F, 6.3F)
            });
            itemDisplays.Add("EngiBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(-0.21797F, 0.15683F, 0.1464F),
                localAngles = new Vector3(358.3696F, 41.82095F, 119.6954F),
                localScale = new Vector3(1.3F, 1.3F, 1.3F)
            });
            itemDisplays.Add("EngiTurretBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "HeadCenter",
                localPos = new Vector3(-0.24876F, 0.19378F, -1.5768F),
                localAngles = new Vector3(14.18015F, 105.1516F, 329.6412F),
                localScale = new Vector3(3.4F, 3.4F, 3.4F)
            });
            itemDisplays.Add("EngiWalkerTurretBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Head",
                localPos = new Vector3(-0.19885F, 1.41107F, -1.62046F),
                localAngles = new Vector3(5.86609F, 99.17147F, 350.9476F),
                localScale = new Vector3(3.4F, 3.4F, 3.4F)
            });
            itemDisplays.Add("MageBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(-0.20679F, -0.06411F, 0.03039F),
                localAngles = new Vector3(19.81575F, 14.03523F, 164.8867F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("MercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(-0.2226F, 0.08986F, 0.04772F),
                localAngles = new Vector3(349.0326F, 28.7613F, 147.3924F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("TreebotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "PlatformBase",
                localPos = new Vector3(-0.63247F, 0.4062F, 0.40208F),
                localAngles = new Vector3(358.5712F, 208.6945F, 297.8586F),
                localScale = new Vector3(1.8F, 1.8F, 1.8F)
            });
            itemDisplays.Add("LoaderBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.26313F, -0.03259F, 0.06062F),
                localAngles = new Vector3(0.50537F, 357.1437F, 26.99998F),
                localScale = new Vector3(1.1F, 1.1F, 1.1F)
            });
            itemDisplays.Add("CrocoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "SpineChest1",
                localPos = new Vector3(-1.56606F, 0.59469F, -0.2076F),
                localAngles = new Vector3(289.1052F, 349.0121F, 248.9141F),
                localScale = new Vector3(8F, 8F, 8F)
            });
            itemDisplays.Add("CaptainBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(-0.18504F, -0.07401F, -0.13251F),
                localAngles = new Vector3(4.40974F, 331.9464F, 128.888F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("RailgunnerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Backpack",
                localPos = new Vector3(0.30733F, -0.53248F, 0.03276F),
                localAngles = new Vector3(358.6321F, 357.5471F, 272.1167F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("VoidSurvivorBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.16324F, 0.13798F, -0.27632F),
                localAngles = new Vector3(15.04345F, 9.1204F, 291.9918F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("SeekerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(-0.30547F, -0.04314F, -0.01677F),
                localAngles = new Vector3(296.6968F, 200.5731F, 338.2218F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("FalseSonBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.21694F, -0.22017F, -0.50652F),
                localAngles = new Vector3(347.4637F, 344.0567F, 273.1992F),
                localScale = new Vector3(1.6F, 1.6F, 1.6F)
            });
            itemDisplays.Add("ChefBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.33505F, -0.2963F, -0.09813F),
                localAngles = new Vector3(287.0714F, 56.76455F, 310.3874F),
                localScale = new Vector3(1.55F, 1.55F, 1.55F)
            });
            itemDisplays.Add("DroneTechBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Backpack",
                localPos = new Vector3(-0.20796F, -0.35189F, -0.09145F),
                localAngles = new Vector3(13.74731F, 86.6589F, 275.9486F),
                localScale = new Vector3(1.2F, 1.2F, 1.2F)
            });
            itemDisplays.Add("DrifterBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "BagBulgeRight",
                localPos = new Vector3(0.02959F, -0.19342F, 0.30996F),
                localAngles = new Vector3(331.3999F, 108.6055F, 68.72771F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("BrotherBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject("a8dc7aa7f61d18e4db92a79611b96f90"),
                childName = "chest",
                localPos = new Vector3(-0.10737F, -0.07244F, 0.03244F),
                localAngles = new Vector3(357.3963F, 25.96571F, 331.5166F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            itemDisplays.Add("ScavBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(9.56316F, 0.82885F, 6.07451F),
                localAngles = new Vector3(27.9713F, 262.4429F, 246.5358F),
                localScale = new Vector3(12F, 12F, 12F)
            });
            itemDisplays.Add("NemCommandoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(1.17536F, 0.17947F, 0.19889F),
                localAngles = new Vector3(358.0197F, 174.0244F, 143.3098F),
                localScale = new Vector3(5F, 5F, 5F)
            });
            itemDisplays.Add("NemMercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(0.05588F, 0.03317F, 0.18721F),
                localAngles = new Vector3(4.37816F, 98.56442F, 142.2975F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("Executioner2Body", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(-0.20708F, 0.06335F, 0.12973F),
                localAngles = new Vector3(2.40916F, 31.61024F, 144.5665F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("ChirrBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(instructorsTeaCupDisplayModel.AssetGUID),
                childName = "Stomach",
                localPos = new Vector3(0.16217F, -0.10737F, -0.68958F),
                localAngles = new Vector3(6.17634F, 64.66472F, 325.5513F),
                localScale = new Vector3(3.4F, 3.4F, 3.4F)
            });
            return itemDisplays;
        }
    }

    public class InstructorsTeaCupQuality : CharacterBody.ItemBehavior
    {
        public int uncommonItemCount;
        public int rareItemCount;
        public int epicItemCount;
        public int legendaryItemCount;

        private void Start()
        {
            ElementalReactionManager.onElementalReactionTriggered += OnElementalReaction;
        }

        private void OnElementalReaction(ElementalReactionDef reaction, ElementDef element1, ElementDef element2, CharacterBody victim, GameObject attacker)
        {
            if (attacker && attacker == gameObject && body)
            {
                QualityTier highestQuality = QualityTier.None;
                if (legendaryItemCount > 0) highestQuality = QualityTier.Legendary;
                else if (epicItemCount > 0) highestQuality = QualityTier.Epic;
                else if (rareItemCount > 0) highestQuality = QualityTier.Rare;
                else if (uncommonItemCount > 0) highestQuality = QualityTier.Uncommon;
                if (highestQuality != QualityTier.None)
                {
                    BuffDef buff = BuffCatalog.GetBuffDef(QualityCatalog.GetBuffIndexOfQuality(Buffs.instructorsTeaCupQualityBase.buffIndex, highestQuality));
                    body.ConvertQualityBuffsToTier(QualityCatalog.FindBuffQualityGroupIndex(Buffs.instructorsTeaCupQualityBase.buffIndex), highestQuality);
                    body.AddTimedBuff(buff,
                        StaticValues.instructorsTeaCupQualityDuration + (StaticValues.instructorsTeaCupQualityDurationPerQuality * (float)highestQuality),
                        StaticValues.instructorsTeaCupQualityMaxStacks + (StaticValues.instructorsTeaCupQualityStacksPerQuality * (int)highestQuality));
                    body.SetTimedBuffDurationIfPresent(buff, StaticValues.instructorsTeaCupQualityDuration + (StaticValues.instructorsTeaCupQualityDurationPerQuality * (float)highestQuality), true);
                }
            }
        }

        private void OnDisable()
        {
            if (body)
            {
                BuffQualityGroup buffs = QualityCatalog.GetBuffQualityGroup(QualityCatalog.FindBuffQualityGroupIndex(Buffs.instructorsTeaCupQualityBase.buffIndex));
                if (buffs)
                {
                    body.RemoveAllQualityBuffs(buffs);
                }
            }
            ElementalReactionManager.onElementalReactionTriggered -= OnElementalReaction;
        }
    }
}
