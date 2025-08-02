using Asce.Game.Items;
using Asce.Managers.Attributes;
using Asce.Managers.Pools;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Shops
{
    public class UIShopDetailsItem : UIObject
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected TextMeshProUGUI _description;

        [Space]
        [SerializeField] protected Pool<UIShopBuyButton> _uiBuyButtonPool = new();

        [Space]
        [SerializeField, Readonly] protected UIShopWindow _shopWindow;

        protected ShopItem _item;

        public Pool<UIShopBuyButton> BuyButtonPool => _uiBuyButtonPool;

        public UIShopWindow ShopWindow
        {
            get => _shopWindow;
            set => _shopWindow = value;
        }
        public ShopItem Item => _item;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _shopWindow);
        }


        public virtual void Set(ShopItem shopItem)
        {
            if (_item == shopItem) return;
            this.Unregister();
            _item = shopItem;
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

            foreach (ShopItemCost cost in _item.Costs)
            {
                if (cost == null) continue;
                UIShopBuyButton uiBuyButton = BuyButtonPool.Activate();
                if (uiBuyButton == null) continue;

                uiBuyButton.Details = this;
                uiBuyButton.Set(cost);
            }
        }

        protected virtual void Unregister()
        {
            if (_item == null) return;
            BuyButtonPool.Clear();
        }
    }
}
