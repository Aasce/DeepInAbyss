using Asce.Game.Enviroments;
using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Managers.Pools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Shops
{
    public class UIShopWindow : UIWindow
    {
        [SerializeField] protected UIShopDetailsItem _details;

        [Space]
        [SerializeField] protected Button _equipmentButtonTab;
        [SerializeField] protected Button _consumableButtonTab;
        [SerializeField] protected Button _currencyButtonTab;

        [Space]
        [SerializeField] protected Pool<UIShopItem> _uiShopItemPool = new();
        [SerializeField] protected Pool<UIShopItemCost> _uiShopItemCostPool = new();

        [Space]
        [SerializeField] protected Shop _shop;
        protected IInventoryController _inventoryController;

        public Pool<UIShopItemCost> UIShopItemCostPool => _uiShopItemCostPool;

        protected override void Start()
        {
            base.Start();
            if (_equipmentButtonTab != null) _equipmentButtonTab.onClick.AddListener(EquipmentButton_OnClick);
            if (_consumableButtonTab != null) _consumableButtonTab.onClick.AddListener(ConsumableButton_OnClick);
            if (_currencyButtonTab != null) _currencyButtonTab.onClick.AddListener(CurrencyButton_OnClick);
        }

        public override void Show()
        {
            if (_shop == null) return;
            base.Show();
        }

        public void SetShop(Shop shop, IInventoryController inventoryController)
        {
            if (_shop == shop) return;

            this.Unregister();
            _shop = shop;
            _inventoryController = inventoryController;
            this.Register();
        }

        public virtual void ItemFocus(UIShopItem uiShopItem)
        {
            if (uiShopItem == null) return;
            if (uiShopItem.Item == null) return;

            if (_details == null) return;
            _details.Set(uiShopItem.Item);
        }


        public virtual void Buy(ShopItemCost cost) 
        {
            if (_details == null) return;
            if (_inventoryController == null) return;

            ShopItem buyItem = _details.Item;
            if (buyItem == null) return;
            if (!buyItem.Costs.Contains(cost)) return;

            InventorySystem.Buy(_inventoryController.Inventory, buyItem, cost);
        }


        protected virtual void Register()
        {
            if (_shop == null) return;
            this.EquipmentButton_OnClick();

            UIShopItem item = _uiShopItemPool.Activities.LastOrDefault();
            this.ItemFocus(item);
        }

        protected virtual void Unregister()
        {
            if (_shop == null) return;
        }

        protected virtual List<ShopItem> Filter(ItemType type)
        {
            return _shop.ShopItems.Items.Where((item) => item != null && item.Item != null && item.Item.Type == type).ToList();
        }

        protected virtual void ShowItem(List<ShopItem> shopItems)
        {
            _uiShopItemPool.Clear();
            foreach (ShopItem shopItem in shopItems)
            {
                if (shopItem == null) continue;
                UIShopItem uiShopItem = _uiShopItemPool.Activate();
                if (uiShopItem == null) continue;

                uiShopItem.RectTransform.SetAsFirstSibling();
                uiShopItem.ShopWindow = this;
                uiShopItem.Set(shopItem);
            }
        }

        protected virtual void EquipmentButton_OnClick()
        {
            List<ShopItem> equipmentItem = this.Filter(ItemType.Equipment);
            equipmentItem.AddRange(this.Filter(ItemType.Weapon));
            this.ShowItem(equipmentItem);
        }

        protected virtual void ConsumableButton_OnClick()
        {
            this.ShowItem(this.Filter(ItemType.Consumable));
        }

        protected virtual void CurrencyButton_OnClick()
        {
            this.ShowItem(this.Filter(ItemType.Currency));
        }
    }
}
