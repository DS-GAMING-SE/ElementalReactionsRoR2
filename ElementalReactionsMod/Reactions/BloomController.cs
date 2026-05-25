using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ElementalReactionsMod.Util;
using ElementalReactionsMod;
using ElementalReactionsMod.Elements;

namespace ElementalReactionsMod.Reactions
{
    public class BloomController : MonoBehaviour, IOnIncomingDamageServerReceiver
    {
        public void OnIncomingDamageServer(DamageInfo damageInfo)
        {
            ElementIndex element = damageInfo.damageType.GetElement();
            if (element == DefaultElementDefs.pyroElement.index)
            {
                Chat.AddMessage("burgeon");
            }
            else if (element == DefaultElementDefs.electroElement.index)
            {
                Chat.AddMessage("hyperbloom");
            }
            damageInfo.rejected = true;
        }
    }
}
