using Asce.Game.Items;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using Asce.Manager.Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Shops
{
    public class UIShopBuyButton : UIObject
    {
        [SerializeField] protected Image _costType;
        [SerializeField] protected TextMeshProUGUI _cost;
        [SerializeField] protected Button _button;

        [Space]
        [SerializeField, Readonly] protected UIShopDetailsItem _details;

        protected ShopItemCost _itemCost;

        public UIShopDetailsItem Details
        {
            get => _details;
            set => _details = value;
        }

        protected virtual void Start()
        {
            if (_button != null) _button.onClick.AddListener(Button_OnClick);
        }

        public virtual void Set(ShopItemCost itemCost)
        {
            if (_itemCost == itemCost) return;
            this.Unregister();
            _itemCost = itemCost;
            this.Register();
        }


        protected virtual void Register()
        {
            if (_itemCost == null) return;
            if (_costType != null)
            {
                if (_itemCost.CostType == null) _costType.gameObject.SetActive(false);
                else
                {
                    _costType.gameObject.SetActive(true);
                    _costType.sprite = _itemCost.CostType.Icon;
                }
            }

            if (_cost != null)
            {
                if (_itemCost.CostType == null) _cost.text = $"{_itemCost.Cost}";
                else _cost.text = $"{_itemCost.Cost}x {_itemCost.CostType.Name}";
            }
        }

        protected virtual void Unregister()
        {
            if (_itemCost == null) return;

        }

        protected virtual void Button_OnClick()
        {
            if (_details == null) return;
            bool isBougth = _details.ShopWindow.Buy(_itemCost);
            if (isBougth) AudioManager.Instance.PlaySFX("Buy Success");
            else AudioManager.Instance.PlaySFX("Buy Failure");
        }
    }
}
