using Grumpy;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Pool;

namespace ElementalReactionsMod.Reactions
{
    public class ElementalReactionPooledObject : MonoBehaviour
    {
        public PrefabComponentPool<ElementalReactionPooledObject> pool;
        public Deployable deployable;
        public TeamFilter teamFilter;
        public CharacterBody characterBody;
        public NetworkIdentity networkIdentity;
        public GravitatePickup gravitatePickup;

        public bool newlySpawned = true;

        public void Awake()
        {
            deployable = GetComponent<Deployable>();
            teamFilter = GetComponent<TeamFilter>();
            characterBody = GetComponent<CharacterBody>();
            networkIdentity = GetComponent<NetworkIdentity>();
        }

        public void Start()
        {
            if (deployable) deployable.onUndeploy.AddListener(ReturnObject);
        }

        public void ReturnObject()
        {
            if (pool != null)
            {
                if (deployable && deployable.ownerMaster) deployable.ownerMaster.RemoveDeployable(deployable);
                if (characterBody) CleanseSystem.CleanseBodyServer(characterBody, true, true, true, true, false, false);
                if (gravitatePickup) gravitatePickup.gravitateTarget = null;
                pool.ReturnObject(this);
                new NetworkPooledObjectSetActive(this.networkIdentity.netId, false).Send(R2API.Networking.NetworkDestination.Clients);
                return;
            }
            GameObject.Destroy(this.gameObject);
        }
    }
}
