using ElementalReactionsMod.Elements;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod
{
    [RequireComponent(typeof(EffectComponent), typeof(EffectManagerHelper))]
    public class GenericElementEffectComponent : MonoBehaviour
    {
        public static void SpawnActivatedEffect(Vector3 position, Quaternion rotation, ElementIndex element, bool transmit)
        {
            SpawnActivatedEffect(position, rotation, element, 0.6f, transmit);
        }
        public static void SpawnActivatedEffect(Vector3 position, Quaternion rotation, ElementIndex element, float duration, bool transmit)
        {
            EffectManager.SpawnEffect(Assets.genericElementActivatedEffect, new EffectData { origin = position, rotation = rotation, genericUInt = (uint)element, genericFloat = duration }, transmit);
        }
        public static void SpawnActivatedEffect(Transform parent, ElementIndex element, bool transmit)
        {
            SpawnActivatedEffect(parent, element, 0.6f, transmit);
        }
        public static void SpawnActivatedEffect(Transform parent, ElementIndex element, float duration, bool transmit)
        {
            EffectManager.SpawnEffect(Assets.genericElementActivatedEffect, new EffectData { origin = parent.position, rootObject = parent.gameObject, genericUInt = (uint)element, genericFloat = duration }, transmit);
        }

        private EffectManagerHelper efh;
        private EffectComponent effectComponent;
        public ScaleParticleSystemDuration particleDuration;

        public ParticleSystemRenderer icon;
        public ParticleSystem[] particlesToRecolor;
        public bool scaleDuration;
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
            if (icon) icon.sharedMaterial = element.iconVFX;
            for (int i = 0; i < particlesToRecolor.Length; i++)
            {
                ParticleSystem.MainModule main = particlesToRecolor[i].main;
                main.startColor = new ParticleSystem.MinMaxGradient(element.color);
            }
            if (scaleDuration && particleDuration)
            {
                particleDuration.newDuration = effectComponent.effectData.genericFloat;
            }
        }

    }
}
