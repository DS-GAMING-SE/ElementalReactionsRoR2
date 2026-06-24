using ElementalReactionsMod.Elements;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.AddressableAssets;

namespace ElementalReactionsMod.Loadout
{
    [RequireComponent(typeof(CharacterMaster))]
    public class MasterElementLoadout : MonoBehaviour
    {
        CharacterMaster characterMaster;
        public static void Initialize()
        {
            Addressables.LoadAssetAsync<GameObject>(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Core.PlayerMaster_prefab)).WaitForCompletion().AddComponent<MasterElementLoadout>();
        }

        public void Awake()
        {
            characterMaster = GetComponent<CharacterMaster>();
        }

        public void Start()
        {
            if (characterMaster.hasEffectiveAuthority)
            {
                ElementDef[] elementLoadout = Config.GetElementLoadoutFromConfig(BodyCatalog.GetBodyName(BodyCatalog.FindBodyIndex(characterMaster.bodyPrefab)), out _);
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
        }
        public bool GiveElementLoadoutItems(ElementIndex[] elements)
        {
            return GiveElementLoadoutItems([ElementCatalog.GetElementDef(elements[0]), ElementCatalog.GetElementDef(elements[1]), ElementCatalog.GetElementDef(elements[2]), ElementCatalog.GetElementDef(elements[3])]);
        }
        public bool GiveElementLoadoutItems(ElementDef[] elements)
        {
            if (characterMaster.inventory)
            {
                UpdateLoadoutItem(characterMaster.inventory, elements[0], ElementLoadoutComponent.primaryElementItem);
                UpdateLoadoutItem(characterMaster.inventory, elements[1], ElementLoadoutComponent.secondaryElementItem);
                UpdateLoadoutItem(characterMaster.inventory, elements[2], ElementLoadoutComponent.utilityElementItem);
                UpdateLoadoutItem(characterMaster.inventory, elements[3], ElementLoadoutComponent.specialElementItem);
                return true;
            }
            return false;
        }
        private void UpdateLoadoutItem(Inventory inventory, ElementDef elementDef, ItemDef item)
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
