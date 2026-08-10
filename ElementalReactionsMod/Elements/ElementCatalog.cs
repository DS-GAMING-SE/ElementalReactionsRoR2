using HG;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ElementalReactionsMod.Elements
{
    public static class ElementCatalog
    {
        public static ElementDef[] elementCatalog = Array.Empty<ElementDef>();
        public static BuffDef[] elementBuffs;

        public static ResourceAvailability availability = default(ResourceAvailability);

        internal static DamageAPI.ModdedDamageType[] elementIndexDamageTypeBits;

        [SystemInitializer]
        private static void SystemInit()
        {
            BakeElements();
            Log.Message("ElementCatalog initialized");
            availability.MakeAvailable();
        }

        public static void AddElementDefs(ElementDef[] elementDefs)
        {
            if (availability.available)
            {
                Log.Warning("Elements " + elementDefs + " are trying to be added after the catalog is initialized");
                return;
            }
            int length = elementCatalog.Length;
            Array.Resize(ref elementCatalog, length + elementDefs.Length);
            for (int i = 0; i < elementDefs.Length; i++)
            {
                // Adding element to catalog
                elementCatalog[length + i] = elementDefs[i];
            }

            string allElements = string.Concat(elementCatalog.Select(x => x.ToString() + "\n"));
            Log.Message("ElementDef(s) added to elementCatalog. elementCatalog now contains:\n" + allElements);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ElementDef GetElementDef(ElementIndex index)
        {
            return ArrayUtils.GetSafe(elementCatalog, (int)index);
        }

        internal static void BakeElements()
        {
            elementIndexDamageTypeBits = new DamageAPI.ModdedDamageType[Mathf.CeilToInt(Mathf.Log(elementCatalog.Length, 2))];
            for (int i = 0;i < elementIndexDamageTypeBits.Length; i++)
            {
                elementIndexDamageTypeBits[i] = DamageAPI.ReserveDamageType();
            }
            elementBuffs = new BuffDef[elementCatalog.Length];
            for (int i = 0; i < elementCatalog.Length; i++)
            {
                elementCatalog[i].reactsWith = new bool[elementCatalog.Length];
                elementBuffs[i] = elementCatalog[i].buff;
            }
        }
    }
}
