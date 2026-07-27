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

namespace ElementalReactionsMod.Items
{
    public static class Items
    {
        public static ItemDef delusion;
        public static ItemDef instructorsTeaCup => InstructorsTeaCup.instructorsTeaCup;
        public static ItemDef moonWheel => MoonWheel.moonWheel;

        public static void Initialize()
        {
            InstructorsTeaCup.Initialize();

            MoonWheel.Initialize();

            CharacterBody.onBodyInventoryChangedGlobal += AddItemBehaviours;

            ElementLoadoutComponent.Initialize();
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
            // QUALITY COMPAT?
            RerollItem(item.itemIndex, GetRerollItem());
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
