using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;
using RoR2.Projectile;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
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

        public static GameObject genericElementActivatedEffect;
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
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
                vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
                vfx.DoNotPool = false;
                x.Result.AddComponent<NetworkIdentity>();
                GenericElementEffectComponent component = x.Result.AddComponent<GenericElementEffectComponent>();
                ParticleSystem iconParticle = x.Result.transform.GetChild(0).GetComponent<ParticleSystem>();
                ParticleSystem rayParticle = x.Result.transform.GetChild(1).GetComponent<ParticleSystem>();
                ParticleSystem glowParticle = x.Result.transform.GetChild(2).GetComponent<ParticleSystem>();
                component.icon = x.Result.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
                component.particlesToRecolor = [iconParticle, rayParticle, glowParticle];
                component.scaleDuration = true;
                x.Result.AddComponent<DestroyOnParticleEnd>().trackedParticleSystem = iconParticle;
                var scale = x.Result.AddComponent<ScaleParticleSystemDuration>();
                scale.initialDuration = 0.7f;
                scale.particleSystems = [iconParticle, rayParticle, glowParticle];
                component.particleDuration = scale;
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matLunarGazeFireLaser3_mat)).Completed += y =>
                {
                    x.Result.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = y.Result;
                };
                genericElementActivatedEffect = x.Result;
                AddNewEffectDef(genericElementActivatedEffect);
            };

            #region Reactions
            electroTrailMaterial = new Material(AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC2_FalseSonBoss.matPrimeDevastatorChargeVFX2_mat)).WaitForCompletion());
            electroTrailMaterial.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTeslaCoil_png)).WaitForCompletion());
            electroTrailMaterial.SetFloat("_Boost", 2f);

            darkElectricTrailMaterial = new Material(electroTrailMaterial);
            darkElectricTrailMaterial.SetColor("_TintColor", Color.black);
            darkElectricTrailMaterial.SetInt("_DstBlend", 10);

            dendroElectricTrailMaterial = new Material(electroTrailMaterial);
            dendroElectricTrailMaterial.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampOrbitalLaser_png)).WaitForCompletion());
            dendroElectricTrailMaterial.SetColor("_TintColor", new Color(0.5f, 1f, 0f));
            dendroElectricTrailMaterial.SetFloat("_Boost", 4f);

            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.electroChargeTempVisualEffect).Completed += x =>
            {
                VFXAttributes vfx = x.Result.AddComponent<VFXAttributes>();
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
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
                vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
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

                AddNewEffectDef(x.Result, "Play_LuminousShot_Explosion");
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
                swirlRingParticleRenderer.mesh = AssetAsyncReferenceManager<Mesh>.LoadAsset(new AssetReferenceT<Mesh>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.mdlVFXDonut2_fbx_donut2Mesh_)).WaitForCompletion();
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_BFG.matBeamSphereBeam_mat)).Completed += y =>
                {
                    Material swirlMat = new Material(y.Result);
                    swirlMat.SetTexture("_RemapTex", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTritoneSmoothed_png)).WaitForCompletion());
                    swirlMat.EnableKeyword("VERTEXCOLOR");
                    swirlRingParticleRenderer.sharedMaterial = swirlMat;
                };
                x.Result.AddComponent<DestroyOnTimer>().duration = 0.4f;

                AddNewEffectDef(x.Result);
            };

            AssetAsyncReferenceManager<Material>.LoadAsset(AssetReferences.bloomMaterial).Completed += x =>
            {
                x.Result.SetHopooMaterial();
                AssetAsyncReferenceManager<Texture>.LoadAsset(AssetReferences.bloomFresnelMask).Completed += y =>
                {
                    x.Result.SetTexture("_FresnelMask", y.Result);
                };
                x.Result.SetTexture("_FresnelRamp", AssetAsyncReferenceManager<Texture>.LoadAsset(new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampOrbitalLaser_png)).WaitForCompletion());
                x.Result.EnableKeyword("FRESNEL_EMISSION");
                x.Result.SetFloat("_FresnelBoost", 7f);
                x.Result.SetFloat("_FresnelPower", 1.6f);
                x.Result.SetEmission(0.25f);
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

                Content.AddNetworkedObjectPrefab(x.Result);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(AssetReferences.crystallizePickup).Completed += x =>
            {
                x.Result.AddComponent<NetworkIdentity>();
                var teamFilter = x.Result.AddComponent<TeamFilter>();
                teamFilter.defaultTeam = TeamIndex.Player;
                x.Result.AddComponent<DestroyOnTimer>().duration = 10;
                var networkTransform = x.Result.AddComponent<ProjectileNetworkTransform>();
                networkTransform.interpolationFactor = 2f;
                networkTransform.positionTransmitInterval = 0.66666f;
                var gravitate = x.Result.transform.Find("GravitationController").gameObject.AddComponent<GravitatePickup>();
                gravitate.rigidbody = x.Result.GetComponent<Rigidbody>();
                gravitate.maxSpeed = 40f;
                gravitate.acceleration = 5;
                gravitate.teamFilter = teamFilter;
                gravitate.gravitateAtFullHealth = true;
                var vfxParent = x.Result.transform.GetChild(0);
                var model = vfxParent.GetChild(0).gameObject;
                AssetAsyncReferenceManager<Material>.LoadAsset(new AssetReferenceT<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matOmniRing1Generic_mat)).Completed += x =>
                {
                    model.transform.Find("CrystallizeRings").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
                };
                model.gameObject.AddComponent<RotateItem>().spinSpeed = 120f;
                var controller = x.Result.transform.Find("PickupTrigger").gameObject.AddComponent<CrystallizeController>();
                controller.teamFilter = teamFilter;
                controller.baseGameObject = x.Result;
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
            AssetAsyncReferenceManager<Material>.LoadAsset(AssetReferences.visionHolderMaterial).Completed += x =>
            {
                x.Result.SetHopooMaterial().Specular(0.4f, 3f, false);
                x.Result.SetNormal(1.3f);
                x.Result.SetFloat("_RampInfo", 1);
            };

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
            #endregion
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
            genericElementEffectMaterial.SetVector("_CutoffScroll", new Vector4(10f, -25f, 0, 0));
        }
        public static Material CreateElementEffectMaterial(Texture icon)
        {
            Material newMat = new Material(genericElementEffectMaterial);
            newMat.SetTexture("_MainTex", icon);
            return newMat;
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

            #region Reactions
            public static AssetReferenceT<GameObject> electroChargeTempVisualEffect = new("11147caefb6967a41ab27574e55d2a88");
            public static AssetReferenceT<GameObject> quickenTempVisualEffect = new("8c00cd5f2a1b86c4ba23ceab51149285");

            public static AssetReferenceT<GameObject> overloadEffect = new("a7c7911aead178a499a9803ee81dd5a3");
            public static AssetReferenceT<GameObject> swirlEffect = new("7afc921af72c23941980c334193ee394");

            public static AssetReferenceT<GameObject> crystallizePickup = new("677d6b93d9a81fa44b68adbcd2288857");

            public static AssetReferenceT<Sprite> bloomIcon = new("cd5c3c5b432da044d8ab88e45c4ca562");
            public static AssetReferenceT<GameObject> bloomObject = new("3208908270e8db14489bfb68079f0dcd");
            public static AssetReferenceT<Material> bloomMaterial = new("c74a8fba09c05e843afab90abce26e10");
            public static AssetReferenceT<Texture> bloomFresnelMask = new("77d939fda7dde4048a33fccac924a029");
            #endregion

            #region Items
            #region Common
            public static AssetReferenceT<Material> visionHolderMaterial = new AssetReferenceT<Material>("e3301a4ccd084f4428b3b23e85dc1733");
            #endregion
            #region Delusion
            public static AssetReferenceT<GameObject> delusionPickupModel = new AssetReferenceT<GameObject>("9f3cf544c7630a04fa25214a5197c191");
            public static AssetReferenceT<Texture> delusionLogo = new AssetReferenceT<Texture>("8c75207915d01ff4280ac8f0e15b5aad");
            public static AssetReferenceT<Sprite> delusionItemIcon = new AssetReferenceT<Sprite>("884bdf224e0646e43b6dc1b6a2675c92");

            public static AssetReferenceT<Sprite> delusionCooldownBuffIcon = new AssetReferenceT<Sprite>("11b881fd7c08c0b4faf1b305df7e394d");
            public static AssetReferenceT<Sprite> delusionReadyBuffIcon = new AssetReferenceT<Sprite>("da2c01d04bcb15f43848d28d22db15d9");
            public static AssetReferenceT<Sprite> delusionActiveBuffIcon = new AssetReferenceT<Sprite>("c48688fe6fab6304badbca799ca382be");
            #endregion
            #region Instructor's Tea Cup
            public static AssetReferenceT<GameObject> instructorsTeaCupPickupModel = new("b51b42d6d9466d845a1d09da7a916642");
            public static AssetReferenceT<Material> instructorsTeaCupMaterial = new("21fcdb5cd2cf4b44fa0b04fe96e09702");
            public static AssetReferenceT<Texture> instructorsTeaCupFresnelMask = new("053b6932d6b15d5488c3626eb00c31ec");
            public static AssetReferenceT<Sprite> instructorsTeaCupItemIcon = new("371e6f4214277484c90a8755fa3e42e1");
            #endregion
            #region Moonwheel
            public static AssetReferenceT<GameObject> moonWheelPickupModel = new AssetReferenceT<GameObject>("7e50ea908069f874fac56c93f70a328d");
            public static AssetReferenceT<Texture> moonWheelVisionIcon = new("af83aeca078a68443bf1583507eab088");
            public static AssetReferenceT<Texture> moonWheelVisionRamp = new("1f819082df45cdf44b5b8f019b4a90b5");
            public static AssetReferenceT<Sprite> moonWheelItemIcon = new("00f1f0088ad92dc4eb41b80b7aef67f2");
            public static AssetReferenceT<Sprite> lunarBloomBuffIcon = new("b14c2afa0ea1ba6449927fb72cbbd016");
            #endregion
            #endregion
        }
    }
}
