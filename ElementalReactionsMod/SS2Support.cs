using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Environment;
using ElementalReactionsMod.Items;
using ElementalReactionsMod.Reactions;
using EntityStates.Events;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using RoR2;
using SS2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ElementalReactionsMod
{
    public static class SS2Support
    {
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void Initialize()
        {
            SS2Assets.assetsAvailability.onAvailable += () =>
            {
                SS2Assets.LoadAsset<GameObject>("StormController", SS2Bundle.Events).AddComponent<ElementalRain>().enabled = false;
            };
            PickupCatalog.availability.onAvailable += () =>
            {
                Crafting.AppendDelusionRecipe(Crafting.delusionElectro, ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("LightningOnKill")));
                //Crafting.AppendDelusionRecipe(Crafting.delusionElectro, SS2Assets.LoadAsset<ItemDef>("LightningOnKill", SS2Bundle.Items));
            };

            new Hook(typeof(EntityStates.Events.Storm).GetMethod(nameof(EntityStates.Events.Storm.OnEnter)), SS2StormStartWeather);
            new Hook(typeof(EntityStates.Events.Storm).GetMethod(nameof(EntityStates.Events.Storm.OnExit)), SS2StormEndWeather);
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static void SS2StormStartWeather(Action<EntityStates.Events.Storm> orig, EntityStates.Events.Storm self)
        {
            orig(self);
            if (!Config.CanSS2StormsUseElements().Value || !Stage.instance || Stage.instance.sceneDef.cachedName == "goolake"/*abandoned aqueducts*/) return;
            if (NetworkServer.active && ElementalReactionManager.instance && !ElementalRain.instance && self.gameObject.TryGetComponent<ElementalRain>(out var rain))
            {
                // I cannot be bothered to go through the whole damn process of publicizing just so I can use ss2's GETDUMBASSTOKENDELETELATER method
                if (Stage.instance)
                {
                    switch(Stage.instance.sceneDef.cachedName)
                    {
                        case "frozenwall": //rallypoint
                        case "snowyforest": // siphoned
                            rain.element = DefaultElementDefs.cryoElement;
                            break;
                        case "dampcavesimple": // abyssal
                        case "wispgraveyard": // scorched
                            rain.element = DefaultElementDefs.pyroElement;
                            break;
                    }
                }

                rain.enabled = true;
            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static void SS2StormEndWeather(Action<EntityStates.Events.Storm> orig, EntityStates.Events.Storm self)
        {
            orig(self);
            if (NetworkServer.active && ElementalReactionManager.instance && self.gameObject.TryGetComponent<ElementalRain>(out var rain) && ElementalRain.instance == rain)
            {
                rain.enabled = false;
            }
        }
    }
}
