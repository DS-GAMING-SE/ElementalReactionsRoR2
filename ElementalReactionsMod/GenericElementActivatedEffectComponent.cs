using ElementalReactionsMod.Elements;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod
{
    [RequireComponent(typeof(EffectComponent), typeof(EffectManagerHelper))]
    public class GenericElementActivatedEffectComponent : MonoBehaviour
    {
        public static void SpawnEffect(Vector3 position, Quaternion rotation, ElementIndex element, bool transmit)
        {
            EffectManager.SpawnEffect(Assets.genericElementActivatedEffect, new EffectData { origin = position, rotation = rotation, genericUInt = (uint)element }, transmit);
        }
        public static void SpawnEffect(Transform parent, ElementIndex element, bool transmit)
        {
            EffectManager.SpawnEffect(Assets.genericElementActivatedEffect, new EffectData { origin = parent.position, rootObject = parent.gameObject, genericUInt = (uint)element }, transmit);
        }

        private EffectManagerHelper efh;
        private EffectComponent effectComponent;

        public ParticleSystemRenderer icon;
        public ParticleSystem[] mainRecolors;
        private void Awake()
        {
            efh = GetComponent<EffectManagerHelper>();
            effectComponent = GetComponent<EffectComponent>();
            if (efh)
            {
                efh.OnEffectActivated += SetElementVFX;
            }
        }
        private void SetElementVFX()
        {
            ElementDef element = ElementCatalog.GetElementDef((ElementIndex)effectComponent.effectData.genericUInt);
            if (!element) return;
            icon.sharedMaterial = element.iconVFX;
            for (int i = 0; i < mainRecolors.Length; i++)
            {
                ParticleSystem.MainModule main = mainRecolors[i].main;
                main.startColor = new ParticleSystem.MinMaxGradient(element.color);
            }
        }

    }
}
