using ElementalReactionsMod.Elements;
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
        public static ReactionAuraBuffDef quickenBuff;
        public static BuffDef superconductBuff;
        public static ReactionAuraBuffDef electroChargeBuff;
        public static ReactionAuraBuffDef burningBuff;
        public static ReactionAuraBuffDef strongBurningBuff;

        public static BuffDef delusionActiveBuff;

        public static ReactionAuraBuffDef lunarChargeBuff;
        public static BuffDefStockThresholdIcon lunarBloomBuff;
        public static BuffDef lunarCrystallizeBuff;

        public static BuffDef stellarConductFieldBuff;
        public static BuffDef stellarConductDebuff;

        public static BuffDef instructorsTeaCupQualityBase;

        public static BuffDef elementalEnvironmentHiddenBuff;
        
        public static void Initialize()
        {
            quickenBuff = Util.AddNewBuff<ReactionAuraBuffDef>("QuickenReaction", 
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.quickenBuffIcon).WaitForCompletion(), Color.white, false, false);
            superconductBuff = Util.AddNewBuff("SuperconductReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.superconductBuffIcon).WaitForCompletion(), Color.white, false, true);
            electroChargeBuff = Util.AddNewBuff<ReactionAuraBuffDef>("ElectroChargeReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.electroChargeBuffIcon).WaitForCompletion(), Color.white, false, false);
            electroChargeBuff.isDOT = true;
            burningBuff = Util.AddNewBuff<ReactionAuraBuffDef>("BurningReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.burningBuffIcon).WaitForCompletion(), Color.white, false, false);
            burningBuff.isDOT = true;
            strongBurningBuff = Util.AddNewBuff<ReactionAuraBuffDef>("StrongBurningReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.strongBurningBuffIcon).WaitForCompletion(), Color.white, false, false);
            strongBurningBuff.isDOT = true;

            delusionActiveBuff = Util.AddNewBuff("DelusionActive",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.delusionBuffIcon).WaitForCompletion(), Color.white, true, false);

            lunarChargeBuff = Util.AddNewBuff<ReactionAuraBuffDef>("LunarChargeReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.lunarChargeBuffIcon).WaitForCompletion(), Color.white, false, false);
            lunarChargeBuff.isDOT = true;

            lunarBloomBuff = Util.AddNewBuff<BuffDefStockThresholdIcon>("LunarBloomVerdantDew",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.lunarBloomChargingBuffIcon).WaitForCompletion(), Color.white, true, false);
            lunarBloomBuff.iconOverrides = [new BuffDefStockThresholdIcon.StackIconOverride { stackThreshold = StaticValues.lunarBloomVerdantDewCap,
                iconOverride = Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.lunarBloomBuffIcon).WaitForCompletion(), colorOverride = Color.white }];

            lunarCrystallizeBuff = Util.AddNewBuff("LunarCrystallizeMoondrift", null, Color.white, true, false, false, true);

            stellarConductFieldBuff = Util.AddNewBuff("StellarConductField", null, Color.white, false, false, false, true);
            stellarConductDebuff = Util.AddNewBuff("StellarConductReaction",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.stellarConductBuffIcon).WaitForCompletion(), Color.white, true, true);
            stellarConductDebuff.stackingDisplayMethod = R2API.BuffsAPI.RegisterStackingDisplayMethod((buff) => 
            {
                RoR2.UI.BuffIcon.sharedStringBuilder.AppendInt(Mathf.RoundToInt((StaticValues.stellarConductMinDamageMultiplier + (StaticValues.stellarConductDamageMultiplierPerStack * (buff.buffCount - 1))) * 100f));
                RoR2.UI.BuffIcon.sharedStringBuilder.Append("%"); 
            });

            elementalEnvironmentHiddenBuff = Util.AddNewBuff("HiddenElementalEnvironment", null, Color.white, false, false, false, true);

            if (ElementalReactionsPlugin.qualityModExists)
            {
                instructorsTeaCupQualityBase = Util.AddNewBuff("InstructorsTeaCupQuality",
                Addressables.LoadAssetAsync<Sprite>(Assets.AssetReferences.instructorsTeaCupItemIcon).WaitForCompletion(), Color.white, true, false);
            }
        }
    }

    public class ReactionAuraBuffDef : BuffDef
    {
        public ElementDef element1
        {
            get { return _element1; }
            set 
            {
                if (_element1 && _element1 == value) return;
                if (elementToReactionAuras.TryGetValue(value.buff, out var element1List))
                {
                    if (_element1) element1List.Remove(_element1.buff);
                    element1List.Add(this);
                }
                else
                {
                    elementToReactionAuras.Add(value.buff, [this]);
                }
                _element1 = value;
            }
        }
        private ElementDef _element1;
        public ElementDef element2
        {
            get { return _element2; }
            set
            {
                if (_element2 && _element2 == value) return;
                if (elementToReactionAuras.TryGetValue(value.buff, out var element1List))
                {
                    if (_element2) element1List.Remove(_element2.buff);
                    element1List.Add(this);
                }
                else
                {
                    elementToReactionAuras.Add(value.buff, [this]);
                }
                _element2 = value;
            }
        }
        private ElementDef _element2;

        public static Dictionary<BuffDef, List<BuffDef>> elementToReactionAuras = new Dictionary<BuffDef, List<BuffDef>>();

        // These buffs are always hidden, excluded from noxious thorn, and never timed.
        // Maybe make it not a debuff? I think hidden ones are still counted for death mark
        // Should ApplyElement run in onBuffFirstStackGained for Noxious Thorn? Exclude elements from noxious thorn?

        // In buff removed hook, remove this buff if element1 or element2 buffs are missing
        // Do some [SystemInitializer(typeof(BuffCatalog))] to save the element/aura connection for easy look-up
    }
}
