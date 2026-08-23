using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Reactions;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.XR;
using static ElementalReactionsMod.Assets.AssetReferences;
using static ElementalReactionsMod.Materials;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;

namespace ElementalReactionsMod.Items
{
    public static class Items
    {
        public static ItemDef delusion;
        public static ItemDef instructorsTeaCup => InstructorsTeaCup.instructorsTeaCup;
        public static ItemDef moonWheel => MoonWheel.moonWheel;

        public static ItemDef stellarLinchpin => StellarLinchpin.stellarLinchpin;

        public static void Initialize()
        {
            InstructorsTeaCup.Initialize();

            MoonWheel.Initialize();
#if DEBUG
            StellarLinchpin.Initialize();
#endif
            CharacterBody.onBodyInventoryChangedGlobal += AddItemBehaviours;

            ElementLoadoutComponent.Initialize();

            ElementalReactionManager.onPreElementalReactionTriggered += (ref reaction, element1, element2, victim, ref damage) =>
            {
                if (damage.attacker && damage.attacker.TryGetComponent<CharacterBody>(out var attackerBody) && attackerBody.inventory)
                {
                    if (attackerBody.inventory.GetItemCountWithQuality(moonWheel) > 0)
                    {
                        if (reaction == DefaultElementalReactions.crystallize && (element1 == DefaultElementDefs.hydroElement || element2 == DefaultElementDefs.hydroElement))
                        {
                            reaction = DefaultElementalReactions.lunarCrystallize;
                            return;
                        }
                        else if (reaction == DefaultElementalReactions.electroCharge)
                        {
                            reaction = DefaultElementalReactions.lunarCharge;
                            return;
                        }
                        else if (reaction == DefaultElementalReactions.bloom)
                        {
                            reaction = DefaultElementalReactions.lunarBloom;
                            return;
                        }
                    }
#if DEBUG
                    if (attackerBody.inventory.GetItemCountEffective(stellarLinchpin) > 0)
                    {
                        if (reaction == DefaultElementalReactions.superconduct)
                        {
                            reaction = DefaultElementalReactions.stellarConduct;
                            return;
                        }
                        if (reaction == DefaultElementalReactions.swirl && (element1 == DefaultElementDefs.cryoElement || element2 == DefaultElementDefs.cryoElement))
                        {
                            reaction = DefaultElementalReactions.stellarSwirl;
                            return;
                        }
                    }
#endif
                }
            };
        }
        public static void AddItemBehaviours(CharacterBody characterBody)
        {
            // Delusions
            List<ElementDef> list = characterBody.inventory.GetDelusions(out int count);
            DelusionBehaviour delusionBehaviour = characterBody.AddItemBehavior<DelusionBehaviour>(count);
            if (delusionBehaviour)
            {
                delusionBehaviour.delusionElements = list;
            }

            // Instructor's Tea Cup Quality
            if (ElementalReactionsPlugin.qualityModExists)
            {
                InstructorsTeaCupQuality teaCupBehaviour = characterBody.AddItemBehavior<InstructorsTeaCupQuality>(characterBody.inventory.GetItemCountQualities(instructorsTeaCup, out int uncommon, out int rare, out int epic, out int legendary));
                if (teaCupBehaviour)
                {
                    teaCupBehaviour.uncommonItemCount = uncommon;
                    teaCupBehaviour.rareItemCount = rare;
                    teaCupBehaviour.epicItemCount = epic;
                    teaCupBehaviour.legendaryItemCount = legendary;
                }
            }
        }
        internal static GameObject AddModelPanelParameters(GameObject item)
        {
            ModelPanelParameters panel = item.AddComponent<ModelPanelParameters>();
            panel.focusPointTransform = item.transform.Find("FocusPoint");

            panel.cameraPositionTransform = item.transform.Find("FocusPoint/CameraPosition");

            panel.minDistance = 4f;
            panel.maxDistance = 6f;
            return item;
        }
        internal static CharacterModel.RendererInfo CreateItemRendererInfo(GameObject displayPrefab, int child, Material material)
        {
            return new CharacterModel.RendererInfo
            {
                renderer = displayPrefab.transform.GetChild(child).GetComponent<MeshRenderer>(),
                defaultMaterial = material,
            };
        }
        internal static CharacterModel.RendererInfo CreateItemRendererInfo(GameObject displayPrefab, int child, AssetReferenceT<Material> materialReference)
        {
            return new CharacterModel.RendererInfo
            {
                renderer = displayPrefab.transform.GetChild(child).GetComponent<MeshRenderer>(),
                defaultMaterialAddress = materialReference
            };
        }
        internal static GameObject CreateItemDisplay(GameObject displayPrefab, params CharacterModel.RendererInfo[] rendererInfos)
        {
            ItemDisplay itemDisplay = displayPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = rendererInfos;
            return displayPrefab;
        }
        internal static ItemDef AddNewItem(string itemName, string token, bool canRemove, ItemTierDef itemTierDef, Sprite icon, AssetReferenceT<GameObject> pickupModelReference, ItemDisplayRuleDict itemDisplay, params ItemTag[] tags)
        {
            string prefix = $"{ElementalReactionsPlugin.PREFIX}ITEM_";
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = "ElementalReactions"+itemName;
            itemDef._itemTierDef = itemTierDef;
            if (!itemTierDef) itemDef.deprecatedTier = ItemTier.NoTier;
            itemDef.pickupModelReference = pickupModelReference;
            itemDef.pickupIconSprite = icon;
            itemDef.canRemove = canRemove;
            itemDef.hidden = !canRemove;
            itemDef.requiredExpansion = Assets.elementalReactionExpansionDef;

            itemDef.nameToken = prefix + token+"_NAME"; // stylised name
            itemDef.pickupToken = prefix + token + "_PICKUP";
            itemDef.descriptionToken = prefix + token + "_DESCRIPTION";
            itemDef.loreToken = prefix + token + "_LORE";
            itemDef.tags = tags;

            ItemAPI.Add(new CustomItem(itemDef, itemDisplay));

            return itemDef;
        }
    }
    public abstract class RerollableItem : BaseItemBodyBehavior
    {
        private Run.FixedTimeStamp tryRerollTimer;
        private bool triedReroll;
        private PickupIndex pickupIndex;

        public abstract ItemDef item { get; }
        public abstract HG.ReadOnlyList<PickupIndex> availableRerolls { get; }
        public abstract ElementDef coreElement {get; }
        public abstract ElementDef[] reactingElements { get; }

        private void OnEnable()
        {
            triedReroll = !(Config.RerollUnusableItems().Value && NetworkServer.active && body && body.isPlayerControlled);
            if (!triedReroll) tryRerollTimer = Run.FixedTimeStamp.now + 0.5f;
            pickupIndex = PickupCatalog.FindPickupIndex(item.itemIndex);
        }

        public virtual void FixedUpdate()
        {
            if (!triedReroll && tryRerollTimer.hasPassed)
            {
                if (GetShouldReroll())
                {
                    Reroll();
                }
                triedReroll = true;
            }
        }
        private bool GetShouldReroll()
        {
            bool hasCore = false;
            bool hasReacting = false;

            CheckElement(DamageSource.Primary, ref hasCore, ref hasReacting);
            CheckElement(DamageSource.Secondary, ref hasCore, ref hasReacting);
            CheckElement(DamageSource.Utility, ref hasCore, ref hasReacting);
            CheckElement(DamageSource.Special, ref hasCore, ref hasReacting);
            if ((!hasCore || !hasReacting) && TryGetComponent<DelusionBehaviour>(out var delusion))
            {
                if (!hasCore) hasCore = delusion.delusionElements.Contains(coreElement);
                if (!hasReacting) hasReacting = delusion.delusionElements.Intersect(reactingElements).Count() > 0;
            }

            if (hasCore && hasReacting)
            {
                return false;
            }
            return true;
        }
        private void Reroll()
        {
            if (ElementalReactionsPlugin.qualityModExists)
            {
                RerollQuality();
            }
            else
            {
                RerollItem(item.itemIndex, GetRerollItem());
            }
        }
        private void RerollItem(ItemIndex original, ItemIndex rerolled)
        {
            new Inventory.ItemTransformation
            {
                originalItemIndex = original,
                newItemIndex = rerolled,
                maxToTransform = int.MaxValue,
                transformationType = 0
            }.TryTransform(body.inventory, out _);
        }
        private ItemIndex GetRerollItem()
        {
            Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
            int index = rng.RangeInt(0, availableRerolls.Count - 1);
            PickupIndex item = availableRerolls[index];
            if (item == pickupIndex)
            {
                item = availableRerolls[index + 1];
            }
            return PickupCatalog.GetPickupDef(item).itemIndex;
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private void RerollQuality() // godddd quality is so COMPLICATED
        {
            ItemQualities.ItemQualityGroupIndex groupIndex = ItemQualities.QualityCatalog.FindItemQualityGroupIndex(item.itemIndex);
            if (groupIndex == ItemQualities.ItemQualityGroupIndex.Invalid) 
            {
                RerollItem(item.itemIndex, GetRerollItem());
                return;
            }
            ItemQualities.ItemQualityGroup itemGroup = ItemQualities.QualityCatalog.GetItemQualityGroup(groupIndex);
            ItemQualities.ItemQualityCounts itemCounts = ItemQualities.Utilities.Extensions.InventoryExtensions.GetItemCountsPermanent(body.inventory, itemGroup);
            if (itemCounts.BaseItemCount > 0) RerollItem(item.itemIndex, GetRerollItem());
            if (itemCounts.TotalQualityCount > 0)
            {
                ItemQualities.ItemQualityGroup rerollGroup;
                if (TryFindRandomQualityItemIndex(groupIndex, out var rerollGroupIndex))
                {
                    rerollGroup = ItemQualities.QualityCatalog.GetItemQualityGroup(rerollGroupIndex);
                }
                else
                {
                    // scrap
                    rerollGroup = ItemQualities.QualityCatalog.GetItemQualityGroup(ItemQualities.QualityCatalog.FindItemQualityGroupIndex(PickupCatalog.GetPickupDef(PickupCatalog.FindScrapIndexForItemTier(item.tier)).itemIndex));
                }
                if (itemCounts.UncommonCount > 0) RerollItem(itemGroup.UncommonItemIndex, rerollGroup.UncommonItemIndex);
                if (itemCounts.RareCount > 0) RerollItem(itemGroup.RareItemIndex, rerollGroup.RareItemIndex);
                if (itemCounts.EpicCount > 0) RerollItem(itemGroup.EpicItemIndex, rerollGroup.EpicItemIndex);
                if (itemCounts.LegendaryCount > 0) RerollItem(itemGroup.LegendaryItemIndex, rerollGroup.LegendaryItemIndex);
            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private bool TryFindRandomQualityItemIndex(ItemQualities.ItemQualityGroupIndex originalItemGroup, out ItemQualities.ItemQualityGroupIndex rerollGroup)
        {
            rerollGroup = ItemQualities.ItemQualityGroupIndex.Invalid;
            Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
            int index = rng.RangeInt(0, availableRerolls.Count - 1);
            for (int i = 0; i < availableRerolls.Count - 1; i++)
            {
                PickupIndex rerollItem = availableRerolls[(index + i) % availableRerolls.Count];
                rerollGroup = ItemQualities.QualityCatalog.FindItemQualityGroupIndex(PickupCatalog.GetPickupDef(rerollItem).itemIndex);
                if (rerollGroup != ItemQualities.ItemQualityGroupIndex.Invalid && rerollGroup != originalItemGroup)
                {
                    return true;
                }
            }
            return false;
        }
        private void CheckElement(DamageSource damageSource, ref bool core, ref bool reactor)
        {
            if (core && reactor) return;
            CheckElement(ElementLoadoutComponent.GetElement(body, damageSource), ref core, ref reactor);
        }
        private void CheckElement(ElementDef element, ref bool core, ref bool reactor)
        {
            if (core && reactor) return;
            if (element == coreElement)
            {
                core = true;
            }
            else if (reactingElements.Contains(element))
            {
                reactor = true;
            }
        }
    }
}
