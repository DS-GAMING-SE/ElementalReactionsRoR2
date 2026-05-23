using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace ElementalReactionsMod
{
    public static class Stats
    {
        public static void Initialize()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStats;
        }
        public static void RecalculateStats(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs stats)
        {

        }
    }
}
