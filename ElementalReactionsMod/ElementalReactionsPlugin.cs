using BepInEx;
using BepInEx.Bootstrap;
using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Items;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using ElementalReactionsMod.Environment;
using R2API;
using R2API.ContentManagement;
using R2API.Networking;
using RoR2;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;

[assembly: HG.Reflection.SearchableAttribute.OptIn]
namespace ElementalReactionsMod
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(DamageAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(DeployableAPI.PluginGUID)]
    [BepInDependency(DotAPI.PluginGUID)]
    [BepInDependency(TempVisualEffectAPI.PluginGUID)]
    [BepInDependency(LookingGlass.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(RiskOfOptions.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ItemQualities.ItemQualitiesPlugin.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(SS2.SS2Main.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(Sandswept.Main.ModGuid, BepInDependency.DependencyFlags.SoftDependency)]

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ElementalReactionsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com." + PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ds_gaming";
        public const string PluginName = "ElementalReactions";
        public const string PluginVersion = "1.1.0";
        public const string PREFIX = "DS_GAMING_ELEMENTAL_REACTIONS_";

        public static ElementalReactionsPlugin instance;

        public static bool qualityModExists;

        public void Awake()
        {
            instance = this;
            Log.Init(Logger);

            new ContentPacks().Initialize();

            if (Chainloader.PluginInfos.ContainsKey(ItemQualities.ItemQualitiesPlugin.PluginGUID)) qualityModExists = true;
            if (qualityModExists) QualityInitialize();
            ElementalReactionManager.bloomDeployableSlot = DeployableAPI.RegisterDeployableSlot((self, deployableCountMultiplier) => { return StaticValues.bloomCap; });
            ElementalReactionManager.crystallizeDeployableSlot = DeployableAPI.RegisterDeployableSlot((self, deployableCountMultiplier) => { return StaticValues.crystallizeCap; });

            Assets.Initialize();

            Buffs.Initialize();

            DelusionManager.Initialize();

            Tokens.Initialize();

            ElectroChargedDot.Initialize();

            BurningDot.Initialize();

            DefaultElementDefs.Initialize();

            DefaultElementalReactions.Initialize();

            DamageTypes.Initialize();

            Achievements.Unlockables.Initialize();

            Items.Items.Initialize();

            Hooks.Initialize();

            ElementalRain.Initialize();

            Crafting.Initialize();

            MasterElementLoadout.Initialize();

            if (Chainloader.PluginInfos.ContainsKey(SS2.SS2Main.GUID))
            {
                SS2Support.Initialize();
            }
            if (Chainloader.PluginInfos.ContainsKey(Sandswept.Main.ModGuid))
            {
                SandsweptSupport.Initialize();
            }

            NetworkingAPI.RegisterMessageType<NetworkElementLoadout>();
            //NetworkingAPI.RegisterMessageType<NetworkPooledObjectSetActive>();

            if (Chainloader.PluginInfos.ContainsKey(RiskOfOptions.PluginInfo.PLUGIN_GUID)) ElementalReactionsMod.Config.RiskOfOptionsSetup();
            if (Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID)) LookingGlassSupport.Initialize();
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public void QualityInitialize()
        {
            QualitySupport.Initialize();
        }
    }
}
