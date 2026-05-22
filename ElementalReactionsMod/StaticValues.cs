using System;
using System.Collections.Generic;
using System.Text;

namespace ElementalReactionsMod
{
    public static class StaticValues
    {
        #region Elements
        public const float elementAppliedICD = 1f;
        public const float elementRemovedICD = 0.5f;
        #endregion

        #region Reactions
        public const float genericReactionExplosionRadius = 8f;
        public const float vaporizeMultiplierPyroTrigger = 2f;
        public const float vaporizeMultiplierHydroTrigger = 1.5f;

        public const float overloadDamageCoefficient = 2f;

        public const float electroChargeDamageCoefficient = 1f;

        public const float superconductDamageCoefficient = 1f;
        public const float superconductDamageMultiplier = 1.2f;

        public const float swirlDamageCoefficient = 2f;

        public const float quickenDamageAddCoefficient = 0.3f;
        #endregion
    }
}
