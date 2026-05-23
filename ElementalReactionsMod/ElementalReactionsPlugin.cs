using BepInEx;
using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using R2API;
using R2API.ContentManagement;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Security;
using System.Security.Permissions;
using ElementalReactionsMod.Items;
using R2API.Networking;
using ElementalReactionsMod.Loadout;

[assembly: HG.Reflection.SearchableAttribute.OptIn]
namespace ElementalReactionsMod
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(DamageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(LookingGlass.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(RiskOfOptions.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ElementalReactionsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com." + PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ds_gaming";
        public const string PluginName = "ElementalReactions";
        public const string PluginVersion = "1.0.0";
        public const string PREFIX = "DS_GAMING_ELEMENTAL_REACTIONS_";

        public static ElementalReactionsPlugin instance;

        public void Awake()
        {
            instance = this;
            Log.Init(Logger);

            new ContentPacks().Initialize();

            Assets.Initialize();

            DelusionManager.Initialize();

            Tokens.Initialize();

            DefaultElementDefs.Initialize();

            DefaultElementalReactions.Initialize();

            DamageTypes.Initialize();

            Buffs.Initialize();

            Items.Items.Initialize();

            Hooks.Initialize();

            NetworkingAPI.RegisterMessageType<NetworkElementLoadout>();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID)) ElementalReactionsMod.Config.RiskOfOptionsSetup();
        }
    }
}
