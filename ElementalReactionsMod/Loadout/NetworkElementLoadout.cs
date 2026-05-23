using ElementalReactionsMod.Elements;
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

namespace ElementalReactionsMod.Loadout
{
    public class NetworkElementLoadout : INetMessage
    {
        NetworkInstanceId netId;
        ElementIndex primary;
        ElementIndex secondary;
        ElementIndex utility;
        ElementIndex special;
        public NetworkElementLoadout()
        {

        }

        public NetworkElementLoadout(NetworkInstanceId netId, ElementIndex primary, ElementIndex secondary, ElementIndex utility, ElementIndex special)
        {
            this.netId = netId;
            this.primary = primary;
            this.secondary = secondary;
            this.utility = utility;
            this.special = special;
        }

        public void OnReceived()
        {
            GameObject gameObject = RoR2.Util.FindNetworkObject(this.netId);
            if (gameObject && gameObject.TryGetComponent<ElementLoadoutComponent>(out var loadout))
            {
                loadout.ApplyElementLoadout([primary, secondary, utility, special]);
            }
        }
        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netId);
            writer.WritePackedIndex32((int)primary);
            writer.WritePackedIndex32((int)secondary);
            writer.WritePackedIndex32((int)utility);
            writer.WritePackedIndex32((int)special);
        }

        public void Deserialize(NetworkReader reader)
        {
            netId = reader.ReadNetworkId();
            primary = (ElementIndex)reader.ReadPackedIndex32();
            secondary = (ElementIndex)reader.ReadPackedIndex32();
            utility = (ElementIndex)reader.ReadPackedIndex32();
            special = (ElementIndex)reader.ReadPackedIndex32();
        }
    }
}
