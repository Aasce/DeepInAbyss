using Asce.Game.Equipments;
using Asce.Game.Items;
using System;
using UnityEngine;

namespace Asce.Game.Inventories
{
    public static class InventorySystem
    {
        public static void MoveItem(Inventory source, Inventory target, int sourceItemIndex, int targetItemIndex, int quantity = -1)
        {
            if (source == null || target == null) return;
            if (sourceItemIndex < 0 || sourceItemIndex >= source.SlotCount) return;
            if (targetItemIndex < 0 || targetItemIndex >= target.SlotCount) return;

            Item sourceItem = source.GetItem(sourceItemIndex);
            if (sourceItem.IsNull()) return;

            int actualQuantity = quantity > 0
                ? Math.Min(quantity, sourceItem.GetQuantity())
                : sourceItem.GetQuantity();

            if (actualQuantity <= 0) return;

            // Clone the source item to transfer
            Item transferItem = sourceItem.Clone();
            transferItem.SetQuantity(actualQuantity);

            // Try to add into target
            Item remaining = target.AddAt(transferItem, targetItemIndex);

            int removedQuantity = actualQuantity - (remaining?.GetQuantity() ?? 0);
            if (removedQuantity > 0)
            {
                // Remove transferred part from source
                source.RemoveAt(sourceItemIndex, removedQuantity);
            }
        }


        public static void LootAll(Inventory source, Inventory target)
        {
            if (source == null || target == null) return;

            for (int i = 0; i < source.SlotCount; i++)
            {
                Item item = source.GetItem(i);
                if (item.IsNull()) continue;

                // Attempt to add to target
                Item remaining = target.AddItem(item);
                if (remaining.IsNull()) // If nothing remains, item was fully looted
                {
                    source.RemoveAt(i); // Auto remove if loot all
                    continue;
                }

                int lootedQuantity = item.GetQuantity() - remaining.GetQuantity();
                if (lootedQuantity > 0)
                {
                    source.RemoveAt(i, lootedQuantity);
                }
            }
        }

        public static void MoveItemToEquipment(Inventory inventory, IEquipmentController equipment, int index)
        {
            if (inventory == null || equipment == null) return;
            if (!inventory.IsValidIndex(index)) return;

            Item item = inventory.GetItem(index);
            if (item.IsNull()) return;
            if (!item.Information.HasProperty(ItemPropertyType.Equippable)) return;

            if (item.Information.GetPropertyByType(ItemPropertyType.Equippable) is not EquippableItemProperty equippableProperty) return;

            EquipmentType type = equippableProperty.EquipmentType;
            if (type == EquipmentType.None) return;

            EquipmentSlot slot = equipment.GetSlot(type);
            if (slot == null) return;

            Item equipmentItem = slot.RemoveEquipment();
            slot.AddEquipment(item);
            inventory.RemoveAt(index);
            if (!equipmentItem.IsNull())
            {
                inventory.AddAt(equipmentItem, index);
            }
        }

        public static void MoveEquipmentToInventory(IEquipmentController equipment, Inventory inventory, EquipmentType type, int toIndex)
        {
            if (equipment == null || inventory == null) return;
            if (!inventory.IsValidIndex(toIndex)) return;

            EquipmentSlot slot = equipment.GetSlot(type);
            if (slot == null) return;

            Item item = slot.EquipmentItem;
            if (item.IsNull()) return;

            Item remaining = inventory.AddAt(item, toIndex);
            if (remaining.IsNull())
            {
                slot.RemoveEquipment();
            }
        }

        public static void MoveEquipmentToInventory(IEquipmentController equipment, Inventory inventory, EquipmentType type)
        {
            if (inventory == null || equipment == null) return;
            EquipmentSlot slot = equipment.GetSlot(type);
            if (slot == null) return;

            Item item = slot.EquipmentItem;
            if (item.IsNull()) return;

            Item remaining = inventory.AddItem(item);
            if (remaining.IsNull())
            {
                slot.RemoveEquipment();
            }
        }


        public static bool Buy(Inventory inventory, ShopItem buyItem, ShopItemCost cost)
        {
            if (inventory == null) return false;
            if (buyItem == null) return false;
            if (cost == null) return false;

            bool isEnough = inventory.ContainsWithQuantity(cost.CostType, cost.Cost);
            if (!isEnough) return false;

            Item addingItem = new(buyItem.Item);
            addingItem.SetQuantity(buyItem.Quantity);
            addingItem.SetDurability(addingItem.Information.GetMaxDurability());
            if (inventory.WouldItemOverflow(addingItem)) return false;

            inventory.AddItem(addingItem);
            inventory.RemoveWithQuantity(cost.CostType, cost.Cost);
            return true;
        }

        public static bool UseItemAt(Inventory inventory, int index, UseEventArgs args)
        {
            if (inventory == null) return false;
            if (args == null) return false;

            Item item = inventory.GetItem(index);
            if (item.IsNull()) return false;

            if (item.Information.GetPropertyByType(ItemPropertyType.Usable) is not UsableItemProperty property) return false;
            if (property.UseEvent == null) return false;
            
            switch (property.CostType)
            {
                case ItemUseCostType.RemoveQuantity:
                    inventory.RemoveAt(index, property.QuantityCost);
                    break;

                case ItemUseCostType.DeductDurability:
                    if (!item.HasDurability()) return false;

                    float resultDurability = item.GetDurability() - property.DurabilityCost;
                    if (resultDurability < 0) inventory.RemoveAt(index, 1);
                    else item.SetDurability(resultDurability);
                    break;

                case ItemUseCostType.None:
                default:
                    break;
            }

            bool isUseSuccess = property.UseEvent.OnUse(item, args);
            if (!isUseSuccess) return false;

            return true;
        }
    }
}
