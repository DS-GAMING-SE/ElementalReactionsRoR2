using ElementalReactionsMod.Elements;
using JetBrains.Annotations;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod.Loadout
{
    public class EngineerTurretElements : MonoBehaviour, MasterSummon.IInventorySetupCallback
    {
        public void SetupSummonedInventory([NotNull] MasterSummon masterSummon, [NotNull] Inventory summonedInventory)
        {
            if (masterSummon.summonerBodyObject && masterSummon.summonerBodyObject.TryGetComponent<CharacterBody>(out var body) && body.inventory)
            {
                ElementDef element = ElementLoadoutComponent.GetElement(body, DamageSource.Special);
                MasterElementLoadout.UpdateLoadoutItem(summonedInventory, element, ElementLoadoutComponent.primaryElementItem);
                MasterElementLoadout.UpdateLoadoutItem(summonedInventory, element, ElementLoadoutComponent.secondaryElementItem);
                MasterElementLoadout.UpdateLoadoutItem(summonedInventory, element, ElementLoadoutComponent.utilityElementItem);
                MasterElementLoadout.UpdateLoadoutItem(summonedInventory, element, ElementLoadoutComponent.specialElementItem);
                if (body.isPlayerControlled) summonedInventory.GiveItemPermanent(ElementLoadoutComponent.damageIsFromPlayerItem);
            }
        }
    }
}
