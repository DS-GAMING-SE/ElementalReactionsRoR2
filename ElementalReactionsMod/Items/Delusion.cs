using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ElementalReactionsMod.Assets.AssetReferences;
using UnityEngine.AddressableAssets;
using RoR2.Items;
using RoR2;
using UnityEngine.Networking;
using HG;
using ElementalReactionsMod.Loadout;
using ElementalReactionsMod.Elements;
using System.Linq;
using static ElementalReactionsMod.Items.Items;

namespace ElementalReactionsMod.Items
{
    public static class DelusionManager
    {
        public static List<ItemDef> elementalDelusions = new List<ItemDef>();

        public static void Initialize()
        {
            AssetAsyncReferenceManager<GameObject>.LoadAsset(delusionPickupModel).Completed += x =>
            {
                x.Result.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = Assets.CreateVisionMaterial(delusionLogo, new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampDefault_png));
                AddModelPanelParameters(x.Result);
            };
            delusion = AddNewItem("Delusion", "DELUSION", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(delusionItemIcon).WaitForCompletion(), delusionPickupModel, ItemTag.Damage);
        }
        public static ItemDef CreateNewDelusion(ElementDef element)
        {
            ItemDef delusion = AddNewItem($"Delusion{element.cachedName}", $"DELUSION_{element.cachedName.ToUpper()}", true, Addressables.LoadAssetAsync<ItemTierDef>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.LunarTierDef_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<Sprite>(delusionItemIcon).WaitForCompletion(), delusionPickupModel, ItemTag.Damage, ItemTag.WorldUnique);
            elementalDelusions.Add(delusion);
            return delusion;
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
                new Inventory.ItemTransformation
                {
                    originalItemIndex = GetItemDef().itemIndex,
                    newItemIndex = DecideDelusionElement().itemIndex,
                    maxToTransform = 1,
                    transformationType = 0
                }.TryTransform(body.inventory, out _);
                transformTimeStamp += 1f;
            }
        }

        private ItemDef DecideDelusionElement()
        {
            Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
            List<ItemDef> possibleDelusions = new List<ItemDef>();
            RoR2.Util.CopyList<ItemDef>(DelusionManager.elementalDelusions, possibleDelusions);
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
    public abstract class Delusion : BaseItemBodyBehavior
    {

    }
    public class DelusionPyro : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return GetElementDef().delusion;
        }
        private static ElementDef GetElementDef()
        {
            return DefaultElementDefs.pyroElement;
        }
    }
    public class DelusionHydro : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return GetElementDef().delusion;
        }
        private static ElementDef GetElementDef()
        {
            return DefaultElementDefs.hydroElement;
        }
    }
    public class DelusionElectro : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return GetElementDef().delusion;
        }
        private static ElementDef GetElementDef()
        {
            return DefaultElementDefs.electroElement;
        }
    }
    public class DelusionCryo : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return GetElementDef().delusion;
        }
        private static ElementDef GetElementDef()
        {
            return DefaultElementDefs.cryoElement;
        }
    }
    public class DelusionAnemo : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return GetElementDef().delusion;
        }
        private static ElementDef GetElementDef()
        {
            return DefaultElementDefs.anemoElement;
        }
    }
    public class DelusionGeo : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return GetElementDef().delusion;
        }
        private static ElementDef GetElementDef()
        {
            return DefaultElementDefs.geoElement;
        }
    }
    public class DelusionDendro : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return GetElementDef().delusion;
        }
        private static ElementDef GetElementDef()
        {
            return DefaultElementDefs.dendroElement;
        }
    }
}
