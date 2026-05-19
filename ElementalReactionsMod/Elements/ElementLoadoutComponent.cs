using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;

namespace ElementalReactionsMod.Elements
{
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
    }
}
