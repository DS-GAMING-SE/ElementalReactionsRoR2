using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using R2API;
using ElementalReactionsMod.Items;
using ElementalReactionsMod.Reactions;

namespace ElementalReactionsMod.Elements
{
    public class ElementDef : ScriptableObject
    {
        public string cachedName { get { return _cachedName; } set { name = value; _cachedName = value; } }
        private string _cachedName;
        public string nameToken;
        public string descriptionToken;
        internal string keywordToken;

        public Color color;

        public BuffDef buff;
        public BuffDef cooldownBuff;

        public Sprite skillIcon;

        public Material iconVFX;

        public bool canPersist;

        internal bool[] reactsWith;
        internal List<ReactionIndex> reactions = new List<ReactionIndex>();

        public bool hasDelusion;
        public ItemDef delusion;
        public Sprite delusionItemIcon;

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
        internal static ElementDef CreatePhysical(string internalName, string token, Color color, Sprite skillIcon)
        {
            ElementDef elementDef = ScriptableObject.CreateInstance<ElementDef>();
            elementDef.cachedName = internalName;
            elementDef.nameToken = token + "_NAME";
            elementDef.descriptionToken = token + "_DESCRIPTION";
            elementDef.color = color;
            elementDef.skillIcon = skillIcon;
            elementDef.hasDelusion = false;
            return elementDef;
        }
        public static ElementDef CreateElementDef(string internalName, string token, Color color, Sprite icon, Texture vfxIcon, Sprite skillIcon, bool canPersist, bool hasDelusion, Sprite delusionItemIcon)
        {
            ElementDef elementDef = ScriptableObject.CreateInstance<ElementDef>();
            elementDef.cachedName = internalName;
            elementDef.nameToken = token + "_NAME";
            elementDef.descriptionToken = token + "_DESCRIPTION";
            elementDef.color = color;
            elementDef.buff = Util.AddNewBuff(internalName, icon, color, false, true);
            elementDef.cooldownBuff = Util.AddNewBuff($"{internalName}Cooldown", icon, new Color(0.29f, 0.24f, 0.26f), false, false, false, true);
            elementDef.skillIcon = skillIcon;
            elementDef.iconVFX = Assets.CreateElementEffectMaterial(vfxIcon);
            elementDef.canPersist = canPersist;
            elementDef.hasDelusion = hasDelusion;
            elementDef.delusionItemIcon = delusionItemIcon;
            if (hasDelusion) elementDef.delusion = DelusionManager.CreateNewDelusion(elementDef);
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
            physicalElement = ElementDef.CreatePhysical("Physical", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_PHYSICAL", Color.white * 0.9f,
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.physicalSkillIcon).WaitForCompletion());
            pyroElement = ElementDef.CreateElementDef("Pyro", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_PYRO", new Color(0.9f, 0.51f, 0.384f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.pyroBuffIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Texture>(Assets.AssetReferences.pyroIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.pyroSkillIcon).WaitForCompletion(), 
                true, true,
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionCryoItemIcon).WaitForCompletion());
            hydroElement = ElementDef.CreateElementDef("Hydro", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_HYDRO", ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.hydroBuffIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Texture>(Assets.AssetReferences.hydroIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.hydroSkillIcon).WaitForCompletion(),
                true, true,
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionHydroItemIcon).WaitForCompletion());
            electroElement = ElementDef.CreateElementDef("Electro", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_ELECTRO", ColorCatalog.GetColor(ColorCatalog.ColorIndex.Utility),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.electroBuffIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Texture>(Assets.AssetReferences.electroIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.electroSkillIcon).WaitForCompletion(),
                true, true,
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionElectroItemIcon).WaitForCompletion());
            cryoElement = ElementDef.CreateElementDef("Cryo", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_CRYO", new Color(0.584f, 0.8f, 0.9f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.cryoBuffIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Texture>(Assets.AssetReferences.cryoIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.cryoSkillIcon).WaitForCompletion(),
                true, true,
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionCryoItemIcon).WaitForCompletion());
            anemoElement = ElementDef.CreateElementDef("Anemo", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_ANEMO", new Color(0.5f, 0.9f, 0.8f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.anemoBuffIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Texture>(Assets.AssetReferences.anemoIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.anemoSkillIcon).WaitForCompletion(),
                false, true,
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionAnemoItemIcon).WaitForCompletion());
            geoElement = ElementDef.CreateElementDef("Geo", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_GEO", new Color(0.9f, 0.788f, 0.384f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.geoBuffIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Texture>(Assets.AssetReferences.geoIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.geoSkillIcon).WaitForCompletion(),
                false, true,
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionGeoItemIcon).WaitForCompletion());
            dendroElement = ElementDef.CreateElementDef("Dendro", $"{ElementalReactionsPlugin.PREFIX}ELEMENT_DENDRO", new Color(0.612f, 0.9f, 0.384f),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.dendroBuffIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Texture>(Assets.AssetReferences.dendroIcon).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.dendroSkillIcon).WaitForCompletion(),
                true, true,
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionDendroItemIcon).WaitForCompletion());

            ElementCatalog.AddElementDefs([physicalElement, pyroElement, hydroElement, electroElement, cryoElement, anemoElement, geoElement, dendroElement]);
        }
    }
}
