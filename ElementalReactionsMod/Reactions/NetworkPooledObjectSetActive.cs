using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ElementalReactionsMod.Reactions
{
    public class NetworkPooledObjectSetActive : INetMessage
    {
        NetworkInstanceId netId;
        bool active;
        public NetworkPooledObjectSetActive()
        {

        }

        public NetworkPooledObjectSetActive(NetworkInstanceId netId, bool active)
        {
            this.netId = netId;
            this.active = active;
        }

        public void OnReceived()
        {
            if (NetworkServer.active) return;
            GameObject gameObject = RoR2.Util.FindNetworkObject(this.netId);
            if (gameObject)
            {
                gameObject.SetActive(active);
            }
        }
        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netId);
            writer.Write(active);
        }

        public void Deserialize(NetworkReader reader)
        {
            netId = reader.ReadNetworkId();
            active = reader.ReadBoolean();
        }
    }
}
