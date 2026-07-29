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
        public static int[] uniqueEnemiesWithElement;
        public static List<BodyIndex> enemiesWithMoonWheel = new List<BodyIndex>();
        public static void Initialize()
        {
            uniqueEnemiesWithElement = new int[ElementCatalog.elementCatalog.Length];
            CreateLoadout("AcidLarvaBody", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("VultureBody", anemoElement);
            CreateLoadout("BeetleQueen2Body", hydroElement); // Why is it 2???
            CreateLoadout("BeetleGuardBody", geoElement, geoElement);
            CreateLoadout("BeetleGuardAllyBody", geoElement, geoElement);
            CreateLoadout("BisonBody", physicalElement, geoElement); // for EnemyAbilities
            CreateLoadout("ChildBody", null, null, null, null, pyroElement);
            CreateLoadout("ParentBody", null, null, null, null, pyroElement);
            CreateLoadout("ClayGrenadierBody", hydroElement, hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("ClayBruiserBody", physicalElement, hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("ClayBossBody", hydroElement, hydroElement,hydroElement, hydroElement, hydroElement);
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
            CreateLoadout("JellyfishBody", electroElement, electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("WispBody", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
            CreateLoadout("WispSoulBody", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
            AddMoonWheel(CreateLoadout("LunarWispBody", dendroElement, dendroElement, dendroElement, dendroElement, pyroElement));
            AddMoonWheel(CreateLoadout("LunarGolemBody", geoElement, geoElement, geoElement, geoElement));
            AddMoonWheel(CreateLoadout("LunarExploderBody", electroElement, electroElement, electroElement, electroElement));
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarExploder.LunarExploderProjectileDotZone_prefab, electroElement);
            CreateLoadout("MiniMushroomBody", dendroElement, dendroElement, dendroElement, dendroElement);
            CreateLoadout("ScorchlingBody", physicalElement, pyroElement, pyroElement, pyroElement, pyroElement);
            CreateLoadout("ScorchlingBombProjectile", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
            CreateLoadout("DefectiveUnitBody", electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("FriendUnitBody", electroElement, electroElement, electroElement, electroElement); // Ally
            CreateLoadout("RoboBallMiniBody", electroElement);
            CreateLoadout("RoboBallGreenBuddyBody", electroElement, electroElement, electroElement, electroElement); // ally
            CreateLoadout("RoboBallRedBuddyBody", electroElement, electroElement, electroElement, electroElement); // ally
            CreateLoadout("TankerBody", pyroElement, pyroElement);
            //AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Tanker.TankerAccelerantProjectile_prefab, pyroElement);
            CreateLoadout("IronHaulerBody", anemoElement, anemoElement, anemoElement, anemoElement);
            CreateLoadout("GolemBody", geoElement, electroElement);
            CreateLoadout("VoidBarnacleBody", hydroElement);
            CreateLoadout("VoidJailerBody", hydroElement, anemoElement);
            CreateLoadout("VoidJailerAllyBody", hydroElement, anemoElement); // ally
            CreateLoadout("NullifierBody", hydroElement);
            CreateLoadout("NullifierAllyBody", hydroElement); // ally
            CreateLoadout("GrandParentBody", geoElement, null, null, null, pyroElement);
            CreateLoadout("GravekeeperBody", pyroElement);
            CreateLoadout("MagmaWormBody", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
            CreateLoadout("ElectricWormBody", electroElement, electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("SolusAmalgamatorBody", pyroElement, electroElement, electroElement, electroElement);
            CreateLoadout("SolusAmalgamatorFlamethrowerCannonBody", pyroElement);
            CreateLoadout("SolusAmalgamatorThrusterBody", electroElement);
            CreateLoadout("SolusAmalgamatorMissilePodBody", electroElement);
            CreateLoadout("RoboBallBossBody", electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("SuperRoboBallBossBody", electroElement, electroElement, electroElement, anemoElement);
            CreateLoadout("TitanBody", geoElement, geoElement, electroElement, electroElement);
            CreateLoadout("TitanGoldBody", geoElement, geoElement, electroElement, electroElement);
            CreateLoadout("VoidMegaCrabBody", hydroElement, electroElement, hydroElement, hydroElement);
            CreateLoadout("VoidMegaCrabAllyBody", hydroElement, electroElement, hydroElement, hydroElement); // ally
            CreateLoadout("VagrantBody", electroElement, electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("VagrantTrackingBomb", electroElement, electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("VoidInfestorBody", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("VultureHunterBody", anemoElement, pyroElement, anemoElement, anemoElement);
            //CreateLoadout("ArifactShellBody", electroElement);
            AddMoonWheel(CreateLoadout("FalseSonBossBody", geoElement, electroElement, electroElement, geoElement));
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.FalseSonFissurePillar_prefab, pyroElement);
            // false son statue
            AddMoonWheel(CreateLoadout("FalseSonBossBodyLunarShard", geoElement, electroElement, electroElement, geoElement));
            AddMoonWheel(CreateLoadout("FalseSonBossBodyBrokenLunarShard", geoElement, electroElement, electroElement, electroElement));
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.PrimeDevastatorProjectile_prefab, electroElement);
            RoR2.ContentManagement.AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_meridian_DisableSkillsLightning.LightningStrikeInstance_prefab)).Completed += x =>
            {
                if (x.Result.TryGetComponent<LightningStrikeInstance>(out var lightning))
                {
                    lightning.blastDamageType.SetElement(electroElement.index);
                }
            };
            AddMoonWheel(CreateLoadout("BrotherBody", physicalElement, cryoElement, physicalElement, physicalElement));
            AddMoonWheel(CreateLoadout("BrotherHurtBody", physicalElement, pyroElement));
            //AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Brother.BrotherUltLineProjectileRotateLeft_prefab, pyroElement);
            //AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Brother.BrotherUltLineProjectileRotateRight_prefab, pyroElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Brother.BrotherFirePillar_prefab, pyroElement);
            CreateLoadout("SolusHeartBody", electroElement, electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("TeleportComboLaserProjectile", pyroElement, pyroElement, pyroElement, pyroElement, pyroElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_SolusHeart.TeleportComboLaserProjectile_prefab, pyroElement);
            CreateLoadout("UnderclockSpawnerProjectile", cryoElement, cryoElement, cryoElement, cryoElement, cryoElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_SolusHeart.UnderclockSpawnerProjectile_prefab, cryoElement);
            CreateLoadout("SolusHeart_DDOSProjectile", hydroElement, hydroElement, hydroElement, hydroElement, hydroElement);
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_SolusHeart.SolusHeart_DDOSProjectile_prefab, hydroElement);
            CreateLoadout("SolusWingBody", electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("SolusWing_LaserBurstBlastProjectile", electroElement, electroElement, electroElement, electroElement, electroElement);
            CreateLoadout("MiniVoidCrabBodyPhase1", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("MiniVoidCrabBodyPhase2", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("MiniVoidCrabBodyPhase3", hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("VoidCrabBody", hydroElement, hydroElement, hydroElement, hydroElement);
            /* Make each ring in Voidling's arena have a unique weather element?
             * Void - None
             * Distant Roost - Dendro (It's green?)
             * Titanic Plains - Geo
             * Aphelian Sanctuary - Hydro (Voidling's attacks are hydro. Change something?)
             * Siren's Call - Anemo
             * Abyssal Depths - Pyro
             */

            CreateLoadout("SolusVendorBody", electroElement, electroElement, electroElement, electroElement, electroElement);
            
            // malachite urchin? figure out elites
            AddElementToProjectile(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Collective.CollectiveDeathProjectile_prefab, electroElement);
            CreateLoadout("AffixEarthHealerBody", dendroElement, dendroElement, dendroElement, dendroElement, dendroElement);

            CreateLoadout("SquidTurretBody", hydroElement, hydroElement, hydroElement, hydroElement); // Ally
            CreateLoadout("CopycatDroneBody", cryoElement, cryoElement, cryoElement, cryoElement); // Ally
            CreateLoadout("FlameDroneBody", pyroElement, pyroElement, pyroElement, pyroElement); // Ally
            CreateLoadout("BombardmentDroneBody", electroElement, electroElement, electroElement, electroElement); // Ally
            //CreateLoadout("CopycatDroneBodyRemoteOp", cryoElement, cryoElement, cryoElement, cryoElement);
            //CreateLoadout("FlameDroneBodyRemoteOp", pyroElement, pyroElement, pyroElement, pyroElement);
            //CreateLoadout("BombardmentDroneBodyRemoteOp", electroElement, electroElement, electroElement, electroElement);

            // random shit
            CreateLoadout("ExplosivePotDestructibleBody", hydroElement, hydroElement, hydroElement, hydroElement, hydroElement);
            CreateLoadout("FusionCellDestructibleBody", electroElement, electroElement, electroElement, electroElement, electroElement);

            // Enemies Returns
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Viliger.EnemiesReturns"))
            {
                //CreateLoadout("MechanicalSpiderBody", electroElement);
                //CreateLoadout("MechanicalSpiderDroneBody", electroElement); // Ally
                //CreateLoadout("MechanicalSpiderTurretBody", electroElement); // Ally
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
                CreateLoadout("SandCrabBubbleProjectile", hydroElement, hydroElement, hydroElement, hydroElement, hydroElement);
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
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SS2.SS2Main.GUID))
            {
                CreateLoadout("RunshroomBody", dendroElement, dendroElement, dendroElement, dendroElement);
                CreateLoadout("LampBody", pyroElement, pyroElement, pyroElement, pyroElement);
                CreateLoadout("LampBossBody", pyroElement, pyroElement, pyroElement, pyroElement);
                CreateLoadout("ClayMongerBody", hydroElement, hydroElement, hydroElement, hydroElement, hydroElement);
            }
            // Bootleg Bestiary
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Skeletogne.BootlegBestiary"))
            {
                CreateLoadout("SkyDraconBody", pyroElement);
                CreateLoadout("DemineurBody", hydroElement, hydroElement, hydroElement, hydroElement);
            }
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Sandswept.Main.ModGuid))
            {
                CreateLoadout("CannonJellyBody", physicalElement, physicalElement, physicalElement, physicalElement, pyroElement);
                SandsweptSupport.AddElementToCannonballJellyfishDeath();
            }

            //LogEnemyElementStats();
        }

        public static GameObject CreateLoadout(string bodyName, ElementDef primary = null, ElementDef secondary = null, ElementDef utility = null, ElementDef special = null, ElementDef applied = null)
        {
            GameObject body = BodyCatalog.FindBodyPrefab(bodyName);
            if (body)
            {
                ElementDef[] loadout = [primary ? primary : physicalElement, secondary ? secondary : physicalElement, utility ? utility : physicalElement, special ? special : physicalElement];
                ElementLoadoutComponent loadoutComponent = body.EnsureComponent<ElementLoadoutComponent>();
                loadoutComponent.ApplyElementLoadout(loadout);
                loadoutComponent.naturallyAppliedElement = applied;
                List<ElementDef> duplicateElementDef = new List<ElementDef>();
                foreach (var item in loadout)
                {
                    if (!duplicateElementDef.Contains(item)) uniqueEnemiesWithElement[(int)item.index] += 1;
                    duplicateElementDef.Add(item);
                }
            }
            return body;
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
                if (x.Result.TryGetComponent<ProjectileHurtOwnerOnDeathWithOrb>(out var deathOrb))
                {
                    deathOrb.damageTypeCombo.SetElement(element.index);
                }
            };
        }
        public static void AddMoonWheel(GameObject body)
        {
            if (!body) return;
            enemiesWithMoonWheel.Add(BodyCatalog.FindBodyIndex(body));
        }
        internal static void LogEnemyElementStats()
        {
            Log.Message("Elemental Reactions Enemy Element Stats ----");
            for (int i = 1; i < uniqueEnemiesWithElement.Length; i++)
            {
                Log.Message(Language.GetString(ElementCatalog.elementCatalog[i].nameToken) + ": " + uniqueEnemiesWithElement[i]);
            }
        }
    }
}
