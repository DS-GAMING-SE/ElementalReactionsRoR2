using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Items;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using R2API;
using Rewired.ComponentControls.Effects;
using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;
using RoR2.Projectile;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static Rewired.Controller;

namespace ElementalReactionsMod
{
    public static class Assets
    {
        public static ExpansionDef elementalReactionExpansionDef;
        public static GameObject elementalReactionManagerPrefab;

        public static GameObject elementLoadoutRowUI;

        public static Material electroTrailMaterial;
        public static Material darkElectricTrailMaterial;

        public static Material dendroElectricTrailMaterial;

        public static Material genericElementEffectMaterial;

        public static string AddressablesDirectory { get; private set; }
        internal static void LoadAddressables()
        {
            AddressablesDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ElementalReactionsPlugin.instance.Info.Location), "Addressables");
            Addressables.LoadContentCatalogAsync(System.IO.Path.Combine(AddressablesDirectory, "catalog.json")).WaitForCompletion();
        }
        public static void Initialize()
        {
            LoadAddressables();

            elementalReactionManagerPrefab = PrefabAPI.CreateEmptyPrefab("ElementalReactionManager");
            elementalReactionManagerPrefab.AddComponent<ElementalReactionManager>();
            AddAkBank(elementalReactionManagerPrefab, RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSon.FalseSonBody_prefab);
            AddAkBank(elementalReactionManagerPrefab, RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Bandit2.Bandit2Body_prefab);
            AddAkBank(elementalReactionManagerPrefab, RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Seeker.SeekerBody_prefab);

            elementalReactionExpansionDef = ScriptableObject.CreateInstance<ExpansionDef>();
            elementalReactionExpansionDef.name = "ElementalReactionExpansionDef";
            elementalReactionExpansionDef.nameToken = $"{ElementalReactionsPlugin.PREFIX}EXPANSION_NAME";
            elementalReactionExpansionDef.descriptionToken = $"{ElementalReactionsPlugin.PREFIX}EXPANSION_DESCRIPTION";
            elementalReactionExpansionDef.iconSprite = Addressables.LoadAssetAsync<Sprite>(AssetReferences.expansionIcon).WaitForCompletion();
            elementalReactionExpansionDef.disabledIconSprite = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_MiscIcons.texUnlockIcon_png).WaitForCompletion();
            elementalReactionExpansionDef.runBehaviorPrefab = elementalReactionManagerPrefab;
            elementalReactionExpansionDef.requiredEntitlement = null;

            Content.AddExpansionDef(elementalReactionExpansionDef);

            elementalReactionManagerPrefab.AddComponent<ExpansionRequirementComponent>().requiredExpansion = elementalReactionExpansionDef;

            CreateGenericElementEffectMaterial();

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.genericElementActivatedEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = true;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Medium;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;
                x.Result.AddComponent<NetworkIdentity>();
                GenericElementEffectComponent component = x.Result.AddComponent<GenericElementEffectComponent>();
                ParticleSystem iconParticle = x.Result.transform.GetChild(0).GetComponent<ParticleSystem>();
                ParticleSystem rayParticle = x.Result.transform.GetChild(1).GetComponent<ParticleSystem>();
                ParticleSystem glowParticle = x.Result.transform.GetChild(2).GetComponent<ParticleSystem>();
                component.icon = x.Result.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
                component.particlesToRecolor = [iconParticle, rayParticle, glowParticle];
                x.Result.AddComponent<DestroyOnParticleEnd>().trackedParticleSystem = iconParticle;
                var scale = x.Result.AddComponent<ScaleParticleSystemDuration>();
                scale.initialDuration = 0.7f;
                scale.particleSystems = [iconParticle, rayParticle, glowParticle];
                component.particleDuration = scale;
                x.Result.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matLunarGazeFireLaser3_mat)).WaitForCompletion();
                x.Result.AddComponent<ParentEffectToItemDisplay>();

                AddNewEffectDef(x.Result);
            };

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.genericElementOrbEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = false;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Medium;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;

                RoR2.Orbs.OrbEffect orb = x.Result.AddComponent<RoR2.Orbs.OrbEffect>();
                orb.startVelocity1 = new Vector3(-30, -7f, -30f);
                orb.startVelocity2 = new Vector3(30, 7f, 30f);
                orb.movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
                orb.endEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Treebot.OmniImpactVFXSlashSyringe_prefab)).WaitForCompletion();

                x.Result.AddComponent<NetworkIdentity>();
                var wideGlow = x.Result.transform.GetChild(0).GetComponent<ParticleSystem>();
                wideGlow.GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matWideGlow_mat)).WaitForCompletion();
                GenericElementEffectComponent component = x.Result.AddComponent<GenericElementEffectComponent>();
                component.trailsToRecolor = [x.Result.transform.GetChild(1).GetComponent<TrailRenderer>()];
                component.particlesToRecolor = [wideGlow, x.Result.transform.GetChild(2).GetComponent<ParticleSystem>()];

                AddNewEffectDef(x.Result);
            };

            #region Reactions
            Mesh ringMesh = AssetAsyncReferenceManager<Mesh>.LoadAsset(new AssetReferenceT<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.mdlVFXDonut2_fbx_donut2Mesh_)).WaitForCompletion();
            electroTrailMaterial = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matPrimeDevastatorChargeVFX2_mat)).WaitForCompletion());
            electroTrailMaterial.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTeslaCoil_png)).WaitForCompletion());
            electroTrailMaterial.SetFloat("_Boost", 2f);

            darkElectricTrailMaterial = new Material(electroTrailMaterial);
            darkElectricTrailMaterial.SetColor("_TintColor", Color.black);
            darkElectricTrailMaterial.SetInt("_DstBlend", 10);

            Texture dendroRampTex = AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampOrbitalLaser_png)).WaitForCompletion();
            dendroElectricTrailMaterial = new Material(electroTrailMaterial);
            dendroElectricTrailMaterial.SetTexture("_RemapTex", dendroRampTex);
            dendroElectricTrailMaterial.SetColor("_TintColor", new Color(0.5f, 1f, 0f));
            dendroElectricTrailMaterial.SetFloat("_Boost", 4f);

            Material dendroLeafMat = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Seeker.matSeekerLotus_mat_bfd003ca)).WaitForCompletion();
            dendroLeafMat.SetColor("_TintColor", Color.white);
            Material bloomCoreExplosionMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matOmniRing1Generic_mat)).WaitForCompletion());
            bloomCoreExplosionMat.SetTexture("_MainTex", AssetAsyncReferenceManager<Texture>.LoadAsset(AssetReferences.bloomTransparent).WaitForCompletion());
            bloomCoreExplosionMat.SetTexture("_RemapTex", dendroRampTex);
            bloomCoreExplosionMat.SetFloat("_AlphaBoost", 1f);
            bloomCoreExplosionMat.SetFloat("_Boost", 1f);
            bloomCoreExplosionMat.SetInt("_SrcBlend", 5);
            bloomCoreExplosionMat.SetInt("_DstBlend", 1);

            Material genericRingMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_BFG.matBeamSphereBeam_mat)).WaitForCompletion());
            genericRingMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTwotoneEnvironment_jpg)).WaitForCompletion());
            genericRingMat.SetFloat("_AlphaBoost", 3f);
            genericRingMat.EnableKeyword("VERTEXCOLOR");
            genericRingMat.SetVector("_CutoffScroll", new Vector4(25, 0, -10, 0));

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.electroChargeTempVisualEffect).Completed += x =>
            {
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Medium;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;
                var vfxContainer = x.Result.transform.GetChild(0);
                vfxContainer.GetComponent<ParticleSystemRenderer>().trailMaterial = electroTrailMaterial;
                vfxContainer.GetChild(0).GetComponent<ParticleSystemRenderer>().trailMaterial = darkElectricTrailMaterial;
                var destroyOnTimer = x.Result.AddComponent<DestroyOnTimer>();
                destroyOnTimer.duration = 0.25f;
                TemporaryVisualEffect tempVisualEffect = x.Result.AddComponent<TemporaryVisualEffect>();
                tempVisualEffect.exitComponents = [destroyOnTimer];
                tempVisualEffect.visualTransform = vfxContainer;

                TempVisualEffectAPI.AddTemporaryVisualEffect(x.Result, (body) => { return body.HasBuff(Buffs.electroChargeBuff); });
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.quickenTempVisualEffect).Completed += x =>
            {
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Medium;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;
                var vfxContainer = x.Result.transform.GetChild(0);
                vfxContainer.GetComponent<ParticleSystemRenderer>().trailMaterial = dendroElectricTrailMaterial;
                vfxContainer.GetChild(0).GetComponent<ParticleSystemRenderer>().trailMaterial = darkElectricTrailMaterial;
                var destroyOnTimer = x.Result.AddComponent<DestroyOnTimer>();
                destroyOnTimer.duration = 0.25f;
                TemporaryVisualEffect tempVisualEffect = x.Result.AddComponent<TemporaryVisualEffect>();
                tempVisualEffect.exitComponents = [destroyOnTimer];
                tempVisualEffect.visualTransform = vfxContainer;

                TempVisualEffectAPI.AddTemporaryVisualEffect(x.Result, (body) => { return body.HasBuff(Buffs.quickenBuff); });
            };

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.overloadEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                vfx.DoNotPool = false;
                ShakeEmitter shakeEmitter = x.Result.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.2f;
                shakeEmitter.radius = 45f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.7f,
                    frequency = 30f,
                    cycleOffset = 0f
                };
                x.Result.AddComponent<NetworkIdentity>();
                ParticleSystemRenderer ringParticleRenderer = x.Result.transform.Find("OverloadRing").GetComponent<ParticleSystemRenderer>();
                ringParticleRenderer.mesh = AssetAsyncReferenceManager<Mesh>.LoadAsset(new AssetReferenceT<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.mdlVFXDonut1_fbx_donut1Mesh_)).WaitForCompletion();
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_SolusAmalgamator.matSolusAmalgamatorTrackingBombRing_mat)).Completed += y =>
                {
                    Material ringMat = new Material(y.Result);
                    ringMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTeslaCoil_png)).WaitForCompletion());
                    ringParticleRenderer.sharedMaterial = ringMat;
                };
                var flash = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBright_mat)).WaitForCompletion();
                x.Result.transform.Find("OverloadSmallFlash").GetComponent<ParticleSystemRenderer>().sharedMaterial = flash;
                x.Result.transform.Find("OverloadFlash").GetComponent<ParticleSystemRenderer>().sharedMaterial = flash;
                x.Result.transform.Find("OverloadSparks").GetComponent<ParticleSystemRenderer>().sharedMaterial = flash;
                x.Result.transform.Find("OverloadDistortion").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matDistortionFaded_mat)).WaitForCompletion();
                x.Result.transform.Find("OverloadElectricity").GetComponent<ParticleSystemRenderer>().trailMaterial = electroTrailMaterial;
                x.Result.transform.Find("OverloadElectricityDark").GetComponent<ParticleSystemRenderer>().trailMaterial = darkElectricTrailMaterial;
                x.Result.transform.Find("OverloadFire").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Gravekeeper.matOmniExplosion1ArchWisp_mat)).WaitForCompletion();
                var light = x.Result.transform.Find("OverloadLight");
                vfx.optionalLights = [light.GetComponent<Light>()];
                var lightCurve = light.gameObject.AddComponent<LightIntensityCurve>();
                lightCurve.timeMax = 0.4f;
                lightCurve.curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
                x.Result.AddComponent<DestroyOnTimer>().duration = 0.7f;

                AddNewEffectDef(x.Result, "Play_FalseSon_MeridianWill_Lightning_Initial");
            };

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.superconductEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                vfx.DoNotPool = false;
                x.Result.AddComponent<NetworkIdentity>();
                x.Result.AddComponent<DestroyOnTimer>().duration = 0.7f;

                x.Result.transform.Find("SuperconductSphere").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_EliteIce.matAffixWhiteSphereIndicator_mat)).WaitForCompletion();
                x.Result.transform.Find("SuperconductMist").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.matChefSecondaryFrostVFX_mat)).WaitForCompletion();
                x.Result.transform.Find("SuperconductSnowflakes").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.matChefSecondarySnowflakeVFX_mat)).WaitForCompletion();

                AddNewEffectDef(x.Result, "Play_freezeDrone_impact");
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.superconductTempVisualEffect).Completed += x =>
            {
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Medium;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;
                var vfxContainer = x.Result.transform.GetChild(0);
                vfxContainer.GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.matChefSecondaryFrostVFX_mat)).WaitForCompletion();
                var destroyOnTimer = x.Result.AddComponent<DestroyOnTimer>();
                destroyOnTimer.duration = 0.6f;
                TemporaryVisualEffect tempVisualEffect = x.Result.AddComponent<TemporaryVisualEffect>();
                tempVisualEffect.exitComponents = [destroyOnTimer];
                tempVisualEffect.visualTransform = vfxContainer;

                TempVisualEffectAPI.AddTemporaryVisualEffect(x.Result, (body) => { return body.HasBuff(Buffs.superconductBuff); });
            };

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.swirlEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;
                x.Result.AddComponent<NetworkIdentity>();
                GenericElementEffectComponent component = x.Result.AddComponent<GenericElementEffectComponent>();
                ParticleSystemRenderer swirlRingParticleRenderer = x.Result.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
                component.particlesToRecolor = [swirlRingParticleRenderer.GetComponent<ParticleSystem>()];
                swirlRingParticleRenderer.mesh = ringMesh;
                swirlRingParticleRenderer.sharedMaterial = genericRingMat;
                x.Result.AddComponent<DestroyOnTimer>().duration = 0.4f;

                AddNewEffectDef(x.Result, "Play_bandit2_shift_enter");
            };

            AssetAsyncReferenceManager<Material>.LoadAsset(AssetReferences.bloomMaterial).Completed += x =>
            {
                x.Result.SetHopooMaterial();
                AssetAsyncReferenceManager<Texture>.LoadAsset(AssetReferences.bloomFresnelMask).Completed += y =>
                {
                    x.Result.SetTexture("_FresnelMask", y.Result);
                };
                x.Result.SetTexture("_FresnelRamp", dendroRampTex);
                x.Result.EnableKeyword("FRESNEL_EMISSION");
                x.Result.SetFloat("_FresnelBoost", 7f);
                x.Result.SetFloat("_FresnelPower", 1.6f);
                x.Result.SetEmission(0.25f);
            };

            AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.HealthOrbEffect_prefab)).Completed += x =>
            {
                ElementalReactionManager.bloomSpawnOrb = PrefabAPI.InstantiateClone(x.Result, "BloomSpawnOrbEffect");
                ElementalReactionManager.bloomSpawnOrb.AddComponent<Orbs.OrbEffectTargetPosition>();
                ElementalReactionManager.bloomSpawnOrb.AddComponent<NetworkIdentity>();
                GameObject.Destroy(ElementalReactionManager.bloomSpawnOrb.GetComponent<AkEvent>());
                GameObject.Destroy(ElementalReactionManager.bloomSpawnOrb.GetComponent<AkGameObj>());

                AddNewEffectDef(ElementalReactionManager.bloomSpawnOrb);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.bloomObject).Completed += x =>
            {
                x.Result.AddComponent<NetworkIdentity>();
                var characterBody = x.Result.AddComponent<CharacterBody>();
                characterBody.baseNameToken = $"{ElementalReactionsPlugin.PREFIX}REACTION_BLOOM_OBJECT_NAME";
                characterBody.bodyFlags = CharacterBody.BodyFlags.Masterless | CharacterBody.BodyFlags.HasBackstabImmunity;
                AssetAsyncReferenceManager<Sprite>.LoadAsset(AssetReferences.bloomIcon).Completed += x =>
                {
                    characterBody.portraitIcon = x.Result.texture;
                };
                var healthComponent = x.Result.AddComponent<HealthComponent>();
                healthComponent.dontShowHealthbar = true;
                healthComponent.body = characterBody;
                characterBody.baseMaxHealth = 1f;
                characterBody.healthComponent = healthComponent;
                x.Result.AddComponent<PseudoCharacterMotor>();
                var model = x.Result.transform.GetChild(0).gameObject.AddComponent<HurtBoxGroup>();
                var modelLocator = x.Result.AddComponent<ModelLocator>();
                modelLocator.modelTransform = model.transform;
                modelLocator.dontDetatchFromParent = true;
                model.mainHurtBox = model.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.AddComponent<HurtBox>();
                model.hurtBoxes = [model.mainHurtBox];
                model.mainHurtBox.healthComponent = healthComponent;
                model.mainHurtBox.isBullseye = true;
                var rotate = model.transform.GetChild(0).GetChild(0).gameObject.AddComponent<RotateItem>();
                rotate.spinSpeed = 80f;
                var startScaleCurve = rotate.gameObject.AddComponent<ObjectScaleCurve>();
                startScaleCurve.timeMax = 0.2f;
                startScaleCurve.overallCurve = AnimationCurve.EaseInOut(0f, 0.3f, 1f, 1f);
                startScaleCurve.useOverallCurveOnly = true;
                startScaleCurve.transform.Find("BloomStartDistortion").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matDistortionFaded_mat)).WaitForCompletion();
                var specialObjectAttributes = x.Result.AddComponent<SpecialObjectAttributes>();
                specialObjectAttributes.grabbable = true;
                specialObjectAttributes.massOverride = 0;
                specialObjectAttributes.hullClassification = HullClassification.Human;
                specialObjectAttributes.orientToFloor = true;
                x.Result.AddComponent<BloomController>();
                x.Result.AddComponent<Deployable>();
                x.Result.AddComponent<ForceFriendlyFire>();
                x.Result.AddComponent<ProjectileController>().cannotBeDeleted = true;
                x.Result.AddComponent<ProjectileDamage>();
                x.Result.AddComponent<ProjectileDeployToOwner>().deployableSlot = ElementalReactionManager.bloomDeployableSlot;

                Content.AddProjectilePrefab(x.Result);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.bloomExplosion).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                vfx.DoNotPool = false;
                ShakeEmitter shakeEmitter = x.Result.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.2f;
                shakeEmitter.radius = 55f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.35f,
                    frequency = 30f,
                    cycleOffset = 0f
                };
                x.Result.AddComponent<NetworkIdentity>();
                ParticleSystemRenderer ringParticleRenderer = x.Result.transform.Find("BloomRing").GetComponent<ParticleSystemRenderer>();
                ringParticleRenderer.mesh = ringMesh;
                Material bloomRingMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_BFG.matBeamSphereBeam_mat)).WaitForCompletion());
                bloomRingMat.SetTexture("_RemapTex", dendroRampTex);
                bloomRingMat.SetVector("_CutoffScroll", new Vector4(40, 0, -20, 0));
                ringParticleRenderer.sharedMaterial = bloomRingMat;
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_EliteEarth.matAffixEarthSphereIndicator_mat)).Completed += y =>
                {
                    Material bloomSphereMat = new Material(y.Result);
                    bloomSphereMat.SetTexture("_RemapTex", dendroRampTex);
                    bloomSphereMat.SetFloat("_SrcBlendFloat", 1);
                    bloomSphereMat.SetFloat("_DstBlendFloat", 1);
                    x.Result.transform.Find("BloomSphere").GetComponent<ParticleSystemRenderer>().sharedMaterial = bloomSphereMat;
                };
                x.Result.transform.Find("BloomExplosionCore").GetComponent<ParticleSystemRenderer>().sharedMaterial = bloomCoreExplosionMat;
                var flash = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBright_mat)).WaitForCompletion();
                x.Result.transform.Find("BloomSparks").GetComponent<ParticleSystemRenderer>().sharedMaterial = flash;
                x.Result.transform.Find("BloomPetals").GetComponent<ParticleSystemRenderer>().sharedMaterial = dendroLeafMat;
                var light = x.Result.transform.Find("Light");
                vfx.optionalLights = [light.GetComponent<Light>()];
                var lightCurve = light.gameObject.AddComponent<LightIntensityCurve>();
                lightCurve.timeMax = 0.3f;
                lightCurve.curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
                x.Result.AddComponent<DestroyOnTimer>().duration = 1f;

                AddNewEffectDef(x.Result, "Play_seeker_skill1_impact");
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.burgeonExplosion).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                vfx.DoNotPool = false;
                ShakeEmitter shakeEmitter = x.Result.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.25f;
                shakeEmitter.radius = 55f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.55f,
                    frequency = 30f,
                    cycleOffset = 0f
                };
                x.Result.AddComponent<NetworkIdentity>();
                Texture burgeonRamp = AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampWilloWispExplosion2_png)).WaitForCompletion();
                ParticleSystemRenderer ringParticleRenderer = x.Result.transform.Find("BloomRing").GetComponent<ParticleSystemRenderer>();
                ringParticleRenderer.mesh = ringMesh;
                Material bloomRingMat = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_BFG.matBeamSphereBeam_mat)).WaitForCompletion());
                bloomRingMat.SetTexture("_RemapTex", burgeonRamp);
                bloomRingMat.SetInt("_SrcBlend", 5);
                bloomRingMat.SetVector("_CutoffScroll", new Vector4(55, 0, -25, 0));
                ringParticleRenderer.sharedMaterial = bloomRingMat;
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_BurnNearby.matHelfireRangeIndicator_mat)).Completed += y =>
                {
                    Material bloomSphereMat = new Material(y.Result);
                    bloomSphereMat.SetTexture("_RemapTex", burgeonRamp);
                    bloomSphereMat.SetFloat("_SrcBlendFloat", 5);
                    bloomSphereMat.SetFloat("_DstBlendFloat", 1);
                    x.Result.transform.Find("BloomSphere").GetComponent<ParticleSystemRenderer>().sharedMaterial = bloomSphereMat;
                };
                Material burgeonCoreExplosion = new Material(bloomCoreExplosionMat);
                burgeonCoreExplosion.SetTexture("_RemapTex", burgeonRamp);
                x.Result.transform.Find("BloomExplosionCore").GetComponent<ParticleSystemRenderer>().sharedMaterial = burgeonCoreExplosion;
                var flash = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBright_mat)).WaitForCompletion();
                x.Result.transform.Find("BloomSparks").GetComponent<ParticleSystemRenderer>().sharedMaterial = flash;
                x.Result.transform.Find("BloomPetals").GetComponent<ParticleSystemRenderer>().sharedMaterial = dendroLeafMat;
                var light = x.Result.transform.Find("Light");
                vfx.optionalLights = [light.GetComponent<Light>()];
                var lightCurve = light.gameObject.AddComponent<LightIntensityCurve>();
                lightCurve.timeMax = 0.35f;
                lightCurve.curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
                x.Result.AddComponent<DestroyOnTimer>().duration = 1f;

                AddNewEffectDef(x.Result, "Play_seeker_skill1_impact");
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.hyperbloomOrb).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = false;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;
                RoR2.Orbs.OrbEffect orb = x.Result.AddComponent<RoR2.Orbs.OrbEffect>();
                orb.startVelocity1 = new Vector3(-3, 20f, -3f);
                orb.startVelocity2 = new Vector3(3, 25f, 3f);
                orb.movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
                orb.endEffect = AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Treebot.OmniImpactVFXSlashSyringe_prefab)).WaitForCompletion();
                orb.endEffectScale = 1.5f;

                x.Result.AddComponent<NetworkIdentity>();
                var core = x.Result.transform.Find("HyperbloomOrbCoreBillboard");
                core.GetComponent<MeshRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBright_mat)).WaitForCompletion();
                core.gameObject.AddComponent<Billboard>();
                var leafContainer = x.Result.transform.Find("HyperbloomOrbCoreLeafContainer");
                var rotate = leafContainer.gameObject.AddComponent<RotateAroundAxis>();
                rotate.relativeTo = Space.Self;
                rotate.rotateAroundAxis = RotateAroundAxis.RotationAxis.Z;
                rotate.fastRotationSpeed = 240f;
                rotate.speed = RotateAroundAxis.Speed.Fast;
                Material greenLeaf = new(dendroLeafMat);
                greenLeaf.SetColor("_TintColor", new Color(0.1f, 0.6f, 0f));
                leafContainer.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = greenLeaf;
                leafContainer.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = greenLeaf;
                leafContainer.GetChild(2).GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = greenLeaf;
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_EliteEarth.matAffixEarthTrailBloblets_mat)).Completed += y =>
                {
                    x.Result.transform.Find("HyperbloomOrbTrailOuter").GetComponent<TrailRenderer>().sharedMaterial = y.Result;
                };

                AddNewEffectDef(x.Result);
            };

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.crystallizeSpawnOrbEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = false;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;
                RoR2.Orbs.OrbEffect orb = x.Result.AddComponent<RoR2.Orbs.OrbEffect>();
                orb.startVelocity1 = new Vector3(-3, 20f, -3f);
                orb.startVelocity2 = new Vector3(3, 25f, 3f);
                orb.movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
                AddNewEffectDef(x.Result);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.crystallizePickup).Completed += x =>
            {
                x.Result.AddComponent<NetworkIdentity>();
                var teamFilter = x.Result.AddComponent<TeamFilter>();
                teamFilter.defaultTeam = TeamIndex.Player;
                x.Result.AddComponent<DestroyOnTimer>().duration = 10;
                var networkTransform = x.Result.AddComponent<ProjectileNetworkTransform>();
                networkTransform.interpolationFactor = 2f;
                networkTransform.positionTransmitInterval = 0.066666f;
                var gravitate = x.Result.transform.Find("GravitationController").gameObject.AddComponent<GravitatePickup>();
                gravitate.rigidbody = x.Result.GetComponent<Rigidbody>();
                gravitate.maxSpeed = 40f;
                gravitate.acceleration = 5;
                gravitate.teamFilter = teamFilter;
                gravitate.gravitateAtFullHealth = true;
                var vfxParent = x.Result.transform.GetChild(0);
                var model = vfxParent.GetChild(0).gameObject;
                var crystallizeRings = model.transform.Find("CrystallizeRings");
                var startVFX = model.transform.Find("CrystallizeStartGlow");
                startVFX.Find("CrystallizeStartDistortion").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matDistortionFaded_mat)).WaitForCompletion();
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matOmniRing1Generic_mat)).Completed += x =>
                {
                    crystallizeRings.GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
                    startVFX.Find("CrystallizeStartRing").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
                };
                model.transform.Find("CrystallizeRays").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matLunarGazeFireLaser3_mat)).WaitForCompletion();
                model.gameObject.AddComponent<RotateItem>().spinSpeed = 120f;
                var controller = x.Result.transform.Find("PickupTrigger").gameObject.AddComponent<CrystallizeController>();
                controller.teamFilter = teamFilter;
                controller.baseGameObject = x.Result;
                controller.gravitateCollider = gravitate.GetComponent<SphereCollider>();
                controller.teamRecolorParticle = [crystallizeRings.GetComponent<ParticleSystem>(), startVFX.Find("CrystallizeStartRing").GetComponent<ParticleSystem>()];
                x.Result.AddComponent<Deployable>();
                //x.Result.AddComponent<ElementalReactionPooledObject>().gravitatePickup = gravitate;

                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Elites_EliteAurelionite.matEliteAurelioniteAffixDisplay_mat)).Completed += x =>
                {
                    Material crystallizeMat = new Material(x.Result);
                    crystallizeMat.SetNormal(0.1f);
                    AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampGrandparent_png)).Completed += y =>
                    {
                        crystallizeMat.SetTexture("_FresnelRamp", y.Result);
                    };
                    crystallizeMat.EnableKeyword("FRESNEL_EMISSION");
                    crystallizeMat.SetFloat("_FresnelBoost", 20f);
                    crystallizeMat.SetFloat("_FresnelPower", 1.5f);

                    model.GetComponent<MeshRenderer>().sharedMaterial = crystallizeMat;
                };

                Content.AddNetworkedObjectPrefab(x.Result);
            };
            #endregion

            #region Items
            Material visionHolderMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(AssetReferences.visionHolderMaterial).WaitForCompletion();
            visionHolderMaterial.SetHopooMaterial().Specular(0.4f, 3f, false);
            visionHolderMaterial.SetNormal(1.3f);
            visionHolderMaterial.SetFloat("_RampInfo", 1);

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.instructorsTeaCupPickupModel).Completed += x =>
            {
                Items.Items.AddModelPanelParameters(x.Result);
            };
            AssetAsyncReferenceManager<Material>.LoadAsset(AssetReferences.instructorsTeaCupMaterial).Completed += x =>
            {
                x.Result.SetHopooMaterial().Specular(0.6f, 3f, false);
                x.Result.SetNormal(1.5f);
                x.Result.EnableKeyword("FRESNEL_EMISSION");
                x.Result.SetFloat("_FresnelBoost", 1f);
                x.Result.SetFloat("_FresnelPower", 2f);
                AssetAsyncReferenceManager<Texture>.LoadAsset(AssetReferences.instructorsTeaCupFresnelMask).Completed += y =>
                {
                    x.Result.SetTexture("_FresnelMask", y.Result);
                };
                AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDroneFire_png)).Completed += y =>
                {
                    x.Result.SetTexture("_FresnelRamp", y.Result);
                };
            };
            #region Delusion
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.delusionHitEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                vfx.DoNotPool = false;
                ShakeEmitter shakeEmitter = x.Result.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.25f;
                shakeEmitter.radius = 75f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.2f,
                    frequency = 10f,
                    cycleOffset = 0f
                };
                x.Result.AddComponent<NetworkIdentity>();
                Texture delusionRamp = AssetAsyncReferenceManager<Texture>.LoadAsset(AssetReferences.delusionRamp).WaitForCompletion();
                Material delusionHitspark = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matOmniHitspark3Generic_mat)).WaitForCompletion());
                delusionHitspark.SetTexture("_RemapTex", delusionRamp);
                delusionHitspark.SetFloat("_AlphaBoost", 1.2f);
                x.Result.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBrightTransparent_mat)).WaitForCompletion();
                x.Result.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().sharedMaterial = delusionHitspark;
                Material delusionOrb = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_VoidSurvivor.matVoidSurvivorBlasterSphereAreaIndicator_mat)).WaitForCompletion());
                delusionOrb.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(AssetReferences.delusionOrbRamp).WaitForCompletion());
                delusionOrb.EnableKeyword("VERTEXCOLOR");
                x.Result.transform.GetChild(4).GetComponent<ParticleSystemRenderer>().sharedMaterial = delusionOrb;
                x.Result.transform.GetChild(5).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matOmniHitspark2Generic_mat)).WaitForCompletion();
                x.Result.transform.GetChild(6).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matInverseDistortion_mat)).WaitForCompletion();
                Material delusionSparkle = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matWideGlow_mat)).WaitForCompletion());
                delusionSparkle.SetTexture("_RemapTex", delusionRamp);
                delusionSparkle.SetFloat("_InvFade", 0.25f);
                delusionSparkle.SetFloat("_Boost", 2f);
                delusionSparkle.SetInt("_ZTest", 8);
                delusionSparkle.SetFloat("_DepthOffset", -3f);
                x.Result.transform.GetChild(7).GetComponent<ParticleSystemRenderer>().sharedMaterial = delusionSparkle;
                x.Result.transform.GetChild(8).GetComponent<ParticleSystemRenderer>().sharedMaterial = delusionSparkle;
                var light = x.Result.transform.Find("DelusionHitLight").GetComponent<Light>();
                vfx.optionalLights = [light];
                var lightCurve = light.gameObject.AddComponent<LightIntensityCurve>();
                lightCurve.timeMax = 0.35f;
                lightCurve.curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
                GenericElementEffectComponent component = x.Result.AddComponent<GenericElementEffectComponent>();
                ParticleSystemRenderer delusionRingParticleRenderer = x.Result.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
                delusionRingParticleRenderer.mesh = ringMesh;
                delusionRingParticleRenderer.sharedMaterial = genericRingMat;
                component.icon = x.Result.transform.GetChild(9).GetComponent<ParticleSystemRenderer>();
                component.particlesToRecolor = [
                    delusionRingParticleRenderer.GetComponent<ParticleSystem>(),
                    x.Result.transform.GetChild(1).GetComponent<ParticleSystem>(),
                    x.Result.transform.GetChild(2).GetComponent<ParticleSystem>(),
                    x.Result.transform.GetChild(3).GetComponent<ParticleSystem>(),
                    x.Result.transform.GetChild(4).GetComponent<ParticleSystem>(),
                    x.Result.transform.GetChild(7).GetComponent<ParticleSystem>(),
                    x.Result.transform.GetChild(8).GetComponent<ParticleSystem>(),
                    x.Result.transform.GetChild(9).GetComponent<ParticleSystem>()];
                component.lightToRecolor = light;
                x.Result.AddComponent<DestroyOnTimer>().duration = 0.6f;

                AddNewEffectDef(x.Result, "Play_seeker_skill1_impact");
            };

            AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Tonic.TonicBuffEffect_prefab)).Completed += x =>
            {
                GameObject delusionActiveEffect = PrefabAPI.InstantiateClone(x.Result, "DelusionActiveEffect", false);
                VFXAttributes vfx = delusionActiveEffect.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;
                var particle = delusionActiveEffect.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
                ParticleSystem.MainModule main = particle.main;
                main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.3f, 0.35f, 0.4f));
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;

                var temp = delusionActiveEffect.GetComponent<TemporaryVisualEffect>();
                var delusion = delusionActiveEffect.AddComponent<DelusionTemporaryVisualEffect>();

                temp.enterComponents = temp.enterComponents.Append(delusion).ToArray();
                delusion.enabled = false;

                AssetAsyncReferenceManager<PostProcessProfile>.LoadAsset(new AssetReferenceT<PostProcessProfile>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_title_PostProcessing.ppLocalDoppelganger_asset)).Completed += y =>
                {
                    PostProcessProfile ppProfile = ScriptableObject.Instantiate(y.Result);
                    ppProfile.RemoveSettings<ColorGrading>();

                    var pp = delusionActiveEffect.transform.GetChild(1).GetChild(0).GetComponent<PostProcessVolume>();
                    RuntimeUtilities.DestroyProfile(pp.profile, true);
                    pp.sharedProfile = ppProfile;
                    pp.GetComponent<PostProcessDuration>().ppWeightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0.5f);
                    delusionActiveEffect.GetComponent<PostProcessDuration>().ppWeightCurve = AnimationCurve.EaseInOut(0f, 0.5f, 1f, 0f);
                };

                TempVisualEffectAPI.AddTemporaryVisualEffect(delusionActiveEffect, (body) => { return body.HasBuff(Buffs.delusionActiveBuff); });
            };
            #endregion

            #region Lunar Reactions
            Material lunarVFXSymbol = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matOmniRing1Generic_mat)).WaitForCompletion());
            lunarVFXSymbol.SetTexture("_MainTex", AssetAsyncReferenceManager<Texture>.LoadAsset(AssetReferences.lunarVFXSymbol).WaitForCompletion());
            lunarVFXSymbol.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTritoneSmoothed_png)).WaitForCompletion());
            lunarVFXSymbol.SetFloat("_AlphaBoost", 4.5f);
            lunarVFXSymbol.SetFloat("_DepthOffset", -3f);
            lunarVFXSymbol.SetFloat("_InvFade", 0.35f);
            lunarVFXSymbol.SetFloat("_ZTest", 8f);
            Material kuuvahkiTrail = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Seeker.matSpiritPunchSoftRay_mat)).WaitForCompletion();
            Material lunarLineMaterial = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Elites_EliteBead.matEliteBeadSpikeGrowthRing_mat)).WaitForCompletion());
            lunarLineMaterial.SetTexture("_RemapTex", lunarVFXSymbol.GetTexture("_RemapTex"));
            lunarLineMaterial.SetTextureScale("_MainTex", new Vector2(10f, -0.1f));
            lunarLineMaterial.EnableKeyword("VERTEXCOLOR");
            Mesh lunarLineMesh = AssetAsyncReferenceManager<Mesh>.LoadAsset(new AssetReferenceT<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.mdlVFXDonut1_fbx_donut1Mesh_)).WaitForCompletion();

            Material lunarDecal = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarExploder.matLunarExploderDeathDecal_mat)).WaitForCompletion());
            lunarDecal.name = "matLunarDecal";
            lunarDecal.SetColor("_Color", new Color(8f, 8f, 8f));
            lunarDecal.SetTexture("_MaskTex", AssetAsyncReferenceManager<Texture>.LoadAsset(AssetReferences.lunarDecal).WaitForCompletion());
            lunarDecal.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampSporeGrenadeGas_png)).WaitForCompletion());
            lunarDecal.SetFloat("_AlphaBoost", 0.3f);
            lunarDecal.SetTexture("_Cloud1Tex", null);
            lunarDecal.SetTexture("_Cloud2Tex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.texCloudWaterFoam3_tga)).WaitForCompletion());

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.lunarChargedLightningEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                vfx.DoNotPool = false;
                ShakeEmitter shakeEmitter = x.Result.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.25f;
                shakeEmitter.radius = 40f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.6f,
                    frequency = 40f,
                    cycleOffset = 0f
                };
                x.Result.AddComponent<NetworkIdentity>();
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matPrimeDevastatorChargeVFX1_mat)).Completed += y =>
                {
                    Material lightningMat = new Material(y.Result);
                    lightningMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTeslaCoil_png)).WaitForCompletion());
                    lightningMat.SetColor("_TintColor", Color.white);
                    lightningMat.SetInt("_DstBlend", 10);
                    x.Result.transform.Find("LunarChargedLightning").GetComponent<ParticleSystemRenderer>().trailMaterial = lightningMat;
                };
                ParticleSystemRenderer donutParticleRenderer = x.Result.transform.Find("LunarChargedDonut5").GetComponent<ParticleSystemRenderer>();
                donutParticleRenderer.mesh = AssetAsyncReferenceManager<Mesh>.LoadAsset(new AssetReferenceT<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.mdlVFXDonut5_fbx_donut5Mesh_)).WaitForCompletion();
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_MealPrep.matMealPrepUIFlames2_mat)).Completed += y =>
                {
                    Material donutMat = new Material(y.Result);
                    donutMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2.texRampTritoneHShrine_png)).WaitForCompletion());
                    donutParticleRenderer.sharedMaterial = donutMat;
                };
                var flash = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBright_mat)).WaitForCompletion();
                x.Result.transform.Find("LunarChargedFlash").GetComponent<ParticleSystemRenderer>().sharedMaterial = flash;
                x.Result.transform.Find("LunarChargedSparks").GetComponent<ParticleSystemRenderer>().sharedMaterial = flash;
                x.Result.transform.Find("LunarChargedDistortion").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matInverseDistortion_mat)).WaitForCompletion();
                x.Result.transform.Find("LunarChargedElectricity").GetComponent<ParticleSystemRenderer>().trailMaterial = electroTrailMaterial;
                x.Result.transform.Find("LunarChargedElectricityDark").GetComponent<ParticleSystemRenderer>().trailMaterial = darkElectricTrailMaterial;
                x.Result.transform.Find("LunarChargedLunarSymbol").GetComponent<ParticleSystemRenderer>().sharedMaterial = lunarVFXSymbol;
                var lunarLines = x.Result.transform.Find("LunarChargedLunarLines").GetComponent<ParticleSystemRenderer>();
                lunarLines.sharedMaterial = lunarLineMaterial;
                lunarLines.mesh = lunarLineMesh;
                var kuuvahkiRise = x.Result.transform.Find("LunarChargedKuuvahkiRise");
                kuuvahkiRise.GetComponent<ParticleSystemRenderer>().trailMaterial = kuuvahkiTrail;
                var kuuvahkiRiseRotate = kuuvahkiRise.gameObject.AddComponent<RotateObject>();
                kuuvahkiRiseRotate.rotationSpeed = new Vector3(0, 40, 0);
                var kuuvahkiRing = x.Result.transform.Find("LunarChargedKuuvahkiRing");
                kuuvahkiRing.GetComponent<ParticleSystemRenderer>().trailMaterial = kuuvahkiTrail;
                var kuuvahkiRingRotate = kuuvahkiRing.gameObject.AddComponent<RotateObject>();
                kuuvahkiRingRotate.rotationSpeed = new Vector3(0, 100, 0);
                var light = x.Result.transform.Find("LunarChargedLight");
                vfx.optionalLights = [light.GetComponent<Light>()];
                var lightCurve = light.gameObject.AddComponent<LightIntensityCurve>();
                lightCurve.timeMax = 0.4f;
                lightCurve.curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
                x.Result.AddComponent<DestroyOnTimer>().duration = 0.75f;

                AddNewEffectDef(x.Result, "Play_item_use_lighningArm");
            };
            ElementalReactionManager.lunarChargeEnemyStrikePrefab = PrefabAPI.CreateEmptyPrefab("LunarChargeEnemyInstance");
            ElementalReactionManager.lunarChargeEnemyStrikePrefab.AddComponent<EnemyLunarChargeInstance>();

            AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_meridian_DisableSkillsLightning.LightningStrikePredictionEffect_prefab)).Completed += x =>
            {
                EnemyLunarChargeInstance.warningPrefab = PrefabAPI.InstantiateClone(x.Result, "LunarChargeWarningEffect");
                EnemyLunarChargeInstance.warningPrefab.AddComponent<NetworkIdentity>();
                var retimer = EnemyLunarChargeInstance.warningPrefab.GetComponent<EffectRetimer>();
                retimer.objectScaleCurves = null;
                var light = EnemyLunarChargeInstance.warningPrefab.transform.GetChild(0).GetComponent<Light>();
                light.color = new Color(0.6f, 0.3f, 1f);
                light.intensity = 10f;
                
                var wall = EnemyLunarChargeInstance.warningPrefab.transform.GetChild(1);
                GameObject.Destroy(wall.GetComponent<ObjectScaleCurve>());
                var wallRenderer = wall.GetComponent<MeshRenderer>();
                Material wallMat = new Material(wall.GetComponent<MeshRenderer>().sharedMaterial);
                wallMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTeslaCoil_png)).WaitForCompletion());
                wallRenderer.sharedMaterial = wallMat;
                var wallGlowRenderer = wall.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
                Material wallGlowMat = new Material(wallGlowRenderer.sharedMaterial);
                wallGlowMat.SetColor("_TintColor", new Color(1f, 0.5f, 2f));
                wallGlowRenderer.sharedMaterial = wallGlowMat;
                
                // don't get the decal from team indicator, there is no team
                var decalObject = new GameObject("LunarChargeWarningDecal", typeof(MeshFilter), typeof(Decal), typeof(DecaliciousRenderer), typeof(AnimateShaderAlpha), typeof(MeshRenderer));
                decalObject.transform.SetParent(EnemyLunarChargeInstance.warningPrefab.transform);
                decalObject.transform.localPosition = Vector3.zero;
                decalObject.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                decalObject.GetComponent<MeshFilter>().sharedMesh = AssetAsyncReferenceManager<Mesh>.LoadAsset(new AssetReferenceT<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.Decalicious.DecalCube_asset)).WaitForCompletion();

                decalObject.GetComponent<DecaliciousRenderer>();
                var decal = decalObject.GetComponent<Decal>();
                decal.RenderMode = Decal.DecalRenderMode.Deferred;
                decal.DrawAlbedo = true;
                decal.DrawNormalAndGloss = false;
                decal.Material = lunarDecal;
                decal.Fade = 1f;
                var decalAlpha = decal.gameObject.GetComponent<AnimateShaderAlpha>();
                decalAlpha.decal = decal;
                decalAlpha.alphaCurve = AnimationCurve.Linear(0, 0, 1f, 1f);
                decalAlpha.timeMax = 0.75f;
                retimer.animateShaderAlphas.Add(decalAlpha);
                decal.GetComponent<MeshRenderer>().sharedMaterial = lunarDecal;

                EnemyLunarChargeInstance.warningPrefab.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().trailMaterial = electroTrailMaterial;

                AddNewEffectDef(EnemyLunarChargeInstance.warningPrefab);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.lunarBloomEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = false;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                vfx.DoNotPool = false;
                ShakeEmitter shakeEmitter = x.Result.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 1f;
                shakeEmitter.radius = 75f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.35f,
                    frequency = 2.5f,
                    cycleOffset = 0f
                };
                x.Result.AddComponent<NetworkIdentity>();
                var flash = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBright_mat)).WaitForCompletion();
                x.Result.transform.Find("LunarBloomFlash").GetComponent<ParticleSystemRenderer>().sharedMaterial = flash;
                x.Result.transform.Find("LunarBloomSparks").GetComponent<ParticleSystemRenderer>().sharedMaterial = flash;
                x.Result.transform.Find("LunarBloomDistortion").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_TeamWarCry.matTeamWarCryDistortion_mat)).WaitForCompletion();
                x.Result.transform.Find("LunarBloomLunarSymbol").GetComponent<ParticleSystemRenderer>().sharedMaterial = lunarVFXSymbol;
                x.Result.transform.Find("LunarBloomSplash").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Seeker.matSpiritPunchSplashOpaque_mat)).WaitForCompletion();
                var ripple = x.Result.transform.Find("LunarBloomRipple").GetComponent<ParticleSystemRenderer>();
                ripple.gameObject.AddComponent<Billboard>();
                ripple.trailMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_AcidLarva.matAcidLarvaTrail_mat)).WaitForCompletion();
                x.Result.transform.Find("LunarBloomOrbitTrails").GetComponent<ParticleSystemRenderer>().trailMaterial = ripple.trailMaterial;
                x.Result.transform.Find("LunarBloomSplash").GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_AcidLarva.matAcidLarvaBlood_mat)).WaitForCompletion();
                var lunarLines = x.Result.transform.Find("LunarBloomLunarLines").GetComponent<ParticleSystemRenderer>();
                lunarLines.sharedMaterial = lunarLineMaterial;
                lunarLines.mesh = lunarLineMesh;
                var lunarLines2 = x.Result.transform.Find("LunarBloomLunarLines2").GetComponent<ParticleSystemRenderer>();
                lunarLines2.sharedMaterial = lunarLineMaterial;
                lunarLines2.mesh = lunarLineMesh;
                var kuuvahkiRing = x.Result.transform.Find("LunarBloomKuuvahkiRing");
                kuuvahkiRing.GetComponent<ParticleSystemRenderer>().trailMaterial = kuuvahkiTrail;
                var kuuvahkiRingRotate = kuuvahkiRing.gameObject.AddComponent<RotateObject>();
                kuuvahkiRingRotate.rotationSpeed = new Vector3(0, 100, 0);
                var light = x.Result.transform.Find("LunarBloomLight");
                vfx.optionalLights = [light.GetComponent<Light>()];
                var lightCurve = light.gameObject.AddComponent<LightIntensityCurve>();
                lightCurve.timeMax = 0.8f;
                lightCurve.curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
                x.Result.AddComponent<DestroyOnTimer>().duration = 1.5f;

                AddNewEffectDef(x.Result, "Play_seeker_skill4_win");
            };

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.lunarCrystallizeController).Completed += x =>
            {
                Material moondrift = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Grandparent.matGrandParentSunCore_mat)).WaitForCompletion());
                moondrift.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_Common_ColorRamps.texRampConstructLaserTypeB_png)).WaitForCompletion());
                moondrift.SetFloat("_Boost", 1.5f);
                moondrift.SetFloat("_AlphaBoost", 7f);
                moondrift.SetFloat("_AlphaBias", 0.5f);
                moondrift.SetFloat("_FresnelPower", -1f);

                Material glow = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Items_ExtraStatsOnLevelUp.matBeadsEnlightenGlow_mat)).WaitForCompletion());
                glow.SetColor("_TintColor", new Color(1f, 0.5f, 0.3f));
                glow.SetTextureScale("_MainTex", new Vector2(1, 0.5f));
                glow.SetTextureOffset("_MainTex", new Vector2(0, 0.5f));
                glow.SetTextureScale("_Cloud2Tex", new Vector2(5, -0.6f));
                glow.SetTextureOffset("_Cloud2Tex", new Vector2(0, 0.6f));
                glow.SetFloat("_Boost", 1f);
                glow.SetFloat("_AlphaBoost", 0.5f);
                glow.SetFloat("_FresnelPower", 0f);

                var controller = x.Result.AddComponent<LunarCrystallizeController>();
                controller.moondrift1 = x.Result.transform.GetChild(0).GetChild(0).gameObject;
                CreateMoondrift(controller.moondrift1, moondrift, glow);
                controller.moondrift2 = x.Result.transform.GetChild(1).GetChild(0).gameObject;
                CreateMoondrift(controller.moondrift2, moondrift, glow);
                controller.moondrift3 = x.Result.transform.GetChild(2).GetChild(0).gameObject;
                CreateMoondrift(controller.moondrift3, moondrift, glow);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.lunarCrystallizeActivatedEffect).Completed += x =>
            {
                EffectComponent effect = x.Result.AddComponent<EffectComponent>();
                effect.positionAtReferencedTransform = true;
                effect.parentToReferencedTransform = true;
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                vfx.DoNotPool = false;
                ShakeEmitter shakeEmitter = x.Result.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.5f;
                shakeEmitter.radius = 75f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 0.2f,
                    frequency = 10f,
                    cycleOffset = 0f
                };
                x.Result.AddComponent<NetworkIdentity>();
                var distortion = x.Result.transform.Find("LunarCrystallizeSpinDistortion").GetComponent<ParticleSystemRenderer>();
                distortion.mesh = ringMesh;
                distortion.sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_ancientloft.matAncientLoft_WaterfallDistortion_mat)).WaitForCompletion();
                var spin = x.Result.transform.Find("LunarCrystallizeSpin").GetComponent<ParticleSystemRenderer>();
                spin.mesh = ringMesh;
                spin.sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2.matGeodeCleanseVFX1_mat)).WaitForCompletion();
                x.Result.transform.Find("LunarCrystallizeLunarSymbol").GetComponent<ParticleSystemRenderer>().sharedMaterial = lunarVFXSymbol;
                var lunarLines = x.Result.transform.Find("LunarCrystallizeLunarLines").GetComponent<ParticleSystemRenderer>();
                lunarLines.sharedMaterial = lunarLineMaterial;
                lunarLines.mesh = lunarLineMesh;
                var kuuvahkiRing = x.Result.transform.Find("LunarCrystallizeKuuvahkiRing");
                kuuvahkiRing.GetComponent<ParticleSystemRenderer>().trailMaterial = kuuvahkiTrail;
                var kuuvahkiRingRotate = kuuvahkiRing.gameObject.AddComponent<RotateObject>();
                kuuvahkiRingRotate.rotationSpeed = new Vector3(0, 100, 0);
                var light = x.Result.transform.Find("LunarCrystallizeLight");
                vfx.optionalLights = [light.GetComponent<Light>()];
                var lightCurve = light.gameObject.AddComponent<LightIntensityCurve>();
                lightCurve.timeMax = 0.55f;
                lightCurve.curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
                x.Result.AddComponent<DestroyOnTimer>().duration = 0.7f;

                AddNewEffectDef(x.Result, "Play_Seeker_PalmBlast_HealImpact");
            };
            // use more seeker sfx for lunar crystallize. shift enter for crystallize spawn, primary for attack?
            //Play_Seeker_PalmBlast_HealImpact for reaching 3 moondrifts
            #endregion
            #endregion
        }
        private static void CreateMoondrift(GameObject moondrift, Material material, Material glowMaterial)
        {
            moondrift.transform.GetComponent<MeshRenderer>().sharedMaterial = material;
            moondrift.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = glowMaterial;
            var mainAlpha = moondrift.AddComponent<AnimateShaderAlpha>();
            mainAlpha.timeMax = 0.2f;
            mainAlpha.alphaCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);
            var glowAlpha = moondrift.transform.GetChild(0).gameObject.AddComponent<AnimateShaderAlpha>();
            glowAlpha.timeMax = 0.4f;
            glowAlpha.alphaCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);
            moondrift.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matInverseDistortion_mat)).WaitForCompletion();
            moondrift.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matGenericFlash_mat)).WaitForCompletion();
        }

        public static Material CreateVisionMaterial(AssetReferenceT<Texture> icon, AssetReferenceT<Texture> remapTex, float alphaBoost = 1f)
        {
            Material vision = new Material(Addressables.LoadAssetAsync<Shader>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Shaders.HGOpaqueCloudRemap_shader).WaitForCompletion());
            vision.name = "ElementalReactionsVision";
            vision.EnableKeyword("EMISSIONFROMALBEDO");
            vision.EnableKeyword("DITHER");
            vision.EnableKeyword("USE_CLOUDS");
            vision.EnableKeyword("USE_UV1");
            vision.EnableKeyword("_EMISSION");
            AssetAsyncReferenceManager<Texture>.LoadAsset(icon).Completed += x =>
            {
                vision.SetTexture("_MainTex", x.Result);
            };
            AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_Chef.texChefOilDecalMask_png)).Completed += x =>
            {
                vision.SetTexture("_Cloud1Tex", x.Result);
                vision.SetTextureScale("_Cloud1Tex", new Vector2(1, 0.3f));
            };
            AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX_ParticleMasks.texAlphaGradient2_png)).Completed += x =>
            {
                vision.SetTexture("_Cloud2Tex", x.Result);
                vision.SetTextureScale("_Cloud2Tex", new Vector2(1, -1));
                vision.SetTextureOffset("_Cloud2Tex", new Vector2(0, 1.08f));
            };
            AssetAsyncReferenceManager<Texture>.LoadAsset(remapTex).Completed += x =>
            {
                vision.SetTexture("_RemapTex", x.Result);
            };
            vision.SetVector("_CutoffScroll", new Vector4(0, -1.5f, 0, 0));
            vision.SetFloat("_AlphaBoost", alphaBoost);
            vision.SetFloat("_Cutoff", 0f);
            vision.Specular(0.7f, 9f, false);
            vision.SetFloat("_RampInfo", 1);

            return vision;
        }
        private static void CreateGenericElementEffectMaterial()
        {
            genericElementEffectMaterial = new Material(Addressables.LoadAssetAsync<Shader>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Shaders.HGCloudRemap_shader).WaitForCompletion());
            genericElementEffectMaterial.name = "ElementalReactionsElementIconVFX";
            genericElementEffectMaterial.EnableKeyword("CLOUDOFFSET");
            genericElementEffectMaterial.EnableKeyword("USE_CLOUDS");
            genericElementEffectMaterial.EnableKeyword("VERTEXCOLOR");
            genericElementEffectMaterial.EnableKeyword("EMISSIONFROMALBEDO");
            genericElementEffectMaterial.EnableKeyword("_EMISSION");
            genericElementEffectMaterial.SetFloat("_Boost", 5f);
            genericElementEffectMaterial.SetFloat("_AlphaBoost", 3f);
            genericElementEffectMaterial.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDefault_png)).WaitForCompletion());
            genericElementEffectMaterial.SetFloat("_DepthOffset", -5f);
            genericElementEffectMaterial.SetFloat("_ZTest", 8f);
            genericElementEffectMaterial.SetTexture("_Cloud1Tex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_TiledTextures.texCloudDifferenceBW2_png)).WaitForCompletion());
            genericElementEffectMaterial.SetTexture("_Cloud2Tex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_TiledTextures.texCloudOrganicNormal_png)).WaitForCompletion());
            genericElementEffectMaterial.SetVector("_CutoffScroll", new Vector4(10f, -25f, 0, -10));
        }
        public static Material CreateElementEffectMaterial(Texture icon)
        {
            Material newMat = new Material(genericElementEffectMaterial);
            newMat.SetTexture("_MainTex", icon);
            return newMat;
        }

        private static void AddAkBank(GameObject prefab, string bodyGUID)
        {
            AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(bodyGUID)).Completed += x =>
            {
                AkBank bank = prefab.AddComponent<AkBank>();
                AkBank sourceBank = x.Result.GetComponent<AkBank>();
                if (sourceBank)
                {
                    bank.data.WwiseObjectReference = sourceBank.data.WwiseObjectReference;
                }
            };
        }

        [SystemInitializer(typeof(ElementCatalog))]
        public static void AfterElementCatalogReady()
        {
            AddElementLoadoutMenu();
            var bloomSpecialObjectAttributes = Assets.AssetReferences.bloomObject.LoadAssetAsync<GameObject>().WaitForCompletion().GetComponent<SpecialObjectAttributes>();
            bloomSpecialObjectAttributes.damageTypeOverride.SetElement(DefaultElementDefs.dendroElement.index);
            bloomSpecialObjectAttributes.damageTypeOverride.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
        }
        private static void AddElementLoadoutMenu()
        {
            GameObject characterSelectMenu = AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_UI.CharacterSelectUIMain_prefab)).WaitForCompletion();
            CharacterSelectController characterSelectController = characterSelectMenu.GetComponent<CharacterSelectController>();
            Array.Resize(ref characterSelectController.primaryColorImages, characterSelectController.primaryColorImages.Length + 1);
            Transform menu = characterSelectMenu.transform.Find("SafeArea/LeftHandPanel (Layer: Main)/SurvivorInfoPanel, Active (Layer: Secondary)");
            Transform headerButtons = menu.Find("SubheaderPanel (Overview, Skills, Loadout)");
            Transform contentPanel = menu.Find("ContentPanel (Overview, Skills, Loadout)");
            GameObject elementButton = GameObject.Instantiate(headerButtons.Find("GenericMenuButton (Loadout)").gameObject);
            elementButton.transform.SetParent(headerButtons, false);
            elementButton.transform.localScale = Vector3.one; // why do unity transforms break so damn hard when parenting anything
            elementButton.transform.localEulerAngles = Vector3.zero;
            elementButton.name = "GenericMenuButton (Elements)";
            elementButton.transform.SetSiblingIndex(4);
            elementButton.GetComponent<LanguageTextMeshController>().token = $"{ElementalReactionsPlugin.PREFIX}LOADOUT_ELEMENTS";
            characterSelectController.primaryColorImages[characterSelectController.primaryColorImages.Length - 1] = elementButton.GetComponent<Image>();

            GameObject elementPanel = GameObject.Instantiate(contentPanel.Find("LoadoutPanel").gameObject);
            elementPanel.transform.SetParent(contentPanel, false);
            elementPanel.transform.localScale = Vector3.one;
            elementPanel.transform.localEulerAngles = Vector3.zero;
            elementPanel.name = "ElementsPanel";
            GameObject.Destroy(elementPanel.GetComponent<LoadoutPanelController>());
            elementPanel.SetActive(false);
            ElementLoadoutPanelController elementPanelController = elementPanel.AddComponent<ElementLoadoutPanelController>();
            elementPanelController.hoverTextDescription = elementPanelController.transform.Find("DescriptionPanel, Loadout/DescriptionPanelContent (Layer: Secondary)/DescriptionText").GetComponent<LanguageTextMeshController>();
            elementPanelController.requiredUILayerKey = menu.GetComponent<UILayerKey>();

            headerButtons.GetComponent<HGHeaderNavigationController>().headers.Add(new HGHeaderNavigationController.Header { 
                headerButton = elementButton.GetComponent<HGButton>(),
                headerName = Language.GetString($"{ElementalReactionsPlugin.PREFIX}LOADOUT_ELEMENTS"),
                tmpHeaderText = elementButton.transform.Find("ButtonText").GetComponent<HGTextMeshProUGUI>(),
                headerRoot = elementPanel,
                AreConsolePlatformsSupported = true,
                isPrimaryPlayerOnly = false
            });
            // not enough vertical room in the menu for this to look good I guess
            elementLoadoutRowUI = PrefabAPI.InstantiateClone(AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_UI.Row_prefab)).WaitForCompletion(), "ElementLoadoutRow", false);
            elementLoadoutRowUI.GetComponent<LayoutElement>().preferredHeight = 116;
            var buttonContainer = elementLoadoutRowUI.transform.GetChild(0);
            GameObject.DestroyImmediate(buttonContainer.gameObject.GetComponent<HorizontalLayoutGroup>());
            var buttonGrid = buttonContainer.gameObject.AddComponent<GridLayoutGroup>();
            buttonGrid.cellSize = new Vector2(56f, 56f);
            buttonGrid.childAlignment = TextAnchor.MiddleLeft;
            buttonGrid.padding.left = 128;
            buttonGrid.spacing = new Vector2(0f, 0f);
            GameObject.Destroy(buttonContainer.GetChild(0).gameObject);
        }
        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }
        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            Content.AddEffectDef(newEffectDef);
        }

        public static class AssetReferences
        {
            public static AssetReferenceT<Sprite> expansionIcon = new AssetReferenceT<Sprite>("64c84609674948a4d80e1fc588a1d8a9");

            public static AssetReferenceT<Sprite> pyroBuffIcon = new AssetReferenceT<Sprite>("0daee0d7c66cf3349a5c729e74a12925");
            public static AssetReferenceT<Sprite> hydroBuffIcon = new AssetReferenceT<Sprite>("7e274e591617cd94f851984046f5c921");
            public static AssetReferenceT<Sprite> electroBuffIcon = new AssetReferenceT<Sprite>("a416b58f4ed2331488afcf4e6eba114f");
            public static AssetReferenceT<Sprite> cryoBuffIcon = new AssetReferenceT<Sprite>("e38221f0cc59ebd4f96bfcc5034fde14");
            public static AssetReferenceT<Sprite> anemoBuffIcon = new AssetReferenceT<Sprite>("2e957546a6b4ecf4088b8cb9a754aaaf");
            public static AssetReferenceT<Sprite> geoBuffIcon = new AssetReferenceT<Sprite>("61c6caa41fc7e8c4abf6534fadeb89b0");
            public static AssetReferenceT<Sprite> dendroBuffIcon = new AssetReferenceT<Sprite>("87b63bb49e9e2fa4f9d2a66665ee07a7");

            public static AssetReferenceT<Sprite> quickenBuffIcon = new AssetReferenceT<Sprite>("fd1a80b8adab48644bde7e4c5d73fd13");
            public static AssetReferenceT<Sprite> superconductBuffIcon = new AssetReferenceT<Sprite>("5fc2055e4d7c33348889a483e5a0df1b");
            public static AssetReferenceT<Sprite> electroChargeBuffIcon = new AssetReferenceT<Sprite>("151966ab1f5e45043ab88e997d14ae7d");

            public static AssetReferenceT<Texture> pyroIcon = new("13e481dbe51e4d640ad603f14cb6bfe9");
            public static AssetReferenceT<Texture> hydroIcon = new("3eb8865e5a13b7742b66a47fbdd2aee7");
            public static AssetReferenceT<Texture> electroIcon = new("56c4fbbb5b62cb84abd0f8a9575caf3f");
            public static AssetReferenceT<Texture> cryoIcon = new("9435d25802bf8994a89c7275c9264810");
            public static AssetReferenceT<Texture> anemoIcon = new("38a3562c6487d3d459a11ab8ce19b418");
            public static AssetReferenceT<Texture> geoIcon = new("f972169ff42390a4da00e9b3eb915dbe");
            public static AssetReferenceT<Texture> dendroIcon = new("cdc71ccb0e628c74ba052480a5bc3cd4");

            public static AssetReferenceT<Sprite> physicalSkillIcon = new AssetReferenceT<Sprite>("d4a1b90b0494f41468cc3891bf05e53a");
            public static AssetReferenceT<Sprite> pyroSkillIcon = new AssetReferenceT<Sprite>("dc58e8263293feb4d800d43015d479c5");
            public static AssetReferenceT<Sprite> hydroSkillIcon = new AssetReferenceT<Sprite>("e186d9590b77d8248abd9e77fbbd3f48");
            public static AssetReferenceT<Sprite> electroSkillIcon = new AssetReferenceT<Sprite>("993b4bd8d5b61934f93b53275220256d");
            public static AssetReferenceT<Sprite> cryoSkillIcon = new AssetReferenceT<Sprite>("d5fb27b74cc81cf409874d173f52e728");
            public static AssetReferenceT<Sprite> anemoSkillIcon = new AssetReferenceT<Sprite>("7eee6493b891a1746a05e1b540b4eb4d");
            public static AssetReferenceT<Sprite> geoSkillIcon = new AssetReferenceT<Sprite>("c88d96edd23da354696d9d9d66cd102f");
            public static AssetReferenceT<Sprite> dendroSkillIcon = new AssetReferenceT<Sprite>("4e802f5ad484760438a8692950a15773");

            public static AssetReferenceT<GameObject> genericElementActivatedEffect = new("0dcaf09df2cb8ab4c838c88c6d997a39");
            public static AssetReferenceT<GameObject> genericElementOrbEffect = new("383d58c1367764647bb30cbdd0339ac0");

            #region Reactions
            public static AssetReferenceT<GameObject> electroChargeTempVisualEffect = new("11147caefb6967a41ab27574e55d2a88");
            public static AssetReferenceT<GameObject> quickenTempVisualEffect = new("8c00cd5f2a1b86c4ba23ceab51149285");

            public static AssetReferenceT<GameObject> overloadEffect = new("a7c7911aead178a499a9803ee81dd5a3");

            public static AssetReferenceT<GameObject> superconductEffect = new("b2ee9cb3889cd7c4bb620fd72233099a");
            public static AssetReferenceT<GameObject> superconductTempVisualEffect = new("b46ea4e21788bdc418384e2de927414e");

            public static AssetReferenceT<GameObject> swirlEffect = new("7afc921af72c23941980c334193ee394");

            public static AssetReferenceT<GameObject> crystallizeSpawnOrbEffect = new("b71c0d9d0e04e4f42a27e5067bc57920");
            public static AssetReferenceT<GameObject> crystallizePickup = new("677d6b93d9a81fa44b68adbcd2288857");

            public static AssetReferenceT<Sprite> bloomIcon = new("cd5c3c5b432da044d8ab88e45c4ca562");
            public static AssetReferenceT<GameObject> bloomObject = new("3208908270e8db14489bfb68079f0dcd");
            public static AssetReferenceT<Material> bloomMaterial = new("c74a8fba09c05e843afab90abce26e10");
            public static AssetReferenceT<Texture> bloomFresnelMask = new("77d939fda7dde4048a33fccac924a029");
            public static AssetReferenceT<Texture> bloomTransparent = new("ff18c4fae117474409f084cf24b74999");

            public static AssetReferenceT<GameObject> bloomExplosion = new("1a5da254110bef7488484791f3220e83");
            public static AssetReferenceT<GameObject> burgeonExplosion = new("016343dff8c6ed345a37610401ac69b4");
            public static AssetReferenceT<GameObject> hyperbloomOrb = new("84548caf27cac6b44b9f23e11e0c447d");
            #endregion

            #region Items
            #region Common
            public static AssetReferenceT<Material> visionHolderMaterial = new AssetReferenceT<Material>("e3301a4ccd084f4428b3b23e85dc1733");
            #endregion
            #region Delusion
            public static AssetReferenceT<GameObject> delusionPickupModel = new AssetReferenceT<GameObject>("9f3cf544c7630a04fa25214a5197c191");
            public static AssetReferenceT<GameObject> delusionDisplayModel = new AssetReferenceT<GameObject>("79b9b2b62c2a66a439fad33cb020271a");
            public static AssetReferenceT<Texture> delusionLogo = new AssetReferenceT<Texture>("8c75207915d01ff4280ac8f0e15b5aad");
            public static AssetReferenceT<Sprite> delusionItemIcon = new AssetReferenceT<Sprite>("884bdf224e0646e43b6dc1b6a2675c92");

            public static AssetReferenceT<Sprite> delusionPyroItemIcon = new AssetReferenceT<Sprite>("c43b3123149962f4ba691794e52b3e43");
            public static AssetReferenceT<Sprite> delusionHydroItemIcon = new AssetReferenceT<Sprite>("cf1b789b5d942b142bd2ceff8f57f3a7");
            public static AssetReferenceT<Sprite> delusionElectroItemIcon = new AssetReferenceT<Sprite>("6556071d8b734a34c9ed73fe8f085b54");
            public static AssetReferenceT<Sprite> delusionCryoItemIcon = new AssetReferenceT<Sprite>("c18bd1206fb57a545b75e8ebe1277ae4");
            public static AssetReferenceT<Sprite> delusionAnemoItemIcon = new AssetReferenceT<Sprite>("d2783fcd777fdc443b2413bea0a67ee4");
            public static AssetReferenceT<Sprite> delusionGeoItemIcon = new AssetReferenceT<Sprite>("41fc2191bdd38634aa9d5d2f99a338c4");
            public static AssetReferenceT<Sprite> delusionDendroItemIcon = new AssetReferenceT<Sprite>("3bedb54f341b8b448b9c0a1cd9dcdd7b");

            public static AssetReferenceT<Sprite> delusionBuffIcon = new AssetReferenceT<Sprite>("6eea3353a4d395843b4cd63335aa6de1");

            public static AssetReferenceT<GameObject> delusionHitEffect = new("65c647fb4bb695846b19becd2e583081");
            public static AssetReferenceT<Texture> delusionRamp = new("77c55f158f600e048ab3cecb73f7b2b6");
            public static AssetReferenceT<Texture> delusionOrbRamp = new("18f28cc4480fabe408617d74510efcb1");
            #endregion
            #region Instructor's Tea Cup
            public static AssetReferenceT<GameObject> instructorsTeaCupPickupModel = new("b51b42d6d9466d845a1d09da7a916642");
            public static AssetReferenceT<Material> instructorsTeaCupMaterial = new("21fcdb5cd2cf4b44fa0b04fe96e09702");
            public static AssetReferenceT<Texture> instructorsTeaCupFresnelMask = new("053b6932d6b15d5488c3626eb00c31ec");
            public static AssetReferenceT<Sprite> instructorsTeaCupItemIcon = new("371e6f4214277484c90a8755fa3e42e1");
            #endregion
            #region Moonwheel
            public static AssetReferenceT<GameObject> moonWheelPickupModel = new AssetReferenceT<GameObject>("7e50ea908069f874fac56c93f70a328d");
            public static AssetReferenceT<GameObject> moonWheelDisplayModel = new AssetReferenceT<GameObject>("061359f9d76adfe429aa42f3b25caeb8");
            public static AssetReferenceT<Texture> moonWheelVisionIcon = new("af83aeca078a68443bf1583507eab088");
            public static AssetReferenceT<Texture> moonWheelVisionRamp = new("1f819082df45cdf44b5b8f019b4a90b5");
            public static AssetReferenceT<Sprite> moonWheelItemIcon = new("00f1f0088ad92dc4eb41b80b7aef67f2");
            public static AssetReferenceT<Sprite> elementalMasteryAchievementIcon = new("9547acf0f18b5a547a0533b84299b100");

            public static AssetReferenceT<Texture> lunarVFXSymbol = new("044f325621315c64a8a6b0b7277ff9b3");
            public static AssetReferenceT<Texture> lunarDecal = new("4cff496a700287245a5591fc5f7e1906");

            public static AssetReferenceT<GameObject> lunarChargedLightningEffect = new("558b1898670f29e4f8aba548a3357090");
            public static AssetReferenceT<Sprite> lunarChargeBuffIcon = new("ef41e115d6e9b3b45816e1afb73c5a0f");

            public static AssetReferenceT<GameObject> lunarBloomEffect = new("623c28827e49bf041ade36cdd7cdf922");
            public static AssetReferenceT<Sprite> lunarBloomBuffIcon = new("b14c2afa0ea1ba6449927fb72cbbd016");
            public static AssetReferenceT<Sprite> lunarBloomChargingBuffIcon = new("dc667ee18bd31314a9c85d4ba5590cbf");

            public static AssetReferenceT<GameObject> lunarCrystallizeController = new("151ca516e4a36dc4b9071f8c9957d1e4");
            public static AssetReferenceT<GameObject> lunarCrystallizeActivatedEffect = new("6eb753463343f764385e8aaad94cbecf");
            #endregion
            #endregion
        }
    }
}
