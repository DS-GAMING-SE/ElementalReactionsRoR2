using ElementalReactionsMod.Items;
using LookingGlass;
using LookingGlass.ItemStatsNameSpace;
using LookingGlass.LookingGlassLanguage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static ElementalReactionsMod.StaticValues;
using static ElementalReactionsMod.Tokens;

namespace ElementalReactionsMod
{
    public static class LookingGlassSupport
    {
        public static void Initialize()
        {
            RoR2Application.onLoad += CreateLookingGlassSupport;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void CreateLookingGlassSupport()
        {
            if (Language.languagesByName.TryGetValue("en", out Language en))
            {
                RegisterLookingGlassBuff(en, Buffs.quickenBuff, "Quicken", $"Increases {ElectroText()} and {DendroText()} base damage by {quickenDamageAddCoefficient * 100f}%.");
                RegisterLookingGlassBuff(en, Buffs.superconductBuff, "Superconduct", $"Increases non-elemental damage taken by {superconductDamageMultiplier * 100f}%.");
                RegisterLookingGlassBuff(en, Buffs.electroChargeBuff, "Electro-Charged", $"Requires {ElectroText()} and {HydroText()}. Deals {DamageValueBaseText(electroChargeDamageCoefficient*100f)} per tick.");
                RegisterLookingGlassBuff(en, Buffs.burningBuff, "Burning", $"Requires {DendroText()} and {PyroText()}. Deals {DamageValueBaseText(burningDamagePerSecond * 100f)} over time, and {HealthText("disables health regeneration")}."); // These are worded so differently. I'm just copying the normal burn descriptions
                RegisterLookingGlassBuff(en, Buffs.strongBurningBuff, "Wild Burning", $"Requires {DendroText()} and {PyroText()}. A deadly burn with at least 4x effectiveness of the regular Burning reaction. Health regeneration is disabled.");
                RegisterLookingGlassBuff(en, Buffs.delusionActiveBuff, "Delusion Activated", $"Decreases healing received by {HealthText(delusionHealingReceivedReduction * 100f + "%")}.");
                RegisterLookingGlassBuff(en, Buffs.lunarChargeBuff, "Lunar-Charged", $"Requires {ElectroText()} and {HydroText()}. Deals {DamageValueBaseText(lunarChargeDamageCoefficient * 100f)} per tick.");
                RegisterLookingGlassBuff(en, Buffs.lunarBloomBuff, "Verdant Dew", $"Consume three of this buff to increase your next hit of {DamageText("over 400% damage")} by {DamageText(lunarBloomDamageMultiplier * 100f + "%")}.");
                RegisterLookingGlassBuff(en, Buffs.stellarConductDebuff, "Stellar-Conduct", $"Increases non-elemental damage taken by {stellarConductMinDamageMultiplier * 100f}%-{stellarConductMaxDamageMultiplier * 100f}%.");
            }

            ItemStatsDef teaCupStatsDef = new ItemStatsDef();
            teaCupStatsDef.descriptions.Add("Bonus Damage: ");
            teaCupStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            teaCupStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            teaCupStatsDef.calculateValues = (master, stackCount) =>
            {
                List<float> values = new();
                values.Add(instructorsTeaCupDamageMultiplier * master.inventory.GetItemCountWithQuality(Items.Items.instructorsTeaCup));
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(teaCupStatsDef, Items.Items.instructorsTeaCup.itemIndex);

            ItemStatsDef moonWheelStatsDef = new ItemStatsDef();
            moonWheelStatsDef.descriptions.Add("Bonus Damage: ");
            moonWheelStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            moonWheelStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            moonWheelStatsDef.calculateValues = (master, stackCount) =>
            {
                List<float> values = new();
                values.Add(moonWheelLunarDamagePerStack * Mathf.Max(master.inventory.GetItemCountWithQuality(Items.Items.moonWheel) - 1, 0));
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(moonWheelStatsDef, Items.Items.moonWheel.itemIndex);

            ItemStatsDef stellarLinchpinStatsDef = new ItemStatsDef();
            stellarLinchpinStatsDef.descriptions.Add("Bonus Damage: ");
            stellarLinchpinStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            stellarLinchpinStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            stellarLinchpinStatsDef.calculateValues = (master, stackCount) =>
            {
                List<float> values = new();
                values.Add(stellarLinchpinStellarDamagePerStack * Mathf.Max(master.inventory.GetItemCountWithQuality(Items.Items.stellarLinchpin) - 1, 0));
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(stellarLinchpinStatsDef, Items.Items.stellarLinchpin.itemIndex);

            ItemStatsDef delusionStatsDef = new ItemStatsDef();
            delusionStatsDef.descriptions.Add("Healing Reduction: ");
            delusionStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Death);
            delusionStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            delusionStatsDef.descriptions.Add("Damage: ");
            delusionStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            delusionStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            delusionStatsDef.descriptions.Add("Proc Coefficient: ");
            delusionStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            delusionStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.ProcCoeff);
            delusionStatsDef.descriptions.Add("Total Health Cost: ");
            delusionStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Death);
            delusionStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            delusionStatsDef.calculateValues = (master, stackCount) =>
            {
                List<float> values = new();
                int count = master.inventory.GetDelusionCount();
                values.Add(delusionHealingReceivedReduction * count);
                values.Add(delusionDamageCoefficient * stackCount);
                values.Add(1f);
                values.Add(delusionHealthPercentCost * count);
                return values;
            };
            List<ItemDef> delusions = DelusionManager.delusionToElement.Keys.ToList();
            foreach (var item in delusions)
            {
                ItemDefinitions.RegisterItemStatsDef(delusionStatsDef, item.itemIndex);
            }

            if (ElementalReactionsPlugin.qualityModExists)
            {
                ItemStatsDef teaCupQualityStatsDef = new ItemStatsDef();
                teaCupQualityStatsDef.descriptions.Add("Bonus Damage: ");
                teaCupQualityStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                teaCupQualityStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                teaCupQualityStatsDef.descriptions.Add("Quality Damage Per Stack: ");
                teaCupQualityStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                teaCupQualityStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                teaCupQualityStatsDef.descriptions.Add("Max Damage From Quality: ");
                teaCupQualityStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                teaCupQualityStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                ItemIndex[] items = QualitySupport.GetQualityItemIndices(Items.Items.instructorsTeaCup);
                teaCupQualityStatsDef.calculateValues = (master, stackCount) => new List<float>
                {
                    instructorsTeaCupDamageMultiplier * master.inventory.GetItemCountWithQuality(Items.Items.instructorsTeaCup),
                    instructorsTeaCupQualityDamageIncrease * QualitySupport.GetItemCountQualities(master.inventory, Items.Items.instructorsTeaCup),
                    InstructorsTeaCupQualityMaxDamage(master)
                };
                for (int i = 0; i < items.Length; i++)
                {
                    ItemDefinitions.RegisterItemStatsDef(teaCupQualityStatsDef, items[i]);
                }

                ItemStatsDef moonWheelQualityStatsDef = new ItemStatsDef();
                moonWheelQualityStatsDef.descriptions.Add("Bonus Damage: ");
                moonWheelQualityStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                moonWheelQualityStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                moonWheelQualityStatsDef.descriptions.Add("Quality Doubling Chance: ");
                moonWheelQualityStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                moonWheelQualityStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                moonWheelQualityStatsDef.calculateValues = (master, stackCount) => new List<float>
                {
                    moonWheelLunarDamagePerStack * Mathf.Max(master.inventory.GetItemCountWithQuality(Items.Items.moonWheel) - 1, 0),
                    MoonWheelQualityLookingGlassChance(master)
                };
                items = QualitySupport.GetQualityItemIndices(Items.Items.moonWheel);
                for (int i = 0; i < items.Length; i++)
                {
                    ItemDefinitions.RegisterItemStatsDef(moonWheelQualityStatsDef, items[i]);
                }
            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        internal static float InstructorsTeaCupQualityMaxDamage(CharacterMaster master)
        {
            ItemQualities.ItemQualityCounts count = ItemQualities.Utilities.Extensions.InventoryExtensions.GetItemCountsEffective(master.inventory, ItemQualities.QualityCatalog.FindItemQualityGroupIndex(Items.Items.instructorsTeaCup.itemIndex));
            switch (count.HighestQuality)
            {
                case ItemQualities.QualityTier.Rare:
                    return (instructorsTeaCupQualityMaxStacks + instructorsTeaCupQualityStacksPerQuality) * instructorsTeaCupQualityDamageIncrease * count.TotalQualityCount;
                case ItemQualities.QualityTier.Epic:
                    return (instructorsTeaCupQualityMaxStacks + (instructorsTeaCupQualityStacksPerQuality * 2)) * instructorsTeaCupQualityDamageIncrease * count.TotalQualityCount;
                case ItemQualities.QualityTier.Legendary:
                    return (instructorsTeaCupQualityMaxStacks + (instructorsTeaCupQualityStacksPerQuality * 3)) * instructorsTeaCupQualityDamageIncrease * count.TotalQualityCount;
                default:
                    return instructorsTeaCupQualityMaxStacks *instructorsTeaCupQualityDamageIncrease * count.TotalQualityCount;
            }
        }
        internal static float MoonWheelQualityLookingGlassChance(CharacterMaster master)
        {
            float chance = Mathf.Clamp01(QualitySupport.GetMoonWheelQualityChance(master.inventory) / 100f);
            if (master.luck == 0) return chance;
            if (master.luck > 0) return 1f - Mathf.Pow(1f - chance, master.luck + 1);
            else return 1f - Mathf.Pow(chance, Mathf.Abs(master.luck) + 1);
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        internal static void RegisterLookingGlassBuff(Language lang, BuffDef buff, string name, string description)
        {
            LookingGlassLanguageAPI.SetupToken(lang, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(lang, $"DESCRIPTION_{buff.name}", description);
        }
    }
}
