using ElementalReactionsMod.Reactions;
using R2API;
using RoR2.ExpansionManagement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;

namespace ElementalReactionsMod
{
    public static class Assets
    {
        public static ExpansionDef elementalReactionExpansionDef;
        public static GameObject elementalReactionManagerPrefab;
        public static string AddressablesDirectory { get; private set; }
        internal static void LoadAddressables()
        {
            AddressablesDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ElementalReactionsPlugin.instance.Info.Location), "Addressables");
            Addressables.LoadContentCatalogAsync(System.IO.Path.Combine(AddressablesDirectory, "catalog.json")).WaitForCompletion();
        }
        public static void Initialize()
        {
            LoadAddressables();
            
            elementalReactionManagerPrefab = PrefabAPI.CreateEmptyPrefab("ElementalReactionManager");
            elementalReactionManagerPrefab.AddComponent<ElementalReactionManager>();

            elementalReactionExpansionDef = ScriptableObject.CreateInstance<ExpansionDef>();
            elementalReactionExpansionDef.name = "ElementalReactionExpansionDef";
            elementalReactionExpansionDef.nameToken = $"{ElementalReactionsPlugin.PREFIX}EXPANSION_NAME";
            elementalReactionExpansionDef.descriptionToken = $"{ElementalReactionsPlugin.PREFIX}EXPANSION_DESCRIPTION";
            elementalReactionExpansionDef.iconSprite = Addressables.LoadAssetAsync<Sprite>(AssetReferences.expansionIcon).WaitForCompletion();
            elementalReactionExpansionDef.disabledIconSprite = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_MiscIcons.texUnlockIcon_png).WaitForCompletion();
            elementalReactionExpansionDef.runBehaviorPrefab = elementalReactionManagerPrefab;
            elementalReactionExpansionDef.requiredEntitlement = null;

            Content.AddExpansionDef(elementalReactionExpansionDef);

            elementalReactionManagerPrefab.AddComponent<ExpansionRequirementComponent>().requiredExpansion = elementalReactionExpansionDef;
        }

        public static class AssetReferences
        {
            public static AssetReferenceT<Sprite> expansionIcon = new AssetReferenceT<Sprite>("64c84609674948a4d80e1fc588a1d8a9");

            public static AssetReferenceT<Sprite> pyroBuffIcon = new AssetReferenceT<Sprite>("0daee0d7c66cf3349a5c729e74a12925");
            public static AssetReferenceT<Sprite> hydroBuffIcon = new AssetReferenceT<Sprite>("7e274e591617cd94f851984046f5c921");
            public static AssetReferenceT<Sprite> electroBuffIcon = new AssetReferenceT<Sprite>("a416b58f4ed2331488afcf4e6eba114f");
            public static AssetReferenceT<Sprite> cryoBuffIcon = new AssetReferenceT<Sprite>("e38221f0cc59ebd4f96bfcc5034fde14");
            public static AssetReferenceT<Sprite> anemoBuffIcon = new AssetReferenceT<Sprite>("2e957546a6b4ecf4088b8cb9a754aaaf");
            public static AssetReferenceT<Sprite> geoBuffIcon = new AssetReferenceT<Sprite>("61c6caa41fc7e8c4abf6534fadeb89b0");
            public static AssetReferenceT<Sprite> dendroBuffIcon = new AssetReferenceT<Sprite>("87b63bb49e9e2fa4f9d2a66665ee07a7");

            public static AssetReferenceT<Sprite> quickenBuffIcon = new AssetReferenceT<Sprite>("fd1a80b8adab48644bde7e4c5d73fd13");
            public static AssetReferenceT<Sprite> superconductBuffIcon = new AssetReferenceT<Sprite>("5fc2055e4d7c33348889a483e5a0df1b");
        }
    }
}
