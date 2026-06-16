using ElementalReactionsMod.Elements;
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
        // Move to after item catalog or element catalog?
        public static void Initialize()
        {
            CraftableDef moonWheel = ScriptableObject.CreateInstance<CraftableDef>();
            (moonWheel as ScriptableObject).name = "cdElementalReactionsMoonWheel";
            moonWheel.pickup = Items.moonWheel;
            moonWheel.recipes = new Recipe[]
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
            Content.AddCraftableDef(moonWheel);
            CraftableDef instructorsTeaCup = ScriptableObject.CreateInstance<CraftableDef>();
            (instructorsTeaCup as ScriptableObject).name = "cdElementalReactionsInstructorsTeaCup";
            instructorsTeaCup.pickup = Items.instructorsTeaCup;
            instructorsTeaCup.recipes =
            [
                new()
                {
                    amountToDrop = 4,
                    ingredients =
                    [
                        new RecipeIngredient()
                        {
                            pickup = Items.moonWheel,
                            type = IngredientTypeIndex.AssetReference
                        },
                        new RecipeIngredient()
                        {
                            pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Scrap.ScrapWhite_asset).WaitForCompletion(),
                            type = IngredientTypeIndex.AssetReference
                        }
                    ]
                }
            ];
            Content.AddCraftableDef(instructorsTeaCup);
            #region Delusion Conversion
            /* Delusion Element Converting Ideas
             * - Pyro = Kjaro's Band, Ignition Tank, Will o Wisp
             * - Hydro = Squid Pog? Breaching Fin?
             * - Electro = Ukelele, Luminous Shot, Faraday Spur, Man o War SS2
             * - Cryo = Runald's Band
             * - Anemo = Hopoo Feather
             * - Geo = Ghor's Tome, Crown's Diamond Sandswept
             * - Dendro = Lepton Daisy, Leeching Seed
             * - Uncommon Scrap returns default Delusion, allowing for reroll?
             */
            CreateDelusionConversion(DefaultElementDefs.pyroElement.delusion, 
                new RecipeIngredient() // Kjaro's Band
            {
                pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_ElementalRings.FireRing_asset).WaitForCompletion(),
                type = IngredientTypeIndex.AssetReference
            }, new RecipeIngredient() // Will-o-Wisp
            {
                pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_ExplodeOnDeath.ExplodeOnDeath_asset).WaitForCompletion(),
                type = IngredientTypeIndex.AssetReference
            }, new RecipeIngredient() // Ignition Tank
            {
                pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC1_StrengthenBurn.StrengthenBurn_asset).WaitForCompletion(),
                type = IngredientTypeIndex.AssetReference
            });
            #endregion
        }
        public static void CreateDelusionConversion(ItemDef delusion, params RecipeIngredient[] recipeIngredient)
        {
            CraftableDef delusionCraft = ScriptableObject.CreateInstance<CraftableDef>();
            (delusionCraft as ScriptableObject).name = "cdElementalReactionsDelusion" + DelusionManager.delusionToElement[delusion].cachedName;
            delusionCraft.pickup = delusion;
            delusionCraft.recipes = new Recipe[recipeIngredient.Length];
            for (int i = 0; i < recipeIngredient.Length; i++)
            {
                delusionCraft.recipes[i] = new Recipe()
                {
                    ingredients =
                    [
                        new RecipeIngredient()
                        {
                            requiredTags = [DelusionManager.delusionItemTag],
                            itemTier = ItemTier.Lunar,
                            type = IngredientTypeIndex.AnyItem,
                            forbiddenTags = []
                        },
                        recipeIngredient[i]
                    ]
                };
            }
            Content.AddCraftableDef(delusionCraft);
        }
    }
}
