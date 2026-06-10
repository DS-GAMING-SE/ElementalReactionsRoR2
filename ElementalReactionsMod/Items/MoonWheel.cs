using ElementalReactionsMod.Reactions;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace ElementalReactionsMod.Items
{
    public static class MoonWheel
    {
        public static void Initialize()
        {
            ElementalReactionManager.onPreElementalReactionTriggered += (ref reaction, element1, element2, victim, ref damage) =>
            {
                if (damage.attacker && damage.attacker.TryGetComponent<CharacterBody>(out var attackerBody) && attackerBody.inventory && attackerBody.inventory.GetItemCountWithQuality(Items.moonWheel) > 0)
                {
                    if (reaction == DefaultElementalReactions.electroCharge)
                    {
                        reaction = DefaultElementalReactions.lunarCharge;
                    }
                    else if (reaction == DefaultElementalReactions.bloom)
                    {
                        reaction = DefaultElementalReactions.lunarBloom;
                    }
                    else if (reaction == DefaultElementalReactions.crystallize)
                    {
                        reaction = DefaultElementalReactions.lunarCrystallize;
                    }
                }
            };
        }
    }
}
