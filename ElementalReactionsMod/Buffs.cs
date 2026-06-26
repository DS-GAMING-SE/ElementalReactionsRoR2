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
        public static BuffDef electroChargeBuff;

        public static BuffDef delusionActiveBuff;

        public static BuffDef lunarChargeBuff;
        public static BuffDefStockThresholdIcon lunarBloomBuff;
        public static BuffDef lunarCrystallizeBuff;

        public static BuffDef instructorsTeaCupQualityBase;
        
        public static void Initialize()
        {
            quickenBuff = Util.AddNewBuff("QuickenReaction", 
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.quickenBuffIcon).WaitForCompletion(), Color.white, false, true);
            superconductBuff = Util.AddNewBuff("SuperconductReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.superconductBuffIcon).WaitForCompletion(), Color.white, false, true);
            electroChargeBuff = Util.AddNewBuff("ElectroChargeReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.electroChargeBuffIcon).WaitForCompletion(), Color.white, false, true);
            electroChargeBuff.isDOT = true;

            delusionActiveBuff = Util.AddNewBuff("DelusionActive",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionBuffIcon).WaitForCompletion(), Color.white, true, false);

            lunarChargeBuff = Util.AddNewBuff("LunarChargeReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.lunarChargeBuffIcon).WaitForCompletion(), Color.white, false, true);
            lunarChargeBuff.isDOT = true;

            lunarBloomBuff = Util.AddNewBuff<BuffDefStockThresholdIcon>("LunarBloomVerdantDew",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.lunarBloomChargingBuffIcon).WaitForCompletion(), Color.white, true, false);
            lunarBloomBuff.iconOverrides = [new BuffDefStockThresholdIcon.StackIconOverride { stackThreshold = StaticValues.lunarBloomVerdantDewCap,
                iconOverride = Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.lunarBloomBuffIcon).WaitForCompletion(), colorOverride = Color.white }];

            lunarCrystallizeBuff = Util.AddNewBuff("LunarCrystallizeMoondrift", null, Color.white, true, false, false, true);

            if (ElementalReactionsPlugin.qualityModExists)
            {
                instructorsTeaCupQualityBase = Util.AddNewBuff("InstructorsTeaCupQuality",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.instructorsTeaCupItemIcon).WaitForCompletion(), Color.white, true, false);
            }
        }
    }
}
