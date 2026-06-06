using ElementalReactionsMod.Elements;
using HG;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ElementalReactionsMod.Elements.DefaultElementDefs;

namespace ElementalReactionsMod.Loadout
{
    public static class EnemyElementLoadouts
    {
        public static void Initialize()
        {
            CreateLoadout("VultureBody", anemoElement);
            CreateLoadout("BeetleGuardBody", geoElement, geoElement);
            CreateLoadout("BeetleGuardAllyBody", geoElement, geoElement);
            CreateLoadout("BisonBody", physicalElement, geoElement); // for EnemyAbilities
            CreateLoadout("ChildBody", null, null, null, null, pyroElement);
            //CreateLoadout("ClayGrenadierBody", DefaultElementDefs.hydroElement, DefaultElementDefs.hydroElement);
            //CreateLoadout("ClayBruiserBody", DefaultElementDefs.physicalElement, DefaultElementDefs.hydroElement);
            CreateLoadout("DevotedLemurianBruiserBody", pyroElement, pyroElement);
            CreateLoadout("LemurianBruiserBody", pyroElement, pyroElement);
            CreateLoadout("DevotedLemurianBody", pyroElement);
            CreateLoadout("LemurianBody", pyroElement);
            CreateLoadout("GreaterWispBody", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
            CreateLoadout("GupBody", dendroElement, dendroElement, dendroElement, dendroElement, dendroElement);
            CreateLoadout("GeepBody", dendroElement, dendroElement, dendroElement, dendroElement, dendroElement);
            CreateLoadout("GipBody", dendroElement, dendroElement, dendroElement, dendroElement, dendroElement);
            CreateLoadout("HalcyoniteBody", geoElement, electroElement, geoElement, geoElement);
            CreateLoadout("HermitCrabBody", hydroElement);
            CreateLoadout("JellyfishBody", null, electroElement);
            CreateLoadout("WispBody", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
            CreateLoadout("WispSoulBody", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
            //AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarExploder.LunarExploderProjectileDotZone_prefab, dendroElement);
            // lunar chimeras
            CreateLoadout("MiniMushroomBody", dendroElement, dendroElement, dendroElement, dendroElement);
            CreateLoadout("ScorchlingBody", geoElement, pyroElement);
            CreateLoadout("DefectiveUnitBody", electroElement, electroElement, electroElement, electroElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Collective.CollectiveDeathProjectile_prefab, electroElement);
            CreateLoadout("RoboBallMiniBody", electroElement);
            CreateLoadout("RoboBallGreenBuddyBody", electroElement, electroElement, electroElement, electroElement); // ally
            CreateLoadout("RoboBallRedBuddyBody", electroElement, electroElement, electroElement, electroElement); // ally
            CreateLoadout("TankerBody", pyroElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Tanker.TankerAccelerantProjectile_prefab, pyroElement);
            // grease puddles
            CreateLoadout("IronHaulerBody", electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("GolemBody", geoElement, electroElement);
            CreateLoadout("VoidBarnacleBody", hydroElement);
            CreateLoadout("VoidJailerBody", hydroElement, hydroElement);
            CreateLoadout("VoidJailerAllyBody", hydroElement, hydroElement); // ally
            CreateLoadout("NullifierBody", hydroElement);
            CreateLoadout("NullifierAllyBody", hydroElement); // ally
            CreateLoadout("ClayBossBody", pyroElement, pyroElement);
            CreateLoadout("GrandParentBody", geoElement, null, null, null, pyroElement);
            CreateLoadout("GravekeeperBody", pyroElement);
            CreateLoadout("MagmaWormBody", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
            CreateLoadout("ElectricWormBody", electroElement, electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("SolusAmalgamatorBody", pyroElement, electroElement, electroElement, electroElement);
            CreateLoadout("SolusAmalgamatorFlamethrowerCannonBody", pyroElement);
            CreateLoadout("SolusAmalgamatorThrusterBody", electroElement);
            CreateLoadout("SolusAmalgamatorMissilePodBody", electroElement);
            CreateLoadout("RoboBallBossBody", electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("SuperRoboBallBossBody", electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("TitanBody", geoElement, geoElement, electroElement, electroElement);
            CreateLoadout("TitanGoldBody", geoElement, geoElement, electroElement, electroElement);
            CreateLoadout("VoidMegaCrabBody", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("VoidMegaCrabAllyBody", hydroElement, hydroElement, hydroElement, hydroElement); // ally
            CreateLoadout("VagrantBody", electroElement, electroElement, electroElement, electroElement, electroElement);
            // malachite urchin? figure out elites
            CreateLoadout("VoidInfestorBody", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("VultureHunterBody", electroElement, pyroElement);
            CreateLoadout("ArifactShellBody", electroElement);
            CreateLoadout("FalseSonBossBody", geoElement, electroElement, electroElement, geoElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.FalseSonFissurePillar_prefab, pyroElement);
            // false son statue
            CreateLoadout("FalseSonBossBodyLunarShard", geoElement, electroElement, electroElement, geoElement);
            CreateLoadout("FalseSonBossBodyBrokenLunarShard", geoElement, electroElement, electroElement, electroElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.PrimeDevastatorProjectile_prefab, electroElement);
            // manually make Mithrix projectiles and spinny pyro
            CreateLoadout("BrotherHurtBody", physicalElement, pyroElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Brother.BrotherUltLineProjectileRotateLeft_prefab, pyroElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Brother.BrotherUltLineProjectileRotateRight_prefab, pyroElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Brother.BrotherFirePillar_prefab, pyroElement);
            CreateLoadout("SolusHeartBody", electroElement, electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("SolusWingBody", electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("MiniVoidCrabBodyPhase1", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("MiniVoidCrabBodyPhase2", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("MiniVoidCrabBodyPhase3", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("VoidCrabBody", hydroElement, hydroElement, hydroElement, hydroElement);

            CreateLoadout("CopycatDroneBody", cryoElement, cryoElement, cryoElement, cryoElement); // Ally
            CreateLoadout("FlameDroneBody", pyroElement, pyroElement, pyroElement, pyroElement); // Ally
            CreateLoadout("BombardmentDroneBody", electroElement, electroElement, electroElement, electroElement); // Ally
            CreateLoadout("CopycatDroneBodyRemoteOp", cryoElement, cryoElement, cryoElement, cryoElement);
            CreateLoadout("FlameDroneBodyRemoteOp", pyroElement, pyroElement, pyroElement, pyroElement);
            CreateLoadout("BombardmentDroneBodyRemoteOp", electroElement, electroElement, electroElement, electroElement);

            // Enemies Returns
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Viliger.EnemiesReturns"))
            {
                CreateLoadout("MechanicalSpiderBody", electroElement);
                CreateLoadout("MechanicalSpiderDroneBody", electroElement); // Ally
                CreateLoadout("MechanicalSpiderTurretBody", electroElement); // Ally
                CreateLoadout("SwiftBody", geoElement);
                CreateLoadout("LynxScoutBody", dendroElement);
                CreateLoadout("LynxScoutAllyBody", dendroElement); // Ally
                CreateLoadout("LynxArcherBody", dendroElement);
                CreateLoadout("LynxArcherAllyBody", dendroElement); // Ally
                CreateLoadout("LynxHunterBody", dendroElement);
                CreateLoadout("LynxHunterAllyBody", dendroElement); // Ally
                CreateLoadout("LynxShamanBody", dendroElement, dendroElement, dendroElement, dendroElement);
                CreateLoadout("LynxShamanAllyBody", dendroElement, dendroElement, dendroElement, dendroElement); // Ally
                CreateLoadout("SandCrabBody", physicalElement, hydroElement);
                CreateLoadout("ColossusBody", geoElement, geoElement, electroElement, electroElement);
                CreateLoadout("IfritBody", physicalElement, pyroElement, pyroElement, pyroElement, pyroElement);
                CreateLoadout("IfritPylonEnemyBody", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
                CreateLoadout("IfritPylonPlayerBody", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement); // Ally
                                                                                                                        // lynx totem tornado anemo?
                CreateLoadout("ArraignHauntBody", electroElement, electroElement, electroElement, electroElement);
                CreateLoadout("ArraignP1Body", physicalElement, physicalElement, physicalElement, electroElement);
                CreateLoadout("ArraignP2Body", physicalElement, physicalElement, electroElement, electroElement);
            }
            // Star Storm 2
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm"))
            {
                CreateLoadout("RunshroomBody", dendroElement, dendroElement, dendroElement, dendroElement);
                CreateLoadout("LampBody", pyroElement, pyroElement, pyroElement, pyroElement);
                CreateLoadout("LampBossBody", pyroElement, pyroElement, pyroElement, pyroElement);
            }
        }

        public static void CreateLoadout(string bodyName, ElementDef primary = null, ElementDef secondary = null, ElementDef utility = null, ElementDef special = null, ElementDef applied = null)
        {
            GameObject body = BodyCatalog.FindBodyPrefab(bodyName);
            if (body)
            {
                ElementDef[] loadout = [primary ? primary : physicalElement, secondary ? secondary : physicalElement, utility ? utility : physicalElement, special ? special : physicalElement];
                ElementLoadoutComponent loadoutComponent = body.EnsureComponent<ElementLoadoutComponent>();
                loadoutComponent.ApplyElementLoadout(loadout);
                loadoutComponent.permanentlyAppliedElement = applied;
            }
        }
        public static void AddElementToProjectile(string projectilePrefab, ElementDef element)
        {
            if (!element) return;
            RoR2.ContentManagement.AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(projectilePrefab)).Completed += x =>
            {
                if (x.Result.TryGetComponent<ProjectileDamage>(out var damage))
                {
                    damage.damageType.SetElement(element.index);
                }
            };
        }
    }
}
