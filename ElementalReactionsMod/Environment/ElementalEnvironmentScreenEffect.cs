using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using ElementalReactionsMod.Elements;

namespace ElementalReactionsMod.Environment
{
    public class ElementalEnvironmentScreenEffect : MonoBehaviour
    {
        public MeshRenderer halfL;
        private MaterialPropertyBlock propertyBlock;
        public MeshRenderer halfR;

        private float alpha;
        private float targetAlpha;

        private bool dirty = true;

        private static int tintColorKey = -1;
        private static int alphaKey = -1;

        private const float alphaSpeed = 2.5f;

        private ElementIndex element;
        private Color color;

        private void Start()
        {
            if (tintColorKey == -1) { tintColorKey = Shader.PropertyToID("_TintColor"); }
            if (alphaKey == -1) { alphaKey = Shader.PropertyToID("_ExternalAlpha"); }
            propertyBlock = new MaterialPropertyBlock();
        }
        public void SetElement(ElementDef element)
        {
            if (this.element == element.index) return;
            this.element = element.index;
            color = element.color;
            dirty = true;
        }

        private void Update()
        {
            if (ElementalRain.instance) SetElement(ElementalRain.instance.element);
            if (alpha != targetAlpha)
            {
                alpha = Mathf.MoveTowards(alpha, targetAlpha, alphaSpeed * Time.deltaTime);
                dirty = true;
            }

            if (dirty)
            {
                // I fucking hate property blocks
                SetProperty(ref halfL);
                SetProperty(ref halfR);
                dirty = false;
            }
        }
        // WHY IS CHANGING TWO PROPERTIES ON TWO MATERIALS FUCKING IMPOSSIBLE? I'VE BEEN AT THIS FOR DAYS
        private void SetProperty(ref MeshRenderer renderer)
        {
            propertyBlock.Clear();
            propertyBlock.SetColor(tintColorKey, color);
            propertyBlock.SetFloat(alphaKey, alpha);
            renderer.SetPropertyBlock(propertyBlock);
        }

        public void SetActive(bool active)
        {
            targetAlpha = active ? 1.0f : 0.0f;
        }
    }
}
