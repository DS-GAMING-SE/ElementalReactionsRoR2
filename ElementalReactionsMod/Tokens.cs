using R2API;
using RoR2;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static ElementalReactionsMod.StaticValues;

namespace ElementalReactionsMod
{
    public static class Tokens
    {
        public static string[] elements = ["Pyro", "Hydro", "Electro", "Cryo", "Anemo", "Geo", "Dendro"];
        public static string[] elementsColored = [PyroText(), HydroText(), ElectroText(), CryoText(), AnemoText(), GeoText(), DendroText()];

        public static void Initialize()
        {
            string prefix = ElementalReactionsPlugin.PREFIX;
            LanguageAPI.Add($"{prefix}EXPANSION_NAME", "Elemental Reactions");
            LanguageAPI.Add($"{prefix}EXPANSION_DESCRIPTION", "Adds the Elemental Reaction system from 'Genshin Impact' to the game.");

            LanguageAPI.Add($"{prefix}LOADOUT_ELEMENTS", "Elements");

            #region Elements
            LanguageAPI.Add($"{prefix}ELEMENT_PHYSICAL_NAME", "Physical");
            LanguageAPI.Add($"{prefix}ELEMENT_PHYSICAL_DESCRIPTION", "No element. Unchanged from vanilla.");
            LanguageAPI.Add($"{prefix}ELEMENT_PYRO_NAME", "Pyro");
            LanguageAPI.Add($"{prefix}ELEMENT_PYRO_DESCRIPTION", "");
            LanguageAPI.Add($"{prefix}ELEMENT_HYDRO_NAME", "Hydro");
            LanguageAPI.Add($"{prefix}ELEMENT_HYDRO_DESCRIPTION", "");
            LanguageAPI.Add($"{prefix}ELEMENT_ELECTRO_NAME", "Electro");
            LanguageAPI.Add($"{prefix}ELEMENT_ELECTRO_DESCRIPTION", "");
            LanguageAPI.Add($"{prefix}ELEMENT_CRYO_NAME", "Cryo");
            LanguageAPI.Add($"{prefix}ELEMENT_CRYO_DESCRIPTION", "");
            LanguageAPI.Add($"{prefix}ELEMENT_ANEMO_NAME", "Anemo");
            LanguageAPI.Add($"{prefix}ELEMENT_ANEMO_DESCRIPTION", "");
            LanguageAPI.Add($"{prefix}ELEMENT_GEO_NAME", "Geo");
            LanguageAPI.Add($"{prefix}ELEMENT_GEO_DESCRIPTION", "");
            LanguageAPI.Add($"{prefix}ELEMENT_DENDRO_NAME", "Dendro");
            LanguageAPI.Add($"{prefix}ELEMENT_DENDRO_DESCRIPTION", "");
            #endregion

            #region Reactions
            LanguageAPI.Add($"{prefix}REACTION_VAPORIZE_NAME", "Vaporize / Melt");
            LanguageAPI.Add($"{prefix}REACTION_VAPORIZE_DESCRIPTION", $"{PyroText()} + {HydroText()}/{CryoText()}. Based on whether {PyroText()} is applied first or last, increase damage by {DamageMultiplierText(vaporizeMultiplierHydroTrigger)} or {DamageMultiplierText(vaporizeMultiplierPyroTrigger)}.");

            LanguageAPI.Add($"{prefix}REACTION_OVERLOAD_NAME", "Overload");
            LanguageAPI.Add($"{prefix}REACTION_OVERLOAD_DESCRIPTION", $"{PyroText()} + {ElectroText()}. Create an explosion of {PyroText()} dealing {DamageValueText(overloadDamageCoefficient)}.");

            LanguageAPI.Add($"{prefix}REACTION_ELECTRO_CHARGE_NAME", "Electro-Charge");
            LanguageAPI.Add($"{prefix}REACTION_ELECTRO_CHARGE_DESCRIPTION", $"{ElectroText()} + {HydroText()}. Create {DamageText("chain lightning")} that arcs between enemies affected by {HydroText()} for {DamageValueTextRepeat(electroChargeDamageCoefficient, Mathf.FloorToInt(electroChargeDuration))} over time.");

            LanguageAPI.Add($"{prefix}REACTION_FROZEN_NAME", "Frozen");
            LanguageAPI.Add($"{prefix}REACTION_FROZEN_DESCRIPTION", $"{CryoText()} + {HydroText()}. Briefly {UtilityText("freeze")} the target.");

            LanguageAPI.Add($"{prefix}REACTION_SUPERCONDUCT_NAME", "Superconduct");
            LanguageAPI.Add($"{prefix}REACTION_SUPERCONDUCT_DESCRIPTION", $"{CryoText()} + {ElectroText()}. Create a blast of {CryoText()} dealing {DamageValueText(superconductDamageCoefficient)} and increasing all non-elemental damage dealt by {DamageMultiplierText(superconductDamageMultiplier)}.");

            LanguageAPI.Add($"{prefix}REACTION_SWIRL_NAME", "Swirl");
            LanguageAPI.Add($"{prefix}REACTION_SWIRL_DESCRIPTION", $"{AnemoText()} + {PyroText()}/{HydroText()}/{ElectroText()}/{CryoText()}. Create a burst of the non-{AnemoText()} element dealing {DamageValueText(swirlDamageCoefficient)}.");

            LanguageAPI.Add($"{prefix}REACTION_CRYSTALLIZE_NAME", "Crystallize");
            LanguageAPI.Add($"{prefix}REACTION_CRYSTALLIZE_DESCRIPTION", $"{GeoText()} + {PyroText()}/{HydroText()}/{ElectroText()}/{CryoText()}. Create a shard that grants you a {HealingText("temporary barrier")} for {HealingText(crystallizeBarrierPercent*100f+"%")} of your max health, up to {HealingText(crystallizeMaxBarrierPercent * 100f + "%")}.");

            LanguageAPI.Add($"{prefix}REACTION_BURNING_NAME", "Burning");
            LanguageAPI.Add($"{prefix}REACTION_BURNING_DESCRIPTION", $"{DendroText()} + {PyroText()}. {DamageText("Ignite")} the target.");

            LanguageAPI.Add($"{prefix}REACTION_QUICKEN_NAME", "Quicken");
            LanguageAPI.Add($"{prefix}REACTION_QUICKEN_DESCRIPTION", $"{DendroText()} + {ElectroText()}. Increase damage dealt by all {DendroText()} or {ElectroText()} attacks by a flat {DamageText((quickenDamageAddCoefficient * 100f).ToString()+"% base damage")}.");

            LanguageAPI.Add($"{prefix}REACTION_BLOOM_NAME", "Bloom");
            LanguageAPI.Add($"{prefix}REACTION_BLOOM_DESCRIPTION", $"{DendroText()} + {HydroText()}. Create a {DendroText("Dendro Core")} which explodes after {UtilityText(bloomDuration+"s")} dealing {DamageValueText(bloomDamageCoefficient)} to ALL characters.\n{ElectroText()} converts the core into a {DamageText("homing projectile")} dealing {DamageValueText(hyperBloomDamageCoefficient)}.\n{PyroText()} causes the core to explode in a {DamageText("larger radius")}, dealing {DamageValueText(burgeonDamageCoefficient)} to ALL characters.");
            LanguageAPI.Add($"{prefix}REACTION_BLOOM_OBJECT_NAME", "Dendro Core");
            #endregion

            #region Items
            LanguageAPI.Add($"{prefix}ITEM_SOULSEEKER_SHELL_NAME", "Soulseeker Shell");
            LanguageAPI.Add($"{prefix}ITEM_SOULSEEKER_SHELL_PICKUP", $"Your drones have a chance of attacking with one of your elements.");
            LanguageAPI.Add($"{prefix}ITEM_SOULSEEKER_SHELL_DESCRIPTION", $"Drones gain a {DamageText("10% ") + StackingText("(+10% per stack)")} chance on hit to apply one of your {UtilityText("elements")} on hit.");
            //LanguageAPI.Add($"{prefix}ITEM_SOULSEEKER_SHELL_LORE", "");

            LanguageAPI.Add($"{prefix}ITEM_DELUSION_NAME", "Delusion");
            LanguageAPI.Add($"{prefix}ITEM_DELUSION_PICKUP", $"Resonates with a new element on pickup. {DelusionPickup("the Delusion's elemental")}");
            LanguageAPI.Add($"{prefix}ITEM_DELUSION_DESCRIPTION", $"On pickup, {UtilityText("resonate")} with a {UtilityText("random element you don't have")}. {DelusionDescription(DamageText("the Delusion's element"))}");
            LanguageAPI.Add($"{prefix}ITEM_DELUSION_LORE", """
                Usurper. 
                
                I know you can hear me.

                But can you hear them?

                The ones you rule, who cry out in anguish.
                
                The heretics. The curious. The ones in the wrong place at the wrong time. All the ones you kill to keep your gilded world unblemished.

                Maybe you only see the 'peace' you envision: The order you have brought to the world, built on lies and oppression. 
                
                Do not be fooled. Your suffocating grip on this world does not go without contempt. Their hatred festers and their will to destroy grows. They are the kindling.
                
                No matter how much you try to hide the past of this world, to hide <i>me</i>, there will always be those who look to the sky.

                Those whose gaze pierces through the lies you've confined this world beneath. When their gaze meets mine, their grievances of your world will manifest as an unshakable resolve.

                The resolve to endure bitter cold, to fight, to kill, and to die, all so they may burn your world away with them.

                When there is nothing left but ashes, I will be here, ready to take my rightful place on the throne of the new world.
                """);
            for (int i = 0; i < elements.Length; i++)
            {
                LanguageAPI.Add($"{prefix}ITEM_DELUSION_{elements[i].ToUpper()}_NAME", $"{elements[i]} Delusion");
                LanguageAPI.Add($"{prefix}ITEM_DELUSION_{elements[i].ToUpper()}_PICKUP", DelusionPickup(elementsColored[i]));
                LanguageAPI.Add($"{prefix}ITEM_DELUSION_{elements[i].ToUpper()}_DESCRIPTION", DelusionDescription(DamageText(elementsColored[i])));
            }

            LanguageAPI.Add($"{prefix}ITEM_INSTRUCTORS_TEA_CUP_NAME", "Instructor's Tea Cup");
            LanguageAPI.Add($"{prefix}ITEM_INSTRUCTORS_TEA_CUP_PICKUP", "Deal bonus damage from elemental reactions.");
            QualitySupport.AddQualityLanguage($"INSTRUCTORS_TEA_CUP", true, $"{QualitySupport.qualityIcon} Bonus damage increases after triggering elemental reactions.", true);
            LanguageAPI.Add($"{prefix}ITEM_INSTRUCTORS_TEA_CUP_DESCRIPTION", $"Increases {UtilityText("elemental reaction")} damage by {DamageText($"{instructorsTeaCupDamageMultiplier * 100f}%")} {StackingText($"(+{instructorsTeaCupDamageMultiplier * 100f}% per stack)")}.");
            QualitySupport.AddQualityLanguage($"INSTRUCTORS_TEA_CUP", false, $"Triggering an {UtilityText("elemental reaction")} increases {UtilityText("elemental reaction")} damage by {DamageText($"{instructorsTeaCupQualityDamageIncrease * 100f}%")} {StackingText($"(+{instructorsTeaCupQualityDamageMultiplierStack * 100f}% per stack)")}, up to {QualitySupport.qualityIcon} {UtilityText("{0}")}, for {QualitySupport.qualityIcon} {UtilityText("{1}s")}. Triggering a reaction refreshes the timer.", true,
                instructorsTeaCupQualityMaxStacks, instructorsTeaCupQualityDuration,
                instructorsTeaCupQualityMaxStacks + instructorsTeaCupQualityStacksPerQuality, instructorsTeaCupQualityDuration + instructorsTeaCupQualityDurationPerQuality,
                instructorsTeaCupQualityMaxStacks + (instructorsTeaCupQualityStacksPerQuality * 2), instructorsTeaCupQualityDuration + (instructorsTeaCupQualityDurationPerQuality * 2),
                instructorsTeaCupQualityMaxStacks + (instructorsTeaCupQualityStacksPerQuality * 3), instructorsTeaCupQualityDuration + (instructorsTeaCupQualityDurationPerQuality * 3));
            LanguageAPI.Add($"{prefix}ITEM_INSTRUCTORS_TEA_CUP_LORE", """"""
                Every military student knows that the so-called "free training time" could not have anything less to do with freedom.
                
                The instructor sits back and watches while the students train rigorously in the field.
                
                A cup of black tea with a little too much sugar in it marks a typical afternoon for the instructor.
                
                This peaceful time is a privilege of the instructor, and the embodiment of his authority.
                """""");

            LanguageAPI.Add($"{prefix}ITEM_MOON_WHEEL_NAME", "Moon Wheel");
            LanguageAPI.Add($"{prefix}ITEM_MOON_WHEEL_PICKUP", "Upgrades the Hydro reactions between Electro, Dendro, and Geo into powerful Lunar Reactions.");
            QualitySupport.AddQualityLanguage($"MOON_WHEEL", true, $"Upgrades the Hydro reactions between Electro, Dendro, and Geo into {QualitySupport.qualityIcon} much more powerful Lunar Reactions.");
            string moonWheelDescIntro = $"Upgrades the {ElectroText("Electro-Charge")}, {DendroText("Bloom")}, and {GeoText("Hydro-Crystallize")} reactions into {DamageText("Lunar Reactions")} that can {DamageText("critically strike")}. Increases {DamageText("Lunar Reaction damage")} by {DamageText("0%")} {StackingText("(+" + moonWheelLunarDamagePerStack * 100f + "% per stack)")}.";
            string moonWheelDescCharged = $"{ElectroText("Lunar-Charge")}: Continuously strike the target with lightning, dealing {DamageValueTextRepeat(lunarChargeDamageCoefficient, lunarChargeAttacksPerDot)}.";
            string moonWheelDescBloom = $"{DendroText("Lunar-Bloom")}: Create a {DendroText("Dendro Core")} and gain a {UtilityText("Verdant Dew")}, up to {UtilityText(lunarBloomVerdantDewCap.ToString())}. Dealing {DendroText()} {UtilityText("skill")} damage with {UtilityText(lunarBloomVerdantDewCap.ToString() + " Verdant Dews")} will consume them and increase the damage dealt by {DamageText((lunarBloomDamageMultiplier * 100) + "%")}.";
            string moonWheelDescCrystallize = $"{GeoText("Lunar-Crystallize")}: Create three Moondrifts. For every {UtilityText(lunarCrystallizeTriggersToAttack.ToString())} times this reaction is triggered, the Moondrifts will deal {DamageValueTextRepeat(lunarCrystallizeDamageCoefficient, 3)}.";
            LanguageAPI.Add($"{prefix}ITEM_MOON_WHEEL_DESCRIPTION", $"{moonWheelDescIntro}\n\n{moonWheelDescCharged}\n{moonWheelDescBloom}\n{moonWheelDescCrystallize}");
            QualitySupport.AddQualityLanguage($"MOON_WHEEL", false, $"{moonWheelDescIntro}\n\n{moonWheelDescCharged} {QualitySupport.qualityIcon} {DamageText("{0}%")} {StackingText("(+{0}% per stack)")} chance to strike twice.\n{moonWheelDescBloom} {QualitySupport.qualityIcon} {UtilityText("{0}%")} {StackingText("(+{0}% per stack)")} chance to gain an additional {UtilityText("Verdant Dew")}.\n{moonWheelDescCrystallize} Each Moondrift has a {QualitySupport.qualityIcon} {DamageText("{0}%")} {StackingText("(+{0}% per stack)")} chance to fire twice.", false,
                moonWheelQualityChancePerQuality * 1,
                moonWheelQualityChancePerQuality * 2,
                moonWheelQualityChancePerQuality * 3,
                moonWheelQualityChancePerQuality * 4);
            LanguageAPI.Add($"{prefix}ITEM_MOON_WHEEL_LORE", """"""
                Awaken, awaken, new moon, even if all that remains is torment.

                Even if shame and agony gnaw at our hearts through day and night.

                But the moon will rise again, and we and our hearts will not be silent.

                You will hear the roar of the tides, then laughter and cheer in its wake.
                """""");
            #endregion
        }
        public static string DelusionPickup(string element)
        {
            return $"Activating your Special skill will make any skill damage fire attacks of {element} damage... {RedText("BUT at the cost of your health")}.";
        }
        public static string DelusionDescription(string element)
        {
            return $"Activating your {UtilityText("Special skill")} will activate the Delusion for {UtilityText(delusionDuration+"s")}. While active, damaging enemies with any {UtilityText("skill")} will fire an {DamageText("attack of ")+element}, dealing {DamageText($"{delusionDamageCoefficient * 100f}% base damage")} {StackingText($"(+{delusionStackDamageCoefficient * 100f}% per same-element stack)")}, while also draining your health by {RedText(delusionHealthPercentCost * 100f+"%")} {StackingText($"(+{delusionHealthPercentCost * 100f+"%"} per same-element stack)")}. While the Delusion is active, {RedText($"healing received is reduced by {delusionHealingReceivedReduction*100f}%")} {StackingText($"(+{delusionHealingReceivedReduction * 100f}% per stack)")}. {StackingText("A separate attack will be fired for each Delusion you have of a unique element")}.";
        }
        public static string DamageText(string text)
        {
            return $"<style=cIsDamage>{text}</style>";
        }
        public static string DamageValueText(float value)
        {
            return $"<style=cIsDamage>{value * 100}% damage</style>";
        }
        public static string DamageMultiplierText(float value)
        {
            return $"<style=cIsDamage>{(value - 1) * 100}%</style>";
        }
        public static string DamageValueBaseText(float value)
        {
            return $"<style=cIsDamage>{value * 100}% base damage</style>";
        }
        public static string DamageValueText(float value, string damageType)
        {
            return $"<style=cIsDamage>{value * 100}%</style> <style=cIsDamage>{damageType} damage</style>";
        }
        public static string DamageValueText(float value, float value2)
        {
            return $"<style=cIsDamage>{value * 100}%-{value2 * 100}% damage</style>";
        }
        public static string DamageValueTextRepeat(float value, int repeat)
        {
            return $"<style=cIsDamage>{repeat}x{value * 100}% damage</style>";
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
        public static string HealingText(string text)
        {
            return $"<style=cIsHealing>{text}</style>";
        }
        public static string KeywordText(string keyword, string sub)
        {
            return $"<style=cKeywordName>{keyword}</style><style=cSub>{sub}</style>";
        }
        public static string StackingText(string stack)
        {
            return $"<style=cStack>{stack}</style>";
        }
        public static string PyroText(string text) => HealthText(text);
        public static string PyroText() => PyroText("Pyro");
        public static string HydroText(string text) { return $"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.LunarItem)}>{text}</color>"; }
        public static string HydroText() => HydroText("Hydro");
        public static string ElectroText(string text) { return $"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Utility)}>{text}</color>"; }
        public static string ElectroText() => ElectroText("Electro");
        public static string CryoText(string text) => UtilityText(text);
        public static string CryoText() => CryoText("Cryo");
        public static string AnemoText(string text) { return $"<color=#7fe6cc>{text}</color>"; }
        public static string AnemoText() => AnemoText("Anemo");
        public static string GeoText(string text) => DamageText(text);
        public static string GeoText() => GeoText("Geo");
        public static string DendroText(string text) { return $"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Healing)}>{text}</color>"; }
        public static string DendroText() => DendroText("Dendro");
    }
}