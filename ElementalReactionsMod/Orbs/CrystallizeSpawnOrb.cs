using ElementalReactionsMod.Reactions;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.SolusWing.SolusWingPodAI.Simulation.SimulationState;

namespace ElementalReactionsMod.Orbs
{
    public class CrystallizeSpawnOrb : Orb
    {
        public TeamIndex team;
        public CharacterMaster owner;
        public Vector3 targetPosition;
        public override void Begin()
        {
            base.duration = 0.2f;
            EffectData effectData = new EffectData
            {
                origin = this.origin,
                start = targetPosition,
                genericFloat = base.duration
            };
            EffectManager.SpawnEffect(ElementalReactionManager.crystallizeSpawnOrbEffect.WaitForCompletion(), effectData, true);
        }
        public override void OnArrival()
        {
            base.OnArrival();
            GameObject crystallize = GameObject.Instantiate(ElementalReactionManager.crystallizePickup.WaitForCompletion(), targetPosition, Quaternion.identity);
            if (owner) owner.AddDeployable(crystallize.GetComponent<Deployable>(), ElementalReactionManager.crystallizeDeployableSlot);
            if (crystallize) crystallize.GetComponent<TeamFilter>().teamIndex = team;
            NetworkServer.Spawn(crystallize);
        }

        public static void SpawnCrystallize(CharacterMaster owner, TeamIndex team, Vector3 targetPosition, Vector3 originPosition)
        {
            OrbManager.instance.AddOrb(new CrystallizeSpawnOrb
            {
                owner = owner,
                team = team,
                targetPosition = targetPosition,
                origin = originPosition
            });
        }
    }
}
