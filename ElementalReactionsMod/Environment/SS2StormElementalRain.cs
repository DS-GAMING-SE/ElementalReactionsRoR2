using MSU;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod.Environment
{
    public class SS2StormElementalRain : ElementalRain
    {
        // Add this to the stormcontroller prefab. Set default _raining to false
        // get MSU.GameplayEvent component, sub to start and end events to enable and disable raining
        // disable this whole component if there is already an elementalrain.instance. This should spawn after the original one because of difference in spawning events
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void InitializeSS2Storm()
        {
            SS2.SS2Assets.assetsAvailability.onAvailable += () => SS2.SS2Assets.LoadAsset<GameObject>("StormController", SS2.SS2Bundle.Events).AddComponent<SS2StormElementalRain>()._raining = false;
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private void Start()
        {
            if (ElementalRain.instance != this)
            {
                this.enabled = false;
                return;
            }// ss2 storms don't use the damn gameplay events even though they have the gameplay event component
            /*if (TryGetComponent<GameplayEvent>(out var gameplayEvent))
            {
                gameplayEvent.onEventStart += (_) => { if (ElementalRain.instance == this && Config.CanSS2StormsUseElements().Value) raining = true; };
                gameplayEvent.onEventEnd += (_) => raining = false;
            }*/
        }
    }
}
