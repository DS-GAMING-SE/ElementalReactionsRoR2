using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElementalReactionsMod
{
    public static class Hooks
    {
        public static void Initialize()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += TakeDamageIL;
            On.RoR2.HealthComponent.TakeDamageProcess += TakeDamageHook;
        }
        private static void TakeDamageIL(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchLdfld<HealthComponent>(nameof(HealthComponent.onIncomingDamageReceivers))))
            {
                c.Emit(OpCodes.Ldarg_0); // healthcomponent
                c.Emit(OpCodes.Ldarg_1); // damageInfo
                c.EmitDelegate<Action<HealthComponent, DamageInfo>>((self, damage) =>
                {
                    if (!damage.rejected && ElementalReactionManager.instance)
                    {
                        ElementDef element = ElementCatalog.GetElementDef(damage.damageType.GetElement());
                        if (element)
                        {
                            ElementDef reacting = ElementalReactionCatalog.GetFirstReactableElement(element, self.body);
                            if (reacting)
                            {
                                ElementalReactionDef reaction = ElementalReactionCatalog.GetElementalReaction(element, reacting);
                                if (reaction)
                                {
                                    self.body.ClearTimedBuffs(element.buff.buffIndex);
                                    reaction.TriggerReaction(element, reacting, self, ref damage);
                                    return;
                                }
                            }
                            if (element.canPersist)
                            {
                                self.body.AddTimedBuff(element.buff, 10, 1);
                            }
                        }
                    }
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
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
            }
            orig(self, damageInfo);
        }
    }
}
