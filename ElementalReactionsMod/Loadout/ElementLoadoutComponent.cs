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

        private void Start()
        {
            if (!NetworkServer.active && characterBody.isPlayerControlled)
            {
                new NetworkElementLoadout(characterBody.netId, primaryElement.index, secondaryElement.index, utilityElement.index, specialElement.index).Send(R2API.Networking.NetworkDestination.Server);
            }
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
        }
    }
}
