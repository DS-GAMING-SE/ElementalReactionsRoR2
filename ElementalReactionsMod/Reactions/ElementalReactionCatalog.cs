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
using Newtonsoft.Json.Utilities;

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
            foreach (var reaction in elementalReactionCatalog) // Filling out elementalReactionGrid and element.reactsWith
            {
                foreach (var element in reaction.reactingElements)
                {
                    if (reaction.baseElement.reactsWith[(int)element.index])
                    {
                        Log.Warning($"Reaction {reaction.ToString()} is trying to be added for two elements that already react. This reaction will need to be triggered manually to work");
                        continue;
                    }
                    elementalReactionGrid[(int)reaction.baseElement.index, (int)element.index] = reaction.index;
                    elementalReactionGrid[(int)element.index, (int)reaction.baseElement.index] = reaction.index;
                    reaction.baseElement.reactsWith[(int)element.index] = true;
                    element.reactsWith[(int)reaction.baseElement.index] = true;

                    element.reactions.AddDistinct(reaction.index);
                    reaction.baseElement.reactions.AddDistinct(reaction.index);
                }
            }
            for (int i = 0; i < ElementCatalog.elementCatalog.Length; i++) // Filling out keyword tokens
            {
                StringBuilder stringBuilder = HG.StringBuilderPool.RentStringBuilder();
                for (int j = 0; j < ElementCatalog.elementCatalog[i].reactions.Count; j++)
                {
                    ElementalReactionDef reactionDef = GetElementalReaction(ElementCatalog.elementCatalog[i].reactions[j]);
                    stringBuilder.Append(Tokens.KeywordText(Language.GetString(reactionDef.nameToken), Language.GetString(reactionDef.descriptionToken)));
                    stringBuilder.Append("\n\n");
                }
                ElementCatalog.elementCatalog[i].keywordToken = stringBuilder.ToString();
                stringBuilder = HG.StringBuilderPool.ReturnStringBuilder(stringBuilder);
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
            return elements.DefaultIfEmpty(ElementIndex.Physical).First(x => element.reactsWith[(int)x]);
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
