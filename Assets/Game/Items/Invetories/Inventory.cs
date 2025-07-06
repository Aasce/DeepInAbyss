using Asce.Game.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Inventories
{
    /// <summary>
    ///     Represents an inventory system that holds a collection of items with a limited number of slots.
    /// </summary>
    [Serializable]
    public class Inventory
    {
        [SerializeField] protected int _slotCount = 0;
        [SerializeField] List<Item> _items = new();

        protected ReadOnlyCollection<Item> _readonlyItems;

        /// <summary> Called when the number of slots changes. </summary>
        public event Action<object, int> OnSlotCountChanged;

        /// <summary> Called when an item is added, removed, or changed. </summary>
        public event Action<object, ItemChangedEventArgs> OnItemChanged;

        /// <summary> Gets the current number of item slots in the inventory. </summary>
        public int SlotCount => _items.Count;

        /// <summary> Gets a read-only list of items currently in the inventory. </summary>
        public ReadOnlyCollection<Item> Items => _readonlyItems ??= _items.AsReadOnly();

        /// <summary> Initializes an empty inventory. </summary>
        public Inventory() { }

        /// <summary>
        ///     Initializes the inventory with a specific number of slots.
        /// </summary>
        /// <param name="slotCount"> The number of slots to initialize with. </param>
        public Inventory(int slotCount)
        {
            this.SetSlotCount(slotCount);
        }

        /// <summary>
        ///     Sets the number of slots and synchronizes the internal item list.
        /// </summary>
        /// <param name="slotCount"> The new slot count. </param>
        public virtual void SetSlotCount(int slotCount)
        {
            _slotCount = slotCount;
            this.SyncInventorySlots();
            OnSlotCountChanged?.Invoke(this, _slotCount);
        }

        public virtual Item GetItem(int index)
        {
            if (index < 0 || index >= _items.Count) return null;
            return _items[index];
        }

        /// <summary>
        ///     Tries to add the given item stack into the inventory.
        ///     Will add to existing stacks first, then to empty slots.
        /// </summary>
        /// <param name="addingItem"> The item stack to add. </param>
        /// <returns> The remaining stack that couldn't be added. </returns>
        public virtual ItemStack AddItem(ItemStack addingItem)
        {
            SO_ItemInformation itemInfo = addingItem.GetItemInfo();
            if (itemInfo == null) return addingItem;

            int quantityLeft = addingItem.Quantity;
            quantityLeft = AddToExistingStacks(itemInfo, quantityLeft); // Try to stack into existing items
            quantityLeft = AddToEmptySlots(itemInfo, quantityLeft); // Try to place into empty slots

            return new ItemStack(addingItem.Name, quantityLeft);
        }

        /// <summary>
        ///     Removes items at a specific index.
        /// </summary>
        /// <param name="index"> Slot index to remove from. </param>
        /// <param name="count"> Number of items to remove. If negative, removes all. </param>
        /// <returns> The removed item stack. </returns>
        public virtual ItemStack RemoveAt(int index, int count = -1)
        {
            if (index < 0 || index >= _items.Count)
                return new ItemStack(string.Empty, 0);

            Item item = _items[index];
            if (item == null || item.Information == null)
                return new ItemStack(string.Empty, 0);

            int currentQuantity = item.GetQuantity();
            int removedQuantity = (count < 0 || count >= currentQuantity) ? currentQuantity : count; // If count is negative, removes all.

            ItemStack removedStack = new(item.Information.Name, removedQuantity);

            // If removing all, clear the slot
            if (removedQuantity >= currentQuantity) _items[index] = null;
            else item.SetQuantity(currentQuantity - removedQuantity);

            OnItemChanged?.Invoke(this, new ItemChangedEventArgs(index));

            return removedStack;
        }

        /// <summary>
        ///     Attempts to merge two items if they are the same type and stackable.
        ///     If the target slot is empty, move the item. Otherwise, swap.
        /// </summary>
        /// <param name="fromIndex"> The source item index. </param>
        /// <param name="toIndex"> The target item index. </param>
        /// <returns> True if a merge, move, or swap occurred; false otherwise. </returns>
        public virtual bool SwapOrMerge(int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex ||
                fromIndex < 0 || fromIndex >= _items.Count ||
                toIndex < 0 || toIndex >= _items.Count)
                return false;

            Item fromItem = _items[fromIndex];
            Item toItem = _items[toIndex];

            if (fromItem == null || fromItem.Information == null)
                return false;

            // If toIndex is empty, move
            if (toItem == null || toItem.Information == null)
            {
                _items[toIndex] = fromItem;
                _items[fromIndex] = null;

                OnItemChanged?.Invoke(this, new ItemChangedEventArgs(fromIndex));
                OnItemChanged?.Invoke(this, new ItemChangedEventArgs(toIndex));
                return true;
            }

            // If same item type, try merge
            if (toItem.Information == fromItem.Information && fromItem.Information.GetMaxStack() > 1)
            {
                Merge(fromIndex, toIndex);
                return true;
            }

            // Otherwise swap
            Swap(fromIndex, toIndex);
            return true;
        }

        /// <summary>
        ///     Merges all similar items and sorts them to occupy fewer slots.
        /// </summary>
        public virtual void SortAndMerge()
        {
            Dictionary<SO_ItemInformation, int> merged = new();

            // Count total quantities for each item type
            foreach (Item item in _items)
            {
                if (item == null || item.Information == null) continue;

                if (!merged.ContainsKey(item.Information))
                    merged[item.Information] = item.GetQuantity();
                else
                    merged[item.Information] += item.GetQuantity();
            }

            // Clear inventory
            for (int i = 0; i < _items.Count; i++)
                _items[i] = null;

            int slotIndex = 0;

            // Distribute merged items into inventory
            foreach (KeyValuePair<SO_ItemInformation, int> pair in merged)
            {
                SO_ItemInformation info = pair.Key;
                int totalQuantity = pair.Value;
                int maxStack = info.GetMaxStack();

                while (totalQuantity > 0 && slotIndex < _items.Count)
                {
                    int stackQuantity = Math.Min(totalQuantity, maxStack);
                    _items[slotIndex] = new Item(info);
                    _items[slotIndex].SetQuantity(stackQuantity);

                    OnItemChanged?.Invoke(this, new ItemChangedEventArgs(slotIndex));

                    totalQuantity -= stackQuantity;
                    slotIndex++;
                }
            }
        }

        /// <summary>
        ///     Finds the first empty or null slot in the inventory.
        /// </summary>
        /// <returns> Index of the first empty slot, or -1 if none are available. </returns>
        public virtual int GetFirstEmptySlotIndex() => _items.FindIndex(item => item == null || item.Information == null);

        /// <summary>
        ///     Check slot at <paramref name="index"/> is empty or not.
        /// </summary>
        /// <param name="index"> Slot index to be check. </param>
        /// <returns> Returns true if item at Slot <paramref name="index"/> is null or item.Information is null. </returns>
        public virtual bool IsEmptyAt(int index) => _items[index] == null || _items[index].Information == null;
        

        /// <summary>
        ///     Synchronizes the internal item list with the current slot count.
        /// </summary>
        public virtual void SyncInventorySlots()
        {
            _items ??= new List<Item>(_slotCount);

            int diff = _slotCount - _items.Count;
            if (diff > 0)
            {
                // Add empty slots
                for (int i = 0; i < diff; i++)
                    _items.Add(null);
            }
            else if (diff < 0)
            {
                // Remove extra slots
                _items.RemoveRange(_slotCount, -diff);
            }
        }

        /// <summary>
        ///     Attempts to stack items into existing stacks of the same type.
        /// </summary>
        /// <param name="itemInfo"> Item information object. </param>
        /// <param name="quantityLeft"> Quantity to try adding. </param>
        /// <returns> Remaining quantity that could not be stacked. </returns>
        protected int AddToExistingStacks(SO_ItemInformation itemInfo, int quantityLeft)
        {
            int maxStack = itemInfo.GetMaxStack();

            for (int i = 0; i < _items.Count; i++)
            {
                Item item = _items[i];
                if (item == null || item.Information == null) continue;
                if (item.Information != itemInfo) continue;

                int currentQuantity = item.GetQuantity();
                int spaceLeft = maxStack - currentQuantity;
                if (spaceLeft <= 0) continue;

                int addedQuantity = Math.Min(spaceLeft, quantityLeft);
                item.SetQuantity(currentQuantity + addedQuantity);
                quantityLeft -= addedQuantity;

                OnItemChanged?.Invoke(this, new ItemChangedEventArgs(i));

                if (quantityLeft == 0)
                    break;
            }

            return quantityLeft;
        }

        /// <summary>
        ///     Attempts to add remaining items into empty inventory slots.
        /// </summary>
        /// <param name="itemInfo"> Item information object. </param>
        /// <param name="quantityLeft"> Quantity to try adding. </param>
        /// <returns> Remaining quantity that could not be added to inventory. </returns>
        protected int AddToEmptySlots(SO_ItemInformation itemInfo, int quantityLeft)
        {
            int maxStack = itemInfo.GetMaxStack();

            for (int i = 0; i < _items.Count && quantityLeft > 0; i++)
            {
                if (this.IsEmptyAt(i))
                {
                    int stackQuantity = Math.Min(maxStack, quantityLeft);
                    _items[i] = new Item(itemInfo);
                    _items[i].SetQuantity(stackQuantity);
                    quantityLeft -= stackQuantity;

                    OnItemChanged?.Invoke(this, new ItemChangedEventArgs(i));
                }
            }

            return quantityLeft;
        }

        /// <summary>
        ///     Swaps two items at the given indices.
        /// </summary>
        protected virtual void Swap(int fromIndex, int toIndex)
        {
            // Swap
            (_items[fromIndex], _items[toIndex]) = (_items[toIndex], _items[fromIndex]);

            OnItemChanged?.Invoke(this, new ItemChangedEventArgs(fromIndex));
            OnItemChanged?.Invoke(this, new ItemChangedEventArgs(toIndex));
        }

        /// <summary>
        ///     Merges items from fromIndex to toIndex if possible. Transfers as much quantity as possible.
        ///     Leaves remaining in fromIndex if not fully transferred.
        /// </summary>
        protected virtual void Merge(int fromIndex, int toIndex)
        {
            if (this.IsEmptyAt(fromIndex) || this.IsEmptyAt(toIndex)) return;

            Item fromItem = _items[fromIndex];
            Item toItem = _items[toIndex];

            if (fromItem.Information != toItem.Information) return;

            int maxStack = toItem.Information.GetMaxStack();
            int toQuantity = toItem.GetQuantity();
            int fromQuantity = fromItem.GetQuantity();

            int space = maxStack - toQuantity;
            if (space <= 0) return; // toIndex is full

            int transfer = Math.Min(space, fromQuantity); // Amount to merge
            toItem.SetQuantity(toQuantity + transfer);
            fromItem.SetQuantity(fromQuantity - transfer);

            // Remove Item if quantity is zero
            if (fromItem.GetQuantity() <= 0) _items[fromIndex] = null;

            OnItemChanged?.Invoke(this, new ItemChangedEventArgs(fromIndex));
            OnItemChanged?.Invoke(this, new ItemChangedEventArgs(toIndex));
        }

    }
}
