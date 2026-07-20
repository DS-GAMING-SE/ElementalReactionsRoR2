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

namespace ElementalReactionsMod.Environment
{
    public class ElementalRain : MonoBehaviour
    {
        public static GameObject elementalRainPrefab;
        public static void Initialize()
        {
            elementalRainPrefab = PrefabAPI.CreateEmptyPrefab("ElementalRainManager");
            elementalRainPrefab.AddComponent<ElementalRain>();
            Stage.onServerStageBegin += TrySpawnElementalRain;

            // SS2 backthruster code yoinked for the screen effect
            CameraRigController.onCameraEnableGlobal += AddEnvironmentScreenEffect;
            CameraRigController.onCameraDisableGlobal += RemoveEnvironmentScreenEffect;
            SceneCamera.onSceneCameraPreCull += (sceneCam) =>
            {
                if (camerasToScreenEffects.TryGetValue(sceneCam.cameraRigController, out ElementalEnvironmentScreenEffect effect))
                {
                    effect.gameObject.layer = LayerIndex.defaultLayer.intVal;
                }
            };
            SceneCamera.onSceneCameraPostRender += (sceneCam) =>
            {
                if (camerasToScreenEffects.TryGetValue(sceneCam.cameraRigController, out ElementalEnvironmentScreenEffect effect))
                {
                    effect.gameObject.layer = LayerIndex.noDraw.intVal;
                }
            };
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
                GameObject rainParent = GameObject.Find("Weather, Meridian/CAMERA PARTICLES: RainParticles");
                if (rainParent)
                {
                    Instantiate(elementalRainPrefab, rainParent.transform);
                }
            }
        }
        internal static Dictionary<CameraRigController, ElementalEnvironmentScreenEffect> camerasToScreenEffects = new Dictionary<CameraRigController, ElementalEnvironmentScreenEffect>();
        private static void AddEnvironmentScreenEffect(CameraRigController camera)
        {
            if (ElementalReactionManager.instance)
            {
                if (!camerasToScreenEffects.ContainsKey(camera))
                {
                    GameObject screenEffect = GameObject.Instantiate(ElementalReactionManager.elementalEnvironmentScreenEffect.WaitForCompletion(), camera.sceneCam.transform);
                    screenEffect.transform.localPosition = ElementalReactionManager.elementalEnvironmentScreenEffect.WaitForCompletion().transform.position;
                    screenEffect.transform.localRotation = ElementalReactionManager.elementalEnvironmentScreenEffect.WaitForCompletion().transform.rotation;
                    camerasToScreenEffects.Add(camera, screenEffect.GetComponent<ElementalEnvironmentScreenEffect>());
                }
            }
        }
        private static void RemoveEnvironmentScreenEffect(CameraRigController camera)
        {
            if (camerasToScreenEffects.ContainsKey(camera))
            {
                camerasToScreenEffects.Remove(camera);
            }
        }
        public static ElementalRain instance;
        public bool raining { get { return _raining; } 
            set 
            {
                if (value == _raining) return;
                if (_raining)
                {
                    EnableRain();
                }
                else
                {
                    DisableRain();
                }
                _raining = value;
            } }
        public bool _raining = true;
        public ElementDef element;
        public int[] colliderInstanceIDsToIgnore;
        public void OnEnable()
        {
            SingletonHelper.Assign(ref instance, this);
            if (!element || element == DefaultElementDefs.physicalElement)
            {
                element = DefaultElementDefs.hydroElement;
            }
            if (raining)
            {
                EnableRain();
            }
        }
        private void EnableRain()
        {
            CharacterBody.onBodyDestroyGlobal += RemoveCachedLoadout;
            characterBodies = new List<CharacterBody>();
            cachedLoadoutComponents = new Dictionary<CharacterBody, ElementLoadoutComponent>();
            ActivationChatMessage();
        }
        public void OnDisable()
        {
            SingletonHelper.Unassign(ref instance, this);
            DisableRain();
        }
        private void DisableRain()
        {
            CharacterBody.onBodyDestroyGlobal -= RemoveCachedLoadout;
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
            if (element == DefaultElementDefs.hydroElement)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = $"{ElementalReactionsPlugin.PREFIX}EVENT_ELEMENTAL_RAIN_HYDRO"
                });
                return;
            }
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = $"{ElementalReactionsPlugin.PREFIX}EVENT_ELEMENTAL_RAIN"
            });
        }

        private float rainStopwatch;
        private const float rainInterval = 0.25f;
        private JobHandle? rainRaycastJob;
        private NativeArray<RaycastCommand> rainRaycastCommands;
        private NativeArray<RaycastHit> rainRaycastHitBuffer;
        private List<CharacterBody> characterBodies;
        private Dictionary<CharacterBody, ElementLoadoutComponent> cachedLoadoutComponents;
        private void FixedUpdate()
        {
            if (NetworkServer.active && raining)
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
            for (int i = 0; i < characterBodies.Count; i++)
            {
                if (characterBodies[i] && (rainRaycastHitBuffer[i].colliderInstanceID == 0 || (colliderInstanceIDsToIgnore != null && colliderInstanceIDsToIgnore.Contains(rainRaycastHitBuffer[i].colliderInstanceID))))
                {
                    ElementalReactionManager.ApplyElement(element, characterBodies[i], 0.25f, null, false, 1f);
                    characterBodies[i].AddTimedBuff(Buffs.elementalEnvironmentHiddenBuff.buffIndex, rainInterval * 1.5f);
                }
            }
            rainRaycastCommands.Dispose();
            rainRaycastHitBuffer.Dispose();
            characterBodies.Clear();
        }
        private void ScheduleRaycastJob()
        {
            CancelRaycastJob();
            // Copy CharacterBody list. Can't use original list directly because we can't have the list change between now and when we check again next frame
            characterBodies.Clear();
            TransformAccessArray transformAccessArray = new TransformAccessArray(CharacterBody.readOnlyInstancesList.Count);
            // Copy all CharacterBody transforms to TransformAccessArray
            for (int i = 0; i < CharacterBody.readOnlyInstancesList.Count; i++)
            {
                if (BodyAffectedByRain(CharacterBody.readOnlyInstancesList[i])) // cache elementloadoutcomponents? memoizedgetcomponent? write your own?
                {
                    characterBodies.Add(CharacterBody.readOnlyInstancesList[i]);
                    transformAccessArray.Add(CharacterBody.readOnlyInstancesList[i].transform);
                }
            }
            // A job creates the raycast commands using the TransformAccessArray
            rainRaycastCommands = new NativeArray<RaycastCommand>(characterBodies.Count, Allocator.TempJob);
            CreateRaycastCommandsJob createRaycastCommandsJob = new CreateRaycastCommandsJob
            {
                output = rainRaycastCommands,
                physicsScene = Physics.defaultPhysicsScene,
                raycastMask = LayerIndex.world.mask
            };
            // The job that actually does the raycasts, using the commands generated by the last job
            JobHandle createRaycastJobHandle = createRaycastCommandsJob.ScheduleReadOnly(transformAccessArray, 32);
            rainRaycastHitBuffer = new NativeArray<RaycastHit>(characterBodies.Count, Allocator.TempJob);
            rainRaycastJob = RaycastCommand.ScheduleBatch(rainRaycastCommands, rainRaycastHitBuffer, characterBodies.Count, createRaycastJobHandle);
            transformAccessArray.Dispose();
        }
        private void RemoveCachedLoadout(CharacterBody body)
        {
            if (cachedLoadoutComponents.ContainsKey(body))
            {
                cachedLoadoutComponents.Remove(body);
            }
        }
        private bool TryGetElementLoadout(CharacterBody body, out ElementLoadoutComponent elementLoadoutComponent)
        {
            elementLoadoutComponent = null;
            if (cachedLoadoutComponents.TryGetValue(body, out elementLoadoutComponent))
            {
            }
            else
            {
                elementLoadoutComponent = body.GetComponent<ElementLoadoutComponent>();
                cachedLoadoutComponents.Add(body, elementLoadoutComponent);
            }
            return elementLoadoutComponent;
        }
        protected virtual bool BodyAffectedByRain(CharacterBody characterBody)
        {
            return !characterBody.HasBuff(DLC1Content.Buffs.ImmuneToDebuffReady) &&
                    (!TryGetElementLoadout(characterBody, out var loadout) || !loadout.permanentlyAppliedElement || 
                    loadout.permanentlyAppliedElement == DefaultElementDefs.physicalElement);
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
