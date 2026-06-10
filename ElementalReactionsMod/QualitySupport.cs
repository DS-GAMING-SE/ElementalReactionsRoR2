using BepInEx.Bootstrap;
using ElementalReactionsMod.Items;
using RoR2;
using ItemQualities;
using ItemQualities.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using System.Linq;

namespace ElementalReactionsMod
{
    // Thank you Faithful mod
    internal static class QualitySupport
    {
        public const string qualityIcon = "<sprite name=\"Quality\">";

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void Initialize()
        {
            QualityContentManager.LoadContentAsync += LoadQuality;
        }

        public static IEnumerator LoadQuality(QualityContentLoadArgs args)
        {
            args.CreateItemQualityGroup(Items.Items.instructorsTeaCup);
            args.CreateBuffQualityGroup(Buffs.instructorsTeaCupQualityBase);

            args.CreateItemQualityGroup(Items.Items.moonWheel);


            yield break;
        }

        public static void AddQualityLanguage(string key, bool isPickup, string value, bool appendToOriginalDescription = false, params object[] args)
        {
            if (!ElementalReactionsPlugin.qualityModExists) return;
            int argsPerQuality = args.Length / 4;
            for (int i=0;i<4;i++)
            {
                string description = args.Length > 0 ? String.Format(value, args.Skip(i*(argsPerQuality)).Take(argsPerQuality).ToArray()) : value;
                string qualityName;
                switch(i)
                {
                    case 0:
                        qualityName = "UNCOMMON";
                        break;
                    case 1:
                        qualityName = "RARE";
                        break;
                    case 2:
                        qualityName = "EPIC";
                        break;
                    case 3:
                        qualityName = "LEGENDARY";
                        break;
                    default:
                        qualityName = "";
                        break;
                }
                LanguageAPI.Add($"ITEM_ELEMENTALREACTIONS{key.Replace("_", "")}_{qualityName}_{(isPickup ? "PICKUP" : "DESC")}", appendToOriginalDescription ? Language.GetString($"{ElementalReactionsPlugin.PREFIX}ITEM_{key}_{(isPickup ? "PICKUP" : "DESCRIPTION")}") + " " + description : description);
            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static int GetItemCountTotalQuality(this Inventory inventory, ItemDef item)
        {
            ItemQualityGroupIndex index = QualityCatalog.FindItemQualityGroupIndex(item.itemIndex);
            if (index != ItemQualityGroupIndex.Invalid)
            {
                return ItemQualities.Utilities.Extensions.InventoryExtensions.GetItemCountsEffective(inventory, index).TotalCount;
            }
            return inventory.GetItemCountEffective(item);
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static int GetItemCountQualities(this Inventory inventory, ItemDef item, out int uncommon, out int rare, out int epic, out int legendary)
        {
            uncommon = 0; rare = 0; epic = 0; legendary = 0;
            ItemQualityGroupIndex index = QualityCatalog.FindItemQualityGroupIndex(item.itemIndex);
            if (index != ItemQualityGroupIndex.Invalid)
            {
                ItemQualityCounts itemQualityCounts = ItemQualities.Utilities.Extensions.InventoryExtensions.GetItemCountsEffective(inventory, index);
                uncommon = itemQualityCounts.UncommonCount;
                rare = itemQualityCounts.RareCount;
                epic = itemQualityCounts.EpicCount;
                legendary = itemQualityCounts.LegendaryCount;
                return itemQualityCounts.TotalQualityCount;
            }
            return 0;
        }
        public static int GetWeightedQualityItemCount(int uncommon, int rare, int epic, int legendary)
        {
            return uncommon + (rare * 2) + (epic * 3) + (legendary * 4);
        }
    }
}
