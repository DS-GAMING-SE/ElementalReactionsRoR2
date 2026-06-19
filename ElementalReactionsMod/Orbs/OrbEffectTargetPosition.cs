using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod.Orbs
{
    [RequireComponent(typeof(EffectManagerHelper))]
    [RequireComponent(typeof(OrbEffect))]
    public class OrbEffectTargetPosition : MonoBehaviour
    {
        public EffectManagerHelper emh;
        public OrbEffect orb;
        public void Awake()
        {
            emh = GetComponent<EffectManagerHelper>();
            emh.OnEffectActivated += HijackOrbTarget;
            orb = GetComponent<OrbEffect>();
        }
        public void HijackOrbTarget()
        {
            if (!emh.effectComponent || emh.effectComponent.noEffectData) return;

            orb.targetTransform = null;
            orb.lastKnownTargetPosition = emh.effectComponent.effectData.start;
        }
    }
}
