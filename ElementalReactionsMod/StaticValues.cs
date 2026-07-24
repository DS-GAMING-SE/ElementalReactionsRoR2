using System;
using System.Collections.Generic;
using System.Text;

namespace ElementalReactionsMod
{
    public static class StaticValues
    {
        #region Elements
        public const float elementAppliedICD = 0.4f;
        public const float elementRemovedICD = 0.2f;
        public const float permanentElementICD = 1f;

        public const float elementAppliedDuration = 10f;
        public const float elementAppliedTaxMultiplier = 0.8f;
        #endregion

        #region Reactions
        public const float genericReactionExplosionRadius = 10f;
        public const float genericReactionProcCoefficient = 0.5f;

        public const float vaporizeMultiplierPyroTrigger = 1.75f;
        public const float vaporizeMultiplierHydroTrigger = 1.5f;

        public const float overloadDamageCoefficient = 2.5f;

        public const float electroChargeDamageCoefficient = 1f;
        public const float electroChargeDuration = 5f;
        public const float electroChargeRadius = 15f;

        public const float superconductDamageCoefficient = 1f;
        public const float superconductDamageMultiplier = 1.3f;
        public const float superconductDuration = 5f;

        public const float swirlDamageCoefficient = 1.5f;

        public const float quickenDamageAddCoefficient = 0.5f;

        public const int crystallizeCap = 3;
        public const float crystallizeBarrierPercent = 0.1f;
        public const float crystallizeBossBarrierPercent = 0.03f;
        public const float crystallizeMaxBarrierPercent = 0.3f;
        public const float crystallizePlayerGravitateRange = 4f;
        public const float crystallizeEnemyGravitateRange = 7f;

        public const int bloomCap = 5;
        public const float bloomDamageCoefficient = 3.5f;
        public const float bloomDuration = 5f;
        public const float bloomRadius = 17f;
        public const float hyperBloomDamageCoefficient = 9f;
        public const float burgeonDamageCoefficient = 6f;
        public const float burgeonRadius = 22f;
        public const float hyperBloomRadius = 30f;
        #endregion

        #region Items
        public const int delusionDuration = 10;
        public const float delusionDamageCoefficient = 6f;
        public const float delusionStackDamageCoefficient = 6f;
        public const float delusionAttacksPerSecond = 0.5f;
        public const float delusionHealthPercentCost = 0.05f;
        public const float delusionHealingReceivedReduction = 0.2f;

        public const float instructorsTeaCupDamageMultiplier = 0.2f;

        public const float instructorsTeaCupQualityDamageIncrease = 0.025f;
        public const int instructorsTeaCupQualityMaxStacks = 10;
        public const int instructorsTeaCupQualityStacksPerQuality = 5;
        public const int instructorsTeaCupQualityDuration = 5;
        public const int instructorsTeaCupQualityDurationPerQuality = 3;

        #region Moon Wheel / Lunar Reactions
        public const float moonWheelLunarDamagePerStack = 0.5f;

        public const float moonWheelQualityChancePerQuality = 25f;

        public const float lunarChargeDamageCoefficient = 6f;
        public const float lunarChargeTimeBetweenAttacks = 1.5f;
        public const int lunarChargeAttacksPerDot = 4;
        public const float lunarChargeDotDuration = lunarChargeTimeBetweenAttacks * lunarChargeAttacksPerDot;
        public const float lunarChargeRadius = 6f;
        public const float lunarChargeEnemyDelay = 0.75f;

        public const int lunarBloomVerdantDewCap = 3;
        public const float lunarBloomDamageMultiplier = 2f;

        public const int lunarCrystallizeTriggersToAttack = 3;
        public const float lunarCrystallizeDamageCoefficient = 5f;
        public const float lunarCrystallizePlayerResistMultiplier = 0.25f;
        public const float lunarCrystallizeRadius = 50f;
        #endregion
        #endregion
    }
}
