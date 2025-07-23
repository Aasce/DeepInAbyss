using Asce.Game.Entities;
using Asce.Game.Items;
using Asce.Game.UIs.Equipments;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Inventories
{
    public class UIInventoryWindow : UIWindow
    {
        [Header("Inventory Window")]
        [SerializeField, Readonly] protected UIEquipment _equipment;        
        [SerializeField, Readonly] protected UIInventory _inventory;        
        [SerializeField, Readonly] protected UIItemDetails _itemDetails; // Reference to the item information display panel (tooltip/details panel)
        [SerializeField] protected Button _sortButton;

        protected ICreature _creature;

        public UIEquipment UIEquipment => _equipment;
        public UIInventory Inventory => _inventory;
        public UIItemDetails ItemDetails => _itemDetails;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _equipment);
            this.LoadComponent(out _inventory);
            this.LoadComponent(out _itemDetails);
        }

        protected override void Start()
        {
            base.Start();
            if (_itemDetails != null) _itemDetails.Set(null);
            if (_inventory != null) _inventory.OnFocusAt += Inventory_OnFocusAt;
            if (_equipment != null) _equipment.OnFocusAt += Equipment_OnFocusAt;
            if (_sortButton != null) _sortButton.onClick.AddListener(SortButton_OnClick);
        }
        public void SetCreature(ICreature creature)
        {
            if (_creature == creature) return;

            this.Unregister();
            _creature = creature;
            this.Register();
        }

        protected virtual void Register() 
        {
            if (_creature == null) return;
            if (_inventory != null)
                _inventory.SetInventory(_creature.Inventory);
            if (_equipment != null) 
                _equipment.SetEquipment(_creature.Equipment);
        }
        protected virtual void Unregister() 
        {
            if (_creature == null) return;
        }

        protected virtual void Inventory_OnFocusAt(object sender, int index)
        {
            if (_itemDetails == null) return;
            Item item = _inventory.Controller.Inventory.GetItem(index);
            if (item.IsNull()) return;

            _itemDetails.Set(item);
        }

        protected virtual void Equipment_OnFocusAt(object sender, int index)
        {
            if (_itemDetails == null) return;
            UIItemSlot slot = _equipment.GetSlotByIndex(index);
            if (slot == null) return;
            if (slot.Item == null) return;
            Item item = slot.Item.Item;
            if (item.IsNull()) return;

            _itemDetails.Set(item);
        }

        protected virtual void SortButton_OnClick()
        {
            if (_inventory == null) return;
            _inventory.Controller.Inventory.SortAndMerge();
        }

    }
}
