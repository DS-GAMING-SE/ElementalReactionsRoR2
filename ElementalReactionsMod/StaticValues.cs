using System;
using System.Collections.Generic;
using System.Text;

namespace ElementalReactionsMod
{
    public static class StaticValues
    {
        #region Elements
        public const float elementAppliedICD = 0.3f;
        public const float elementRemovedICD = 0.15f;
        public const float permanentElementICD = 1f;

        public const float elementAppliedDuration = 10f;
        public const float elementAppliedTaxMultiplier = 0.8f;
        #endregion

        #region Reactions
        public const float genericReactionExplosionRadius = 10f;
        public const float genericReactionProcCoefficient = 0.5f;

        public const float vaporizeMultiplierPyroTrigger = 1.75f;
        public const float vaporizeMultiplierHydroTrigger = 1.5f;

        public const float overloadDamageCoefficient = 2f;

        public const float electroChargeDamageCoefficient = 0.7f;
        public const float electroChargeDuration = 5f;
        public const float electroChargeRadius = 15f;

        public const float superconductDamageCoefficient = 1f;
        public const float superconductDamageMultiplier = 1.3f;

        public const float swirlDamageCoefficient = 2f;

        public const float quickenDamageAddCoefficient = 0.3f;

        public const int crystallizeCap = 3;
        public const float crystallizeBarrierPercent = 0.1f;
        public const float crystallizeMaxBarrierPercent = 0.3f;
        public const float crystallizePlayerGravitateRange = 3f;
        public const float crystallizeEnemyGravitateRange = 6f;

        public const int bloomCap = 5;
        public const float bloomDamageCoefficient = 2f;
        public const float bloomDuration = 5f;
        public const float hyperBloomDamageCoefficient = 3f;
        public const float burgeonDamageCoefficient = 4f;
        public const float burgeonRadius = 15f;
        public const float hyperBloomRadius = 20f;
        #endregion

        #region Items
        public const int delusionDuration = 10;
        public const int delusionCooldown = 10;
        public const float delusionDamageCoefficient = 6f;
        public const float delusionStackDamageCoefficient = 6f;
        public const float delusionAttacksPerSecond = 0.5f;
        public const float delusionHealthPercentCost = 0.05f;
        public const float delusionHealingReceivedReduction = 0.3f;

        public const float instructorsTeaCupDamageMultiplier = 0.2f;

        #region Moon Wheel / Lunar Reactions
        /* 
         * Columbina's 4th passive could make for good Quality Moon Wheel
         * Lunar Charged has a 33% chance of striking lightning an additional time
         * Lunar Bloom, basically, increases Verdant Dew cap
         * Lunar Crystallize, each Moondrift has a 33% chance of attacking an extra time
         */
        public const float moonWheelLunarDamagePerStack = 0.5f;

        public const float lunarChargeDamageCoefficient = 5f;
        public const float lunarChargeTimeBetweenAttacks = 1.5f;
        public const int lunarChargeAttacksPerDot = 4;
        public const float lunarChargeDotDuration = lunarChargeTimeBetweenAttacks * lunarChargeAttacksPerDot;
        public const float lunarChargeRadius = 6f;

        public const int lunarBloomVerdantDewCap = 3;
        public const float lunarBloomDamageMultiplier = 1.5f;

        public const int lunarCrystallizeTriggersToAttack = 3;
        public const float lunarCrystallizeDamageCoefficient = 3f;
        #endregion
        #endregion
    }
}
