using Asce.Game.Items;
using System;

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
                if (remaining.IsNull()) continue; // If nothing remains, item was fully looted

                int lootedQuantity = item.GetQuantity() - remaining.GetQuantity();
                if (lootedQuantity > 0)
                {
                    // Remove the quantity that was looted
                    source.RemoveAt(i, lootedQuantity); // Auto remove if loot all
                }
            }
        }
    }
}
