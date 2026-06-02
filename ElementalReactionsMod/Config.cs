using BepInEx.Configuration;
using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ElementalReactionsMod
{
    public static class Config
    {
        #region Regular Configs
        public static ConfigEntry<bool> CanSurvivorsUseElements()
        {
            return ElementalReactionsPlugin.instance.Config.Bind<bool>("Characters", "Survivors Use Elements", true, "Whether survivors are able to deal elemental damage with their skills.\nHost's config takes priority.");
        }
        public static ConfigEntry<bool> CanEnemiesUseElements()
        {
            return ElementalReactionsPlugin.instance.Config.Bind<bool>("Characters", "Enemies Use Elements", true, "Whether enemies are able to deal elemental damage with their skills.\nHost's config takes priority.");
        }
        public static ConfigEntry<float> PlayerReactionResistance()
        {
            return ElementalReactionsPlugin.instance.Config.Bind<float>("Characters", "Player Reaction Resistance", 50f, "The percent of the normal elemental reaction damage that will be done to players.\nHost's config takes priority.");
        }
        public static ConfigEntry<float> PlayerBloomResistance()
        {
            return ElementalReactionsPlugin.instance.Config.Bind<float>("Characters", "Player Bloom Resistance", 10f, "The percent of the normal Bloom and pyro Bloom reaction damage that will be done to players. This is applied on top of the Enemy Reaction Damage config.\nHost's config takes priority.");
        }
        #endregion
        #region Loadout
        /*
         * Writing my own stupid system for making configs because I just wanted to not write anything to the config UNLESS the value was different from default but nooooooo
         * bepinex won't let that happen. They've got a OrphanedEntries property in the config file that would've let me side step that and make my own things but nooooo
         * it's private and even after trying NStrip and AssemblyPublicizer I couldn't get it to show up
         */
        public static string loadoutConfigFilePath = ElementalReactionsPlugin.instance.Config.ConfigFilePath.Replace(".cfg", "Loadouts.cfg");
        public static Dictionary<string, ElementDef[]> elementLoadoutConfigs = new Dictionary<string, ElementDef[]>();

        [SystemInitializer(typeof(ElementCatalog), typeof(SurvivorCatalog))]
        internal static void ReadConfigFile()
        {
            if (File.Exists(loadoutConfigFilePath))
            {
                foreach (var line in File.ReadAllLines(loadoutConfigFilePath))
                {
                    var split = line.Split("=", 2);
                    if (split.Length != 2)
                    { 
                        continue; 
                    }
                    string[] elementStrings = split[1].Split(", ").ToArray();
                    if (elementStrings.Length == 4)
                    {
                        ElementDef[] elements = [DefaultElementDefs.physicalElement, DefaultElementDefs.physicalElement, DefaultElementDefs.physicalElement, DefaultElementDefs.physicalElement];
                        for (int i = 0; i < elementStrings.Length; i++)
                        {
                            string elementString = elementStrings[i].Trim();
                            foreach (var element in ElementCatalog.elementCatalog)
                            {
                                if (element.cachedName == elementString)
                                {
                                    elements[i] = element;
                                    break;
                                }
                            }
                        }
                        elementLoadoutConfigs.Add(split[0].Trim(), elements);
                    }
                }
            }
            ElementLoadoutComponent.AddElementLoadoutComponents();
        }
        public static ElementDef[] GetElementLoadoutFromConfig(string bodyName, out bool configExists)
        {
            configExists = false;
            ElementDef[] elements = [DefaultElementDefs.physicalElement, DefaultElementDefs.physicalElement, DefaultElementDefs.physicalElement, DefaultElementDefs.physicalElement];
            if (!ElementCatalog.availability.available) return elements;
            if (!string.IsNullOrEmpty(bodyName) && File.Exists(loadoutConfigFilePath) && elementLoadoutConfigs.TryGetValue(bodyName, out ElementDef[] elementsFromConfig))
            {
                configExists = true;
                return elementsFromConfig;
            }
            return elements;
        }
        public static void SetElementLoadoutConfig(string bodyName, ElementDef element, SkillSlot skillSlot)
        {
            ElementDef[] elementDefs = GetElementLoadoutFromConfig(bodyName, out bool exists);
            if (elementDefs[(int)skillSlot] == element) return;
            elementDefs[(int)skillSlot] = element;
            if (exists)
            {
                WriteElementLoadoutConfig(bodyName, elementDefs);
            }
            else
            {
                AppendNewElementLoadoutConfig(bodyName, elementDefs);
            }
        }
        public static void SetElementLoadoutConfig(string bodyName, ElementDef[] elementDefs)
        {
            if (elementLoadoutConfigs.ContainsKey(bodyName))
            {
                WriteElementLoadoutConfig(bodyName, elementDefs);
            }
            else
            {
                AppendNewElementLoadoutConfig(bodyName, elementDefs);
            }
        }
        public static void WriteElementLoadoutConfig(string bodyName, ElementDef[] elementDefs)
        {
            elementLoadoutConfigs[bodyName] = elementDefs;
            using (var writer = new StreamWriter(loadoutConfigFilePath, false))
            {
                foreach (var loadout in elementLoadoutConfigs)
                {
                    string newConfig = loadout.Key + " = " + string.Join(", ", loadout.Value.Select(x => x ? x.cachedName : DefaultElementDefs.physicalElement.cachedName));
                    writer.WriteLine(newConfig);
                }
                Log.Message("Rewrote Element Loadout config");
            }
        }
        public static void AppendNewElementLoadoutConfig(string bodyName, ElementDef[] elements)
        {
            string newConfig = bodyName+" = " + string.Join(", ", elements.Select(x => x ? x.cachedName : DefaultElementDefs.physicalElement.cachedName));
            using (var writer = new StreamWriter(loadoutConfigFilePath, false))
            {
                writer.Write(newConfig);
                Log.Message("Appended new Element Loadout: " + newConfig);
            }
            elementLoadoutConfigs.Add(bodyName, elements);
        }
        #endregion
        #region Risk Of Options
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void RiskOfOptionsSetup()
        {
            ModSettingsManager.SetModIcon(Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.expansionIcon).WaitForCompletion());

            ModSettingsManager.AddOption(new ChoiceOption(Config.CanSurvivorsUseElements()));
            ModSettingsManager.AddOption(new ChoiceOption(Config.CanEnemiesUseElements()));
            ModSettingsManager.AddOption(new SliderOption(Config.PlayerReactionResistance(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = 0, max = 100 }));
            ModSettingsManager.AddOption(new SliderOption(Config.PlayerBloomResistance(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = 0, max = 100 }));
        }
        #endregion
    }
}