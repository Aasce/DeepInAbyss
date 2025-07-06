using Asce.Game.Items;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.Inventories
{
    /// <summary>
    ///     Represents a UI element for a single item, including drag-and-drop behavior and property display.
    /// </summary>
    public class UIItem : UIObject, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // UI references
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected Image _icon;
        [SerializeField] protected TextMeshProUGUI _quantity;
        [SerializeField] protected Slider _durability;

        // Slot this item is currently attached to
        [SerializeField, Readonly] protected UIItemSlot _uiSlot;

        // Runtime data
        protected Item _item;
        protected Vector3 _originalPosition;
        protected Transform _originalParent;
        protected bool _dragging = false;

        /// <summary>
        ///     Gets or sets the UI slot this item belongs to.
        /// </summary>
        public UIItemSlot UISlot
        {
            get => _uiSlot;
            set => _uiSlot = value;
        }

        /// <summary>
        ///     Assigns this UIItem to a slot.
        /// </summary>
        /// <param name="uiSlot"> The slot to assign. </param>
        public virtual void SetSlot(UIItemSlot uiSlot)
        {
            if (uiSlot == null) return;
            uiSlot.SetItem(this);
        }

        /// <summary>
        ///     Sets the visual state of this item using the provided data.
        /// </summary>
        /// <param name="item"> The item to represent. </param>
        public virtual void SetItem(Item item)
        {
            _item = item;
            if (_item == null || _item.Information == null)
            {
                this.ClearItem();
                return;
            }

            this.SetIcon();
            this.SetQuantity();
            this.SetDurability();
        }

        /// <summary>
        ///     Hides all visuals when the item is null or invalid.
        /// </summary>
        public virtual void ClearItem()
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

            _quantity.gameObject.SetActive(true);
            _quantity.text = _item.GetQuantity().ToString();
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
        }

        /// <summary>
        ///     Called when the user begins dragging this item.
        /// </summary>
        /// <param name="eventData"> Pointer data associated with the drag event. </param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_uiSlot == null) return;

            _originalPosition = transform.position;
            _originalParent = transform.parent;

            // Move to top canvas so it doesn't get masked/clipped
            transform.SetParent(UIScreenCanvasManager.Instance.Canvas.transform);

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = false; // Let raycasts pass through while dragging
            }

            _dragging = true;
        }

        /// <summary>
        ///     Called during the drag; updates position of the dragged item.
        /// </summary>
        /// <param name="eventData"> Pointer drag data. </param>
        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragging) return;
            transform.position = eventData.position;
        }

        /// <summary>
        ///     Called when the user releases the drag. Handles swapping items or resetting position.
        /// </summary>
        /// <param name="eventData"> Pointer data when drag ends. </param>
        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = true;
            }

            GameObject target = eventData.pointerEnter;
            UIItemSlot targetSlot = target != null ? target.GetComponentInParent<UIItemSlot>() : null;

            if (targetSlot != null && targetSlot != _uiSlot)
            {
                if (_uiSlot != null && _uiSlot.Inventory != null)
                {
                    _uiSlot.Inventory.MoveItem(_uiSlot.Index, targetSlot.Index);
                }
            }

            // Reset UI to original parent/position
            transform.SetParent(_uiSlot.transform);
            transform.localPosition = Vector3.zero;
        }
    }
}
