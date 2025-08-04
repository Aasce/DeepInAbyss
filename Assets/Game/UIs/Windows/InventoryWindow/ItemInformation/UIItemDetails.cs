using Asce.Game.Items;
using Asce.Managers.UIs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Inventories
{
    public class UIItemDetails : UIObject
    {
        [Space]
        [SerializeField] protected RectTransform _content;

        [Space]
        [SerializeField] protected Image _iconHolder;
        [SerializeField] protected Image _icon;
        [SerializeField] protected Slider _durability;

        [Space]
        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected UIItemDescriptionInformation _description;

        [Space]
        [SerializeField] protected TextMeshProUGUI _quantity;
        [SerializeField] protected TextButton _useOrEquipButton;

        protected Item _item;
        protected int _itemIndex;
        protected bool _isItemInInventory;


        protected virtual void Start()
        {
            if (_useOrEquipButton != null) _useOrEquipButton.Button.onClick.AddListener(UseOrEquipButton_OnClick);
        }


        public virtual void Set(Item item, int index, bool isInventory = true)
        {
            _item = item;
            if (_item.IsNull())
            {
                _content.gameObject.SetActive(false);
                return;
            }
            
            _itemIndex = index;
            _isItemInInventory = isInventory;
            _content.gameObject.SetActive(true);
            this.SetName();
            this.SetIcon();
            this.SetQuantity();
            this.SetDurability();
            this.SetDescription();
            this.SetUseOrEquipButton();
        }


        protected virtual void SetName()
        {
            if (_name == null) return;
            _name.text = _item.Information.Name;
        }
        protected virtual void SetIcon()
        {
            // Icon Holder
            if (_iconHolder != null)
            {

            }

            // Icon
            if (_icon != null)
            {
                _icon.sprite = _item.Information.Icon;
            }
        }
        protected virtual void SetQuantity()
        {
            if (_quantity == null) return;
            if (!_item.Information.HasProperty(ItemPropertyType.Stackable))
            {
                _quantity.gameObject.SetActive(false);
                return;
            }

            _quantity.gameObject.SetActive(true);
            int maxStack = _item.Information.GetMaxStack();
            int quantity = _item.GetQuantity();
            _quantity.text = $"{quantity}/{maxStack}";
        }
        protected virtual void SetDurability()
        {
            if (_durability == null) return;
            if (!_item.Information.HasProperty(ItemPropertyType.Durabilityable))
            {
                _durability.gameObject.SetActive(false);
                return;
            }

            _durability.gameObject.SetActive(true);
            _durability.maxValue = _item.Information.GetMaxDurability();
            _durability.value = _item.GetDurability();
        }
        protected virtual void SetDescription()
        {
            if (_description == null) return;
            _description.Set(_item.Information);
        }
        protected virtual void SetUseOrEquipButton()
        {
            if (_useOrEquipButton == null) return;
            if (_item.Information.HasProperty(ItemPropertyType.Equippable))
            {
                _useOrEquipButton.gameObject.SetActive(true);
                if (_isItemInInventory) _useOrEquipButton.Text.text = "Equip";
                else _useOrEquipButton.Text.text = "Unequip";

                return;
            }

            if (_item.Information.HasProperty(ItemPropertyType.Usable))
            {
                _useOrEquipButton.gameObject.SetActive(true);
                _useOrEquipButton.Text.text = "Use";

                return;
            }

            _useOrEquipButton.gameObject.SetActive(false);
        }

        protected virtual void UseOrEquipButton_OnClick()
        {
            if (_item.IsNull()) return;
            if (_item.Information.HasProperty(ItemPropertyType.Equippable))
            {
                if(_item.Information.GetPropertyByType(ItemPropertyType.Equippable) is EquippableItemProperty equipProperty)
                {
                    UIInventoryWindow window = UIScreenCanvasManager.Instance.WindowsController.GetWindow<UIInventoryWindow>();
                    if (window != null)
                    {
                        if (_isItemInInventory) window.Equip(_itemIndex);
                        else window.Unequip(equipProperty.EquipmentType);
                    }
                }
            }

            if (_item.Information.HasProperty(ItemPropertyType.Usable))
            {

            }
        }
    }
}
