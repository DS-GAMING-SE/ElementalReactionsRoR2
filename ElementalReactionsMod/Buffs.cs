using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ElementalReactionsMod
{
    public static class Buffs
    {
        public static BuffDef quickenBuff;
        public static BuffDef superconductBuff;
        
        public static void Initialize()
        {
            quickenBuff = Util.AddNewBuff("bdElementalReactionsQuickenReaction", 
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.quickenBuffIcon).WaitForCompletion(), Color.white, false, true);
            superconductBuff = Util.AddNewBuff("bdElementalReactionsSuperconductReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.superconductBuffIcon).WaitForCompletion(), Color.white, false, true);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID))
            {
                RoR2Application.onLoad += LookingGlassSetup;
            }
        }

        private static void LookingGlassSetup()
        {
            if (Language.languagesByName.TryGetValue("en", out Language en))
            {
                Util.RegisterLookingGlassBuff(en, quickenBuff, "Quicken", $"Increases {Tokens.ElectroText("Electro")} and {Tokens.DendroText("Dendro")} base damage by {StaticValues.quickenDamageAddCoefficient * 100f}%.");
                Util.RegisterLookingGlassBuff(en, superconductBuff, "Superconduct", $"Increases non-elemental damage by {(StaticValues.superconductDamageMultiplier - 1f) * 100f}%.");
            }
        }
    }
}
