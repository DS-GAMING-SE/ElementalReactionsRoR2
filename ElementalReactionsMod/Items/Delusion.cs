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

namespace ElementalReactionsMod.Items
{
    public class NoElementDelusion : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusion;
        }

        private Run.FixedTimeStamp transformTimeStamp;

        private void OnEnable()
        {
            transformTimeStamp = Run.FixedTimeStamp.now + 1f;
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
            return Items.elementalDelusions[rng.RangeInt(0, Items.elementalDelusions.Length)];
        }

        public static void Initialize()
        {
            AssetAsyncReferenceManager<GameObject>.LoadAsset(delusionPickupModel).Completed += x =>
            {
                x.Result.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial = Assets.CreateVisionMaterial(delusionLogo, new AssetReferenceT<Texture>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_ColorRamps.texRampTritone_png));
                Items.ModelPanelParameters(x.Result);
            };
        }
    }
    public abstract class Delusion : BaseItemBodyBehavior
    {

    }
    public abstract class DelusionPyro : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusionPyro;
        }
    }
    public abstract class DelusionHydro : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusionHydro;
        }
    }
    public abstract class DelusionElectro : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusionElectro;
        }
    }
    public abstract class DelusionCryo : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusionCryo;
        }
    }
    public abstract class DelusionAnemo : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusionAnemo;
        }
    }
    public abstract class DelusionGeo : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusionGeo;
        }
    }
    public abstract class DelusionDendro : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
        private static ItemDef GetItemDef()
        {
            return Items.delusionDendro;
        }
    }
}
