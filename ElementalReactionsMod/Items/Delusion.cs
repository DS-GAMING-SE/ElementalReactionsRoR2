using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Orbs;
using HG;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static ElementalReactionsMod.Assets.AssetReferences;
using static ElementalReactionsMod.Items.Items;

namespace ElementalReactionsMod.Items
{
    public static class DelusionManager
    {
        public static ItemTag delusionItemTag;
        public static Dictionary<ItemDef, ElementDef> delusionToElement = new Dictionary<ItemDef, ElementDef>();

        private static Material delusionMat;

        public static void Initialize()
        {
            delusionItemTag = ItemAPI.AddItemTag("ElementalReactionsDelusion");
            delusionMat = Assets.CreateVisionMaterial(delusionLogo, new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDefault_png), 0.75f);
            AssetAsyncReferenceManager<GameObject>.LoadAsset(delusionPickupModel).Completed += x =>
            {
                x.Result.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = delusionMat;
                AddModelPanelParameters(x.Result);
            };
            delusion = AddNewItem("Delusion", "DELUSION", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(delusionItemIcon).WaitForCompletion(), delusionPickupModel, InitializeItemDisplays(), ItemTag.Damage, ItemTag.AllowedForUseAsCraftingIngredient);
            ItemAPI.ApplyTagToItem(delusionItemTag, delusion);
        }
        public static ItemDef CreateNewDelusion(ElementDef element, Sprite itemIcon)
        {
            ItemDef delusion = AddNewItem($"Delusion{element.cachedName}", $"DELUSION_{element.cachedName.ToUpper()}", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                itemIcon ? itemIcon : Addressables.LoadAssetAsync<Sprite>(delusionItemIcon).WaitForCompletion(), delusionPickupModel, null, ItemTag.Damage, ItemTag.WorldUnique, ItemTag.AllowedForUseAsCraftingIngredient);
            ItemAPI.ApplyTagToItem(delusionItemTag, delusion);
            delusionToElement.Add(delusion, element);
            return delusion;
        }
        public static List<ElementDef> GetDelusions(this Inventory inventory, out int count)
        {
            List<ElementDef> list = new();
            count = 0;
            foreach (var element in delusionToElement.Values)
            {
                if (inventory.GetItemCountEffective(element.delusion) > 0)
                {
                    list.Add(element);
                    count += inventory.GetItemCountEffective(element.delusion);
                }
            }
            return list;
        }
        public static int GetDelusionCount(this Inventory inventory)
        {
            int count = 0;
            foreach (var delusion in delusionToElement.Keys)
            {
                count += inventory.GetItemCountEffective(delusion);
            }
            return count;
        }
        public static ItemDisplayRuleDict InitializeItemDisplays()
        {
            GameObject displayPrefab = AssetAsyncReferenceManager<GameObject>.LoadAsset(delusionDisplayModel).WaitForCompletion();
            displayPrefab.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = delusionMat;
            displayPrefab.AddComponent<DelusionDisplay>();
            CreateItemDisplay(displayPrefab, CreateItemRendererInfo(displayPrefab, 0, Assets.visionMaterial), CreateItemRendererInfo(displayPrefab, 1, delusionMat));
            ItemDisplayRuleDict itemDisplays = new ItemDisplayRuleDict();
            itemDisplays.Add("CommandoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "LowerArmR",
                localPos = new Vector3(-0.03206F, 0.23382F, -0.05406F),
                localAngles = new Vector3(5.56553F, 0.75563F, 359.0454F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("HuntressBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(-0.16971F, 0.01192F, -0.08042F),
                localAngles = new Vector3(332.1566F, 50.95617F, 189.2482F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("Bandit2Body", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "MainWeapon",
                localPos = new Vector3(-0.08156F, 0.8256F, -0.05666F),
                localAngles = new Vector3(0F, 0F, 180F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F)
            });
            itemDisplays.Add("ToolbotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Head",
                localPos = new Vector3(0F, -0.35118F, 1.1199F),
                localAngles = new Vector3(55.43135F, 0.00005F, 0.00007F),
                localScale = new Vector3(7F, 7F, 7F)
            });
            itemDisplays.Add("EngiBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.44991F, 0.46288F, -0.31321F),
                localAngles = new Vector3(0F, 90F, 0F),
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
            });
            itemDisplays.Add("EngiTurretBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Head",
                localPos = new Vector3(0.61612F, 0.54662F, -1.31641F),
                localAngles = new Vector3(0F, 320.9564F, 0F),
                localScale = new Vector3(2.5F, 2.5F, 2.5F)
            });
            itemDisplays.Add("EngiWalkerTurretBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Neck",
                localPos = new Vector3(0.7483F, 0.65663F, 0F),
                localAngles = new Vector3(0F, 90F, 0F),
                localScale = new Vector3(2.5F, 2.5F, 2.5F)
            });
            itemDisplays.Add("MageBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.21887F, 0.2151F, -0.1934F),
                localAngles = new Vector3(5F, 90F, 9F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("MercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "LowerArmL",
                localPos = new Vector3(-0.00477F, 0.15047F, -0.06726F),
                localAngles = new Vector3(3.47613F, 19.27756F, 185.9751F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F)
            });
            itemDisplays.Add("TreebotBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "PlatformBase",
                localPos = new Vector3(-0.63358F, 0.68115F, -0.77494F),
                localAngles = new Vector3(46.13254F, 43.97253F, 4.15217F),
                localScale = new Vector3(1.3F, 1.3F, 1.3F)
            });
            itemDisplays.Add("LoaderBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.2F, 0.28F, 0.28F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
            });
            itemDisplays.Add("CrocoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "LowerArmL",
                localPos = new Vector3(0.89492F, 3.7274F, -0.77738F),
                localAngles = new Vector3(1.39027F, 313.4044F, 86.03119F),
                localScale = new Vector3(5.5F, 5.5F, 5.5F)
            });
            itemDisplays.Add("CaptainBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Stomach",
                localPos = new Vector3(0.1565F, 0.04206F, 0.17186F),
                localAngles = new Vector3(352.0692F, 27.6311F, 2.48244F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("RailgunnerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Backpack",
                localPos = new Vector3(0.24035F, -0.38056F, -0.08505F),
                localAngles = new Vector3(0F, 120F, 0F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F)
            });
            itemDisplays.Add("VoidSurvivorBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "CannonEnd",
                localPos = new Vector3(-0.01739F, -0.20445F, -0.09182F),
                localAngles = new Vector3(355.2102F, 51.1422F, 184.9114F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("SeekerBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "ThighR",
                localPos = new Vector3(0.10212F, 0.07662F, 0.07998F),
                localAngles = new Vector3(11.78787F, 41.68037F, 183.0371F),
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
            });
            itemDisplays.Add("FalseSonBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(0.10128F, 0.47448F, -0.34967F),
                localAngles = new Vector3(348.2777F, 356.4035F, 11.26319F),
                localScale = new Vector3(1.2F, 1.2F, 1.2F)
            });
            itemDisplays.Add("ChefBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "OvenDoor",
                localPos = new Vector3(-0.44195F, -0.15999F, 0.03149F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("DroneTechBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "LowerArmR",
                localPos = new Vector3(0.69052F, -0.03821F, 0.04073F),
                localAngles = new Vector3(273.9966F, 13.89896F, 95.11175F),
                localScale = new Vector3(0.9F, 0.9F, 0.9F)
            });
            itemDisplays.Add("DrifterBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.13434F, 0.07791F, -0.25401F),
                localAngles = new Vector3(34.96383F, 47.45707F, 157.2111F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            itemDisplays.Add("BrotherBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject("de62e89840152434cb622e434bf9bb62"),
                childName = "chest",
                localPos = new Vector3(-0.22503F, 0.31914F, 0.08435F),
                localAngles = new Vector3(7.70729F, 20.07879F, 317.6756F),
                localScale = new Vector3(0.11F, 0.11F, 0.11F)
            });
            itemDisplays.Add("NemCommandoBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Pelvis",
                localPos = new Vector3(-0.51642F, 0.21685F, -0.47749F),
                localAngles = new Vector3(352.9823F, 18.85301F, 194.7385F),
                localScale = new Vector3(2.5F, 2.5F, 2.5F)
            });
            itemDisplays.Add("NemMercBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "UpperLegR",
                localPos = new Vector3(-0.17763F, 0.32555F, -0.0382F),
                localAngles = new Vector3(1.14985F, 91.64176F, 169.1972F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F)
            });
            itemDisplays.Add("Executioner2Body", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "ElbowR",
                localPos = new Vector3(0.01307F, 0.1587F, -0.0572F),
                localAngles = new Vector3(4.00469F, 342.2709F, 354.1962F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F)
            });
            itemDisplays.Add("ChirrBody", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(delusionDisplayModel.AssetGUID),
                childName = "Chest",
                localPos = new Vector3(-0.49649F, 0.61212F, 0.24153F),
                localAngles = new Vector3(16.17279F, 77.65852F, 358.2969F),
                localScale = new Vector3(1.2F, 1.2F, 1.2F)
            });
            return itemDisplays;
        }
    }
    public class NoElementDelusion : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusion;
        }

        private Run.FixedTimeStamp transformTimeStamp;
        private ElementLoadoutComponent elementLoadout;
        private void OnEnable()
        {
            transformTimeStamp = Run.FixedTimeStamp.now + 1f;
        }

        private void Start()
        {
            elementLoadout = GetComponent<ElementLoadoutComponent>();
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active && Run.FixedTimeStamp.now > transformTimeStamp && body.inventory)
            {
                if (RoR2.Artifacts.CommandArtifactManager.IsCommandArtifactEnabled)
                {
                    body.inventory.RemoveItemPermanent(Items.delusion);
                    PickupPickerController.Option[] delusionOptions = new PickupPickerController.Option[DelusionManager.delusionToElement.Count];
                    ItemDef[] delusions = DelusionManager.delusionToElement.Keys.ToArray();
                    for (int i = 0; i < DelusionManager.delusionToElement.Count; i++)
                    {
                        delusionOptions[i] = new PickupPickerController.Option
                        {
                            pickup = new UniquePickup
                            {
                                pickupIndex = PickupCatalog.FindPickupIndex(delusions[i].itemIndex)
                            },
                            available = true,
                            overrideSelectedBGColor = DelusionManager.delusionToElement[delusions[i]].color
                        };
                    }
                    GenericPickupController.CreatePickupInfo pickupInfo = new GenericPickupController.CreatePickupInfo
                    {
                        pickerOptions = delusionOptions,
                        // Trying to use the command cube prefab breaks the whole thing??
                        prefabOverride = AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC1_OptionPickup.OptionPickup_prefab)).WaitForCompletion(),
                        position = transform.position + Vector3.up * 1f,
                        pickup = new UniquePickup
                        {
                            pickupIndex = delusionOptions[0].pickup.pickupIndex
                        },
                        artifactFlag = GenericPickupController.PickupArtifactFlag.DELUSION, // Haha, Delusion. Prevents command from rerolling it
                    };
                    PickupDropletController.CreatePickupDroplet(pickupInfo, pickupInfo.position, Vector3.up * 20f);
                }
                else
                {
                    if (new Inventory.ItemTransformation
                    {
                        originalItemIndex = GetItemDef().itemIndex,
                        newItemIndex = DecideDelusionElement().itemIndex,
                        maxToTransform = 1,
                        transformationType = 0
                    }.TryTransform(body.inventory, out var results))
                    {
                        PickupIndex pickup = PickupCatalog.FindPickupIndex(results.givenItem.itemIndex);
                        if (pickup != PickupIndex.none)
                        {
                            if (body.master && body.master.playerCharacterMasterController && body.master.playerCharacterMasterController.networkUser != null && body.master.playerCharacterMasterController.networkUser.localUser != null)
                            {
                                body.master.playerCharacterMasterController.networkUser.localUser.userProfile.DiscoverPickup(pickup);
                            }
                        }
                        if (DelusionManager.delusionToElement.TryGetValue(ItemCatalog.GetItemDef(results.givenItem.itemIndex), out var element))
                        {
                            GenericElementEffectComponent.SpawnActivatedEffect(gameObject.transform, ParentEffectToItemDisplay.ItemDisplayParent.Delusion, element.index, 1.5f, true);
                        }
                    ;
                    }
                    else
                    {
                        transformTimeStamp += 1f;
                    }
                }
            }
        }

        private ItemDef DecideDelusionElement()
        {
            Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
            List<ItemDef> possibleDelusions = DelusionManager.delusionToElement.Keys.ToList();
            List<ItemDef> loadoutElements = new List<ItemDef>();
            if (elementLoadout)
            {
                if (elementLoadout.primaryElement && elementLoadout.primaryElement.hasDelusion) loadoutElements.Add(elementLoadout.primaryElement.delusion);
                if (elementLoadout.secondaryElement && elementLoadout.secondaryElement.hasDelusion) loadoutElements.Add(elementLoadout.secondaryElement.delusion);
                if (elementLoadout.utilityElement && elementLoadout.utilityElement.hasDelusion) loadoutElements.Add(elementLoadout.utilityElement.delusion);
                if (elementLoadout.specialElement && elementLoadout.specialElement.hasDelusion) loadoutElements.Add(elementLoadout.specialElement.delusion);
            }
            List<ItemDef> ownedDelusions = new List<ItemDef>();
            for (int i = 0; i < possibleDelusions.Count; i++)
            {
                if (body.inventory.GetItemCountPermanent(possibleDelusions[i]) > 0)
                {
                    ownedDelusions.Add(possibleDelusions[i]);
                }
            }
            if (loadoutElements.Count + ownedDelusions.Count >= possibleDelusions.Count)
            {
                if (ownedDelusions.Count >= possibleDelusions.Count)
                {
                    return possibleDelusions[rng.RangeInt(0, possibleDelusions.Count)];
                }
                possibleDelusions = possibleDelusions.Except(ownedDelusions).ToList();
                return possibleDelusions[rng.RangeInt(0, possibleDelusions.Count)];

            }
            possibleDelusions = possibleDelusions.Except(ownedDelusions).Except(loadoutElements).ToList();
            return possibleDelusions[rng.RangeInt(0, possibleDelusions.Count)];
        }
    }
    public class DelusionBehaviour : CharacterBody.ItemBehavior, IOnDamageDealtServerReceiver
    {
        protected float attackCooldown;
        public List<ElementDef> delusionElements;
        public GameObject delusionDisplay;

        private void Start()
        {
            if (body)
            {
                body.onSkillActivatedServer += OnSkillActivated;
            }
        }
        private void OnSkillActivated(GenericSkill skill)
        {
            if (body.skillLocator && body.skillLocator.special && body.skillLocator.special == skill)
            {
                body.ClearTimedBuffs(Buffs.delusionActiveBuff);
                body.AddTimedBuffTimer(Buffs.delusionActiveBuff, StaticValues.delusionDuration);
                attackCooldown = 0;
            }
        }
        public void OnDamageDealtServer(DamageReport damageReport)
        {
            if (body.HasBuff(Buffs.delusionActiveBuff) && damageReport.damageInfo.damageType.IsDamageSourceSkillBased && attackCooldown == 0 && damageReport.victimBody)
            {
                StartCoroutine(FireDelusionsOrbs(damageReport.victimBody.mainHurtBox));
            }
        }
        private IEnumerator FireDelusionsOrbs(HurtBox target)
        {
            RoR2.Util.ShuffleList(delusionElements);
            foreach (var element in delusionElements)
            {
                if (!target || !target.healthComponent || !target.healthComponent.alive || !body || !body.healthComponent || !body.inventory)
                {
                    break;
                }
                if (element.hasDelusion && body.inventory.GetItemCountEffective(element.delusion) > 0)
                {
                    body.healthComponent.TakeDamage(new DamageInfo
                    {
                        attacker = null,
                        inflictor = null,
                        damage = (StaticValues.delusionHealthPercentCost * body.inventory.GetItemCountEffective(element.delusion)) * body.healthComponent.fullHealth,
                        damageType = DamageType.BypassArmor | DamageType.NonLethal,
                        position = body.corePosition,
                        crit = false,
                        procCoefficient = 0f,
                        inflictedHurtbox = body.mainHurtBox,
                        damageColorIndex = DamageColorIndex.Item
                    });
                    DelusionOrb.FireDelusionOrb(body, delusionDisplay ? delusionDisplay.transform.position : body.corePosition, target, body.inventory.GetItemCountEffective(element.delusion), body.RollCrit(), element.index);
                    GenericElementEffectComponent.SpawnActivatedEffect(gameObject.transform, ParentEffectToItemDisplay.ItemDisplayParent.Delusion, element.index, 0.7f, true);
                    attackCooldown = 1 / StaticValues.delusionAttacksPerSecond;
                    yield return new WaitForSeconds((1 / StaticValues.delusionAttacksPerSecond) / DelusionManager.delusionToElement.Keys.Count);
                }
            }
        }
        private void FixedUpdate()
        {
            if (body.HasBuff(Buffs.delusionActiveBuff)) attackCooldown = Mathf.Max(attackCooldown - Time.fixedDeltaTime, 0);
        }

        private void OnDisable()
        {
            if (body)
            {
                body.onSkillActivatedServer -= OnSkillActivated;
                if (body.HasBuff(Buffs.delusionActiveBuff)) body.ClearTimedBuffs(Buffs.delusionActiveBuff);
            }
        } 
    }
    public class DelusionDisplay : MonoBehaviour
    {
        public DelusionBehaviour delusion;
        private void Start()
        {
            CharacterModel characterModel = base.GetComponentInParent<CharacterModel>();
            if (characterModel && characterModel.body && characterModel.body.TryGetComponent<DelusionBehaviour>(out delusion))
            {
                delusion.delusionDisplay = gameObject;
            }
        }
    }

    [RequireComponent(typeof(TemporaryVisualEffect))]
    public class DelusionTemporaryVisualEffect : MonoBehaviour
    {
        public TemporaryVisualEffect tempVisualEffect;
        private void Awake()
        {
            tempVisualEffect = GetComponent<TemporaryVisualEffect>();
        }

        private void OnEnable()
        {
            if (tempVisualEffect.healthComponent && tempVisualEffect.healthComponent.TryGetComponent<DelusionBehaviour>(out var delusion) && delusion.delusionDisplay)
            {
                tempVisualEffect.parentTransform = delusion.delusionDisplay.transform;
                tempVisualEffect.radius = 0.65f;
            }
        }
    }
}
