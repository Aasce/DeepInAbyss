using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Inventories
{
    public class UIItemDetails : UIObject
    {
        [SerializeField, Readonly] protected UIInventoryWindow _window;

        [Space]
        [SerializeField] protected RectTransform _content;

        [Space]
        [SerializeField] protected Image _iconHolder;
        [SerializeField] protected Image _icon;

        [SerializeField] protected Slider _durability;
        [SerializeField] protected TextMeshProUGUI _durabilityText;

        [Space]
        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected UIItemDescriptionInformation _description;

        [Space]
        [SerializeField] protected TextMeshProUGUI _quantity;
        [SerializeField] protected TextButton _useOrEquipButton;

        protected Item _item;
        protected int _itemIndex;
        protected bool _isItemInInventory;


        public UIInventoryWindow Window => _window;


        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _window);
        }


        protected virtual void Start()
        {
            if (_useOrEquipButton != null) _useOrEquipButton.Button.onClick.AddListener(UseOrEquipButton_OnClick);
        }


        public virtual void Set(Item item, int index, bool isInventory = true)
        {
            this.Unregister();
            _item = item;
            _itemIndex = index;
            _isItemInInventory = isInventory;
            this.Register();
        }

        protected virtual void Register()
        {
            if (_item.IsNull())
            {
                _content.gameObject.SetActive(false);
                return;
            }

            _content.gameObject.SetActive(true);
            this.SetName();
            this.SetIcon();
            this.SetQuantity();
            this.SetDurability();
            this.SetDescription();
            this.SetUseOrEquipButton();

            DurabilityPropertyData durability = _item.GetProperty<DurabilityPropertyData>(ItemPropertyType.Durabilityable);
            if (durability != null) durability.OnDurabilityChanged += Item_OnDurabilityChanged;
        }

        protected virtual void Unregister()
        {
            if (_item.IsNull()) return;
            DurabilityPropertyData durability = _item.GetProperty<DurabilityPropertyData>(ItemPropertyType.Durabilityable);
            if (durability != null) durability.OnDurabilityChanged -= Item_OnDurabilityChanged;
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
            if (_durabilityText != null) _durabilityText.text = $"{_durability.value:0}/{_durability.maxValue:0}";
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
            if (_window == null) return;

            if (_item.Information.HasProperty(ItemPropertyType.Equippable))
            {
                if(_item.Information.GetPropertyByType(ItemPropertyType.Equippable) is EquippableItemProperty equipProperty)
                {
                    if (_isItemInInventory) _window.Equip(_itemIndex);
                    else _window.Unequip(equipProperty.EquipmentType);
                    return;
                }
            }

            if (_item.Information.HasProperty(ItemPropertyType.Usable))
            {
                if (_item.Information.GetPropertyByType(ItemPropertyType.Usable) is UsableItemProperty useProperty)
                {
                    _window.UseItem(_itemIndex);
                    return;
                }
            }
        }

        public virtual void Inventory_OnItemChanged(object sender, int index)
        {
            if (index != _itemIndex) return;
            if (sender is not Inventory inventory) return;
            this.Set(inventory.GetItem(index), index, true);
        }

        protected virtual void Item_OnDurabilityChanged(object sender, ValueChangedEventArgs<float> args)
        {
            this.SetDurability();
        }

    }
}
