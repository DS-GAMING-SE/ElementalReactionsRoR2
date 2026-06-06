using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Items;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using R2API;
using RoR2;
using RoR2.EntitlementManagement;
using RoR2.ExpansionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod
{
    public static class Hooks
    {
        public static void Initialize()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += TakeDamageIL;
            On.RoR2.HealthComponent.TakeDamageProcess += ApplyElementToSkillDamage;
            On.RoR2.UI.LogBook.LogBookController.CanSelectItemEntry += NoDelusionsWithElementInLogbook;
            On.RoR2.HealthComponent.Heal += HealingReceivedDebuff;
            On.RoR2.OverheatSystem.BodyOverheatInfo.AddChanneledBuff += AddOverheatPyro;
            IL.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += SolusWingCoolingCryo;
        }
        // Right after IOnincomingDamageReceiver does its thing, since that's where many things (including bloom dendro cores) reject damage
        private static void TakeDamageIL(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchLdfld<HealthComponent>(nameof(HealthComponent.onIncomingDamageReceivers))) && 
                c.TryGotoNext(MoveType.After, x => x.MatchBrfalse(out ILLabel falseBranch), x => x.MatchRet(), x => x.MatchLdloc(0)))
            {
                c.Emit(OpCodes.Ldarg_0); // healthcomponent
                c.Emit(OpCodes.Ldarg_1); // damageInfo
                c.EmitDelegate<Action<HealthComponent, DamageInfo>>((self, damage) =>
                {
                    if (!damage.rejected && ElementalReactionManager.instance)
                    {
                        ElementDef element = ElementCatalog.GetElementDef(damage.damageType.GetElement());
                        ElementalReactionManager.ApplyElement(element, self.body, ref damage, out float damageIncreaseFromReactions);
                        CharacterBody attackerBody = damage.attacker ? damage.attacker.GetComponent<CharacterBody>() : null;

                        if (self.body.HasBuff(Buffs.superconductBuff) && element == DefaultElementDefs.physicalElement)
                        {
                            damageIncreaseFromReactions += damage.damage * StaticValues.superconductDamageMultiplier;
                        }
                        else if (damage.damage > 0 && self.body.HasBuff(Buffs.quickenBuff) && attackerBody && element == DefaultElementDefs.dendroElement || element == DefaultElementDefs.electroElement)
                        {
                            damageIncreaseFromReactions += (StaticValues.quickenDamageAddCoefficient * damage.procCoefficient * attackerBody.damage);
                        }

                        if (damage.damageType.HasModdedDamageType(DamageTypes.superconductDamageType))
                        {
                            self.body.AddTimedBuff(Buffs.superconductBuff, 5, 1);
                        }

                        if (damage.damageType.HasModdedDamageType(DamageTypes.elementalReactionDamageType))
                        {
                            damage.damage += damageIncreaseFromReactions;
                            ModifyElementalReactionDamage(self, damage.attacker, attackerBody, ref damage.damage);
                        }
                        else
                        {
                            ModifyElementalReactionDamage(self, damage.attacker, attackerBody, ref damageIncreaseFromReactions);
                            damage.damage += damageIncreaseFromReactions;
                        }
                    }
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void ModifyElementalReactionDamage(HealthComponent victim, GameObject attacker, CharacterBody attackerBody, ref float damage)
        {
            if (attackerBody && attackerBody.inventory)
            {
                damage *= 1 + (StaticValues.instructorsTeaCupDamageMultiplier * attackerBody.inventory.GetItemCountEffective(Items.Items.instructorsTeaCup));
            }
            if (victim.body.teamComponent && victim.body.teamComponent.teamIndex == TeamIndex.Player)
            {
                damage *= Config.PlayerReactionResistance().Value / 100f;
            }
        }
        private static void ApplyElementToSkillDamage(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self && ElementalReactionManager.instance)
            {
                ElementDef element = ElementCatalog.GetElementDef(damageInfo.damageType.GetElement());

                if (element == DefaultElementDefs.physicalElement && damageInfo.attacker && damageInfo.damageType.IsDamageSourceSkillBased && 
                    damageInfo.attacker.TryGetComponent<ElementLoadoutComponent>(out var elementLoadout) && elementLoadout.isActiveAndEnabled
                    && ((elementLoadout.team == TeamIndex.Player && 
                        (elementLoadout.characterBody.isPlayerControlled && Config.CanSurvivorsUseElements().Value) || 
                        (!elementLoadout.characterBody.isPlayerControlled && Config.CanAlliesUseElements().Value)) || 
                    (elementLoadout.team != TeamIndex.Player && Config.CanEnemiesUseElements().Value)))
                {
                    element = elementLoadout.GetElement(damageInfo.damageType.damageSource);
                    if (element) damageInfo.damageType.SetElement(element.index);
                }
            }
            orig(self, damageInfo);
        }

        private static bool NoDelusionsWithElementInLogbook(On.RoR2.UI.LogBook.LogBookController.orig_CanSelectItemEntry orig, ItemDef item, Dictionary<ExpansionDef, bool> entitlements)
        {
            return orig(item, entitlements) && item && !DelusionManager.delusionToElement.Keys.Contains(item);
        }

        private static float HealingReceivedDebuff(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask proc, bool nonRegen)
        {
            if (self.body.HasBuff(Buffs.delusionActiveBuff) && self.body.inventory)
            {
                amount *= Mathf.Max(0, 1 - (DelusionManager.GetDelusionCount(self.body.inventory) * StaticValues.delusionHealingReceivedReduction));
            }
            return orig(self, amount, proc, nonRegen);
        }

        private static void AddOverheatPyro(On.RoR2.OverheatSystem.BodyOverheatInfo.orig_AddChanneledBuff orig, OverheatSystem.BodyOverheatInfo self)
        {
            orig(self);
            if (ElementalReactionManager.instance && self.characterBody)
            {
                ElementalReactionManager.ApplyElement(DefaultElementDefs.pyroElement, self.characterBody, 0.3f);
            }
        }
        private static void SolusWingCoolingCryo(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC3Content.Buffs), nameof(DLC3Content.Buffs.Brittle))) &&
                c.TryGotoNext(MoveType.After, x => x.MatchBrfalse(out ILLabel falseBranch)))
            {
                c.Emit(OpCodes.Ldarg_0); // characterBody
                c.Emit(OpCodes.Ldarg_2); // duration
                c.EmitDelegate<Action<CharacterBody, float>>((self, duration) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        ElementalReactionManager.ApplyElement(DefaultElementDefs.cryoElement, self, 1f);
                    }
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
    }
}
