using System;
using System.Collections.Generic;
using System.Linq;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;

namespace LMSPFikaFix
{
    // Every Player keeps the armor it wears in _preAllocatedArmorComponents: hit
    // detection, penetration, durability loss and the plate colliders all read it.
    //
    // The game rebuilds that list at the start of every hit. Fika doesn't: it rebuilds
    // it only when an armor equipment slot (Headwear, FaceCover, Eyewear, TacticalVest,
    // ArmorVest) gets a different item. A plate taken out of, or put into, armor that
    // stays worn doesn't change the slot's item, so the list keeps the old plate. That
    // plate then takes the hits with its own class and durability, wherever it is now,
    // and the new one protects nothing.
    //
    // This rebuilds the list when an item with armor enters or leaves worn armor.
    internal static class WornArmor
    {
        private static readonly AccessTools.FieldRef<Player, List<ArmorComponent>> Cache =
            AccessTools.FieldRefAccess<Player, List<ArmorComponent>>("_preAllocatedArmorComponents");

        internal static void OnItemMoved(Player player, Item item, ItemAddress location, bool added)
        {
            try
            {
                if (player == null || item == null || location == null)
                {
                    return;
                }

                InventoryEquipment equipment = player.InventoryController?.Inventory?.Equipment;
                if (equipment == null || !IsInsideArmorSlot(equipment, location))
                {
                    return;
                }

                if (!item.GetItemComponentsInChildren<ArmorComponent>().Any())
                {
                    return;
                }

                List<ArmorComponent> worn = Cache(player);
                worn.Clear();
                player.Inventory.GetPutOnArmorsNonAlloc(worn);
                // Re-enables the plate colliders of the new set, and updates the
                // armor's ergonomics and speed penalties.
                player.RecalculateEquipmentParams();

                Plugin.LogSource.LogInfo(
                    $"{player.Profile?.Nickname}: {item.LocalizedShortName()} {(added ? "put in" : "taken out of")} " +
                    $"{Where(location)}, worn armor is now: {Describe(worn)}");
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"Could not refresh worn armor after moving {item?.LocalizedShortName()}: {ex}");
            }
        }

        // Same test as the game's Player.OnItemAddedOrRemoved: follow the containers up
        // through slots only (a grid stops the walk, so a plate in a rig's pocket isn't
        // worn) and look for an armor equipment slot.
        private static bool IsInsideArmorSlot(InventoryEquipment equipment, ItemAddress location)
        {
            foreach (ItemAddress address in location.GetAllParentLocations(onlyMerged: true))
            {
                foreach (EquipmentSlot slotName in Inventory.ArmorSlots)
                {
                    if (address.Container == equipment.GetSlot(slotName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static string Where(ItemAddress location)
        {
            IContainer container = location.Container;
            if (container?.ParentItem is InventoryEquipment)
            {
                return $"the {container.ID} slot";
            }
            return $"{container?.ParentItem?.LocalizedShortName()} ({container?.ID})";
        }

        private static string Describe(List<ArmorComponent> worn)
        {
            if (worn.Count == 0)
            {
                return "none";
            }
            return string.Join(", ", worn.Select(armor =>
                $"{armor.Item.LocalizedShortName()} (class {armor.ArmorClass}, " +
                $"{armor.Repairable.Durability:0.#}/{armor.Repairable.MaxDurability:0.#})"));
        }
    }
}
