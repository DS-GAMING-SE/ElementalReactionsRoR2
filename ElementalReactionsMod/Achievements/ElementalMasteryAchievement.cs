using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using ElementalReactionsMod.Reactions;

namespace ElementalReactionsMod.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 3U, null)]
    public class ElementalMasteryAchievement : BaseEndingAchievement
    {
        public const string identifier = ElementalReactionsPlugin.PREFIX + "elementalMasteryAchievement";
        public const string unlockableIdentifier = ElementalReactionsPlugin.PREFIX + "elementalMasteryUnlockable";
        public override bool ShouldGrant(RunReport runReport)
        {
            return runReport.gameEnding == RoR2Content.GameEndings.MainEnding && ElementalReactionManager.instance;
        }
    }
}
