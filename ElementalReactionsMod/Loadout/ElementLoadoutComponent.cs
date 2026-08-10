using ElementalReactionsMod.Elements;
using HG;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.UI.GridLayoutGroup;

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

        public static ItemDef primaryElementItem;
        public static ItemDef secondaryElementItem;
        public static ItemDef utilityElementItem;
        public static ItemDef specialElementItem;
        public static ItemDef damageIsFromPlayerItem;

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
                            ElementalReactionManager.ApplyElement(permanentlyAppliedElement, characterBody, float.MaxValue, null, true);
                        }
                    }
                    if (specialAppliedElement && specialAppliedElementEndTime.hasPassed)
                    {
                        SetSpecialAppliedElement(null);
                    }
                }
            }
        }

        public static ElementDef GetElement(GameObject gameObject, DamageSource damageSource)
        {
            if (gameObject.TryGetComponent<CharacterBody>(out var body))
            {
                return GetElement(body, damageSource);
            }
            return DefaultElementDefs.physicalElement;
        }
        public static ElementDef GetElement(CharacterBody characterBody, DamageSource damageSource)
        {
            if (!characterBody) return DefaultElementDefs.physicalElement;
            Inventory inventory = characterBody.inventory ? characterBody.inventory : (characterBody.master && characterBody.master.inventory ? characterBody.master.inventory : null);
            switch (damageSource)
            {
                case DamageSource.Primary:
                    if (inventory)
                    {
                        ElementDef inventoryElement = GetElementItem(inventory, primaryElementItem);
                        if (inventoryElement && inventoryElement != DefaultElementDefs.physicalElement) return inventoryElement;
                    }
                    if (characterBody.TryGetComponent<ElementLoadoutComponent>(out var loadoutPrimary))
                    {
                        if (loadoutPrimary && loadoutPrimary.primaryElement && loadoutPrimary.primaryElement != DefaultElementDefs.physicalElement) return loadoutPrimary.primaryElement;
                    }
                    break;
                case DamageSource.Secondary:
                    if (inventory)
                    {
                        ElementDef inventoryElement = GetElementItem(inventory, secondaryElementItem);
                        if (inventoryElement && inventoryElement != DefaultElementDefs.physicalElement) return inventoryElement;
                    }
                    if (characterBody.TryGetComponent<ElementLoadoutComponent>(out var loadoutSecondary))
                    {
                        if (loadoutSecondary && loadoutSecondary.secondaryElement && loadoutSecondary.secondaryElement != DefaultElementDefs.physicalElement) return loadoutSecondary.secondaryElement;
                    }
                    break;
                case DamageSource.Utility:
                    if (inventory)
                    {
                        ElementDef inventoryElement = GetElementItem(inventory, utilityElementItem);
                        if (inventoryElement && inventoryElement != DefaultElementDefs.physicalElement) return inventoryElement;
                    }
                    if (characterBody.TryGetComponent<ElementLoadoutComponent>(out var loadoutUtility))
                    {
                        if (loadoutUtility && loadoutUtility.utilityElement && loadoutUtility.utilityElement != DefaultElementDefs.physicalElement) return loadoutUtility.utilityElement;
                    }
                    break;
                case DamageSource.Special:
                    if (inventory)
                    {
                        ElementDef inventoryElement = GetElementItem(inventory, specialElementItem);
                        if (inventoryElement && inventoryElement != DefaultElementDefs.physicalElement) return inventoryElement;
                    }
                    if (characterBody.TryGetComponent<ElementLoadoutComponent>(out var loadoutSpecial))
                    {
                        if (loadoutSpecial && loadoutSpecial.specialElement && loadoutSpecial.specialElement != DefaultElementDefs.physicalElement) return loadoutSpecial.specialElement;
                    }
                    break;
                default:
                    break;
            }
            return inventory ? Elites.GetFirstEliteElementDef(inventory) : DefaultElementDefs.physicalElement;
        }
        private static ElementDef GetElementItem(Inventory inventory, ItemDef loadoutItem)
        {
            return ElementCatalog.GetElementDef((ElementIndex)inventory.GetItemCountPermanent(loadoutItem));
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
            ApplyElementLoadout([ElementCatalog.GetElementDef(elements[0]), ElementCatalog.GetElementDef(elements[1]), ElementCatalog.GetElementDef(elements[2]), ElementCatalog.GetElementDef(elements[3])]);
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

        public static void Initialize()
        {
            primaryElementItem = Items.Items.AddNewItem("ElementPrimary", "ELEMENT_PRIMARY", false, null,
                null, null, null, ItemTag.WorldUnique, ItemTag.IgnoreForDropList);
            secondaryElementItem = Items.Items.AddNewItem("ElementSecondary", "ELEMENT_SECONDARY", false, null,
                null, null, null, ItemTag.WorldUnique, ItemTag.IgnoreForDropList);
            utilityElementItem = Items.Items.AddNewItem("ElementUtility", "ELEMENT_UTILITY", false, null,
                null, null, null, ItemTag.WorldUnique, ItemTag.IgnoreForDropList);
            specialElementItem = Items.Items.AddNewItem("ElementSpecial", "ELEMENT_SPECIAL", false, null,
                null, null, null, ItemTag.WorldUnique, ItemTag.IgnoreForDropList);
            damageIsFromPlayerItem = Items.Items.AddNewItem("ElementPlayer", "ELEMENT_PLAYER", false, null,
                null, null, null, ItemTag.WorldUnique, ItemTag.IgnoreForDropList);
        }

        public static void AddElementLoadoutComponents()
        {
            foreach (var survivor in SurvivorCatalog.allSurvivorDefs)
            {
                //string bodyName = BodyCatalog.GetBodyName(BodyCatalog.FindBodyIndex(survivor.bodyPrefab));
                survivor.bodyPrefab.EnsureComponent<ElementLoadoutComponent>();
                /*if (exists)
                {
                    loadout.ApplyElementLoadout(config);
                }*/
                if (survivor.cachedName == "Engi")
                {
                    survivor.bodyPrefab.AddComponent<EngineerTurretElements>();
                }
            }
            EnemyElementLoadouts.Initialize();
        }
    }
}
