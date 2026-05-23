using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

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

            #region Items

            #endregion
        }

        public static Material CreateVisionMaterial(AssetReferenceT<Texture> icon, AssetReferenceT<Texture> remapTex)
        {
            Material vision = new Material(Addressables.LoadAssetAsync<Shader>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Shaders.HGOpaqueCloudRemap_shader).WaitForCompletion());
            vision.EnableKeyword("EMISSIONFROMALBEDO");
            vision.EnableKeyword("DITHER");
            vision.EnableKeyword("USE_CLOUDS");
            vision.EnableKeyword("USE_UV1");
            vision.EnableKeyword("_EMISSION");
            AssetAsyncReferenceManager<Texture>.LoadAsset(icon).Completed += x =>
            {
                vision.SetTexture("_MainTex", x.Result);
            };
            AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.texChefOilDecalMask_png)).Completed += x =>
            {
                vision.SetTexture("_Cloud1Tex", x.Result);
                vision.SetTextureScale("_Cloud1Tex", new Vector2(1, 0.3f));
            };
            AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX_ParticleMasks.texAlphaGradient2_png)).Completed += x =>
            {
                vision.SetTexture("_Cloud2Tex", x.Result);
                vision.SetTextureScale("_Cloud2Tex", new Vector2(1, -1));
                vision.SetTextureOffset("_Cloud2Tex", new Vector2(0, 1.08f));
            };
            AssetAsyncReferenceManager<Texture>.LoadAsset(remapTex).Completed += x =>
            {
                vision.SetTexture("_RemapTex", x.Result);
            };
            vision.SetVector("_CutoffScroll", new Vector4(0, -1.5f, 0, 0));
            vision.SetFloat("_AlphaBoost", 0.75f);
            vision.SetFloat("_Cutoff", 0f);
            vision.Specular(0.7f, 9f, false);
            vision.SetFloat("_RampInfo", 1);

            return vision;
        }

        [SystemInitializer(typeof(ElementCatalog))]
        public static void AddElementLoadoutMenu()
        {
            GameObject characterSelectMenu = AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_UI.CharacterSelectUIMain_prefab)).WaitForCompletion();
            CharacterSelectController characterSelectController = characterSelectMenu.GetComponent<CharacterSelectController>();
            Array.Resize(ref characterSelectController.primaryColorImages, characterSelectController.primaryColorImages.Length + 1);
            Transform menu = characterSelectMenu.transform.Find("SafeArea/LeftHandPanel (Layer: Main)/SurvivorInfoPanel, Active (Layer: Secondary)");
            Transform headerButtons = menu.Find("SubheaderPanel (Overview, Skills, Loadout)");
            Transform contentPanel = menu.Find("ContentPanel (Overview, Skills, Loadout)");
            GameObject elementButton = GameObject.Instantiate(headerButtons.Find("GenericMenuButton (Loadout)").gameObject);
            elementButton.transform.SetParent(headerButtons, false);
            elementButton.transform.localScale = Vector3.one; // why do unity transforms break so damn hard when parenting anything
            elementButton.transform.localEulerAngles = Vector3.zero;
            elementButton.name = "GenericMenuButton (Elements)";
            elementButton.transform.SetSiblingIndex(4);
            elementButton.GetComponent<LanguageTextMeshController>().token = $"{ElementalReactionsPlugin.PREFIX}LOADOUT_ELEMENTS";
            characterSelectController.primaryColorImages[characterSelectController.primaryColorImages.Length - 1] = elementButton.GetComponent<Image>();

            GameObject elementPanel = GameObject.Instantiate(contentPanel.Find("LoadoutPanel").gameObject);
            elementPanel.transform.SetParent(contentPanel, false);
            elementPanel.transform.localScale = Vector3.one;
            elementPanel.transform.localEulerAngles = Vector3.zero;
            elementPanel.name = "ElementsPanel";
            GameObject.Destroy(elementPanel.GetComponent<LoadoutPanelController>());
            elementPanel.SetActive(false);
            ElementLoadoutPanelController elementPanelController = elementPanel.AddComponent<ElementLoadoutPanelController>();
            elementPanelController.hoverTextDescription = elementPanelController.transform.Find("DescriptionPanel, Loadout/DescriptionPanelContent (Layer: Secondary)/DescriptionText").GetComponent<LanguageTextMeshController>();
            elementPanelController.requiredUILayerKey = menu.GetComponent<UILayerKey>();

            headerButtons.GetComponent<HGHeaderNavigationController>().headers.Add(new HGHeaderNavigationController.Header { 
                headerButton = elementButton.GetComponent<HGButton>(),
                headerName = Language.GetString($"{ElementalReactionsPlugin.PREFIX}LOADOUT_ELEMENTS"),
                tmpHeaderText = elementButton.transform.Find("ButtonText").GetComponent<HGTextMeshProUGUI>(),
                headerRoot = elementPanel,
                AreConsolePlatformsSupported = true,
                isPrimaryPlayerOnly = false
            });
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

            #region Items
            #region Common
            public static AssetReferenceT<Material> visionHolderMaterial = new AssetReferenceT<Material>("e3301a4ccd084f4428b3b23e85dc1733");
            #endregion
            #region Delusion
            public static AssetReferenceT<GameObject> delusionPickupModel = new AssetReferenceT<GameObject>("9f3cf544c7630a04fa25214a5197c191");
            public static AssetReferenceT<Texture> delusionLogo = new AssetReferenceT<Texture>("8c75207915d01ff4280ac8f0e15b5aad");
            public static AssetReferenceT<Sprite> delusionItemIcon = new AssetReferenceT<Sprite>("884bdf224e0646e43b6dc1b6a2675c92");

            public static AssetReferenceT<Sprite> delusionCooldownBuffIcon = new AssetReferenceT<Sprite>("11b881fd7c08c0b4faf1b305df7e394d");
            public static AssetReferenceT<Sprite> delusionReadyBuffIcon = new AssetReferenceT<Sprite>("da2c01d04bcb15f43848d28d22db15d9");
            public static AssetReferenceT<Sprite> delusionActiveBuffIcon = new AssetReferenceT<Sprite>("c48688fe6fab6304badbca799ca382be");
            #endregion
            #endregion
        }
    }
}
