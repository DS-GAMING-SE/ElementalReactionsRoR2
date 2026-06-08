using LookingGlass;
using LookingGlass.ItemStatsNameSpace;
using LookingGlass.LookingGlassLanguage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
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

        public static void CreateLookingGlassSupport()
        {
            if (Language.languagesByName.TryGetValue("en", out Language en))
            {
                RegisterLookingGlassBuff(en, Buffs.quickenBuff, "Quicken", $"Increases {Tokens.ElectroText("Electro")} and {Tokens.DendroText("Dendro")} base damage by {StaticValues.quickenDamageAddCoefficient * 100f}%.");
                RegisterLookingGlassBuff(en, Buffs.superconductBuff, "Superconduct", $"Increases non-elemental damage by {(StaticValues.superconductDamageMultiplier - 1f) * 100f}%.");
                RegisterLookingGlassBuff(en, Buffs.delusionActiveBuff, "Delusion Activated", $"Decreases healing received by {HealthText(delusionHealingReceivedReduction * 100f + "%")}.");
                RegisterLookingGlassBuff(en, Buffs.lunarBloomBuff, "Verdant Dew", $"Consume three of this buff to increase your next hit of {DendroText()} skill damage by {DamageText(lunarBloomDamageMultiplier * 100f + "%")}.");
            }

            ItemStatsDef teaCupStatsDef = new ItemStatsDef();
            teaCupStatsDef.descriptions.Add("Bonus Damage: ");
            teaCupStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            teaCupStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            teaCupStatsDef.calculateValuesFlat = (stackCount) =>
            {
                List<float> values = new();
                values.Add(instructorsTeaCupDamageMultiplier * stackCount);
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(teaCupStatsDef, Items.Items.instructorsTeaCup.itemIndex);

            ItemStatsDef moonWheelStatsDef = new ItemStatsDef();
            moonWheelStatsDef.descriptions.Add("Bonus Damage: ");
            moonWheelStatsDef.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            moonWheelStatsDef.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            moonWheelStatsDef.calculateValuesFlat = (stackCount) =>
            {
                List<float> values = new();
                values.Add(moonWheelLunarDamagePerStack * (stackCount - 1));
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(moonWheelStatsDef, Items.Items.moonWheel.itemIndex);
        }

        internal static void RegisterLookingGlassBuff(Language lang, BuffDef buff, string name, string description)
        {
            LookingGlassLanguageAPI.SetupToken(lang, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(lang, $"DESCRIPTION_{buff.name}", description);
        }
    }
}
