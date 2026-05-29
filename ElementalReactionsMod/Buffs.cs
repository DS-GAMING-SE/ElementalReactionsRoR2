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

        public static BuffDef delusionActiveBuff;
        
        public static void Initialize()
        {
            quickenBuff = Util.AddNewBuff("QuickenReaction", 
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.quickenBuffIcon).WaitForCompletion(), Color.white, false, true);
            superconductBuff = Util.AddNewBuff("SuperconductReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.superconductBuffIcon).WaitForCompletion(), Color.white, false, true);
            delusionActiveBuff = Util.AddNewBuff("DelusionActive",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionActiveBuffIcon).WaitForCompletion(), Color.white, false, false, false);
        }
    }
}
