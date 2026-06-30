using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;

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
            EffectManager.SpawnEffect(ElementalReactionManager.genericElementActivatedEffect.WaitForCompletion(), new EffectData { origin = position, rotation = rotation, genericUInt = (uint)element, genericFloat = duration }, transmit);
        }
        public static void SpawnActivatedEffect(Transform parent, ElementIndex element, bool transmit)
        {
            SpawnActivatedEffect(parent, element, 0.7f, transmit);
        }
        public static void SpawnActivatedEffect(Transform parent, ElementIndex element, float duration, bool transmit)
        {
            EffectManager.SpawnEffect(ElementalReactionManager.genericElementActivatedEffect.WaitForCompletion(), new EffectData { origin = parent.position, rootObject = parent.gameObject, genericUInt = (uint)element, genericFloat = duration }, transmit);
        }
        public static void SpawnActivatedEffect(Transform parent, ParentEffectToItemDisplay.ItemDisplayParent itemDisplayParent, ElementIndex element, float duration, bool transmit)
        {
            EffectManager.SpawnEffect(ElementalReactionManager.genericElementActivatedEffect.WaitForCompletion(), new EffectData { origin = parent.position, rootObject = parent.gameObject, genericUInt = (uint)element, genericFloat = duration, genericBool = true, modelChildIndex = (short)itemDisplayParent }, transmit);
        }

        private EffectManagerHelper efh;
        private EffectComponent effectComponent;
        public ScaleParticleSystemDuration particleDuration;

        public ParticleSystemRenderer icon;
        public ParticleSystem[] particlesToRecolor;
        public TrailRenderer[] trailsToRecolor;
        public bool darkenTrailColor = true;
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
            for (int i = 0; i < trailsToRecolor.Length; i++)
            {
                var gradient = trailsToRecolor[i].GetColorGradientCopy();
                Color trailColor = element.color;
                if (darkenTrailColor)
                {
                    trailColor.r -= 0.4f;
                    trailColor.g -= 0.4f;
                    trailColor.b -= 0.4f;
                }
                gradient.colorKeys = [new GradientColorKey(trailColor, 0f)];
                trailsToRecolor[i].SetColorGradient(gradient);
            }
            if (particleDuration)
            {
                particleDuration.newDuration = effectComponent.effectData.genericFloat;
            }
        }

    }
}
