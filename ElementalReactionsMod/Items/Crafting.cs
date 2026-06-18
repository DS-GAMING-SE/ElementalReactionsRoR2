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

            // ADD PROJECT REF AND SOFT DEP TO SS2 AND SANDSWEPT TO ADD THEM TO RECIPES
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

            CreateDelusionConversion(DefaultElementDefs.hydroElement.delusion,
                new RecipeIngredient() // Squid Polyp
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Squid.Squid_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                }, new RecipeIngredient() // Breaching Fin
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Items_KnockBackHitEnemies.KnockBackHitEnemies_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                });
            List<RecipeIngredient> electroRecipes = [new RecipeIngredient() // Ukelele
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_ChainLightning.ChainLightning_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                }, new RecipeIngredient() // Luminous Shot
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Items_IncreasePrimaryDamage.IncreasePrimaryDamage_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                }, new RecipeIngredient() // Faraday Spur
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC3_Items_JumpDamageStrike.JumpDamageStrike_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                }];
            /*if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm"))
            {
                ItemDef manOWarSS2 = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("LightningOnKill"));
                if (manOWarSS2)
                {
                    electroRecipes.Add(new RecipeIngredient() // Man-O-War
                    {
                        pickup = manOWarSS2,
                        type = IngredientTypeIndex.AssetReference
                    });
                }
            }*/
            CreateDelusionConversion(DefaultElementDefs.electroElement.delusion, electroRecipes.ToArray());

            CreateDelusionConversion(DefaultElementDefs.cryoElement.delusion, 
                new RecipeIngredient() // Runald's Band
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_ElementalRings.IceRing_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                });

            CreateDelusionConversion(DefaultElementDefs.anemoElement.delusion,
                new RecipeIngredient() // Hopoo Feather
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Feather.Feather_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                });

            List<RecipeIngredient> geoRecipes = [new RecipeIngredient() // Ukelele
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_BonusGoldPackOnKill.BonusGoldPackOnKill_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                }];
            /*if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamSandswept.Sandswept"))
            {
                ItemDef crownsDiamond = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("ITEM_SANDSWEPT_CROWNS_DIAMOND"));
                if (crownsDiamond)
                {
                    geoRecipes.Add(new RecipeIngredient() // Crown's Diamond
                    {
                        pickup = crownsDiamond,
                        type = IngredientTypeIndex.AssetReference
                    });
                }
            }*/
            CreateDelusionConversion(DefaultElementDefs.geoElement.delusion, geoRecipes.ToArray());

            CreateDelusionConversion(DefaultElementDefs.dendroElement.delusion,
                new RecipeIngredient() // Lepton Daisy
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_TPHealingNova.TPHealingNova_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                }, new RecipeIngredient() // Leeching Seed
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Seed.Seed_asset).WaitForCompletion(),
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
