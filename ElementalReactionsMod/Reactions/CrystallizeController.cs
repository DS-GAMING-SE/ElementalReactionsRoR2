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
            RoR2.Util.PlaySound("Play_moonBrother_m1_laser_shoot", gameObject);
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
                if (other.TryGetComponent<CharacterBody>(out var characterBody))
                {
                    characterBody.OnPickup(CharacterBody.PickupClass.Minor);
                    if (characterBody.healthComponent && characterBody.healthComponent.alive)
                    {
                        float barrier = (characterBody.healthComponent.fullBarrier * StaticValues.crystallizeMaxBarrierPercent) - characterBody.healthComponent.barrier;
                        if (barrier > 0) characterBody.healthComponent.AddBarrier(Mathf.Min(barrier, characterBody.healthComponent.fullBarrier * (characterBody.isBoss || characterBody.isChampion ? StaticValues.crystallizeBossBarrierPercent: StaticValues.crystallizeBarrierPercent)));
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
