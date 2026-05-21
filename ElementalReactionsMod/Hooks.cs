using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using R2API;
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
                        if (element && element != DefaultElementDefs.physicalElement)
                        {
                            bool reactionTriggered = false;
                            ElementDef reacting = ElementalReactionCatalog.GetFirstReactableElement(element, self.body);
                            if (reacting)
                            {
                                ElementalReactionDef reaction = ElementalReactionCatalog.GetElementalReaction(element, reacting);
                                if (reaction)
                                {
                                    self.body.ClearTimedBuffs(reacting.buff.buffIndex);
                                    reaction.TriggerReaction(element, reacting, self, ref damage);
                                    reactionTriggered = true;
                                }
                            }
                            if (element.canPersist && !reactionTriggered)
                            {
                                self.body.AddTimedBuff(element.buff, 10, 1);
                            }
                        }

                        if (self.body.HasBuff(Buffs.superconductBuff) && element == DefaultElementDefs.physicalElement)
                        {
                            damage.damage *= StaticValues.superconductDamageMultiplier;
                        }

                        if (self.body.HasBuff(Buffs.quickenBuff) && element == DefaultElementDefs.dendroElement || element == DefaultElementDefs.electroElement)
                        {
                            damage.damage += StaticValues.quickenDamageAddCoefficient * damage.procCoefficient;
                        }

                        if (damage.damageType.HasModdedDamageType(DamageTypes.superconductDamageType))
                        {
                            self.body.AddTimedBuff(Buffs.superconductBuff, 5, 1);
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

                if (element == DefaultElementDefs.physicalElement && damageInfo.attacker && damageInfo.damageType.IsDamageSourceSkillBased && damageInfo.attacker.TryGetComponent<ElementLoadoutComponent>(out var elementLoadout))
                {
                    element = elementLoadout.GetElement(damageInfo.damageType.damageSource);
                    if (element) damageInfo.damageType.SetElement(element.index);
                }
            }
            orig(self, damageInfo);
        }
    }
}
