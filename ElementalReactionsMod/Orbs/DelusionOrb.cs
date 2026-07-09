using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ElementalReactionsMod.Elements;
using R2API;
using ElementalReactionsMod.Reactions;

namespace ElementalReactionsMod.Orbs
{
    public class DelusionOrb : GenericDamageOrb, IOrbFixedUpdateBehavior
    {
        public ElementIndex element;
        private Vector3 lastKnownTargetPosition;

        public static void FireDelusionOrb(CharacterBody attacker, Vector3 origin, HurtBox target, int stacks, bool crit, ElementIndex element)
        {
            DelusionOrb delusionOrb = new DelusionOrb
            {
                attacker = attacker.gameObject,
                origin = origin,
                damageValue = (StaticValues.delusionDamageCoefficient + (StaticValues.delusionStackDamageCoefficient * (stacks - 1))) * attacker.damage,
                isCrit = crit,
                target = target,
                damageType = new DamageTypeCombo(),
                procCoefficient = 1,
                teamIndex = attacker.teamComponent.teamIndex,
                element = element,
                speed = 75
            };
            delusionOrb.damageType.SetElement(element);
            OrbManager.instance.AddOrb(delusionOrb);
        }
        public override void Begin()
        {
            duration = (distanceToTarget / speed);
            if (this.GetOrbEffect())
            {
                EffectData effectData = new EffectData
                {
                    scale = this.scale,
                    origin = this.origin,
                    genericFloat = base.duration,
                    genericUInt = (uint)element
                };
                effectData.SetHurtBoxReference(this.target);
                EffectManager.SpawnEffect(this.GetOrbEffect(), effectData, true);
            }
        }
        public override GameObject GetOrbEffect()
        {
            return ElementalReactionManager.genericElementOrbEffect.WaitForCompletion();
        }
        public override void OnArrival()
        {
            Util.CreateBlastAttack(attacker, teamIndex, damageValue, isCrit, StaticValues.genericReactionExplosionRadius, BlastAttack.FalloffModel.Linear, procCoefficient, damageType, lastKnownTargetPosition, 0f).Fire();
            EffectManager.SpawnEffect(ElementalReactionManager.delusionHitEffect.WaitForCompletion(), new EffectData { origin = lastKnownTargetPosition, genericUInt = (uint)element }, true);
        }
        public void FixedUpdate()
        {
            if (this.target)
            {
                this.lastKnownTargetPosition = this.target.transform.position;
            }
        }
    }
}
