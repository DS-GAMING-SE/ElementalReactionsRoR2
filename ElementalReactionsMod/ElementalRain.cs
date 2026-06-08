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

namespace ElementalReactionsMod
{
    public class ElementalRain : MonoBehaviour
    {
        public static GameObject elementalRainPrefab;
        public static void Initialize()
        {
            elementalRainPrefab = PrefabAPI.CreateEmptyPrefab("ElementalRainManager");
            elementalRainPrefab.AddComponent<ElementalRain>();
            //Stage.onServerStageBegin += TrySpawnElementalRain;
        }
        private static void TrySpawnElementalRain(Stage stage)
        {
            if (stage.sceneDef.cachedName == "moon2" || stage.sceneDef.cachedName == "meridian")
            {
                GameObject.Instantiate(elementalRainPrefab);
            }
        }
        
        public static ElementalRain instance;
        public void OnEnable()
        {
            SingletonHelper.Assign(ref instance, this);
        }
        public void OnDisable()
        {
            SingletonHelper.Unassign(ref instance, this);
            CancelRaycastJob();
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
                if (this.rainRaycastJob != null)
                {
                    this.rainRaycastJob.Value.Complete();
                    this.rainRaycastJob = null;
                    RainApplyHydro();
                }
                this.rainStopwatch += Time.fixedDeltaTime;
                if (this.rainStopwatch >= rainInterval)
                {
                    this.rainStopwatch = Mathf.Min(this.rainStopwatch - rainInterval, 0f);
                    this.ScheduleRaycastJob();
                }
            }
        }
        private void RainApplyHydro()
        {
            for (int i = 0; i < characterBodies.Length; i++)
            {
                // stupid dumb raycast NEVER hits anything
                if (characterBodies[i] && rainRaycastHitBuffer[i].colliderInstanceID == 0 && !characterBodies[i].HasBuff(DLC1Content.Buffs.ImmuneToDebuffReady) &&
                    (!characterBodies[i].TryGetComponent<ElementLoadoutComponent>(out var loadout) || !loadout.permanentlyAppliedElement || loadout.permanentlyAppliedElement == DefaultElementDefs.physicalElement))
                {
                    ElementalReactionManager.ApplyElement(DefaultElementDefs.hydroElement, characterBodies[i], 0.5f);
                }
            }
            rainRaycastCommands.Dispose();
            rainRaycastHitBuffer.Dispose();
        }
        private void ScheduleRaycastJob()
        {
            CancelRaycastJob();

            characterBodies = CharacterBody.readOnlyInstancesList.ToArray();
            this.rainRaycastCommands = new NativeArray<RaycastCommand>(characterBodies.Length, Allocator.TempJob);
            this.rainRaycastHitBuffer = new NativeArray<RaycastHit>(characterBodies.Length, Allocator.TempJob);
            for (int i = 0; i < characterBodies.Length; i++)
            {
                rainRaycastCommands[i] = new RaycastCommand(characterBodies[i].corePosition, Vector3.up, float.MaxValue, LayerIndex.world.intVal);
                Debug.DrawLine(characterBodies[i].corePosition, characterBodies[i].corePosition + (Vector3.up * 10000f), Color.red, rainInterval);
                Log.Message($"body pos {characterBodies[i].corePosition}");
            }
            rainRaycastJob = RaycastCommand.ScheduleBatch(this.rainRaycastCommands, this.rainRaycastHitBuffer, characterBodies.Length);
        }
        private void CancelRaycastJob()
        {
            if (this.rainRaycastJob != null)
            {
                this.rainRaycastJob.GetValueOrDefault().Complete();
                rainRaycastCommands.Dispose();
                rainRaycastHitBuffer.Dispose();
            }
            this.rainRaycastJob = null;
        }
    }
}
