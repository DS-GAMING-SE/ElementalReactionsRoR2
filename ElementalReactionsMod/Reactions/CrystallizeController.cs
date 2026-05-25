using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;

namespace ElementalReactionsMod.Reactions
{
    public class CrystallizeController : MonoBehaviour
    {
        public TeamFilter teamFilter;
        public GameObject baseGameObject;
        private void OnTriggerStay(Collider other)
        {
            if (NetworkServer.active && teamFilter.teamIndex == TeamIndex.None || TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
            {
                CharacterBody component = other.GetComponent<CharacterBody>();
                if (component)
                {
                    component.OnPickup(CharacterBody.PickupClass.Minor);
                    HealthComponent healthComponent = component.healthComponent;
                    if (healthComponent && healthComponent.alive)
                    {
                        float barrier = (component.healthComponent.fullBarrier * StaticValues.crystallizeMaxBarrierPercent) - component.healthComponent.barrier;
                        if (barrier > 0) component.healthComponent.AddBarrier(Mathf.Min(barrier, component.healthComponent.fullBarrier * StaticValues.crystallizeBarrierPercent));
                        GameObject.Destroy(baseGameObject);
                    }
                }
            }
        }
    }
}
