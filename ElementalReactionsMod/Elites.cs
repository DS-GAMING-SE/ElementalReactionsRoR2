using ElementalReactionsMod.Elements;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElementalReactionsMod
{
    public static class Elites
    {
        public static EliteElement[] eliteElements;
        public static bool zetAspectsExists = false;
        [SystemInitializer(typeof(ItemCatalog), typeof(EquipmentCatalog))]
        public static void CacheAspects()
        {
            zetAspectsExists = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetAspects");
            eliteElements = [
                new EliteElement(RoR2Content.Equipment.AffixRed.equipmentIndex, "ZetAspectRed", DefaultElementDefs.pyroElement), // Blazing
                new EliteElement (DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex, "ZetAspectVoid", DefaultElementDefs.hydroElement), // Voidtouched
                new EliteElement (RoR2Content.Equipment.AffixBlue.equipmentIndex, "ZetAspectBlue", DefaultElementDefs.electroElement), // Overloading
                new EliteElement (DLC3Content.Equipment.EliteCollectiveEquipment.equipmentIndex, "ZetAspectCollective", DefaultElementDefs.electroElement), // Collective
                new EliteElement (RoR2Content.Equipment.AffixWhite.equipmentIndex, "ZetAspectWhite", DefaultElementDefs.cryoElement), // Glacial
                new EliteElement (DLC2Content.Equipment.EliteAurelioniteEquipment.equipmentIndex, "ZetAspectAurelionite", DefaultElementDefs.geoElement), // Gilded
                new EliteElement (DLC1Content.Elites.Earth.eliteEquipmentDef.equipmentIndex, "ZetAspectEarth", DefaultElementDefs.dendroElement) // Mending
                ];
        }
        public static EliteElement? GetFirstEliteElement(Inventory inventory)
        {
            foreach (var elite in eliteElements)
            {
                if (elite.HasEliteAspect(inventory))
                {
                    return elite;
                }
            }
            return null;
        }
        public static ElementDef GetFirstEliteElementDef(Inventory inventory)
        {
            EliteElement? elite = GetFirstEliteElement(inventory);
            if (elite.HasValue)
            {
                return elite.Value.element;
            }
            return DefaultElementDefs.physicalElement;
        }

        public struct EliteElement
        {
            public EliteElement(EquipmentIndex equipment, string zetAspectItemName, ElementDef element)
            {
                aspectIndex = equipment;
                if (zetAspectsExists)
                {
                    zetAspectIndex = ItemCatalog.FindItemIndex(zetAspectItemName);
                }
                this.element = element;
            }
            public EquipmentIndex aspectIndex = EquipmentIndex.None;
            public ItemIndex zetAspectIndex = ItemIndex.None;
            public ElementDef element;
            public bool HasEliteAspect(Inventory inventory)
            {
                return inventory && ((zetAspectIndex != ItemIndex.None && inventory.GetItemCountEffective(zetAspectIndex) > 0) || inventory.GetActiveEquipment().equipmentIndex == aspectIndex);
            }
        }
    }
}
