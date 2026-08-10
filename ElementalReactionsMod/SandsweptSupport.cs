using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using RoR2;
using Sandswept;
using ElementalReactionsMod.Items;
using ElementalReactionsMod.Elements;

namespace ElementalReactionsMod
{
    public static class SandsweptSupport
    {
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void Initialize()
        {
            PickupCatalog.availability.onAvailable += () =>
            {
                Crafting.AppendDelusionRecipe(Crafting.delusionPyro, ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("ITEM_SANDSWEPT_SUN_FRAGMENT")));

                Crafting.AppendDelusionRecipe(Crafting.delusionPyro, ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("ITEM_SANDSWEPT_SMOULDERING_DOCUMENT")));

                Crafting.AppendDelusionRecipe(Crafting.delusionGeo, ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("ITEM_SANDSWEPT_CROWNS_DIAMOND")));
            };
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void AddElementToCannonballJellyfishDeath()
        {
            if (Sandswept.Enemies.CannonballJellyfish.CannonballJellyfish.JellyCoreProjectile && 
                Sandswept.Enemies.CannonballJellyfish.CannonballJellyfish.JellyCoreProjectile.TryGetComponent<RoR2.Projectile.ProjectileDamage>(out var jellyDamage))
            {
                jellyDamage.damageType.SetElement(DefaultElementDefs.pyroElement.index);
            }
            if (Sandswept.Drones.Inferno.InfernoDrone.SigmaProjectile2 &&
                Sandswept.Drones.Inferno.InfernoDrone.SigmaProjectile2.TryGetComponent<RoR2.Projectile.ProjectileDamage>(out var infernoDamage))
            {
                infernoDamage.damageType.SetElement(DefaultElementDefs.pyroElement.index);
            }
            if (Sandswept.Drones.Voltaic.VoltaicDrone.SpikeProjectile &&
                Sandswept.Drones.Voltaic.VoltaicDrone.SpikeProjectile.TryGetComponent<RoR2.Projectile.ProjectileDamage>(out var voltaicDamage))
            {
                voltaicDamage.damageType.SetElement(DefaultElementDefs.electroElement.index);
            }
        }
    }
}
