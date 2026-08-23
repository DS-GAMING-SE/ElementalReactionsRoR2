using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Items;
using ElementalReactionsMod.Reactions;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ElementalReactionsMod.Items.MoonWheel;

namespace ElementalReactionsMod
{
    [RequireComponent(typeof(EffectComponent), typeof(EffectManagerHelper))]
    public class ParentEffectToItemDisplay : MonoBehaviour
    {
        private EffectManagerHelper efh;
        private EffectComponent effectComponent;

        private Transform parentTransform;

        private void Awake()
        {
            efh = GetComponent<EffectManagerHelper>();
            effectComponent = GetComponent<EffectComponent>();
            if (efh)
            {
                efh.OnEffectActivated += ReparentEffect;
            }
        }
        private void ReparentEffect()
        {
            if (effectComponent.effectData.genericBool)
            {
                // Using negatives for the model child index so it fails to parent the effect to the normal transform in effectdata
                if (effectComponent.effectData.modelChildIndex - 256 ==(short)ItemDisplayParent.Delusion)
                {
                    if (effectComponent.effectData.rootObject && effectComponent.effectData.rootObject.TryGetComponent<DelusionBehaviour>(out var delusion) && delusion.delusionDisplay)
                    {
                        parentTransform = delusion.delusionDisplay.transform;
                        return;
                    }
                }
                else if (effectComponent.effectData.modelChildIndex - 256 ==(short)ItemDisplayParent.MoonWheel)
                {
                    if (effectComponent.effectData.rootObject && effectComponent.effectData.rootObject.TryGetComponent<MoonWheel.MoonWheelBehaviour>(out var moonWheel) && moonWheel.moonWheelDisplay)
                    {
                        parentTransform = moonWheel.moonWheelDisplay.transform;
                        return;
                    }
                }
                else if (effectComponent.effectData.modelChildIndex - 256 == (short)ItemDisplayParent.StellarLinchpin)
                {
                    if (effectComponent.effectData.rootObject && effectComponent.effectData.rootObject.TryGetComponent<StellarLinchpin.StellarLinchpinBehaviour>(out var stellarLinchpin) && stellarLinchpin.stellarLinchpinDisplay)
                    {
                        parentTransform = stellarLinchpin.stellarLinchpinDisplay.transform;
                        return;
                    }
                }
            }
            parentTransform = null;
        }

        private void LateUpdate()
        {
            if (parentTransform)
            {
                transform.position = parentTransform.position;
            }
        }
        public enum ItemDisplayParent : short
        {
            Delusion = -2,
            MoonWheel = -3,
            StellarLinchpin = -4
        }
    }
}
