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
        public static ItemDef delusionPyro;
        public static ItemDef delusionHydro;
        public static ItemDef delusionElectro;
        public static ItemDef delusionCryo;
        public static ItemDef delusionAnemo;
        public static ItemDef delusionGeo;
        public static ItemDef delusionDendro;
        public static ItemDef[] elementalDelusions;

        public static void Initialize()
        {
            AssetAsyncReferenceManager<Material>.LoadAsset(visionMaterial).Completed += x =>
            {
                x.Result.SetHopooMaterial().Specular(0.4f, 3f);
                x.Result.SetNormal(3f);
            };

            delusion = AddNewItem("Delusion", "DELUSION", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                null, delusionPickupModel, ItemTag.Damage);
            delusionPyro = AddNewItem("DelusionPyro", "DELUSION_PYRO", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                null, delusionPickupModel, ItemTag.Damage, ItemTag.WorldUnique);
            delusionHydro = AddNewItem("DelusionHydro", "DELUSION_HYDRO", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                null, delusionPickupModel, ItemTag.Damage, ItemTag.WorldUnique);
            delusionElectro = AddNewItem("DelusionElectro", "DELUSION_ELECTRO", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                null, delusionPickupModel, ItemTag.Damage, ItemTag.WorldUnique);
            delusionCryo = AddNewItem("DelusionCryo", "DELUSION_CRYO", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                null, delusionPickupModel, ItemTag.Damage, ItemTag.WorldUnique);
            delusionAnemo = AddNewItem("DelusionAnemo", "DELUSION_ANEMO", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                null, delusionPickupModel, ItemTag.Damage, ItemTag.WorldUnique);
            delusionGeo = AddNewItem("DelusionGeo", "DELUSION_GEO", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                null, delusionPickupModel, ItemTag.Damage, ItemTag.WorldUnique);
            delusionDendro = AddNewItem("DelusionDendro", "DELUSION_DENDRO", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                null, delusionPickupModel, ItemTag.Damage, ItemTag.WorldUnique);
            NoElementDelusion.Initialize();

            elementalDelusions = [delusionPyro, delusionHydro, delusionElectro, delusionCryo, delusionAnemo, delusionGeo, delusionDendro];
        }
        internal static GameObject ModelPanelParameters(GameObject item)
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
