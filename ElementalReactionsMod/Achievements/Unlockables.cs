using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod.Achievements
{
    public static class Unlockables
    {
        public static UnlockableDef elementalMasteryUnlockableDef;

        public static void Initialize()
        {
            elementalMasteryUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            elementalMasteryUnlockableDef.achievementIconRef = Assets.AssetReferences.elementalMasteryAchievementIcon;
            elementalMasteryUnlockableDef.cachedName = ElementalMasteryAchievement.unlockableIdentifier;
            elementalMasteryUnlockableDef.nameToken = Tokens.GetAchievementNameToken(ElementalMasteryAchievement.identifier);
            Content.AddUnlockableDef(elementalMasteryUnlockableDef);
        }
    }
}
