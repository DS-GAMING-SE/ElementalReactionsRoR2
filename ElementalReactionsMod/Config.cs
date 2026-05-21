using BepInEx.Configuration;
using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            ElementIndex[] elements = [ElementIndex.Physical, ElementIndex.Physical, ElementIndex.Physical, ElementIndex.Physical];
            if (!ElementCatalog.availability.available) return elements;
            if (config != null && !string.IsNullOrEmpty(config.Value))
            {
                string[] elementString = config.Value.Split(", ").ToArray();
                if (elementString.Length < 4)
                {
                    ElementDef physical = ElementCatalog.GetElementDef(ElementIndex.Physical);
                    SetElementLoadoutConfig(config, [physical, physical, physical, physical]);
                    return elements;
                }
                for (int i = 0; i < 4; i++)
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
            return elements;
        }
        public static ConfigEntry<string> GetElementLoadoutConfig(string bodyName)
        {
            if (!string.IsNullOrEmpty(bodyName) && ElementalReactionsPlugin.instance.Config.TryGetEntry<string>("Loadouts", bodyName, out var entry))
            {
                return entry;
            }
            return null;
        }
        public static ConfigEntry<string> SetElementLoadoutConfig(string bodyName, ElementIndex element, SkillSlot skillSlot)
        {
            ConfigEntry<string> config = GetElementLoadoutConfig(bodyName);
            ElementIndex[] elements = GetElementLoadoutFromConfig(config);
            if (elements[(int)skillSlot] == element) return config;
            elements[(int)skillSlot] = element;
            ElementDef[] elementDefs = [ElementCatalog.GetElementDef(elements[0]), ElementCatalog.GetElementDef(elements[1]),
                ElementCatalog.GetElementDef(elements[2]), ElementCatalog.GetElementDef(elements[3])];
            if (config == null)
            {
                return CreateElementLoadoutConfig(bodyName, elementDefs);
            }
            else
            {
                return SetElementLoadoutConfig(config, elementDefs);
            }
        }
        public static ConfigEntry<string> SetElementLoadoutConfig(string bodyName, ElementIndex[] elements)
        {
            if (elements.Length < 4 || !ElementCatalog.availability.available || string.IsNullOrEmpty(bodyName)) return null;
            ConfigEntry<string> config = GetElementLoadoutConfig(bodyName);
            ElementDef[] elementDefs = [ElementCatalog.GetElementDef(elements[0]), ElementCatalog.GetElementDef(elements[1]),
            ElementCatalog.GetElementDef(elements[2]), ElementCatalog.GetElementDef(elements[3])];
            if (config == null) 
            { 
                return CreateElementLoadoutConfig(bodyName, elementDefs); 
            }
            return SetElementLoadoutConfig(config, elementDefs);
        }
        public static ConfigEntry<string> SetElementLoadoutConfig(ConfigEntry<string> config, ElementDef[] elementDefs)
        {
            string newConfig = elementDefs[0].cachedName + ", " + elementDefs[1].cachedName + ", " + elementDefs[2].cachedName + ", " + elementDefs[3].cachedName;
            Log.Message("Set Element Loadout for " + config.Definition.Key + ": " + newConfig);
            config.Value = newConfig;
            return config;
        }
        public static ConfigEntry<string> CreateElementLoadoutConfig(string bodyName, ElementDef[] elements)
        {
            ConfigEntry<string> config = ElementalReactionsPlugin.instance.Config.Bind<string>("Loadouts", bodyName, 
                DefaultElementDefs.physicalElement.cachedName+", "+ DefaultElementDefs.physicalElement.cachedName+", "+ DefaultElementDefs.physicalElement.cachedName+", "+DefaultElementDefs.physicalElement.cachedName, 
                "The elements selected for the survivor's loadout. This value should be set in game through the Element tab of the character select screen.");
            string newConfig = string.Join(", ", elements.Select(x => x ? x.cachedName : DefaultElementDefs.physicalElement.cachedName));
            config.Value = newConfig;
            Log.Message("Set Element Loadout for " + bodyName + ": " + newConfig);
            return config;
        }
        #endregion
        #region Risk Of Options
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void RiskOfOptionsSetup()
        {
            Sprite icon = Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.expansionIcon).WaitForCompletion();
            ModSettingsManager.SetModIcon(icon);

            //ModSettingsManager.AddOption(new ChoiceOption(Config.EnableLogs()));
        }
        #endregion
    }
}