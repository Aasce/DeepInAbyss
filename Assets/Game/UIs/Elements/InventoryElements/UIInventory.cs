using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Game.UIs.ContextMenus;
using Asce.Managers.Pools;
using Asce.Managers.UIs;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.Inventories
{
    /// <summary>
    ///     Represents the visual UI panel for displaying and interacting with a creature's inventory.
    /// </summary>
    public class UIInventory : UIObject
    {
        // Pools used for efficient management of UI elements
        [SerializeField] protected Pool<UIItemSlot> _slotsPool = new();
        [SerializeField] protected Pool<UIItem> _itemsPool = new();

        // The associated creature's inventory controller
        protected IInventoryController _inventoryController;
        protected UIItemContextMenu _itemContextMenu;

        protected bool _isSplit = false;
        protected int _quantityToSplit = -1;

        public event Action<object, int> OnFocusAt;


        public IInventoryController Controller => _inventoryController;


        protected override void RefReset()
        {
            base.RefReset();
        }


        protected virtual void Start() 
        {
            _itemContextMenu = UIScreenCanvasManager.Instance.ContextMenusController.GetMenu<UIItemContextMenu>();

            if (_itemContextMenu != null)
            {
                this.OnHide += (sender) => _itemContextMenu.Hide();
            }
        }


        /// <summary>
        ///     Sets the inventory to display in this UI panel.
        /// </summary>
        /// <param name="creatureInventory"> The creature inventory to bind to. </param>
        public virtual void SetInventory(IInventoryController creatureInventory)
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

        public virtual void FocusAt(int index)
        {
            if (_inventoryController == null) return;

            Inventory inventory = _inventoryController.Inventory;
            Item item = inventory.GetItem(index);

            if (_itemContextMenu != null && _itemContextMenu.Index != index)
            {
                _itemContextMenu.Hide();
            }

            OnFocusAt?.Invoke(this, index);
        }

        public virtual void ShowMenuContextAt(int index)
        {
            if (_itemContextMenu == null) return;

            UIItemSlot slot = GetSlotByIndex(index);
            if (slot == null) return;

            Inventory inventory = _inventoryController.Inventory;
            Item item = inventory.GetItem(index);
            if (item.IsNull())
            {
                _itemContextMenu.Index = -1;
                _itemContextMenu.Hide();
                return;
            }

            Vector2 position = slot.RectTransform.position;
            position.x -= slot.RectTransform.sizeDelta.x * 0.5f;
            position.y += slot.RectTransform.sizeDelta.y * 0.5f;
            _itemContextMenu.RectTransform.position = position;

            _itemContextMenu.Index = index;
            _itemContextMenu.Set(item);
            _itemContextMenu.Show();
        }

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

        public virtual void DropItemAt(int index, int quantity = -1)
        {
            if (_inventoryController == null) return;
            _inventoryController.Drop(index, quantity);
        }

        /// <summary>
        ///     Handles drag-and-drop logic and UI updates for inventory interactions.
        /// </summary>
        public virtual void BeginDragItem(UIItem sender, PointerEventData eventData)
        {
            if (sender.UISlot == null || eventData.button == PointerEventData.InputButton.Right)
                return;

            Item item = sender.Item;
            if (item.IsNull()) return;

            // Elevate UIItem in the hierarchy
            sender.transform.SetParent(UIScreenCanvasManager.Instance.Canvas.transform);
            sender.CanvasGroup.blocksRaycasts = false;
            sender.IsDragging = true;

            // Determine if the item is being split
            _quantityToSplit = _itemContextMenu != null ? _itemContextMenu.QuantityToSplit : -1;
            _isSplit = item.HasQuantity() && _itemContextMenu != null && _itemContextMenu.IsShow && _quantityToSplit != -1;

            if (!_isSplit) return;

            (Item first, Item second) = item.Split(_quantityToSplit);
            sender.SetItem(second);

            if (sender.UISlot != null)
            {
                sender.UISlot.SetItem(first.IsNull() ? null : CreateSplitItem(first));
            }

            if (_itemContextMenu != null) _itemContextMenu.Hide();
        }

        /// <summary>
        ///     Handles the logic when an item drag operation ends.
        ///     Determines whether to move, drop, or revert the item.
        /// </summary>
        public virtual void EndDragItem(UIItem sender, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                return;

            sender.IsDragging = false;
            sender.CanvasGroup.blocksRaycasts = true;

            GameObject target = eventData.pointerEnter;
            UIItemSlot targetSlot = target != null ? target.GetComponentInParent<UIItemSlot>() : null;
            UIItemSlot originSlot = sender.UISlot;

            int fromIndex = originSlot != null ? originSlot.Index : -1;
            int toIndex = targetSlot != null ? targetSlot.Index : -1;

            if (_isSplit) HandleSplitEndDrag(sender, fromIndex, toIndex, target, originSlot, targetSlot);
            else HandleNormalEndDrag(sender, fromIndex, target, originSlot, targetSlot);            

            if (_itemContextMenu != null) _itemContextMenu.Hide();
        }

        /// <summary>
        ///     Registers to inventory events and loads current data into the UI.
        /// </summary>
        protected virtual void Register()
        {
            if (_inventoryController == null) return;

            this.LoadInventory();
            _inventoryController.Inventory.OnItemChanged += Inventory_OnItemChanged;
        }

        /// <summary>
        ///     Unregisters from inventory events and clears the UI.
        /// </summary>
        protected virtual void Unregister()
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

                uiItem.Inventory = this;
                uiItem.SetItem(item);
                uiSlot.SetItem(uiItem);
            }
            else
            {
                // Update existing UI item
                uiSlot.Item.SetItem(item);
                uiSlot.ResetItemPosition();
            }
        }

        /// <summary>
        ///     Creates a UI item for a split operation and binds it to the inventory.
        /// </summary>
        /// <param name="item">The item to be displayed in the new UI element.</param>
        /// <returns>The created UIItem or null if activation failed.</returns>
        private UIItem CreateSplitItem(Item item)
        {
            UIItem newUIItem = _itemsPool.Activate();
            if (newUIItem != null)
            {
                newUIItem.Inventory = this;
                newUIItem.SetItem(item);
            }
            return newUIItem;
        }

        /// <summary>
        ///     Handles item drag end logic when a split operation was active.
        /// </summary>
        /// <param name="sender">The UIItem that was being dragged.</param>
        /// <param name="fromIndex">The original slot index.</param>
        /// <param name="toIndex">The target slot index.</param>
        /// <param name="target">The GameObject under the pointer.</param>
        /// <param name="originSlot">The slot the item came from.</param>
        /// <param name="targetSlot">The slot the item was dropped on.</param>
        private void HandleSplitEndDrag(
            UIItem sender,
            int fromIndex,
            int toIndex,
            GameObject target,
            UIItemSlot originSlot,
            UIItemSlot targetSlot)
        {
            // Dropped outside of any slot -> drop the item to the ground
            if (target == null)
            {
                DropItemAt(fromIndex, _quantityToSplit);
                _itemsPool.Deactivate(sender);
                return;
            }

            // Invalid drop target -> return the item to the original slot
            if (targetSlot == null)
            {
                if (originSlot != null) _itemsPool.Deactivate(sender);
                UpdateSlot(fromIndex);
                return;
            }

            // Dropped into another inventory
            if (targetSlot.Inventory != null && targetSlot.Inventory != this)
            {
                _itemsPool.Deactivate(sender);
                InventorySystem.MoveItem(
                    _inventoryController.Inventory,
                    targetSlot.Inventory.Controller.Inventory,
                    fromIndex,
                    targetSlot.Index,
                    _quantityToSplit);
                return;
            }

            // Dropped into another slot within the same inventory
            if (originSlot != null && targetSlot != originSlot)
            {
                _itemsPool.Deactivate(sender);
                if (_inventoryController != null)
                {
                    _inventoryController.Inventory.Split(fromIndex, _quantityToSplit, toIndex);
                }
                return;
            }

            _itemsPool.Deactivate(sender);
            UpdateSlot(fromIndex);
        }


        /// <summary>
        ///     Handles item drag end logic when the item was not split (normal move).
        /// </summary>
        /// <param name="sender">The UIItem that was being dragged.</param>
        /// <param name="fromIndex">The original slot index.</param>
        /// <param name="target">The GameObject under the pointer.</param>
        /// <param name="originSlot">The slot the item came from.</param>
        /// <param name="targetSlot">The slot the item was dropped on.</param>
        private void HandleNormalEndDrag(
            UIItem sender,
            int fromIndex,
            GameObject target,
            UIItemSlot originSlot,
            UIItemSlot targetSlot)
        {
            // Dropped outside of any slot -> drop the item to the ground
            if (target == null)
            {
                DropItemAt(fromIndex);
                return;
            }

            // Invalid drop target -> return the item to the original slot
            if (targetSlot == null)
            {
                if (originSlot != null) originSlot.ResetItemPosition();
                return;
            }

            // Dropped into another inventory
            if (targetSlot.Inventory != null && targetSlot.Inventory != this)
            {
                originSlot.ResetItemPosition();
                InventorySystem.MoveItem(
                    _inventoryController.Inventory,
                    targetSlot.Inventory.Controller.Inventory,
                    fromIndex,
                    targetSlot.Index);

                return;
            }

            // Dropped into another slot within the same inventory
            if (originSlot != null && targetSlot != originSlot)
            {
                MoveItem(originSlot.Index, targetSlot.Index);
                originSlot.ResetItemPosition();
            }
        }

        /// <summary>
        ///     Called when the inventory updates a specific slot.
        /// </summary>
        /// <param name="sender"> The inventory object (unused). </param>
        /// <param name="index">   Event arguments containing the slot index. </param>
        protected virtual void Inventory_OnItemChanged(object sender, int index)
        {
            this.UpdateSlot(index);
        }

    }
}
