using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod
{
    internal static class ShaderPropertyKeys
    {
        // Optimize load times? Most of my asset stuff is async so its hard to tell how much this helps
        internal static int tintColorKey = -1;
        internal static int invFadeKey = -1;
        internal static int mainTexKey = -1;
        internal static int remapTexKey = -1;
        internal static int cloud1TexKey = -1;
        internal static int cloud2TexKey = -1;
        internal static int boostKey = -1;
        internal static int alphaBoostKey = -1;
        internal static int alphaBiasKey = -1;
        internal static int externalAlphaKey = -1;
        internal static void Initialize()
        {
            tintColorKey = Shader.PropertyToID("_TintColor");
            invFadeKey = Shader.PropertyToID("_InvFade");
            mainTexKey = Shader.PropertyToID("_MainTex");
            remapTexKey = Shader.PropertyToID("_RemapTex");
            cloud1TexKey = Shader.PropertyToID("_Cloud1Tex");
            cloud2TexKey = Shader.PropertyToID("_Cloud2Tex");
            boostKey = Shader.PropertyToID("_Boost");
            alphaBoostKey = Shader.PropertyToID("_AlphaBoost");
            alphaBiasKey = Shader.PropertyToID("_AlphaBias");
            externalAlphaKey = Shader.PropertyToID("_ExternalAlpha");
        }
    }
}
