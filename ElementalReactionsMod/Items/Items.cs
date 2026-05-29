using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ElementalReactionsMod.Assets.AssetReferences;
using static ElementalReactionsMod.Materials;

namespace ElementalReactionsMod.Items
{
    public static class Items
    {
        public static ItemDef delusion;
        public static ItemDef instructorsTeaCup;

        public static void Initialize()
        {
            instructorsTeaCup = AddNewItem("InstructorsTeaCup", "INSTRUCTORS_TEA_CUP", true,
                Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.Tier1Def_asset).WaitForCompletion(),
                instructorsTeaCupItemIcon.LoadAssetAsync<Sprite>().WaitForCompletion(), instructorsTeaCupPickupModel, ItemTag.Damage, ItemTag.CanBeTemporary);
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
        internal static ItemDef AddNewItem(string itemName, string token, bool canRemove, ItemTierDef itemTierDef, Sprite icon, AssetReferenceT<GameObject> pickupModelReference, params ItemTag[] tags)
        {
            string prefix = $"{ElementalReactionsPlugin.PREFIX}ITEM_";
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = "ElementalReactions"+itemName;
            itemDef._itemTierDef = itemTierDef;
            itemDef.pickupModelReference = pickupModelReference;
            itemDef.pickupIconSprite = icon;
            itemDef.canRemove = canRemove;
            itemDef.requiredExpansion = Assets.elementalReactionExpansionDef;

            itemDef.nameToken = prefix + token+"_NAME"; // stylised name
            itemDef.pickupToken = prefix + token + "_PICKUP";
            itemDef.descriptionToken = prefix + token + "_DESCRIPTION";
            itemDef.loreToken = prefix + token + "_LORE";
            itemDef.tags = tags;

            Content.AddItemDef(itemDef);

            return itemDef;
        }
    }
}
