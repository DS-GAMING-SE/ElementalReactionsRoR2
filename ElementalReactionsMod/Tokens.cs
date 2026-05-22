using RoR2;
using System;
using System.Text;
using UnityEngine;
using R2API;

namespace ElementalReactionsMod
{
    public static class Tokens
    {
        public static string[] elements = ["Pyro", "Hydro", "Electro", "Cryo", "Anemo", "Geo", "Dendro"];
        public static string[] elementsColored = ["Pyro", "Hydro", "Electro", Tokens.UtilityText("Cryo"), "Anemo", Tokens.DamageText("Geo"), "Dendro"];

        public static void Initialize()
        {
            string prefix = ElementalReactionsPlugin.PREFIX;
            LanguageAPI.Add($"{prefix}EXPANSION_NAME", "Elemental Reactions");
            LanguageAPI.Add($"{prefix}EXPANSION_DESCRIPTION", "Adds the Elemental Reaction system from 'Genshin Impact' to the game.");

            LanguageAPI.Add($"{prefix}LOADOUT_ELEMENTS", "Elements");

            #region Elements
            LanguageAPI.Add($"{prefix}ELEMENT_PHYSICAL", "Physical");
            LanguageAPI.Add($"{prefix}ELEMENT_PHYSICAL_DESCRIPTION", "Physical");
            LanguageAPI.Add($"{prefix}ELEMENT_PYRO", "Pyro");
            LanguageAPI.Add($"{prefix}ELEMENT_PYRO_DESCRIPTION", "Pyro");
            LanguageAPI.Add($"{prefix}ELEMENT_HYDRO", "Hydro");
            LanguageAPI.Add($"{prefix}ELEMENT_HYDRO_DESCRIPTION", "Hydro");
            LanguageAPI.Add($"{prefix}ELEMENT_ELECTRO", "Electro");
            LanguageAPI.Add($"{prefix}ELEMENT_ELECTRO_DESCRIPTION", "Electro");
            LanguageAPI.Add($"{prefix}ELEMENT_CRYO", "Cryo");
            LanguageAPI.Add($"{prefix}ELEMENT_CRYO_DESCRIPTION", "Cryo");
            LanguageAPI.Add($"{prefix}ELEMENT_ANEMO", "Anemo");
            LanguageAPI.Add($"{prefix}ELEMENT_ANEMO_DESCRIPTION", "Anemo");
            LanguageAPI.Add($"{prefix}ELEMENT_GEO", "Geo");
            LanguageAPI.Add($"{prefix}ELEMENT_GEO_DESCRIPTION", "Geo");
            LanguageAPI.Add($"{prefix}ELEMENT_DENDRO", "Dendro");
            LanguageAPI.Add($"{prefix}ELEMENT_DENDRO_DESCRIPTION", "Dendro");
            #endregion

            #region Items
            LanguageAPI.Add($"{prefix}ITEM_SOULSEEKER_SHELL_NAME", "Soulseeker Shell");
            LanguageAPI.Add($"{prefix}ITEM_SOULSEEKER_SHELL_PICKUP", $"Your drones have a chance of attacking with one of your elements.");
            LanguageAPI.Add($"{prefix}ITEM_SOULSEEKER_SHELL_DESCRIPTION", $"Drones gain a {DamageText("10% ") + StackingText("+10% per stack")} chance on hit to apply one of your {UtilityText("elemental types")}.");
            LanguageAPI.Add($"{prefix}ITEM_SOULSEEKER_SHELL_LORE", "");

            LanguageAPI.Add($"{prefix}ITEM_DELUSION_NAME", "Delusion");
            LanguageAPI.Add($"{prefix}ITEM_DELUSION_PICKUP", $"Resonates with an element on pickup. {DelusionPickup("that element")}");
            LanguageAPI.Add($"{prefix}ITEM_DELUSION_DESCRIPTION", $"On pickup, {UtilityText("resonate")} with a {UtilityText("random element you don't have")}. {DelusionDescription(DamageText("the Delusion's element"))}");

            for (int i = 0; i < elements.Length; i++)
            {
                LanguageAPI.Add($"{prefix}ITEM_DELUSION_{elements[i].ToUpper()}_NAME", $"{elements[i]} Delusion");
                LanguageAPI.Add($"{prefix}ITEM_DELUSION_{elements[i].ToUpper()}_PICKUP", DelusionPickup(elementsColored[i]));
                LanguageAPI.Add($"{prefix}ITEM_DELUSION_{elements[i].ToUpper()}_DESCRIPTION", DelusionDescription(DamageText(DelusionPickup(elementsColored[i]))));
            }

            /*LanguageAPI.Add($"{prefix}ITEM_DELUSION_LORE", """
                Usurper...
                ---
                ---parallel providence and heavenly principles. Unclear if pov is fatui or mithrix---
                ---
                I will endure the bitter cold for as long as it takes...
                To see your world burn.
                """);*/
            #endregion
        }
        public static string DelusionPickup(string element)
        {
            return $"High damage hits also blast enemies with an attack of {element}... {RedText("BUT some of your life is consumed with each use")}. Recharges over time.";
        }
        public static string DelusionDescription(string element)
        {
            return $"Hits that deal {DamageText("more than 400% damage")} blasts enemies with an {DamageText("attack of ")+element}, dealing {DamageText("1000% base damage")}. Triggering this effect costs {RedText("10%")} of your max health and {RedText("reduces healing received by 30%")} {StackingText("+30% per stack")} while the effect is on cooldown. Recharges every {UtilityText("10")} seconds.";
        }
        public static string DamageText(string text)
        {
            return $"<style=cIsDamage>{text}</style>";
        }
        public static string DamageValueText(float value)
        {
            return $"<style=cIsDamage>{value * 100}% damage</style>";
        }
        public static string DamageValueText(float value, float value2)
        {
            return $"<style=cIsDamage>{value * 100}%-{value2 * 100}% damage</style>";
        }
        public static string UtilityText(string text)
        {
            return $"<style=cIsUtility>{text}</style>";
        }
        public static string RedText(string text) => HealthText(text);
        public static string HealthText(string text)
        {
            return $"<style=cIsHealth>{text}</style>";
        }
        public static string KeywordText(string keyword, string sub)
        {
            return $"<style=cKeywordName>{keyword}</style><style=cSub>{sub}</style>";
        }
        public static string StackingText(string stack)
        {
            return $"<style=cStack>{stack}</style>";
        }
    }
}