using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Items;
using ElementalReactionsMod.Loadout;
using HarmonyLib;
using HG;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.EntitlementManagement;
using RoR2.ExpansionManagement;
using SS2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ElementalReactionsMod
{
    public static class Hooks // I wrote my first IL hook for this mod. It was scary. It is still scary sometimes. That's progress, I guess
    {
        public static void Initialize()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += TakeDamageIL;
            On.RoR2.HealthComponent.TakeDamageProcess += ApplyElementToSkillDamage;
            On.RoR2.UI.LogBook.LogBookController.CanSelectItemEntry += NoDelusionsWithElementInLogbook;
            On.RoR2.HealthComponent.Heal += HealingReceivedDebuff;
            On.RoR2.OverheatSystem.BodyOverheatInfo.AddChanneledBuff += AddOverheatPyro;
            IL.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += SolusWingCoolingCryo;
            IL.RoR2.CharacterBody.InflictLavaDamage += LavaPyro;
            IL.RoR2.Projectile.ProjectileManager.InitializeProjectile += AddLoadoutElementToProjectile;
            On.EntityStates.RoboBallBoss.Weapon.FireSuperDelayKnockup.OnEnter += AlloyWorshipUnitShieldAnemo;
            On.EntityStates.SolusAmalgamator.ShockArmor.StartShock += SolusAmalgamatorShockArmorElectro;
            IL.EntityStates.DefectiveUnit.Detonate.FixedUpdate += SolusInvalidatorSlamAttackElectro;
            On.EntityStates.MiniMushroom.Plant.OnEnter += MiniMushrumHealingStartDendro;
            On.EntityStates.MiniMushroom.Plant.OnExit += MiniMushrumHealingEndDendro;
            IL.EntityStates.VoidInfestor.Infest.FixedUpdate += VoidInfestorHydro;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlacialExplosionCryo;
            IL.RoR2.CharacterModel.UpdateItemDisplay += DelusionsShareItemDisplay;
            On.EntityStates.ClayBruiser.Weapon.FireSonicBoom.AddDebuff += ClayTemplarPushHydro;
            IL.EntityStates.AcidLarva.LarvaLeap.DoImpactAuthority += LarvaHydroThemselves;
            On.EntityStates.FriendUnit.KineticAura.OnEnter += OrphanedCoreElectroOnCharge;
            IL.EntityStates.AffixEarthHealer.Heal.OnEnter += MendingHealingOrbDendro;
            IL.EntityStates.Destructible.ExplosivePotDeath.Explode += DestructibleObjectHydro;
            IL.EntityStates.Destructible.SulfurPodDeath.Explode += DestructibleObjectHydro;
            IL.EntityStates.Destructible.FusionCellDeath.Explode += DestructibleObjectElectro;
            IL.EntityStates.Destructible.LunarRainDeathState.Explode += DestructibleObjectElectro;
            On.RoR2.CharacterBody.OnBuffFirstStackGained += CreateLunarCrystallize;
            On.RoR2.ScavengerItemGranter.Start += ScavengerRandomElement;
            On.EntityStates.ScavMonster.PrepEnergyCannon.OnEnter += ScavengerChargeAttackElement;
            On.RoR2.CameraRigController.LateUpdate += EnvironmentScreenEffect;
            IL.RoR2.CharacterBody.HandleConstructTurret += EngiTurretElements;
            //IL.EntityStates.Tanker.GreasePuddle.IgniteGrease.OnEnter += SolusScorcherPyro;
            IL.RoR2.CharacterModel.UpdateOverlays += OverlayHook;
            IL.RoR2.CharacterModel.UpdateOverlayStates += OverlayHook;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += RemoveReactionAuras;
            IL.RoR2.DotController.UpdateDotVisuals += BurningVFX;
            IL.RoR2.Stats.StatManager.OnCharacterDeath += DeathByBurningIsFieryDeath;
            IL.RoR2.CharacterBody.RecalculateStats += BurningDisablesRegen;
            IL.RoR2.Orbs.VineOrb.OnArrival += NoxiousThornAppliesElements;
        }
        // Right after IOnincomingDamageReceiver does its thing, since that's where many things (including bloom dendro cores) reject damage
        private static void TakeDamageIL(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel damageNotRejected = null;
            if (c.TryGotoNext(x => x.MatchLdfld<HealthComponent>(nameof(HealthComponent.onIncomingDamageReceivers))) && 
                c.TryGotoNext(MoveType.After, x => x.MatchLdfld(typeof(DamageInfo), nameof(DamageInfo.rejected)), x => x.MatchBrfalse(out damageNotRejected)))
            {
                c.Goto(damageNotRejected.Target, MoveType.After, true);
                c.Emit(OpCodes.Ldarg_0); // healthcomponent
                c.Emit(OpCodes.Ldarg_1); // damageInfo
                c.EmitDelegate<Action<HealthComponent, DamageInfo>>((self, damage) =>
                {
                    if (!damage.rejected && ElementalReactionManager.instance)
                    {
                        ElementDef element = ElementCatalog.GetElementDef(damage.damageType.GetElement());
                        CharacterBody attackerBody = damage.attacker ? damage.attacker.GetComponent<CharacterBody>() : null;

                        if (element != DefaultElementDefs.physicalElement && !Util.CanUseElements(attackerBody))
                        {
                            element = DefaultElementDefs.physicalElement;
                        }

                        bool lunarBloomProcced = false;
                        if (attackerBody && damage.damage / attackerBody.damage >= 4f && attackerBody.GetBuffCount(Buffs.lunarBloomBuff) >= 3)
                        {
                            attackerBody.SetBuffCount(Buffs.lunarBloomBuff.buffIndex, attackerBody.GetBuffCount(Buffs.lunarBloomBuff) - 3);
                            lunarBloomProcced = true;
                            damage.damageType.AddModdedDamageType(DamageTypes.lunarDamageType);
                        }

                        ElementalReactionManager.ApplyElement(element, self.body, ref damage, out float damageIncreaseFromReactions);

                        if (lunarBloomProcced)
                        {
                            damageIncreaseFromReactions += damage.damage * StaticValues.lunarBloomDamageMultiplier;
                            EffectManager.SimpleEffect(ElementalReactionManager.lunarBloomEffect.WaitForCompletion(), damage.inflictedHurtbox ? damage.inflictedHurtbox.transform.position : self.body.corePosition, Quaternion.identity, true);
                        }

                        if (element == DefaultElementDefs.physicalElement && !damage.damageType.HasModdedDamageType(DamageTypes.elementalReactionDamageType))
                        {
                            if (self.body.HasBuff(Buffs.superconductBuff))
                            {
                                damageIncreaseFromReactions += damage.damage * StaticValues.superconductDamageMultiplier;
                            }
                            if (self.body.HasBuff(Buffs.stellarConductDebuff))
                            {
                                damageIncreaseFromReactions += damage.damage * (StaticValues.stellarConductMinDamageMultiplier + (StaticValues.stellarConductDamageMultiplierPerStack * (self.body.GetBuffCount(Buffs.stellarConductDebuff) - 1)));
                            }
                        }
                        else if (damage.damage > 0 && self.body.HasBuff(Buffs.quickenBuff) && attackerBody && (element == DefaultElementDefs.dendroElement || element == DefaultElementDefs.electroElement))
                        {
                            damageIncreaseFromReactions += (StaticValues.quickenDamageAddCoefficient * damage.procCoefficient * attackerBody.damage);
                        }

                        if (damage.damageType.HasModdedDamageType(DamageTypes.superconductDamageType))
                        {
                            self.body.AddTimedBuff(Buffs.superconductBuff, StaticValues.superconductDuration, 1);
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

                        if (attackerBody && attackerBody.inventory && damage.damageType.HasModdedDamageType(DamageTypes.lunarDamageType))
                        {
                            damage.damage *= 1 + (Math.Max(attackerBody.inventory.GetItemCountWithQuality(Items.Items.moonWheel) - 1, 0) * StaticValues.moonWheelLunarDamagePerStack);
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
                float elementalReactionDamageIncrease = 1f;
                elementalReactionDamageIncrease += StaticValues.instructorsTeaCupDamageMultiplier * attackerBody.inventory.GetItemCountWithQuality(Items.Items.instructorsTeaCup);
                if (ElementalReactionsPlugin.qualityModExists)
                {
                    elementalReactionDamageIncrease += attackerBody.GetBuffCount(Buffs.instructorsTeaCupQualityBase) * StaticValues.instructorsTeaCupQualityDamageIncrease * attackerBody.inventory.GetItemCountQualities(Items.Items.instructorsTeaCup);
                }
                damage *= elementalReactionDamageIncrease;
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
                if ((!element || element == DefaultElementDefs.physicalElement) && damageInfo.attacker && damageInfo.attacker.TryGetComponent<CharacterBody>(out var body))
                {
                    damageInfo.damageType.SetElement(ElementLoadoutComponent.GetElement(body, damageInfo.damageType.damageSource).index);
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
                ElementalReactionManager.ApplyElement(DefaultElementDefs.pyroElement, self.characterBody, 0.3f, null, false, StaticValues.permanentElementICD);
            }
        }
        private static void SolusWingCoolingCryo(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC3Content.Buffs), nameof(DLC3Content.Buffs.Brittle))) &&
                c.TryGotoNext(MoveType.After, x => x.MatchBrfalse(out ILLabel falseBranch)))
            {
                c.Emit(OpCodes.Ldarg_0); // characterBody
                c.EmitDelegate<Action<CharacterBody>>((self) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        ElementalReactionManager.ApplyElement(DefaultElementDefs.cryoElement, self, 1f, null, false, StaticValues.permanentElementICD);
                    }
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void LavaPyro(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchStfld(typeof(DamageInfo), nameof(DamageInfo.damageType))))
            {
                c.Emit(OpCodes.Ldloc_0); // damageInfo
                c.EmitDelegate<Action<DamageInfo>>((damageInfo) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        damageInfo.damageType.SetElement(DefaultElementDefs.pyroElement.index);
                    }
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void AddLoadoutElementToProjectile(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchStfld(typeof(DamageTypeCombo), nameof(DamageTypeCombo.damageSource))))
            {
                c.Emit(OpCodes.Ldarg_1); // fireProjectileInfo
                Instruction instruction = c.Prev; // saving location of my code
                c.Emit(OpCodes.Ldloc_1); // projectileDamage
                c.EmitDelegate<Action<RoR2.Projectile.FireProjectileInfo, RoR2.Projectile.ProjectileDamage>>((fireProjectileInfo, projectileDamage) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        if (!projectileDamage.damageType.IsElementalDamage() && fireProjectileInfo.owner && fireProjectileInfo.owner.TryGetComponent<CharacterBody>(out var body))
                        {
                            projectileDamage.damageType.SetElement(ElementLoadoutComponent.GetElement(body, projectileDamage.damageType.damageSource).index);
                        }
                    }
                });
                if (c.TryGotoPrev(x => x.MatchBr(out _))) // redirecting projectiledamage related ifs to go to my code instead of leaving the projectiledamage block
                {
                    c.Next.Operand = instruction;
                }
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void AlloyWorshipUnitShieldAnemo(On.EntityStates.RoboBallBoss.Weapon.FireSuperDelayKnockup.orig_OnEnter orig, EntityStates.RoboBallBoss.Weapon.FireSuperDelayKnockup self)
        {
            orig(self);
            if (ElementalReactionManager.instance && NetworkServer.active && self.TryGetComponent<ElementLoadoutComponent>(out var loadout))
            {
                loadout.SetSpecialAppliedElement(DefaultElementDefs.anemoElement, EntityStates.RoboBallBoss.Weapon.FireSuperDelayKnockup.shieldDuration);
            }
        }
        private static void SolusAmalgamatorShockArmorElectro(On.EntityStates.SolusAmalgamator.ShockArmor.orig_StartShock orig, EntityStates.SolusAmalgamator.ShockArmor self)
        {
            orig(self);
            if (ElementalReactionManager.instance && NetworkServer.active)
            {
                ElementalReactionManager.ApplyElement(DefaultElementDefs.electroElement, self.characterBody, 0.5f, self.gameObject, true);
            }
        }
        private static void SolusInvalidatorSlamAttackElectro(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchStfld(typeof(EntityStates.DefectiveUnit.Detonate), nameof(EntityStates.DefectiveUnit.Detonate._hasDetonated))))
            {
                c.Emit(OpCodes.Ldarg_0); // Detonate state
                c.EmitDelegate<Action<EntityStates.DefectiveUnit.Detonate>>((self) =>
                {
                    if (ElementalReactionManager.instance && NetworkServer.active)
                    {
                        ElementalReactionManager.ApplyElement(DefaultElementDefs.electroElement, self.characterBody, 0.5f, self.gameObject);
                    }
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void MiniMushrumHealingStartDendro(On.EntityStates.MiniMushroom.Plant.orig_OnEnter orig, EntityStates.MiniMushroom.Plant self)
        {
            orig(self);
            if (ElementalReactionManager.instance && NetworkServer.active && self.TryGetComponent<ElementLoadoutComponent>(out var loadout))
            {
                loadout.SetSpecialAppliedElement(DefaultElementDefs.dendroElement);
            }
        }
        private static void MiniMushrumHealingEndDendro(On.EntityStates.MiniMushroom.Plant.orig_OnExit orig, EntityStates.MiniMushroom.Plant self)
        {
            if (ElementalReactionManager.instance && NetworkServer.active && self.TryGetComponent<ElementLoadoutComponent>(out var loadout))
            {
                loadout.SetSpecialAppliedElement(null);
            }
            orig(self);
        }
        private static void VoidInfestorHydro(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt(AccessTools.PropertySetter(typeof(TeamComponent), nameof(TeamComponent.teamIndex)))))
            {
                c.Emit(OpCodes.Ldarg_0); // Infest state
                c.Emit(OpCodes.Ldloc_3); // target body
                c.EmitDelegate<Action<EntityStates.VoidInfestor.Infest, CharacterBody>>((self, target) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        ElementalReactionManager.ApplyElement(DefaultElementDefs.hydroElement, target, 3f, self.gameObject);
                    }
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void GlacialExplosionCryo(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int delayBlastComponentLoc = -1;
            // I don't know how to match GetComponent to find where DelayBlast is first stored. Instead, I'm finding a point where the DelayBlast is loaded
            // (setting the base force) and going back to find the DelayBlast loc index from there
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.AffixWhite))) &&
                c.TryGotoNext(MoveType.After, x => x.MatchStfld(typeof(DelayBlast), nameof(DelayBlast.baseForce))) &&
                c.TryGotoPrev(x => x.MatchLdloc(out delayBlastComponentLoc)))
            {
                if (c.TryGotoNext(MoveType.After, x => x.MatchStfld(typeof(DelayBlast), nameof(DelayBlast.damageType))))
                {
                    c.Emit(OpCodes.Ldloc, delayBlastComponentLoc);
                    c.EmitDelegate<Action<DelayBlast>>((self) =>
                    {
                        if (ElementalReactionManager.instance)
                        {
                            self.damageType.SetElement(DefaultElementDefs.cryoElement.index);
                        }
                    });
                }
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void DelusionsShareItemDisplay(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.CalculateEffectiveItemStacks))))
            {
                // Item count comes from CalculateEffectiveItemStacks
                c.Emit(OpCodes.Ldarg_1); // Inventory
                c.Emit(OpCodes.Ldloc_0); // ItemIndex
                c.EmitDelegate<Func<int, Inventory, ItemIndex, int>>((itemCount, inventory, itemIndex) =>
                {
                    if (inventory && itemIndex == Items.Items.delusion.itemIndex)
                    {
                        return itemCount + inventory.GetDelusionCount();
                    }
                    return itemCount;
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void ClayTemplarPushHydro(On.EntityStates.ClayBruiser.Weapon.FireSonicBoom.orig_AddDebuff orig, EntityStates.ClayBruiser.Weapon.FireSonicBoom self, CharacterBody target)
        {
            orig(self, target);
            if (ElementalReactionManager.instance)
            {
                ElementalReactionManager.ApplyElement(DefaultElementDefs.hydroElement, target, 1, self.gameObject);
            }
        }
        private static void LarvaHydroThemselves(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchStfld(typeof(DamageInfo), nameof(DamageInfo.damageType))))
            {
                c.EmitDelegate<Func<DamageTypeCombo, DamageTypeCombo>>((damageType) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        damageType.SetElement(DefaultElementDefs.hydroElement.index);
                    }
                    return damageType;
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void OrphanedCoreElectroOnCharge(On.EntityStates.FriendUnit.KineticAura.orig_OnEnter orig, EntityStates.FriendUnit.KineticAura self)
        {
            orig(self);
            if (ElementalReactionManager.instance && NetworkServer.active)
            {
                ElementalReactionManager.ApplyElement(DefaultElementDefs.electroElement, self.characterBody, 1.5f, self.gameObject);
            }
        }
        private static void MendingHealingOrbDendro(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int targetLocIndex = -1;
            if (c.TryGotoNext(x => x.MatchLdfld(typeof(HealthComponent), nameof(HealthComponent.body))) &&
                c.TryGotoPrev(x => x.MatchLdloc(out targetLocIndex)) &&
                c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(RoR2.Orbs.OrbManager), nameof(RoR2.Orbs.OrbManager.AddOrb))))
            {
                c.Emit(OpCodes.Ldarg_0); // Heal state
                c.Emit(OpCodes.Ldloc, targetLocIndex); // target healthComponent
                c.EmitDelegate<Action<EntityStates.AffixEarthHealer.Heal, HealthComponent>>((self, target) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        ElementalReactionManager.ApplyElement(DefaultElementDefs.dendroElement, target.body, 2f, self.gameObject);
                    }
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void DestructibleObjectHydro(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchStfld(typeof(BlastAttack), nameof(BlastAttack.damageType))))
            {
                c.EmitDelegate<Func<DamageTypeCombo, DamageTypeCombo>>((damageType) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        damageType.SetElement(DefaultElementDefs.hydroElement.index);
                    }
                    return damageType;
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void DestructibleObjectElectro(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(BlastAttack), nameof(BlastAttack.Fire))))
            {
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>((blastAttack) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        blastAttack.damageType.SetElement(DefaultElementDefs.electroElement.index);
                    }
                    return blastAttack;
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void CreateLunarCrystallize(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buff)
        {
            orig(self, buff);
            if (ElementalReactionManager.instance)
            {
                if (buff == Buffs.lunarCrystallizeBuff)
                {
                    ElementalReactionManager.CreateLunarCrystallizeController(self);
                }
                if (buff == Buffs.stellarConductFieldBuff)
                {
                    ElementalReactionManager.CreateStellarConductField(self);
                }
            }
        }

        private static void ScavengerRandomElement(On.RoR2.ScavengerItemGranter.orig_Start orig, ScavengerItemGranter self)
        {
            orig(self);
            if (!ElementalReactionManager.instance) return;

            Inventory inventory = self.GetComponent<Inventory>();
            if (inventory.GetItemCountPermanent(ElementLoadoutComponent.primaryElementItem) == 0)
            {
                if (inventory.GetItemCountWithQuality(MoonWheel.moonWheel) > 0)
                {
                    MasterElementLoadout.UpdateLoadoutItem(inventory, DefaultElementDefs.hydroElement, ElementLoadoutComponent.primaryElementItem);
                }
                else
                {
                    MasterElementLoadout.UpdateLoadoutItem(inventory, ElementCatalog.elementCatalog[ScavengerItemGranter.rng.RangeInt(1, ElementCatalog.elementCatalog.Length)], ElementLoadoutComponent.primaryElementItem);
                }
            }
        }
        private static void ScavengerChargeAttackElement(On.EntityStates.ScavMonster.PrepEnergyCannon.orig_OnEnter orig, EntityStates.ScavMonster.PrepEnergyCannon self)
        {
            orig(self);
            ElementDef element = ElementLoadoutComponent.GetElement(self.characterBody, DamageSource.Primary);
            if (self.muzzleTransform && element != DefaultElementDefs.physicalElement && Util.CanUseElements(self.characterBody))
            {
                EffectManager.SpawnEffect(ElementalReactionManager.genericElementActivatedEffect.WaitForCompletion(),
                    new EffectData
                    {
                        rootObject = self.muzzleTransform.gameObject,
                        origin = self.muzzleTransform.position,
                        genericFloat = self.duration * 0.85f,
                        genericUInt = (uint)element.index,
                        scale = 12f
                    }, false);
            }
        }
        private static void EnvironmentScreenEffect(On.RoR2.CameraRigController.orig_LateUpdate orig, CameraRigController self)
        {
            orig(self);
            bool showEffect = self.targetBody && self.targetBody.HasBuff(Buffs.elementalEnvironmentHiddenBuff);
            if (Environment.ElementalRain.camerasToScreenEffects.TryGetValue(self, out Environment.ElementalEnvironmentScreenEffect screenEffect))
            {
                screenEffect.SetActive(showEffect);
            }
        }
        private static void EngiTurretElements(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int bodyIndex = -1;
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt(AccessTools.PropertyGetter(typeof(CharacterBody), nameof(CharacterBody.master)))) &&
                c.TryGotoPrev(x => x.MatchLdloc(out bodyIndex)) &&
                c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(MasterSummon), nameof(MasterSummon.Perform))))
            {
                c.Emit(OpCodes.Dup);
                c.Emit(OpCodes.Ldloc, bodyIndex);
                c.EmitDelegate<Func<CharacterBody, MasterSummon.IInventorySetupCallback>>((body) =>
                {
                    return body.GetComponent<EngineerTurretElements>();
                });
                c.Emit<MasterSummon>(OpCodes.Stfld, nameof(MasterSummon.inventorySetupCallback));
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        // I don't know why this doesn't work. Adding pyro to these damn scorchers seems impossible
        private static void SolusScorcherPyro(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchStfld<RoR2.Projectile.FireProjectileInfo>(nameof(RoR2.Projectile.FireProjectileInfo.damageTypeOverride))))
            {
                c.EmitDelegate<Func<DamageTypeCombo?, DamageTypeCombo?>>((damageType) =>
                {
                    if (ElementalReactionManager.instance)
                    {
                        DamageTypeCombo damageTypeNotNull = damageType.GetValueOrDefault();
                        damageTypeNotNull.SetElement(DefaultElementDefs.pyroElement.index);
                        return damageTypeNotNull;
                    }
                    return damageType;
                });
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void OverlayHook(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC2Content.Buffs), nameof(DLC2Content.Buffs.Frost)),
                x => x.MatchCallOrCallvirt(typeof(CharacterBody), nameof(CharacterBody.HasBuff))))
            {
                // Turning HasBuff(Frost) into HasBuff(Frost) || HasBuff(superconductBuff)
                Instruction end = c.Next;
                c.Emit(OpCodes.Ldarg_0);
                c.Emit<CharacterModel>(OpCodes.Ldfld, nameof(CharacterModel.body));
                c.Emit(OpCodes.Ldsfld, typeof(Buffs).GetField("superconductBuff"));
                // HasBuff defaults to buffIndex overload and I can't be bothered to pick the overload myself
                c.Emit<BuffDef>(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BuffDef), nameof(BuffDef.buffIndex)).Name);
                c.Emit<CharacterBody>(OpCodes.Callvirt, nameof(CharacterBody.HasBuff));
                c.Emit(OpCodes.Br, end);
                c.Emit(OpCodes.Ldc_I4_1);
                Instruction beforeEnd = c.Previous;
                c.Index -= 7;
                c.Emit(OpCodes.Brtrue, beforeEnd);
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }

        private static void RemoveReactionAuras(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buff)
        {
            orig(self, buff);
            if (NetworkServer.active && ReactionAuraBuffDef.elementToReactionAuras.TryGetValue(buff, out var reactionAuras))
            {
                foreach (BuffDef aura in reactionAuras)
                {
                    if (self.HasBuff(aura))
                    {
                        if (aura.isDOT)
                        {
                            Util.RemoveDot(DotController.FindDotController(self.gameObject), DotController.GetDotDefIndex(aura));
                        }
                        else
                        {
                            self.RemoveBuff(aura);
                        }
                    }
                }
            }
        }
        private static void BurningVFX(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel burningFalse = null;
            if (c.TryGotoNext(x => x.MatchLdarg(0),
                x => x.MatchLdfld(typeof(DotController), nameof(DotController.burnEffectController))))
            {
                // Making it so the generic burn vfx plays if you have normal burn OR my burn
                Instruction burnEffectSuccess = c.Next;
                if (c.TryGotoPrev(x => x.MatchBrfalse(out burningFalse)))
                {
                    c.Emit(OpCodes.Brtrue, burnEffectSuccess);
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldsfld, typeof(Reactions.BurningDot).GetField("burningDot"));
                    c.Emit<DotController>(OpCodes.Callvirt, nameof(DotController.HasDotActive));
                }
            }
            else
            {
                Log.Error($"BurningVFX Burning IL FAILED");
            }

            if (c.TryGotoNext(x => x.MatchLdarg(0),
                x => x.MatchLdfld(typeof(DotController), nameof(DotController.strongerBurnEffectController))))
            {
                // Making it so the generic burn vfx plays if you have normal burn OR my burn
                Instruction burnEffectSuccess = c.Next;
                if (c.TryGotoPrev(x => x.MatchBrfalse(out burningFalse)))
                {
                    c.Emit(OpCodes.Brtrue, burnEffectSuccess);
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldsfld, typeof(Reactions.BurningDot).GetField("strongBurningDot"));
                    c.Emit<DotController>(OpCodes.Callvirt, nameof(DotController.HasDotActive));
                }
            }
            else
            {
                Log.Error($"BurningVFX StrongBurning IL FAILED");
            }
        }
        private static void DeathByBurningIsFieryDeath(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int index = -1;
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(DotController), nameof(DotController.FindDotController)), 
                x => x.MatchStloc(out index)) &&
                c.TryGotoNext(x => x.MatchLdcI4((int)DotController.DotIndex.Burn),
                x => x.MatchCallOrCallvirt(typeof(DotController), nameof(DotController.HasDotActive))) &&
                c.TryGotoNext(x => x.MatchStloc(out _)))
            {
                // For "number of deaths while burning" stat? I think Blast Shower's achievement cares? I dunno I just saw burn and joined in
                c.Emit(OpCodes.Ldloc, index);
                c.Emit(OpCodes.Ldsfld, typeof(Reactions.BurningDot).GetField("burningDot"));
                c.Emit<DotController>(OpCodes.Callvirt, nameof(DotController.HasDotActive));
                c.Emit(OpCodes.Or);
                c.Emit(OpCodes.Ldloc, index);
                c.Emit(OpCodes.Ldsfld, typeof(Reactions.BurningDot).GetField("strongBurningDot"));
                c.Emit<DotController>(OpCodes.Callvirt, nameof(DotController.HasDotActive));
                c.Emit(OpCodes.Or);
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void BurningDisablesRegen(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel burningTrue = null;
            if (c.TryGotoNext(x => x.MatchLdarg(0),
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.OnFire)),
                x => x.MatchCallOrCallvirt(typeof(CharacterBody), nameof(CharacterBody.HasBuff)),
                x => x.MatchBrtrue(out burningTrue)) &&
                c.TryGotoNext(x => x.MatchBrfalse(out _)))
            {
                // Making it so my burn also caps health regen at 0, like base game burn
                c.Emit(OpCodes.Brtrue, burningTrue);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldsfld, typeof(Buffs).GetField("burningBuff"));
                // HasBuff defaults to buffIndex overload and I can't be bothered to pick the overload myself
                c.Emit<BuffDef>(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BuffDef), nameof(BuffDef.buffIndex)).Name);
                c.Emit<CharacterBody>(OpCodes.Callvirt, nameof(CharacterBody.HasBuff));
                c.Emit(OpCodes.Brtrue, burningTrue);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldsfld, typeof(Buffs).GetField("strongBurningBuff"));
                // HasBuff defaults to buffIndex overload and I can't be bothered to pick the overload myself
                c.Emit<BuffDef>(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BuffDef), nameof(BuffDef.buffIndex)).Name);
                c.Emit<CharacterBody>(OpCodes.Callvirt, nameof(CharacterBody.HasBuff));
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        private static void NoxiousThornAppliesElements(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int targetIndex = -1;
            int buffDefIndex = -1;
            if (c.TryGotoNext(x => x.MatchLdfld(typeof(HealthComponent), nameof(HealthComponent.body)),
                x => x.MatchStloc(out targetIndex)) && // grabbing useful info
                c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(BuffCatalog), nameof(BuffCatalog.GetBuffDef)),
                x => x.MatchStloc(out buffDefIndex)) && // grabbing useful info
                c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(CharacterBody), nameof(CharacterBody.AddTimedBuff))))
            {
                // Getting afterTimedBuffApplied so we can skip past adding the timed buff if the buff is an element
                MatchForLoopEnd(ref c);

                NoxiousThornApplyElementsAndSkipBuffs(ref c, targetIndex, buffDefIndex, 
                    (target, buff, debuffInfo) =>
                    {
                        // If the buff is an element, use ApplyElement and skip the AddTimedBuff part
                        int elementBuffIndex = Array.IndexOf(ElementCatalog.elementBuffs, buff);
                        if (elementBuffIndex != -1)
                        {
                           ElementalReactionManager.ApplyElement(ElementCatalog.GetElementDef((ElementIndex)elementBuffIndex), target,
                                debuffInfo.duration / (StaticValues.elementAppliedDuration * StaticValues.elementAppliedTaxMultiplier),
                                debuffInfo.attacker);
                           return true;
                        }
                        return false;
                    });

                if (c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(CharacterBody), nameof(CharacterBody.AddBuff))))
                {
                    MatchForLoopEnd(ref c);

                    NoxiousThornApplyElementsAndSkipBuffs(ref c, targetIndex, buffDefIndex, 
                        (target, buff, debuffInfo) =>
                        {
                            // If the buff is an element, use ApplyElement and skip the AddTimedBuff part
                            int elementBuffIndex = Array.IndexOf(ElementCatalog.elementBuffs, buff);
                            if (elementBuffIndex != -1)
                            {
                                ElementalReactionManager.ApplyElement(ElementCatalog.GetElementDef((ElementIndex)elementBuffIndex), target,
                                    1f,
                                    debuffInfo.attacker);
                                return true;
                            }
                            return false;
                        });
                }
            }
            else
            {
                Log.Error($"{il.Method.Name} IL FAILED");
            }
        }
        // This doesn't work, at least for permanent buffs. There can never be more than 1 element stack anyway, skip the for loop entirely?
        private static void NoxiousThornApplyElementsAndSkipBuffs(ref ILCursor c, int targetIndex, int buffDefIndex, Func<CharacterBody, BuffDef, RoR2.Orbs.VineOrb.SplitDebuffInformation, bool> func)
        {
            ILLabel startOfSection = null;
            Instruction forLoop = c.Next;
            int debuffInfoIndex = -1;
            if (c.TryGotoNext(x => x.MatchLdloc(out debuffInfoIndex),
                x => x.MatchLdfld(typeof(RoR2.Orbs.VineOrb.SplitDebuffInformation), nameof(RoR2.Orbs.VineOrb.SplitDebuffInformation.count)),
                x => x.MatchBlt(out startOfSection)))
            {
                
                c.Goto(startOfSection.Target, MoveType.AfterLabel, true);

                c.Emit(OpCodes.Ldloc, targetIndex);
                c.Emit(OpCodes.Ldloc, buffDefIndex);
                c.Emit(OpCodes.Ldloc, debuffInfoIndex);
                c.EmitDelegate(func);
                c.Emit(OpCodes.Brtrue, forLoop);
            }
        }
        private static void MatchForLoopEnd(ref ILCursor c)
        {
            if (c.TryGotoNext(x => x.MatchLdloc(out _), x => x.MatchLdcI4(1), x => x.MatchAdd(), x => x.MatchStloc(out _), x => x.MatchLdloc(out _), x => x.MatchLdloc(out _), x => x.MatchLdfld(out _), x => x.MatchBlt(out _)))
            {

            }
        }
    }
}
