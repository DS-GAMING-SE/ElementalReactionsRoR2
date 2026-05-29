using LookingGlass;
using LookingGlass.ItemStatsNameSpace;
using LookingGlass.LookingGlassLanguage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static ElementalReactionsMod.StaticValues;

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
        }

        internal static void RegisterLookingGlassBuff(Language lang, BuffDef buff, string name, string description)
        {
            LookingGlassLanguageAPI.SetupToken(lang, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(lang, $"DESCRIPTION_{buff.name}", description);
        }
    }
}
