using Asce.Game.Items;
using Asce.Game.UIs.Inventories;
using Asce.Managers.UIs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.ContextMenus
{
    public class UIItemContextMenu : UIContextMenu
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected Image _iconHolder;

        [SerializeField] protected TextMeshProUGUI _name;

        [Space]
        [SerializeField] protected RectTransform _quantityHolder;
        [SerializeField] protected Slider _quantitySlider;
        [SerializeField] protected TextMeshProUGUI _quantityText;

        [Space]
        [SerializeField] protected RectTransform _useOrEquipButtonHolder;
        [SerializeField] protected TextButton _useOrEquipButton;

        protected Item _item;
        protected int _itemIndex;
        protected bool _isItemInInventory;

        public int Index
        {
            get => _itemIndex;
            set => _itemIndex = value;
        }

        public bool IsItemInInventory
        {
            get => _isItemInInventory;
            set => _isItemInInventory = value;
        }

        /// <summary>
        ///     if not show or _quantitySlider == null, return -1
        /// </summary>
        public int QuantityToSplit => _quantitySlider != null ? Mathf.RoundToInt(_quantitySlider.value) : -1;

        public TextButton UseOrEquipButton => _useOrEquipButton;

        protected virtual void Start()
        {
            if (_quantitySlider != null) _quantitySlider.onValueChanged.AddListener(QuantitySlider_OnValueChanged);
            if (_useOrEquipButton != null) _useOrEquipButton.Button.onClick.AddListener(UseOrEquipButton_OnClick);
        }

        public virtual void Set(Item item)
        {
            if (item.IsNull()) return;

            _item = item;
            this.SetIcon();
            this.SetName();
            this.SetQuantity();
            this.SetUseOrEquipButton();
        }

        public virtual void SetIcon()
        {
            if (_iconHolder != null)
            {

            }

            if (_icon != null)
            {
                _icon.sprite = _item.Information.Icon;
            }
        }

        public virtual void SetName()
        {
            if(_name == null) return;
            _name.text = _item.Information.Name;
        }

        public virtual void SetQuantity()
        {
            if (_quantityHolder == null) return;
            if (!_item.Information.HasProperty(ItemPropertyType.Stackable))
            {
                _quantityHolder.gameObject.SetActive(false);
                return;
            }
;
            _quantityHolder.gameObject.SetActive(true);

            int quantity = _item.GetQuantity();
            if (_quantitySlider != null)
            {
                _quantitySlider.maxValue = quantity;
                _quantitySlider.value = Mathf.RoundToInt(Mathf.Ceil(quantity * 0.5f));
            }

            this.SetQuantityText();
        }

        public virtual void SetQuantityText()
        {
            if (_quantityText == null) return;
            _quantityText.text = $"{_quantitySlider.value}/{_quantitySlider.maxValue}";
        }

        public virtual void SetUseOrEquipButton()
        {
            if (_useOrEquipButton == null) return;
            if (_item.Information.HasProperty(ItemPropertyType.Equippable))
            {
                if (_useOrEquipButtonHolder != null) _useOrEquipButtonHolder.gameObject.SetActive(true);
                if (_isItemInInventory) _useOrEquipButton.Text.text = "Equip";
                else _useOrEquipButton.Text.text = "Unequip";

                return;
            }

            if (_item.Information.HasProperty(ItemPropertyType.Usable))
            {
                if (_useOrEquipButtonHolder != null) _useOrEquipButtonHolder.gameObject.SetActive(true);
                _useOrEquipButton.Text.text = "Use";

                return;
            }

            if (_useOrEquipButtonHolder != null) _useOrEquipButtonHolder.gameObject.SetActive(false);
        }

        protected virtual void QuantitySlider_OnValueChanged(float value)
        {
            this.SetQuantityText();
        }

        protected virtual void UseOrEquipButton_OnClick()
        {
            if (_item.IsNull()) return;
            if (_item.Information.HasProperty(ItemPropertyType.Equippable))
            {
                if (_item.Information.GetPropertyByType(ItemPropertyType.Equippable) is EquippableItemProperty equipProperty)
                {
                    UIInventoryWindow window = UIScreenCanvasManager.Instance.WindowsController.GetWindow<UIInventoryWindow>();
                    if (window != null)
                    {
                        if (_isItemInInventory) window.Equip(_itemIndex);
                        else window.Unequip(equipProperty.EquipmentType);
                    }
                    this.Hide();
                }
                return;
            }

            if (_item.Information.HasProperty(ItemPropertyType.Usable))
            {
                UIInventoryWindow window = UIScreenCanvasManager.Instance.WindowsController.GetWindow<UIInventoryWindow>();
                if (window != null)
                {
                    window.UseItem(_itemIndex);
                }
                return;
            }
        }
    }
}