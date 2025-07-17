using Asce.Game.Items;
using Asce.Managers.UIs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Inventories
{
    public class UIItemDetails : UIObject
    {
        [SerializeField] protected RectTransform _content;

        [Space]
        [SerializeField] protected Image _iconHolder;
        [SerializeField] protected Image _icon;
        [SerializeField] protected Slider _durability;

        [Space]
        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected UIItemEnchantInformation _enchant;

        [Space]
        [SerializeField] protected TextMeshProUGUI _quantity;

        protected Item _item;

        public virtual void Set(Item item)
        {
            _item = item;
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
            this.SetEnchant();
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
        protected virtual void SetEnchant()
        {
            if (_enchant == null) return;
            if (!_item.Information.HasProperty(ItemPropertyType.Enchantable))
            {
                _enchant.gameObject.SetActive(false);
                return;
            }

            _enchant.gameObject.SetActive(true);
        }
    }
}
