using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.XR;
using static ElementalReactionsMod.Assets.AssetReferences;
using static ElementalReactionsMod.Materials;

namespace ElementalReactionsMod.Items
{
    public static class Items
    {
        public static ItemDef delusion;
        public static ItemDef instructorsTeaCup;
        public static ItemDef moonWheel => MoonWheel.moonWheel;

        public static void Initialize()
        {
            instructorsTeaCup = AddNewItem("InstructorsTeaCup", "INSTRUCTORS_TEA_CUP", true,
                Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.Tier1Def_asset).WaitForCompletion(),
                instructorsTeaCupItemIcon.LoadAssetAsync<Sprite>().WaitForCompletion(), instructorsTeaCupPickupModel, null, ItemTag.Damage, ItemTag.CanBeTemporary);

            MoonWheel.Initialize();

            CharacterBody.onBodyInventoryChangedGlobal += AddItemBehaviours;
        }
        public static void AddItemBehaviours(CharacterBody characterBody)
        {
            // Delusions
            List<ElementDef> list = characterBody.inventory.GetDelusions(out int count);
            DelusionBehaviour delusionBehaviour = characterBody.AddItemBehavior<DelusionBehaviour>(count);
            if (delusionBehaviour)
            {
                delusionBehaviour.delusionElements = list;
            }

            // Instructor's Tea Cup Quality
            if (ElementalReactionsPlugin.qualityModExists)
            {
                InstructorsTeaCupQuality teaCupBehaviour = characterBody.AddItemBehavior<InstructorsTeaCupQuality>(characterBody.inventory.GetItemCountQualities(instructorsTeaCup, out int uncommon, out int rare, out int epic, out int legendary));
                if (teaCupBehaviour)
                {
                    teaCupBehaviour.uncommonItemCount = uncommon;
                    teaCupBehaviour.rareItemCount = rare;
                    teaCupBehaviour.epicItemCount = epic;
                    teaCupBehaviour.legendaryItemCount = legendary;
                }
            }
        }
        internal static GameObject AddModelPanelParameters(GameObject item)
        {
            ModelPanelParameters panel = item.AddComponent<ModelPanelParameters>();
            panel.focusPointTransform = item.transform.Find("FocusPoint");

            panel.cameraPositionTransform = item.transform.Find("FocusPoint/CameraPosition");

            panel.minDistance = 4f;
            panel.maxDistance = 6f;
            return item;
        }
        internal static CharacterModel.RendererInfo CreateItemRendererInfo(GameObject displayPrefab, int child, Material material)
        {
            return new CharacterModel.RendererInfo
            {
                renderer = displayPrefab.transform.GetChild(child).GetComponent<MeshRenderer>(),
                defaultMaterial = material,
            };
        }
        internal static CharacterModel.RendererInfo CreateItemRendererInfo(GameObject displayPrefab, int child, AssetReferenceT<Material> materialReference)
        {
            return new CharacterModel.RendererInfo
            {
                renderer = displayPrefab.transform.GetChild(child).GetComponent<MeshRenderer>(),
                defaultMaterialAddress = materialReference
            };
        }
        internal static GameObject CreateItemDisplay(GameObject displayPrefab, params CharacterModel.RendererInfo[] rendererInfos)
        {
            ItemDisplay itemDisplay = displayPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = rendererInfos;
            return displayPrefab;
        }
        internal static ItemDef AddNewItem(string itemName, string token, bool canRemove, ItemTierDef itemTierDef, Sprite icon, AssetReferenceT<GameObject> pickupModelReference, ItemDisplayRuleDict itemDisplay, params ItemTag[] tags)
        {
            string prefix = $"{ElementalReactionsPlugin.PREFIX}ITEM_";
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = "ElementalReactions"+itemName;
            itemDef._itemTierDef = itemTierDef;
            itemDef.pickupModelReference = pickupModelReference;
            itemDef.pickupIconSprite = icon;
            itemDef.canRemove = canRemove;
            itemDef.hidden = !canRemove;
            itemDef.requiredExpansion = Assets.elementalReactionExpansionDef;

            itemDef.nameToken = prefix + token+"_NAME"; // stylised name
            itemDef.pickupToken = prefix + token + "_PICKUP";
            itemDef.descriptionToken = prefix + token + "_DESCRIPTION";
            itemDef.loreToken = prefix + token + "_LORE";
            itemDef.tags = tags;

            ItemAPI.Add(new CustomItem(itemDef, itemDisplay));

            return itemDef;
        }
    }
}
