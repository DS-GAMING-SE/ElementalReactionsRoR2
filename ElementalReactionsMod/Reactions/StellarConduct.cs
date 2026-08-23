using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Orbs;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ElementalReactionsMod.Reactions
{
    public class StellarConduct : ComponentPoolObject
    {
        public CharacterBody characterBody;
        public CameraTargetParams cameraTargetParams;
        public CameraTargetParams.AimRequest aimRequest;
        public bool initialized;

        public Transform starTransform;
        public Transform radiusTransform;
        private float radiusScaleVelocity;
        private Vector3 position;
        private Run.FixedTimeStamp nextReleaseTime;
        private int stacks;
        public void CreateStellarConductField(CharacterBody characterBody)
        {
            this.characterBody = characterBody;
            position = characterBody.corePosition + (Vector3.up * (characterBody.radius + 2f));
            transform.position = position;
            //RoR2.Util.PlaySound("Play_item_proc_icicle", base.gameObject);
            cameraTargetParams = characterBody.GetComponent<CameraTargetParams>();
            if (cameraTargetParams) aimRequest = cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            radiusTransform.localScale = Vector3.zero;
            GenericElementEffectComponent.SpawnActivatedEffect(characterBody.transform, ParentEffectToItemDisplay.ItemDisplayParent.StellarLinchpin, DefaultElementDefs.electroElement.index, 0.7f, false);
            RoR2.Util.PlaySound("Play_seeker_skill3_start", gameObject);
            stacks = 0;
            ReleaseEnergy();
            initialized = true;
        }
        private void FixedUpdate()
        {
            if (initialized)
            {
                if (!characterBody || characterBody.GetBuffCount(Buffs.stellarConductFieldBuff) == 0)
                {
                    PreReturnToPool();
                    ReturnToPool();
                    return;
                }

                if (nextReleaseTime.hasPassed)
                {
                    ReleaseEnergy();
                }
            }
        }

        public void ReleaseEnergy()
        {
            if (!NetworkServer.active) return;
            nextReleaseTime = Run.FixedTimeStamp.now + StaticValues.stellarConductInterval;
            DamageTypeCombo damageType = DamageType.AOE;
            damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
            damageType.AddModdedDamageType(DamageTypes.stellarDamageType);
            Util.ManualBlastAttack(position, StaticValues.stellarConductFieldRadius, characterBody.gameObject, characterBody.teamComponent.teamIndex, characterBody.damage * Mathf.Lerp(StaticValues.stellarConductMinDamage, StaticValues.stellarConductMaxDamage, stacks / StaticValues.stellarConductMaxStacks), characterBody.RollCrit(), damageType, false, false,
                (hit) =>
                {
                    for (int i = 0; i < stacks + 1; i++)
                    {
                        hit.AddTimedBuff(Buffs.stellarConductDebuff, StaticValues.stellarConductInterval, stacks + 1);
                    }
                });
            stacks = 0;
        }
        private void OnEnable()
        {
            SceneCamera.onSceneCameraPreRender += AdjustStarToPOV;
            ElementalReactionManager.onElementApplied += TryAddStack;
        }
        private void OnDisable()
        {
            SceneCamera.onSceneCameraPreRender -= AdjustStarToPOV;
            ElementalReactionManager.onElementApplied -= TryAddStack;
        }
        private void AdjustStarToPOV(SceneCamera camera)
        {
            if (initialized)
            {
                Vector3 forward = (characterBody.corePosition - camera.transform.position).normalized;
                forward = Vector3.Cross(forward, Vector3.up);
                forward.y = 0;
                starTransform.rotation = Quaternion.LookRotation(forward);
            }
        }
        private void Update()
        {
            if (initialized)
            {
                if (characterBody)
                {
                    position = characterBody.corePosition + (Vector3.up * (characterBody.radius + 2f));
                    transform.position = position;
                }
                float num = Mathf.SmoothDamp(radiusTransform.localScale.x, 2f * StaticValues.stellarConductFieldRadius, ref radiusScaleVelocity, 0.5f);
                radiusTransform.localScale = new Vector3(num, num, num);
            }
        }

        public void TryAddStack(ElementDef element, CharacterBody target, DamageInfo damageInfo)
        {
            if (TeamComponent.GetObjectTeam(damageInfo.attacker) == characterBody.teamComponent.teamIndex &&
                (element == DefaultElementDefs.electroElement || element == DefaultElementDefs.cryoElement) &&
                (damageInfo.position - position).sqrMagnitude <= StaticValues.stellarConductFieldRadiusSqr &&
                stacks < StaticValues.stellarConductMaxStacks)
            {
                stacks++;
            }
        }
        public override void PreReturnToPool()
        {
            base.PreReturnToPool();
            RoR2.Util.PlaySound("Stop_seeker_skill3_loop", gameObject);
            EffectManager.SimpleEffect(ElementalReactionManager.stellarConductDespawnEffect.WaitForCompletion(), position, Quaternion.identity, false);
            nextReleaseTime = Run.FixedTimeStamp.positiveInfinity;
            aimRequest?.Dispose();
            initialized = false;
        }
    }
}
