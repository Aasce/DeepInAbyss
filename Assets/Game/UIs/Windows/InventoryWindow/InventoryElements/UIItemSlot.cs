using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using UnityEngine;

namespace Asce.Game.UIs.Inventories
{
    /// <summary>
    ///     Represents a single slot in the inventory UI that can hold a UIItem.
    /// </summary>
    public class UIItemSlot : UIObject
    {
        // References
        [SerializeField, Readonly] protected UIInventory _inventory;
        [SerializeField] protected UIItem _uiItem;

        // Index of this slot in the inventory
        [SerializeField, Readonly] protected int _index = -1;

        /// <summary>
        ///     Gets or sets the parent inventory UI that this slot belongs to.
        /// </summary>
        public UIInventory Inventory
        {
            get => _inventory;
            set => _inventory = value;
        }

        /// <summary>
        ///     Gets or sets the index of this slot in the inventory.
        /// </summary>
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        /// <summary>
        ///     Gets the UIItem currently assigned to this slot.
        /// </summary>
        public UIItem Item => _uiItem;

        /// <summary>
        ///     Checks if this slot currently contains a <see cref="UIItem"/>.
        /// </summary>
        public bool IsContainItem => _uiItem != null;

        /// <summary>
        ///     Assigns a UIItem to this slot and re-parents its RectTransform.
        /// </summary>
        /// <param name="uiItem"> The item to assign to this slot. </param>
        public virtual void SetItem(UIItem uiItem)
        {
            if (_uiItem == uiItem) return;
            if (_uiItem != null) _uiItem.UISlot = null;

            _uiItem = uiItem;
            if (_uiItem == null) return;

            // Re-parent the item's RectTransform to this slot
            _uiItem.RectTransform.SetParent(this.RectTransform);
            _uiItem.UISlot = this;
        }
    }
}
