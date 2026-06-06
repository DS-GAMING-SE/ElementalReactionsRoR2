using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ElementalReactionsMod.Items
{
    public static class Crafting
    {
        public static void Initialize()
        {
            CraftableDef moonWheelFromDelusion = ScriptableObject.CreateInstance<CraftableDef>();
            (moonWheelFromDelusion as ScriptableObject).name = "cdElementalReactionsMoonWheel";
            moonWheelFromDelusion.pickup = Items.moonWheel;
            moonWheelFromDelusion.recipes = new Recipe[]
            {
                new Recipe()
                {
                    ingredients = new RecipeIngredient[]
                    {
                        new RecipeIngredient()
                        {
                            requiredTags = [DelusionManager.delusionItemTag],
                            itemTier = ItemTier.Lunar,
                            type = IngredientTypeIndex.AnyItem,
                            forbiddenTags = []
                        },
                        new RecipeIngredient()
                        {
                            pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_ShinyPearl.ShinyPearl_asset).WaitForCompletion(),
                            type = IngredientTypeIndex.AssetReference
                        }
                    }
                }
            };
            Content.AddCraftableDef(moonWheelFromDelusion);
            /* Delusion Element Converting Ideas
             * - Pyro = Kjaro's Band, Ignition Tank, Will o Wisp
             * - Hydro = Squid Pog?
             * - Electro = Luminous Shot, Man o War SS2
             * - Cryo = Runald's Band
             * - Anemo = Hopoo Feather
             * - Geo = Ghor's Tome?
             * - Dendro = Lepton Daisy
             */
        }
    }
}
