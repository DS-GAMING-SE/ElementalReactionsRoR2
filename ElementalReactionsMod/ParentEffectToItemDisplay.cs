using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using ElementalReactionsMod.Items;

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
            if (!effectComponent.noEffectData && effectComponent.effectData.genericBool)
            {
                // Using negatives for the model child index so it fails to parent the effect to the normal transform in effectdata
                if (effectComponent.effectData.modelChildIndex == (short)ItemDisplayParent.Delusion)
                {
                    if (effectComponent.effectData.rootObject && effectComponent.effectData.rootObject.TryGetComponent<DelusionBehaviour>(out var delusion))
                    {
                        parentTransform = delusion.delusionDisplay.transform;
                        return;
                    }
                }
                else if (effectComponent.effectData.modelChildIndex == (short)ItemDisplayParent.MoonWheel)
                {

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
            Delusion = -1,
            MoonWheel = -2
        }
    }
}
