using Asce.Game.Items;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.Shops
{
    public class UIShopItem : UIObject, IPointerClickHandler
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected RectTransform _costHolder;

        [Space]
        [SerializeField, Readonly] protected UIShopWindow _shopWindow;

        [Space]
        [SerializeField, Min(0)] protected int _maxCostShow = 2;

        protected List<UIShopItemCost> _costs = new();

        protected ShopItem _item;

        public UIShopWindow ShopWindow
        {
            get => _shopWindow;
            set => _shopWindow = value;
        }

        public ShopItem Item => _item;

        public virtual void Set(ShopItem item)
        {
            if (_item == item) return;
            this.Unregister();
            _item = item;
            this.Register();
        }

        protected virtual void Register() 
        {
            if (_item == null) return;
            if (_icon != null)
            {
                if (_item.Item == null) _icon.gameObject.SetActive(false);
                else
                {
                    _icon.gameObject.SetActive(true);
                    _icon.sprite = _item.Item.Icon;
                }
            }

            if (_name != null)
            {
                if (_item.Item == null) _name.gameObject.SetActive(false);
                else
                {
                    _name.gameObject.SetActive(true);
                    _name.text = $"{_item.Item.Name} x{_item.Quantity}";
                }
            }

            if (_shopWindow == null) return;
            if (_costHolder == null) return;
            int min = Mathf.Min(_item.Costs.Count, _maxCostShow);
            for (int i = 0; i < min; i++)
            {
                ShopItemCost cost = _item.Costs[i];
                if (cost == null) continue;
                UIShopItemCost uiCost = _shopWindow.UIShopItemCostPool.Activate();
                if (uiCost == null) continue;

                uiCost.RectTransform.SetParent(_costHolder);
                uiCost.RectTransform.SetAsFirstSibling();
                uiCost.Set(cost);
                _costs.Add(uiCost);
            }
        }
        
        protected virtual void Unregister()
        {
            if (_item == null) return;
            if (_shopWindow == null) return;

            foreach (UIShopItemCost uiCost in _costs)
            {
                _shopWindow.UIShopItemCostPool.Deactivate(uiCost);
            }
            _costs.Clear();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_shopWindow == null) return;
            _shopWindow.ItemFocus(this);
        }

    }
}
