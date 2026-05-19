using LookingGlass.LookingGlassLanguage;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using R2API;

namespace ElementalReactionsMod.Elements
{
    public class ElementDef : ScriptableObject
    {
        public string cachedName { get { return _cachedName; } set { name = value; _cachedName = value; } }
        private string _cachedName;
        public string nameToken;

        public BuffDef buff;
        public Sprite skillIcon;

        public Material iconVFX;

        public bool canPersist;

        internal List<bool> reactsWithBuilder = new();
        internal bool[] reactsWith;

        [Tooltip("Set at runtime, do not set manually")]
        public ElementIndex index
        {
            get
            {
                if (!ElementCatalog.availability.available) { Log.Warning("Can't get ElementIndex before catalog is initialized"); return ElementIndex.None; }
                return (ElementIndex)Array.IndexOf(ElementCatalog.elementCatalog, this);
            }
        }
        public override string ToString()
        {
            return Language.GetString(this.nameToken, Language.currentLanguageName);
        }

        public static ElementDef CreateElementDef(string internalName, string nameToken, Sprite icon, Sprite skillIcon, bool canPersist)
        {
            ElementDef elementDef = ScriptableObject.CreateInstance<ElementDef>();
            elementDef.cachedName = internalName;
            elementDef.nameToken = nameToken;
            elementDef.buff = AddNewBuff($"bd{internalName}", icon, Color.white, false, true, false);
            elementDef.skillIcon = skillIcon;
            elementDef.iconVFX = null;
            elementDef.canPersist = canPersist;
            return elementDef;
        }

        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff, bool hidden = false)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;
            buffDef.isHidden = hidden;

            Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }

    public enum ElementIndex
    {
        None = -1
    }

    public static class DefaultElementDefs
    {
        public static ElementDef pyroElement;
        public static ElementDef hydroElement;
        public static ElementDef electroElement;
        public static ElementDef cryoElement;
        public static ElementDef anemoElement;
        public static ElementDef geoElement;
        public static ElementDef dendroElement;

        public static void Initialize()
        {
            pyroElement = ElementDef.CreateElementDef("PyroElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_PYRO", 
                Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.texBuffOnFireIcon_tif).WaitForCompletion(), 
                null, 
                true);
            hydroElement = ElementDef.CreateElementDef("HydroElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_HYDRO",
                Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.texBuffBleedingIcon_tif).WaitForCompletion(),
                null,
                true);
            electroElement = ElementDef.CreateElementDef("ElectroElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_ELECTRO",
                Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Items_ShockDamageAura.texShockDamageAuraIcon_png).WaitForCompletion(),
                null,
                true);
            cryoElement = ElementDef.CreateElementDef("CryoElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_CRYO",
                Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.texChefFrostDebuffIcon_png).WaitForCompletion(),
                null,
                true);
            anemoElement = ElementDef.CreateElementDef("AnemoElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_ANEMO",
                Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Items_SpeedBoostPickup.texElusiveAntlersSpeedBuff_png).WaitForCompletion(),
                null,
                false);
            geoElement = ElementDef.CreateElementDef("GeoElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_GEO",
                Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSon.texEnergizedCoreBuffIcon_png).WaitForCompletion(),
                null,
                false);
            dendroElement = ElementDef.CreateElementDef("DendroElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_DENDRO",
                Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Treebot.texBuffEntangleIcon_tif).WaitForCompletion(),
                null,
                true);

            ElementCatalog.AddElementDefs([pyroElement, hydroElement, electroElement, cryoElement, anemoElement, geoElement, dendroElement]);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID))
            {
                RoR2Application.onLoad += LookingGlassSetup;
            }
        }

        private static void LookingGlassSetup()
        {
            if (Language.languagesByName.TryGetValue("en", out Language en))
            {
                RegisterLookingGlassBuff(en, pyroElement.buff, "Pyro Element", $"May react to other elements.");
            }
        }

        private static void RegisterLookingGlassBuff(Language lang, BuffDef buff, string name, string description)
        {
            LookingGlassLanguageAPI.SetupToken(lang, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(lang, $"DESCRIPTION_{buff.name}", description);
        }
    }
}
