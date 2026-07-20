using ElementalReactionsMod.Elements;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ElementalReactionsMod.Items
{
    public static class Crafting
    {
        // Move to after item catalog or element catalog?
        public static CraftableDef delusionPyro;
        public static CraftableDef delusionHydro;
        public static CraftableDef delusionElectro;
        public static CraftableDef delusionCryo;
        public static CraftableDef delusionAnemo;
        public static CraftableDef delusionGeo;
        public static CraftableDef delusionDendro;
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
            delusionPyro = CreateDelusionConversion(DefaultElementDefs.pyroElement.delusion, 
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
            // SandsweptSupport adds Sun Fragment and Smouldering Document

            delusionHydro = CreateDelusionConversion(DefaultElementDefs.hydroElement.delusion,
                new RecipeIngredient() // Squid Polyp
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Squid.Squid_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                }, new RecipeIngredient() // Breaching Fin
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Items_KnockBackHitEnemies.KnockBackHitEnemies_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                });

            delusionElectro = CreateDelusionConversion(DefaultElementDefs.electroElement.delusion, 
                new RecipeIngredient() // Ukelele
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
            });
            // SS2Support adds Man-O-War

            delusionCryo = CreateDelusionConversion(DefaultElementDefs.cryoElement.delusion, 
                new RecipeIngredient() // Runald's Band
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_ElementalRings.IceRing_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                });

            delusionAnemo = CreateDelusionConversion(DefaultElementDefs.anemoElement.delusion,
                new RecipeIngredient() // Hopoo Feather
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Feather.Feather_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                });

            delusionGeo = CreateDelusionConversion(DefaultElementDefs.geoElement.delusion, 
            new RecipeIngredient() // Ghor's Tome
            {
                pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_BonusGoldPackOnKill.BonusGoldPackOnKill_asset).WaitForCompletion(),
                type = IngredientTypeIndex.AssetReference
            }, new RecipeIngredient() // Chance Doll
            {
                pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Items_ExtraShrineItem.ExtraShrineItem_asset).WaitForCompletion(),
                type = IngredientTypeIndex.AssetReference
            });
            // SandsweptSupport adds Crown's Diamond

            delusionDendro = CreateDelusionConversion(DefaultElementDefs.dendroElement.delusion,
                new RecipeIngredient() // Lepton Daisy
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_TPHealingNova.TPHealingNova_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                }, new RecipeIngredient() // Leeching Seed
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Seed.Seed_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                }, new RecipeIngredient() // Noxious Thorn
                {
                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Items_TriggerEnemyDebuffs.TriggerEnemyDebuffs_asset).WaitForCompletion(),
                    type = IngredientTypeIndex.AssetReference
                });
            #endregion
        }
        public static CraftableDef CreateDelusionConversion(ItemDef delusion, params RecipeIngredient[] recipeIngredient)
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
            return delusionCraft;
        }
        public static void AppendDelusionRecipe(CraftableDef delusion, ItemDef item)
        {
            if (!item) return;
            AppendDelusionRecipe(delusion, new RecipeIngredient()
            {
                pickup = item,
                type = IngredientTypeIndex.AssetReference
            });
        }
        public static void AppendDelusionRecipe(CraftableDef delusion, RecipeIngredient recipeIngredient)
        {
            delusion.recipes = delusion.recipes.Append(new Recipe()
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
                    recipeIngredient
                ]
            }).ToArray();
        }
    }
}
