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
        public string descriptionToken;

        public Color color;

        public BuffDef buff;
        public BuffDef cooldownBuff;

        public Sprite skillIcon;

        public Material iconVFX;

        public bool canPersist;

        internal bool[] reactsWith;

        [Tooltip("Set at runtime, do not set manually")]
        public ElementIndex index
        {
            get
            {
                return (ElementIndex)Array.IndexOf(ElementCatalog.elementCatalog, this);
            }
        }
        public override string ToString()
        {
            return Language.GetString(this.nameToken, Language.currentLanguageName);
        }
        internal static ElementDef CreatePhysical(string internalName, string nameToken, string descriptionToken, Color color, Sprite skillIcon)
        {
            ElementDef elementDef = ScriptableObject.CreateInstance<ElementDef>();
            elementDef.cachedName = internalName;
            elementDef.nameToken = nameToken;
            elementDef.descriptionToken = descriptionToken;
            elementDef.color = color;
            elementDef.skillIcon = skillIcon;
            return elementDef;
        }
        public static ElementDef CreateElementDef(string internalName, string nameToken, string descriptionToken, Color color, Sprite icon, Sprite skillIcon, bool canPersist)
        {
            ElementDef elementDef = ScriptableObject.CreateInstance<ElementDef>();
            elementDef.cachedName = internalName;
            elementDef.nameToken = nameToken;
            elementDef.descriptionToken = descriptionToken;
            elementDef.color = color;
            elementDef.buff = Util.AddNewBuff($"bdElementalReactions{internalName}", icon, color, false, true);
            elementDef.cooldownBuff = Util.AddNewBuff($"bdElementalReactions{internalName}Cooldown", icon, new Color(0.29f, 0.24f, 0.26f), false, false, true);
            elementDef.skillIcon = icon; // Replace with skillIcon
            elementDef.iconVFX = null;
            elementDef.canPersist = canPersist;
            return elementDef;
        }
    }

    public enum ElementIndex
    {
        Physical = 0
    }

    public static class DefaultElementDefs
    {
        public static ElementDef physicalElement;
        public static ElementDef pyroElement;
        public static ElementDef hydroElement;
        public static ElementDef electroElement;
        public static ElementDef cryoElement;
        public static ElementDef anemoElement;
        public static ElementDef geoElement;
        public static ElementDef dendroElement;

        public static void Initialize()
        {
            physicalElement = ElementDef.CreatePhysical("PhysicalElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_PHYSICAL", 
                $"{ElementalReactionsPlugin.PREFIX}ELEMENT_PHYSICAL_DESCRIPTION", Color.white * 0.9f,
                null);
            pyroElement = ElementDef.CreateElementDef("PyroElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_PYRO",
                $"{ElementalReactionsPlugin.PREFIX}ELEMENT_PYRO_DESCRIPTION", new Color(0.9f, 0.51f, 0.384f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.pyroBuffIcon).WaitForCompletion(), 
                null, 
                true);
            hydroElement = ElementDef.CreateElementDef("HydroElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_HYDRO",
                $"{ElementalReactionsPlugin.PREFIX}ELEMENT_HYDRO_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.hydroBuffIcon).WaitForCompletion(),
                null,
                true);
            electroElement = ElementDef.CreateElementDef("ElectroElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_ELECTRO",
                $"{ElementalReactionsPlugin.PREFIX}ELEMENT_ELECTRO_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.Utility),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.electroBuffIcon).WaitForCompletion(),
                null,
                true);
            cryoElement = ElementDef.CreateElementDef("CryoElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_CRYO",
                $"{ElementalReactionsPlugin.PREFIX}ELEMENT_CRYO_DESCRIPTION", new Color(0.584f, 0.8f, 0.9f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.cryoBuffIcon).WaitForCompletion(),
                null,
                true);
            anemoElement = ElementDef.CreateElementDef("AnemoElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_ANEMO",
                $"{ElementalReactionsPlugin.PREFIX}ELEMENT_ANEMO_DESCRIPTION", new Color(0.5f, 0.9f, 0.8f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.anemoBuffIcon).WaitForCompletion(),
                null,
                false);
            geoElement = ElementDef.CreateElementDef("GeoElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_GEO",
                $"{ElementalReactionsPlugin.PREFIX}ELEMENT_GEO_DESCRIPTION", new Color(0.9f, 0.788f, 0.384f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.geoBuffIcon).WaitForCompletion(),
                null,
                false);
            dendroElement = ElementDef.CreateElementDef("DendroElement", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_DENDRO",
                $"{ElementalReactionsPlugin.PREFIX}ELEMENT_DENDRO_DESCRIPTION", new Color(0.612f, 0.9f, 0.384f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.dendroBuffIcon).WaitForCompletion(),
                null,
                true);

            ElementCatalog.AddElementDefs([physicalElement, pyroElement, hydroElement, electroElement, cryoElement, anemoElement, geoElement, dendroElement]);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID))
            {
                RoR2Application.onLoad += LookingGlassSetup;
            }
        }

        private static void LookingGlassSetup()
        {
            if (Language.languagesByName.TryGetValue("en", out Language en))
            {
                Util.RegisterLookingGlassBuff(en, pyroElement.buff, "Pyro Element", $"May react to other elements.");
            }
        }
    }
}
