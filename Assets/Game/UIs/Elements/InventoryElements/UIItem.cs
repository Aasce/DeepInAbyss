using Asce.Game.Items;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.Inventories
{
    /// <summary>
    ///     Represents a UI element for a single item, including drag-and-drop behavior and property display.
    /// </summary>
    public class UIItem : UIObject, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // UI references
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected Image _icon;
        [SerializeField] protected TextMeshProUGUI _quantity;
        [SerializeField] protected Slider _durability;

        // Ref
        [SerializeField, Readonly] protected UIItemSlot _uiSlot;
        protected Item _item;

        protected bool _isDragging = false;

        public CanvasGroup CanvasGroup => _canvasGroup;

        /// <summary> Gets or sets the UI slot this item belongs to.  </summary>
        public UIItemSlot UISlot
        {
            get => _uiSlot;
            set => _uiSlot = value;
        }
        public Item Item => _item;

        public bool IsDragging
        {
            get => _isDragging;
            set => _isDragging = value;
        }

        /// <summary>
        ///     Sets the visual state of this item using the provided data.
        /// </summary>
        /// <param name="item"> The item to represent. </param>
        public virtual void SetItem(Item item)
        {
            this.Unregister();
            _item = item;
            this.Register();
            
        }

        protected virtual void Register()
        {
            if (_item.IsNull())
            {
                this.Clear();
                return;
            }

            this.SetIcon();
            this.SetQuantity();
            this.SetDurability();
        }

        protected virtual void Unregister()
        {
            if (_item.IsNull()) return;

            DurabilityPropertyData durability = _item.GetProperty<DurabilityPropertyData>(ItemPropertyType.Durabilityable);
            if (durability != null) durability.OnDurabilityChanged -= Item_OnDurabilityChanged;
        }

        /// <summary>
        ///     Hides all visuals when the item is null or invalid.
        /// </summary>
        public virtual void Clear()
        {
            if (_icon != null) _icon.gameObject.SetActive(false);
            if (_quantity != null) _quantity.gameObject.SetActive(false);
            if (_durability != null) _durability.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Updates the icon image to represent the item.
        /// </summary>
        protected virtual void SetIcon()
        {
            if (_icon == null) return;

            _icon.gameObject.SetActive(true);
            _icon.sprite = _item.Information.Icon;
        }

        /// <summary>
        ///     Updates the quantity display for stackable items.
        /// </summary>
        protected virtual void SetQuantity()
        {
            if (_quantity == null) return;

            int quantity = _item.GetQuantity();
            if (quantity <= 1)
            {
                _quantity.gameObject.SetActive(false);
                return;
            }

            _quantity.gameObject.SetActive(true);
            _quantity.text = quantity.ToString();
        }

        /// <summary>
        ///     Updates the durability slider if the item supports durability.
        /// </summary>
        protected virtual void SetDurability()
        {
            if (_durability == null) return;

            float maxDurability = _item.Information.GetMaxDurability();
            if (maxDurability == 0)
            {
                _durability.gameObject.SetActive(false);
            }
            else
            {
                _durability.gameObject.SetActive(true);
                _durability.maxValue = maxDurability;
                _durability.value = _item.GetDurability();
            }
            DurabilityPropertyData durability = _item.GetProperty<DurabilityPropertyData>(ItemPropertyType.Durabilityable);
            if (durability != null) durability.OnDurabilityChanged += Item_OnDurabilityChanged;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_uiSlot == null) return;
            _uiSlot.Focus(eventData);
        }

        protected virtual void Item_OnDurabilityChanged(object sender, ValueChangedEventArgs<float> args)
        {
            _durability.value = args.NewValue;
        }

        /// <summary>
        ///     Called when the user begins dragging this item.
        /// </summary>
        /// <param name="eventData"> Pointer data associated with the drag event. </param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_uiSlot == null || _uiSlot.Inventory == null) return;
            if (_uiSlot.Inventory != null) _uiSlot.Inventory.BeginDragItem(this, eventData);
        }

        /// <summary>
        ///     Called during the drag; updates position of the dragged item.
        /// </summary>
        /// <param name="eventData"> Pointer drag data. </param>
        public void OnDrag(PointerEventData eventData)
        {
            if (!IsDragging) return;
            transform.position = eventData.position;
        }

        /// <summary>
        ///     Called when the user releases the drag. Handles swapping items or resetting position.
        /// </summary>
        /// <param name="eventData"> Pointer data when drag ends. </param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_uiSlot == null || _uiSlot.Inventory == null) return;
            if (_uiSlot.Inventory != null) _uiSlot.Inventory.EndDragItem(this, eventData);
        }
    }
}
