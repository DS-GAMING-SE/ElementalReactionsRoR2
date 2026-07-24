using ElementalReactionsMod.Elements;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.AddressableAssets;
using JetBrains.Annotations;

namespace ElementalReactionsMod.Loadout
{
    [RequireComponent(typeof(PlayerCharacterMasterController))]
    public class MasterElementLoadout : MonoBehaviour
    {
        PlayerCharacterMasterController characterMaster;
        private bool loadoutSet;
        public static void Initialize()
        {
            Addressables.LoadAssetAsync<GameObject>(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Core.PlayerMaster_prefab)).WaitForCompletion().AddComponent<MasterElementLoadout>();
        }

        public void Awake()
        {
            characterMaster = GetComponent<PlayerCharacterMasterController>();
        }
        private void FixedUpdate()
        {
            if (!loadoutSet && characterMaster.hasEffectiveAuthority && characterMaster.networkUser)
            {
                loadoutSet = true;
                NetworkElementLoadout();
            }
        }
        private void NetworkElementLoadout()
        {
            ElementDef[] elementLoadout = Config.GetElementLoadoutFromConfig(BodyCatalog.GetBodyName(characterMaster.networkUser.bodyIndexPreference), out _);
            if (NetworkServer.active)
            {
                GiveElementLoadoutItems(elementLoadout);
            }
            else
            {
                new NetworkElementLoadout(characterMaster.netId,
                    elementLoadout[0].index,
                    elementLoadout[1].index,
                    elementLoadout[2].index,
                    elementLoadout[3].index).Send(R2API.Networking.NetworkDestination.Server);
            }
        }
        public bool GiveElementLoadoutItems(ElementIndex[] elements)
        {
            return GiveElementLoadoutItems([ElementCatalog.GetElementDef(elements[0]), ElementCatalog.GetElementDef(elements[1]), ElementCatalog.GetElementDef(elements[2]), ElementCatalog.GetElementDef(elements[3])]);
        }
        public bool GiveElementLoadoutItems(ElementDef[] elements)
        {
            if (characterMaster.master.inventory)
            {
                UpdateLoadoutItem(characterMaster.master.inventory, elements[0], ElementLoadoutComponent.primaryElementItem);
                UpdateLoadoutItem(characterMaster.master.inventory, elements[1], ElementLoadoutComponent.secondaryElementItem);
                UpdateLoadoutItem(characterMaster.master.inventory, elements[2], ElementLoadoutComponent.utilityElementItem);
                UpdateLoadoutItem(characterMaster.master.inventory, elements[3], ElementLoadoutComponent.specialElementItem);
                return true;
            }
            return false;
        }
        public static void UpdateLoadoutItem(Inventory inventory, ElementDef elementDef, ItemDef item)
        {
            if (inventory.GetItemCountPermanent(item) != (int)elementDef.index)
            {
                if (inventory.GetItemCountPermanent(item) > (int)elementDef.index)
                {
                    inventory.RemoveItemPermanent(item, Math.Abs((int)elementDef.index) - inventory.GetItemCountPermanent(item));
                }
                else
                {
                    inventory.GiveItemPermanent(item, (int)elementDef.index - inventory.GetItemCountPermanent(item));
                }
            }
        }
    }
}
