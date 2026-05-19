using BepInEx;
using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

[assembly: HG.Reflection.SearchableAttribute.OptIn]
namespace ElementalReactionsMod
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(DamageAPI.PluginGUID)]
    [BepInDependency(LookingGlass.PluginInfo.PLUGIN_GUID)]
    [BepInDependency(RiskOfOptions.PluginInfo.PLUGIN_GUID)]

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ElementalReactionsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com." + PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ds_gaming";
        public const string PluginName = "ElementalReactions";
        public const string PluginVersion = "1.0.0";
        public const string PREFIX = "DS_GAMING_ELEMENTAL_REACTIONS_";

        public void Awake()
        {
            Log.Init(Logger);

            Assets.Initialize();

            DefaultElementDefs.Initialize();

            DefaultElementalReactions.Initialize();

            OnHooks.Initialize();
        }
    }
}
