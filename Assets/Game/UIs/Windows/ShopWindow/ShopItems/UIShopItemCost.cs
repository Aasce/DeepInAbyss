using Asce.Game.Items;
using Asce.Managers.UIs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Shops
{
    public class UIShopItemCost : UIObject
    {
        [SerializeField] protected Image _costType;
        [SerializeField] protected TextMeshProUGUI _cost;
        protected ShopItemCost _itemCost;

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
    }
}
