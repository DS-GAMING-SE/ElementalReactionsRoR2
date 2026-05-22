using ElementalReactionsMod.Elements;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod.Reactions
{
    public class ElementalReactionManager : MonoBehaviour
    {
        public static ElementalReactionManager instance;
        public void OnEnable()
        {
            SingletonHelper.Assign(ref instance, this);
        }
        public void OnDisable()
        {
            SingletonHelper.Unassign(ref instance, this);
        }

        public static void ApplyElement(ElementDef element, CharacterBody target)
        {
            if (element && target)
            {
                DamageInfo empty = new DamageInfo();
                ApplyElement(element, target, ref empty);
            }
        }
        public static void ApplyElement(ElementDef element, CharacterBody target, ref DamageInfo damageInfo)
        {
            if (element && element != DefaultElementDefs.physicalElement && !target.HasBuff(element.cooldownBuff))
            {
                bool reactionTriggered = false;
                ElementDef reacting = ElementalReactionCatalog.GetFirstReactableElement(element, target);
                if (reacting)
                {
                    ElementalReactionDef reaction = ElementalReactionCatalog.GetElementalReaction(element, reacting);
                    if (reaction)
                    {
                        target.ClearTimedBuffs(reacting.buff.buffIndex);
                        target.AddTimedBuff(element.cooldownBuff, StaticValues.elementRemovedICD);
                        target.AddTimedBuff(reacting.cooldownBuff, StaticValues.elementRemovedICD);
                        reaction.TriggerReaction(element, reacting, target, ref damageInfo);
                        reactionTriggered = true;
                    }
                }
                if (element.canPersist && !reactionTriggered)
                {
                    target.AddTimedBuff(element.buff, 10);
                    target.AddTimedBuff(element.cooldownBuff, StaticValues.elementAppliedICD);
                }
            }
        }
    }
}
