using BepInEx.Configuration;
using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using RiskOfOptions;
using RiskOfOptions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElementalReactionsMod
{
    public static class Config
    {
        #region Regular Configs
        /*public static ConfigEntry<Logs> EnableLogs()
        {
            return ElementalReactionsPlugin.instance.Config.Bind<Logs>("Misc.", "Enable Logs", Logs.Minimum, "This controls how much information this mod will put in the logs. This information can help the mod creator fix issues should any come up, but it does impact performance a bit. Default is false.");
        }*/
        #endregion
        #region Loadout
        public static ElementIndex[] GetElementLoadoutFromConfig(ConfigEntry<string> config)
        {
            ElementIndex[] elements = [ElementIndex.None, ElementIndex.None, ElementIndex.None, ElementIndex.None];
            if (!ElementCatalog.availability.available) return elements;
            if (config != null && !string.IsNullOrEmpty(config.Value))
            {
                string[] elementString = config.Value.Split(", ").ToArray();
                for (int i = 0; i < 4; i++)
                {
                    if (i < elementString.Length)
                    {
                        foreach (var element in ElementCatalog.elementCatalog)
                        {
                            if (element.cachedName == elementString[i])
                            {
                                elements[i] = element.index;
                            }
                        }
                    }
                }
            }
            return elements;
        }
        public static ConfigEntry<string> GetElementLoadoutConfig(string bodyName)
        {
            if (ElementalReactionsPlugin.instance.Config.TryGetEntry<string>("Loadouts", bodyName, out var entry))
            {
                return entry;
            }
            return null;
        }
        public static void CreateElementLoadoutConfig(string bodyName, ElementDef[] elements)
        {
            ElementalReactionsPlugin.instance.Config.Bind<string>("Loadouts", bodyName, string.Concat(elements.Select(x => x ? x.cachedName : "None" + ", ")), "The elements selected for the survivor's loadout. This value should be set in game through the Element tab of the character select screen.");
        }
        #endregion
        #region Risk Of Options
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void RiskOfOptionsSetup()
        {
            //Sprite icon = (Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperBuffIcon"));
            //ModSettingsManager.SetModIcon(icon);

            //ModSettingsManager.AddOption(new ChoiceOption(Config.EnableLogs()));
        }
        #endregion
    }
}