using Asce.Game.Entities;
using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Managers.Attributes;
using Asce.Managers.Pools;
using Asce.Managers.UIs;
using UnityEngine;

namespace Asce.Game.UIs.Inventories
{
    /// <summary>
    ///     Represents the visual UI panel for displaying and interacting with a creature's inventory.
    /// </summary>
    public class UIInventory : UIObject
    {
        // Reference to the item information display panel (tooltip/details panel)
        [SerializeField, Readonly] protected UIItemInformation _itemInformation;

        [Space]

        // Pools used for efficient management of UI elements
        [SerializeField] protected Pool<UIItemSlot> _slotsPool = new();
        [SerializeField] protected Pool<UIItem> _itemsPool = new();

        // The associated creature's inventory controller
        protected CreatureInventory _inventoryController;

        /// <summary>
        ///     Unity Start method, optionally overridden for initialization.
        /// </summary>
        protected virtual void Start() { }

        /// <summary>
        ///     Sets the inventory to display in this UI panel.
        /// </summary>
        /// <param name="creatureInventory"> The creature inventory to bind to. </param>
        public virtual void SetInventory(CreatureInventory creatureInventory)
        {
            if (_inventoryController == creatureInventory) return;

            this.Unregister();
            _inventoryController = creatureInventory;
            this.Register();
        }

        /// <summary>
        ///     Gets the slot UI element associated with a specific inventory index.
        /// </summary>
        /// <param name="index"> The inventory slot index. </param>
        /// <returns> The matching UIItemSlot, or null if not found. </returns>
        public virtual UIItemSlot GetSlotByIndex(int index) =>
            _slotsPool.Activities.Find(slot => slot.Index == index);

        /// <summary>
        ///     Attempts to move or merge items between two slots in the inventory.
        /// </summary>
        /// <param name="fromIndex"> The source slot index. </param>
        /// <param name="toIndex">   The target slot index. </param>
        public virtual void MoveItem(int fromIndex, int toIndex)
        {
            if (_inventoryController == null) return;

            Inventory inventory = _inventoryController.Inventory;
            if (fromIndex == toIndex) return;

            inventory.SwapOrMerge(fromIndex, toIndex);
        }

        /// <summary>
        ///     Registers to inventory events and loads current data into the UI.
        /// </summary>
        public virtual void Register()
        {
            if (_inventoryController == null) return;

            this.LoadInventory();
            _inventoryController.Inventory.OnItemChanged += Inventory_OnItemChanged;
        }

        /// <summary>
        ///     Unregisters from inventory events and clears the UI.
        /// </summary>
        public virtual void Unregister()
        {
            if (_inventoryController == null) return;

            _itemsPool.Clear(isDeactive: true);
            _slotsPool.Clear(isDeactive: true, slot => slot.SetItem(null));

            _inventoryController.Inventory.OnItemChanged -= Inventory_OnItemChanged;
        }

        /// <summary>
        ///     Creates and initializes UI slots based on current inventory size.
        /// </summary>
        protected virtual void LoadInventory()
        {
            Inventory inventory = _inventoryController.Inventory;

            for (int i = 0; i < inventory.SlotCount; i++)
            {
                this.CreateSlot(i);
            }
        }

        /// <summary>
        ///     Called when the inventory updates a specific slot.
        /// </summary>
        /// <param name="sender"> The inventory object (unused). </param>
        /// <param name="args">   Event arguments containing the slot index. </param>
        protected virtual void Inventory_OnItemChanged(object sender, ItemChangedEventArgs args)
        {
            this.UpdateSlot(args.Index);
        }

        /// <summary>
        ///     Creates and initializes a new slot at the specified index.
        /// </summary>
        /// <param name="index"> The index of the inventory slot. </param>
        protected virtual void CreateSlot(int index)
        {
            UIItemSlot uiSlot = _slotsPool.Activate();
            if (uiSlot == null) return;

            uiSlot.Inventory = this;
            uiSlot.Index = index;
            uiSlot.transform.SetAsLastSibling();

            this.UpdateSlot(index);
        }

        /// <summary>
        ///     Updates the UI for a given inventory slot index.
        ///     Adds, removes, or updates the item displayed in the UI slot.
        /// </summary>
        /// <param name="index"> The inventory index to update. </param>
        protected virtual void UpdateSlot(int index)
        {
            UIItemSlot uiSlot = GetSlotByIndex(index);
            if (uiSlot == null) return;

            Item item = _inventoryController.Inventory.GetItem(index);

            if (item == null)
            {
                if (!uiSlot.IsContainItem) return;

                // Remove current item if exists
                _itemsPool.Deactivate(uiSlot.Item);
                uiSlot.SetItem(null);
                return;
            }

            if (!uiSlot.IsContainItem)
            {
                // Activate new UI item and bind
                UIItem uiItem = _itemsPool.Activate();
                if (uiItem == null)
                {
                    uiSlot.SetItem(null);
                    return;
                }

                uiItem.SetItem(item);
                uiSlot.SetItem(uiItem);
            }
            else
            {
                // Update existing UI item
                uiSlot.Item.SetItem(item);
            }
        }
    }
}
