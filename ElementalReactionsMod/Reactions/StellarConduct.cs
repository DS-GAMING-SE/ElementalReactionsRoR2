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
            nextReleaseTime = Run.FixedTimeStamp.now + StaticValues.stellarConductInterval;
            radiusTransform.localScale = Vector3.zero;
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

                if (NetworkServer.active && nextReleaseTime.hasPassed)
                {
                    nextReleaseTime = Run.FixedTimeStamp.now + StaticValues.stellarConductInterval;
                    DamageTypeCombo damageType = DamageType.AOE;
                    damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                    damageType.AddModdedDamageType(DamageTypes.stellarDamageType);
                    Util.ManualBlastAttack(position, StaticValues.stellarConductFieldRadius, characterBody.gameObject, characterBody.teamComponent.teamIndex, characterBody.damage * Mathf.Lerp(StaticValues.stellarConductMinDamage, StaticValues.stellarConductMaxDamage, stacks / StaticValues.stellarConductMaxStacks), characterBody.RollCrit(), damageType, false, false,
                        (hit) => 
                        {
                            for (int i = 0; i < stacks + 1; i++)
                            {
                                hit.AddTimedBuff(Buffs.stellarConductDebuff, StaticValues.stellarConductInterval);
                            }
                        });
                    stacks = 0;
                }
            }
        }
        private void OnEnable()
        {
            SceneCamera.onSceneCameraPreRender += AdjustStarToPOV;
        }
        private void OnDisable()
        {
            SceneCamera.onSceneCameraPreRender -= AdjustStarToPOV;
        }
        private void AdjustStarToPOV(SceneCamera camera)
        {
            if (initialized)
            {
                Vector3 forward = camera.transform.right;
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
        public override void PreReturnToPool()
        {
            base.PreReturnToPool();
            nextReleaseTime = Run.FixedTimeStamp.positiveInfinity;
            aimRequest?.Dispose();
            initialized = false;
        }
    }
}
