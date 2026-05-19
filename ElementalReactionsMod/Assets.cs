using ElementalReactionsMod.Reactions;
using R2API;
using RoR2.ExpansionManagement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ElementalReactionsMod
{
    public static class Assets
    {
        public static ExpansionDef elementalReactionExpansionDef;
        public static GameObject elementalReactionManagerPrefab;

        public static void Initialize()
        {
            elementalReactionManagerPrefab = PrefabAPI.CreateEmptyPrefab("ElementalReactionManager");
            elementalReactionManagerPrefab.AddComponent<ElementalReactionManager>();
            
            elementalReactionExpansionDef = ScriptableObject.CreateInstance<ExpansionDef>();
            elementalReactionExpansionDef.name = "ElementalReactionExpansionDef";
            elementalReactionExpansionDef.nameToken = $"{ElementalReactionsPlugin.PREFIX}EXPANSION_DEF_NAME";
            elementalReactionExpansionDef.descriptionToken = $"{ElementalReactionsPlugin.PREFIX}EXPANSION_DEF_DESCRIPTION";
            elementalReactionExpansionDef.iconSprite = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_UI.texVoidExpansionIcon_png).WaitForCompletion();
            elementalReactionExpansionDef.disabledIconSprite = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_UI.texVoidExpansionIcon_png).WaitForCompletion();
            elementalReactionExpansionDef.runBehaviorPrefab = elementalReactionManagerPrefab;

            Content.AddExpansionDef(elementalReactionExpansionDef);
        }
    }
}
