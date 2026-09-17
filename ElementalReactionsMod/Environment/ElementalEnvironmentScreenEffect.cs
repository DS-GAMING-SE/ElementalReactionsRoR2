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

        private const float alphaSpeed = 2.5f;

        private ElementIndex element;
        private Color color;

        private void Start()
        {
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
        // this shit was not worth the effort
        private void SetProperty(ref MeshRenderer renderer)
        {
            propertyBlock.Clear();
            propertyBlock.SetFloat(ShaderPropertyKeys.externalAlphaKey, alpha);
            propertyBlock.SetColor(ShaderPropertyKeys.tintColorKey, color);
            renderer.SetPropertyBlock(propertyBlock);
        }

        public void SetActive(bool active)
        {
            targetAlpha = active ? 1.0f : 0.0f;
        }
    }
}
