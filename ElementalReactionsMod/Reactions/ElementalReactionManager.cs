using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;

namespace ElementalReactionsMod.Reactions
{
    public class ElementalReactionManager : MonoBehaviour
    {
        public static ElementalReactionManager instance;
        public void OnEnable()
        {
            SingletonHelper.Assign(ref instance, this);
        }
        public void OnDisable()
        {
            SingletonHelper.Unassign(ref instance, this);
        }
    }
}
