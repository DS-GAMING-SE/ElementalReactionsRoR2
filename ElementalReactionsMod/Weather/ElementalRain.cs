using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using ElementalReactionsMod.Reactions;
using System.Linq;
using R2API;
using UnityEngine.Jobs;
using System.Collections;

namespace ElementalReactionsMod.Weather
{
    public class ElementalRain : MonoBehaviour
    {
        public static GameObject elementalRainPrefab;
        public static void Initialize()
        {
            elementalRainPrefab = PrefabAPI.CreateEmptyPrefab("ElementalRainManager");
            elementalRainPrefab.AddComponent<ElementalRain>();
            Stage.onServerStageBegin += TrySpawnElementalRain;
        }
        private static void TrySpawnElementalRain(Stage stage)
        {
            if (!ElementalReactionManager.instance || !Config.CanWeatherUseElements().Value) return;
            if (stage.sceneDef.cachedName == "moon2" || stage.sceneDef.cachedName == "moon")
            {
                Instantiate(elementalRainPrefab);
                GameObject arenaRoof = GameObject.Find("SceneInfo/BrotherMissionController/ArenaWalls/Ceiling");
                if (arenaRoof && arenaRoof.TryGetComponent<MeshCollider>(out var roofCollider))
                {
                    instance.colliderInstanceIDsToIgnore = [roofCollider.GetInstanceID()];
                }
            }
            if (stage.sceneDef.cachedName == "meridian")
            {
                Transform rainParent = GameObject.Find("Weather, Meridian/CAMERA PARTICLES: RainParticles").transform;
                if (rainParent)
                {
                    Instantiate(elementalRainPrefab, rainParent);
                }
            }
        }
        
        public static ElementalRain instance;
        public ElementDef element;
        public int[] colliderInstanceIDsToIgnore;
        public void OnEnable()
        {
            SingletonHelper.Assign(ref instance, this);
            if (!element || element == DefaultElementDefs.physicalElement)
            {
                element = DefaultElementDefs.hydroElement;
            }
            ActivationChatMessage();
        }
        public void OnDisable()
        {
            SingletonHelper.Unassign(ref instance, this);
            CancelRaycastJob();
        }
        private void ActivationChatMessage()
        {
            if (element == DefaultElementDefs.cryoElement)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = $"{ElementalReactionsPlugin.PREFIX}EVENT_ELEMENTAL_RAIN_CRYO"
                });
                return;
            }
            if (element == DefaultElementDefs.pyroElement)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = $"{ElementalReactionsPlugin.PREFIX}EVENT_ELEMENTAL_RAIN_PYRO"
                });
                return;
            }
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = $"{ElementalReactionsPlugin.PREFIX}EVENT_ELEMENTAL_RAIN_HYDRO"
            });
        }

        private float rainStopwatch;
        private const float rainInterval = 0.25f;
        private JobHandle? rainRaycastJob;
        private NativeArray<RaycastCommand> rainRaycastCommands;
        private NativeArray<RaycastHit> rainRaycastHitBuffer;
        private CharacterBody[] characterBodies;
        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (rainRaycastJob != null)
                {
                    rainRaycastJob.Value.Complete();
                    rainRaycastJob = null;
                    RainApplyElement();
                }
                rainStopwatch += Time.fixedDeltaTime;
                if (rainStopwatch >= rainInterval && element && element != DefaultElementDefs.physicalElement)
                {
                    rainStopwatch = Mathf.Min(rainStopwatch - rainInterval, 0f);
                    ScheduleRaycastJob();
                }
            }
        }
        private void RainApplyElement()
        {
            for (int i = 0; i < characterBodies.Length; i++)
            {
                if (characterBodies[i] && (rainRaycastHitBuffer[i].colliderInstanceID == 0 || (colliderInstanceIDsToIgnore != null && colliderInstanceIDsToIgnore.Contains(rainRaycastHitBuffer[i].colliderInstanceID)))
                    && !characterBodies[i].HasBuff(DLC1Content.Buffs.ImmuneToDebuffReady) &&
                    (!characterBodies[i].TryGetComponent<ElementLoadoutComponent>(out var loadout) || !loadout.permanentlyAppliedElement || loadout.permanentlyAppliedElement == DefaultElementDefs.physicalElement))
                {
                    ElementalReactionManager.ApplyElement(element, characterBodies[i], 0.25f, null, false, 1f);
                }
            }
            rainRaycastCommands.Dispose();
            rainRaycastHitBuffer.Dispose();
        }
        private void ScheduleRaycastJob()
        {
            CancelRaycastJob();
            // Copy CharacterBody list. Can't use original list directly because we can't have the list change between now and when we check again next frame
            characterBodies = CharacterBody.readOnlyInstancesList.ToArray();
            TransformAccessArray transformAccessArray = new TransformAccessArray(characterBodies.Length);
            // Copy all CharacterBody transforms to TransformAccessArray
            for (int i = 0; i < characterBodies.Length; i++)
            {
                transformAccessArray.Add(characterBodies[i].transform);
            }
            // A job creates the raycast commands using the TransformAccessArray
            rainRaycastCommands = new NativeArray<RaycastCommand>(characterBodies.Length, Allocator.TempJob);
            CreateRaycastCommandsJob createRaycastCommandsJob = new CreateRaycastCommandsJob
            {
                output = rainRaycastCommands,
                physicsScene = Physics.defaultPhysicsScene,
                raycastMask = LayerIndex.world.mask
            };
            // The job that actually does the raycasts, using the commands generated by the last job
            JobHandle createRaycastJobHandle = createRaycastCommandsJob.ScheduleReadOnly(transformAccessArray, 32);
            rainRaycastHitBuffer = new NativeArray<RaycastHit>(characterBodies.Length, Allocator.TempJob);
            rainRaycastJob = RaycastCommand.ScheduleBatch(rainRaycastCommands, rainRaycastHitBuffer, characterBodies.Length, createRaycastJobHandle);
            transformAccessArray.Dispose();
        }
        private void CancelRaycastJob()
        {
            if (rainRaycastJob != null)
            {
                rainRaycastJob.GetValueOrDefault().Complete();
                rainRaycastCommands.Dispose();
                rainRaycastHitBuffer.Dispose();
            }
            rainRaycastJob = null;
        }
    }
    public struct CreateRaycastCommandsJob : IJobParallelForTransform
    {
        [ReadOnly]
        public PhysicsScene physicsScene;
        [ReadOnly]
        public LayerMask raycastMask;
        [WriteOnly]
        public NativeArray<RaycastCommand> output;
        public void Execute(int index, TransformAccess access)
        {
            output[index] = new RaycastCommand(physicsScene, access.position, Vector3.up, float.MaxValue, raycastMask, 1);
        }
    }
}
