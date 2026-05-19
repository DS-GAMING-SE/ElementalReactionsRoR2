using ElementalReactionsMod.Elements;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod
{
    public static class Util
    {
        public static ElementIndex GetElement(this DamageTypeCombo damageTypeCombo)
        {
            return (ElementIndex)GetDamageTypeIndex(damageTypeCombo, ElementCatalog.elementIndexDamageTypeBits);
        }

        public static void SetElement(this DamageTypeCombo damageTypeCombo, ElementIndex elementIndex)
        {
            for (int i = 0; i < ElementCatalog.elementIndexDamageTypeBits.Length; i++)
            {
                if (GetBit((int)elementIndex, i))
                {
                    damageTypeCombo.AddModdedDamageType(ElementCatalog.elementIndexDamageTypeBits[i]);
                }
                else
                {
                    damageTypeCombo.RemoveModdedDamageType(ElementCatalog.elementIndexDamageTypeBits[i]);
                }
            }
        }

        public static bool IsElementalDamage(this DamageTypeCombo damageTypeCombo)
        {
            foreach (var item in ElementCatalog.elementIndexDamageTypeBits)
            {
                if (damageTypeCombo.HasModdedDamageType(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetDamageTypeIndex(DamageTypeCombo damageTypeCombo, DamageAPI.ModdedDamageType[] damageTypes)
        {
            int index = 0;
            for (int i = 0; i < damageTypes.Length; i++)
            {
                if (damageTypeCombo.HasModdedDamageType(damageTypes[i]))
                {
                    index += (int)Mathf.Pow(2, i);
                }
            }
            return index - 1;
        }

        public static bool GetBit(int num, int bitIndex)
        {
            return (num & (1 << bitIndex)) != 0;
        }
    }
}
