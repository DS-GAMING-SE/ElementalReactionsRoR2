using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using RoR2;
using Sandswept;
using ElementalReactionsMod.Items;

namespace ElementalReactionsMod
{
    public static class SandsweptSupport
    {
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void Initialize()
        {
            PickupCatalog.availability.onAvailable += () =>
            {
                Crafting.AppendDelusionRecipe(Crafting.delusionPyro, ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("ITEM_SANDSWEPT_SUN_FRAGMENT")));

                Crafting.AppendDelusionRecipe(Crafting.delusionPyro, ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("ITEM_SANDSWEPT_SMOULDERING_DOCUMENT")));

                Crafting.AppendDelusionRecipe(Crafting.delusionGeo, ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("ITEM_SANDSWEPT_CROWNS_DIAMOND")));
            };
        }
    }
}
