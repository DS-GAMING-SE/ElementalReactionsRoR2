using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using ElementalReactionsMod.Elements;
using HG;
using ElementalReactionsMod.Reactions;

namespace ElementalReactionsMod.Loadout
{
    [RequireComponent(typeof(CharacterBody))]
    public class ElementLoadoutComponent : MonoBehaviour
    {
        public ElementDef primaryElement;
        public ElementDef secondaryElement;
        public ElementDef utilityElement;
        public ElementDef specialElement;

        public ElementDef permanentlyAppliedElement;

        public CharacterBody characterBody;
        private void Awake()
        {
            if (!ElementalReactionManager.instance)
            {
                this.enabled = false;
            }
            characterBody = GetComponent<CharacterBody>();
        }

        private void FixedUpdate()
        {
            if (permanentlyAppliedElement && permanentlyAppliedElement.buff)
            {
                characterBody.SetBuffCount(permanentlyAppliedElement.buff.buffIndex, 1);
            }
        }

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

        public void ApplyElementLoadout(ElementDef[] elements)
        {
            primaryElement = ElementCatalog.GetElementDef(elements[0].index);
            secondaryElement = ElementCatalog.GetElementDef(elements[1].index);
            utilityElement = ElementCatalog.GetElementDef(elements[2].index);
            specialElement = ElementCatalog.GetElementDef(elements[3].index);
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
                survivor.bodyPrefab.EnsureComponent<ElementLoadoutComponent>();
            }
        }
    }
}
