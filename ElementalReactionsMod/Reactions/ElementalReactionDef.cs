using ElementalReactionsMod.Elements;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ElementalReactionsMod.Elements.DefaultElementDefs;

namespace ElementalReactionsMod.Reactions
{
    public class ElementalReactionDef : ScriptableObject
    {
        public string cachedName { get { return _cachedName; } set { name = value; _cachedName = value; } }
        private string _cachedName;
        public string nameToken;
        public ElementDef baseElement;
        public ElementDef[] reactingElements;
        public ReactionIndex index
        {
            get
            {
                if (!ElementalReactionCatalog.availability.available) { Log.Warning("Can't get ReactionIndex before catalog is initialized"); return ReactionIndex.None; }
                return (ReactionIndex)Array.IndexOf(ElementalReactionCatalog.elementalReactionCatalog, this);
            }
        }
        public override string ToString()
        {
            return Language.GetString(this.nameToken, Language.currentLanguageName);
        }
        public static ElementalReactionDef CreateElementalReactionDef(string internalName, string nameToken, ElementDef baseElement, ElementDef reactingElement)
        {
            return CreateElementalReactionDef(internalName, nameToken, baseElement, [reactingElement]);
        }

        public static ElementalReactionDef CreateElementalReactionDef(string internalName, string nameToken, ElementDef baseElement, ElementDef[] reactingElements)
        {
            ElementalReactionDef reactionDef = ScriptableObject.CreateInstance<ElementalReactionDef>();
            reactionDef.cachedName = internalName;
            reactionDef.nameToken = nameToken;
            reactionDef.baseElement = baseElement;
            reactionDef.reactingElements = reactingElements;
            return reactionDef;
        }
    }

    public enum ReactionIndex
    {
        None = -1
    }

    public static class DefaultElementalReactions
    {
        public static ElementalReactionDef vaporize;
        public static ElementalReactionDef overload;
        public static ElementalReactionDef melt;
        public static ElementalReactionDef electroCharge;
        public static ElementalReactionDef frozen;
        public static ElementalReactionDef superConduct;
        public static ElementalReactionDef swirl;
        public static ElementalReactionDef crystallize;
        public static ElementalReactionDef burning;
        public static ElementalReactionDef bloom;
        public static ElementalReactionDef burgeon;
        public static ElementalReactionDef hyperBloom;
        public static ElementalReactionDef quicken;

        public static void Initialize()
        {
            vaporize = ElementalReactionDef.CreateElementalReactionDef("Vaporize", $"{ElementalReactionsPlugin.PREFIX}REACTION_VAPORIZE", pyroElement, hydroElement);

            ElementalReactionCatalog.AddElementalReactionDefs([vaporize]);
        }
    }
}
