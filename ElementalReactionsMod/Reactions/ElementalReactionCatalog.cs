using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using LookingGlass.LookingGlassLanguage;
using System.Linq;
using R2API;
using HG;
using ElementalReactionsMod.Elements;

namespace ElementalReactionsMod.Reactions
{
    public static class ElementalReactionCatalog
    {
        public static ElementalReactionDef[] elementalReactionCatalog = Array.Empty<ElementalReactionDef>();

        public static ReactionIndex[,] elementalReactionGrid;

        public static ResourceAvailability availability = default(ResourceAvailability);

        [SystemInitializer(typeof(ElementCatalog))]
        private static void SystemInit()
        {
            BakeElementalReactions();
            Log.Message("ElementalReactionCatalog initialized");
            availability.MakeAvailable();
        }

        public static void AddElementalReactionDefs(ElementalReactionDef[] elementalReactionDefs)
        {
            if (availability.available)
            {
                Log.Warning("Elemental Reactions " + elementalReactionDefs + " are trying to be added after the catalog is initialized");
                return;
            }
            int length = elementalReactionCatalog.Length;
            Array.Resize(ref elementalReactionCatalog, length + elementalReactionDefs.Length);
            for (int i = 0; i < elementalReactionDefs.Length; i++)
            {
                // Adding element to catalog
                elementalReactionCatalog[length + i] = elementalReactionDefs[i];
            }

            string allElements = string.Concat(elementalReactionCatalog.Select(x => x.ToString() + "\n"));
            Log.Message("ElementDef(s) added to elementalReactionCatalog. elementalReactionCatalog now contains:\n" + allElements);
        }

        internal static void BakeElementalReactions()
        {
            elementalReactionGrid = new ReactionIndex[ ElementCatalog.elementCatalog.Length, ElementCatalog.elementCatalog.Length ];
            foreach (var reaction in elementalReactionCatalog)
            {
                foreach (var element in reaction.reactingElements)
                {
                    elementalReactionGrid[(int)reaction.baseElement.index, (int)element.index] = reaction.index;
                    elementalReactionGrid[(int)element.index, (int)reaction.baseElement.index] = reaction.index;
                    reaction.baseElement.reactsWith[(int)element.index] = true;
                    element.reactsWith[(int)reaction.baseElement.index] = true;
                }
            }
        }
        public static ElementDef GetFirstReactableElement(ElementDef element, CharacterBody characterBody)
        {
            if (characterBody)
            {
                foreach (var item in ElementCatalog.elementCatalog)
                {
                    if (characterBody.HasBuff(item.buff) && element.reactsWith[(int)item.index])
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        public static ElementIndex GetFirstReactableElement(ElementDef element, ElementIndex[] elements)
        {
            return elements.DefaultIfEmpty(ElementIndex.None).First(x => element.reactsWith[(int)x]);
        }

        public static ElementalReactionDef GetElementalReaction(ElementDef element1, ElementDef element2)
        {
            if (element1 && element2)
            {
                return GetElementalReaction(elementalReactionGrid[(int)element1.index, (int)element2.index]);
            }
            return null;
        }

        public static ElementalReactionDef GetElementalReaction(ReactionIndex index)
        {
            return ArrayUtils.GetSafe(elementalReactionCatalog, (int)index);
        }
    }
}
