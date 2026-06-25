using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using UnityEngine.Networking;
using ElementalReactionsMod.Orbs;

namespace ElementalReactionsMod.Reactions
{
    public class LunarCrystallizeController : ComponentPoolObject
    {
        public CharacterBody characterBody;
        public bool initialized;
        public GameObject moondrift1;
        public GameObject moondrift2;
        public GameObject moondrift3;

        public int lastMoondriftCount;

        public Run.FixedTimeStamp fireAttacksTime = Run.FixedTimeStamp.positiveInfinity;
        public bool attackQueued;
        public const float delayBeforeAttack = 0.5f;

        public void CreateLunarCrystallizeController(CharacterBody characterBody)
        {
            this.characterBody = characterBody;
            Resize(characterBody.radius);
            UpdateMoondriftCount();
            transform.position = characterBody.corePosition;

            initialized = true;
        }

        private void Resize(float characterRadius)
        {
            float distance = characterRadius + 2.5f;
            moondrift1.transform.localPosition = new Vector3(0, 0, distance);
            moondrift2.transform.localPosition = new Vector3(0, 0, distance);
            moondrift3.transform.localPosition = new Vector3(0, 0, distance);
        }
        private void OnEnable()
        {
            SceneCamera.onSceneCameraPreRender += AdjustMoondriftsToPOV;
        }
        private void OnDisable()
        {
            SceneCamera.onSceneCameraPreRender -= AdjustMoondriftsToPOV;
        }

        private void FixedUpdate()
        {
            if (initialized)
            {
                if (!characterBody)
                {
                    PreReturnToPool();
                    ReturnToPool();
                    return;
                }
                
                UpdateMoondriftCount();

                if (fireAttacksTime.hasPassed)
                {
                    fireAttacksTime = Run.FixedTimeStamp.positiveInfinity;
                    if (NetworkServer.active)
                    {
                        LunarCrystallizeOrb.FireLunarCrystallizeOrbs(characterBody, characterBody.corePosition);
                        characterBody.SetBuffCount(Buffs.lunarCrystallizeBuff.buffIndex, characterBody.GetBuffCount(Buffs.lunarCrystallizeBuff) - StaticValues.lunarCrystallizeTriggersToAttack);
                    }
                    attackQueued = false;
                    EffectManager.SpawnEffect(ElementalReactionManager.lunarCrystallizeActivatedEffect.WaitForCompletion(), new EffectData { origin = characterBody.corePosition, rootObject = characterBody.gameObject }, false);
                }
            }
        }
        private void UpdateMoondriftCount()
        {
            if (characterBody)
            {
                if (!characterBody.HasBuff(Buffs.lunarCrystallizeBuff))
                {
                    PreReturnToPool();
                    ReturnToPool();
                    return;
                }
                if (characterBody.GetBuffCount(Buffs.lunarCrystallizeBuff) != lastMoondriftCount)
                {
                    if (characterBody.GetBuffCount(Buffs.lunarCrystallizeBuff) > lastMoondriftCount)
                    {
                        RoR2.Util.PlaySound("Play_seeker_skill2_alt_fire", gameObject);
                    }
                    moondrift1.SetActive(characterBody.GetBuffCount(Buffs.lunarCrystallizeBuff) >= 1);
                    moondrift2.SetActive(characterBody.GetBuffCount(Buffs.lunarCrystallizeBuff) >= 2);
                    moondrift3.SetActive(characterBody.GetBuffCount(Buffs.lunarCrystallizeBuff) >= 3);
                    lastMoondriftCount = characterBody.GetBuffCount(Buffs.lunarCrystallizeBuff);
                }
                if (characterBody.GetBuffCount(Buffs.lunarCrystallizeBuff) >= StaticValues.lunarCrystallizeTriggersToAttack && !attackQueued)
                {
                    fireAttacksTime = Run.FixedTimeStamp.now + delayBeforeAttack;
                    attackQueued = true;
                }
            }
        }

        private void AdjustMoondriftsToPOV(SceneCamera camera)
        {
            if (initialized)
            {
                Vector3 forward = camera.transform.forward;
                forward.y = 0;
                transform.SetPositionAndRotation(characterBody ? characterBody.corePosition : transform.position, Quaternion.LookRotation(forward));
            }
        }

        public override void PreReturnToPool()
        {
            base.PreReturnToPool();
            fireAttacksTime = Run.FixedTimeStamp.positiveInfinity;
            attackQueued = false;
            lastMoondriftCount = 0;
            initialized = false;
        }
    }
}
