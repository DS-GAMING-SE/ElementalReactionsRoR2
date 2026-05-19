using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;

namespace ElementalReactionsMod
{
    public static class OnHooks
    {
        public static void Initialize()
        {
            On.RoR2.HealthComponent.TakeDamageProcess += TakeDamageHook;
        }
        private static void TakeDamageHook(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self && ElementalReactionManager.instance)
            {
                ElementDef element = ElementCatalog.GetElementDef(damageInfo.damageType.GetElement());

                if (!element && damageInfo.attacker && damageInfo.damageType.IsDamageSourceSkillBased && damageInfo.attacker.TryGetComponent<ElementLoadoutComponent>(out var elementLoadout))
                {
                    element = elementLoadout.GetElement(damageInfo.damageType.damageSource);
                    if (element) damageInfo.damageType.SetElement(element.index);
                }

                if (element) Chat.AddMessage(element.ToString());
            }
            orig(self, damageInfo);
        }
    }
}
