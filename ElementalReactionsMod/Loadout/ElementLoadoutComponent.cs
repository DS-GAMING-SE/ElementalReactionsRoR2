using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using ElementalReactionsMod.Elements;
using HG;
using ElementalReactionsMod.Reactions;
using UnityEngine.Networking;
using R2API.Networking.Interfaces;

namespace ElementalReactionsMod.Loadout
{
    [RequireComponent(typeof(CharacterBody))]
    public class ElementLoadoutComponent : MonoBehaviour
    {
        public ElementDef primaryElement;
        public ElementDef secondaryElement;
        public ElementDef utilityElement;
        public ElementDef specialElement;

        public ElementDef permanentlyAppliedElement
        {
            get
            {
                if (specialAppliedElement)
                {
                    return specialAppliedElement;
                }
                else
                {
                    return naturallyAppliedElement;
                }
            }
        }
        public bool permanentElementWasApplied;
        private float permanentElementStopwatch;
        public ElementDef naturallyAppliedElement;
        public ElementDef specialAppliedElement;
        public Run.FixedTimeStamp specialAppliedElementEndTime = Run.FixedTimeStamp.positiveInfinity;

        public CharacterBody characterBody;
        public SpecialObjectAttributes specialObjectAttributes;
        public TeamIndex team;
        private void Awake()
        {
            if (!ElementalReactionManager.instance)
            {
                this.enabled = false;
            }
            characterBody = GetComponent<CharacterBody>();
            specialObjectAttributes = GetComponent<SpecialObjectAttributes>();
        }

        private void Start()
        {
            if (characterBody.teamComponent)
            {
                team = characterBody.teamComponent.teamIndex;
            }
            if (!NetworkServer.active && characterBody.isPlayerControlled)
            {
                new NetworkElementLoadout(characterBody.netId, 
                    primaryElement ? primaryElement.index : ElementIndex.Physical,
                    secondaryElement ? secondaryElement.index : ElementIndex.Physical,
                    utilityElement ? utilityElement.index : ElementIndex.Physical,
                    specialElement ? specialElement.index : ElementIndex.Physical).Send(R2API.Networking.NetworkDestination.Server);
            }
            if (Config.CanEnemiesBeElemental().Value && permanentlyAppliedElement && specialObjectAttributes)
            {
                specialObjectAttributes.damageTypeOverride.SetElement(permanentlyAppliedElement.index);
            }
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (Config.CanEnemiesBeElemental().Value && permanentlyAppliedElement && permanentlyAppliedElement.buff)
                {
                    if (characterBody.HasBuff(permanentlyAppliedElement.buff))
                    {
                        permanentElementWasApplied = true;
                    }
                    else
                    {
                        if (permanentElementWasApplied)
                        {
                            permanentElementStopwatch = 0;
                            permanentElementWasApplied = false;
                        }
                        permanentElementStopwatch += Time.fixedDeltaTime;
                        if (permanentElementStopwatch >= StaticValues.permanentElementICD)
                        {
                            ElementalReactionManager.ApplyElement(permanentlyAppliedElement, characterBody, float.MaxValue, gameObject, true);
                        }
                    }
                    if (specialAppliedElement && specialAppliedElementEndTime.hasPassed)
                    {
                        SetSpecialAppliedElement(null);
                    }
                }
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
            primaryElement = elements[0];
            secondaryElement = elements[1];
            utilityElement = elements[2];
            specialElement = elements[3];
        }
        public void ApplyElementLoadout(ElementIndex[] elements)
        {
            primaryElement = ElementCatalog.GetElementDef(elements[0]);
            secondaryElement = ElementCatalog.GetElementDef(elements[1]);
            utilityElement = ElementCatalog.GetElementDef(elements[2]);
            specialElement = ElementCatalog.GetElementDef(elements[3]);
        }
        public void SetSpecialAppliedElement(ElementDef element)
        {
            if (!Config.CanEnemiesBeElemental().Value) return;
            
            if (element != specialAppliedElement)
            {
                AppliedElementChanged(specialAppliedElement, element);
                specialAppliedElement = element;
            }
            specialAppliedElementEndTime = Run.FixedTimeStamp.positiveInfinity;
        }
        public void SetSpecialAppliedElement(ElementDef element, float duration)
        {
            if (!Config.CanEnemiesBeElemental().Value) return;

            if (element != specialAppliedElement)
            {
                AppliedElementChanged(specialAppliedElement, element);
                specialAppliedElement = element;
            }
            specialAppliedElementEndTime = Run.FixedTimeStamp.now + duration;
        }
        private void AppliedElementChanged(ElementDef previous, ElementDef current)
        {
            if (specialObjectAttributes) specialObjectAttributes.damageTypeOverride.SetElement(current.index);
            if (previous && previous.buff && characterBody.HasBuff(previous.buff) && permanentElementWasApplied)
            {
                characterBody.RemoveBuff(previous.buff);
            }
            if (current && current.buff)
            {
                ElementalReactionManager.ApplyElement(current, characterBody, float.MaxValue);
            }
        }

        public static void AddElementLoadoutComponents()
        {
            foreach (var survivor in SurvivorCatalog.allSurvivorDefs)
            {
                ElementDef[] config = Config.GetElementLoadoutFromConfig(BodyCatalog.GetBodyName(BodyCatalog.FindBodyIndex(survivor.bodyPrefab)), out var exists);
                ElementLoadoutComponent loadout = survivor.bodyPrefab.EnsureComponent<ElementLoadoutComponent>();
                if (exists)
                {
                    loadout.ApplyElementLoadout(config);
                }
            }
            EnemyElementLoadouts.Initialize();
        }
    }
}
