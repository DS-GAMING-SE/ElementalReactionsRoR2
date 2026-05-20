using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElementalReactionsMod
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType elementalReactionDamageType;
        public static DamageAPI.ModdedDamageType superconductDamageType;
        public static DamageAPI.ModdedDamageType lunarReactionDamageType;

        public static void Initialize()
        {
            elementalReactionDamageType = DamageAPI.ReserveDamageType();
            superconductDamageType = DamageAPI.ReserveDamageType();
            lunarReactionDamageType = DamageAPI.ReserveDamageType();
        }
    }
}
