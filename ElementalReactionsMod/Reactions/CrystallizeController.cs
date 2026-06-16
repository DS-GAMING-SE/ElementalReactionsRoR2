using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.Collections;

namespace ElementalReactionsMod.Reactions
{
    public class CrystallizeController : MonoBehaviour
    {
        public TeamFilter teamFilter;
        public GameObject baseGameObject;
        public SphereCollider gravitateCollider;
        public ParticleSystem[] teamRecolorParticle;
        private Deployable deployable;
        public void Awake()
        {
            deployable = baseGameObject.GetComponent<Deployable>();
            if (deployable)
            {
                deployable.onUndeploy.AddListener(LimitReached);
            }
        }
        private void Start()
        {
            if (NetworkServer.active)
            {
                gravitateCollider.radius = teamFilter.teamIndex == TeamIndex.Player ? StaticValues.crystallizePlayerGravitateRange : StaticValues.crystallizeEnemyGravitateRange;
            }
            if (teamFilter.teamIndex != TeamIndex.Player)
            {
                foreach (var particle in teamRecolorParticle)
                {
                    var main = particle.main;
                    main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.17f, 0.25f));
                }
            }
        }
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
                        if (baseGameObject.TryGetComponent<ElementalReactionPooledObject>(out var pool))
                        {
                            pool.ReturnObject();
                        }
                        else
                        {
                            GameObject.Destroy(baseGameObject);
                        }
                    }
                }
            }
        }
        private void LimitReached()
        {
            DestroyAfterTimer();
        }
        private IEnumerator DestroyAfterTimer()
        {
            yield return new WaitForSeconds(0.5f);
            if (baseGameObject)
            {
                GameObject.Destroy(baseGameObject);
            }
        }
    }
}
