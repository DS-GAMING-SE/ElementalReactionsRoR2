using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;

namespace ElementalReactionsMod.Elements
{
    [RequireComponent(typeof(CharacterBody))]
    public class ElementLoadoutComponent : MonoBehaviour
    {
        public ElementDef primaryElement;
        public ElementDef secondaryElement;
        public ElementDef utilityElement;
        public ElementDef specialElement;

        public ElementDef GetElement(DamageSource damageSource)
        {
            switch (damageSource)
            {
                case DamageSource.Primary:
                    return primaryElement;
                case DamageSource.Secondary:
                    return secondaryElement;
                case DamageSource.Utility:
                    return utilityElement;
                case DamageSource.Special:
                    return specialElement;
                default:
                    return null;
            }
        }

        public void ApplyElementLoadout(ElementIndex[] elements)
        {
            primaryElement = ElementCatalog.GetElementDef(elements[0]);
            secondaryElement = ElementCatalog.GetElementDef(elements[1]);
            utilityElement = ElementCatalog.GetElementDef(elements[2]);
            specialElement = ElementCatalog.GetElementDef(elements[3]);
        }

        [SystemInitializer(typeof(SurvivorCatalog), typeof(ElementCatalog))]
        public static void AddElementLoadoutComponents()
        {
            foreach (var survivor in SurvivorCatalog.allSurvivorDefs)
            {
                survivor.bodyPrefab.AddComponent<ElementLoadoutComponent>().ApplyElementLoadout([(ElementIndex)0, (ElementIndex)1, (ElementIndex)2, (ElementIndex)3]);
            }
        }
    }
}
