using Asce.Game.Entities;
using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Game.UIs.Equipments;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using TMPro;
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
        [SerializeField] protected Button _cleanButton;
        [SerializeField] TextMeshProUGUI _name;

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
            Players.Player.Instance.CameraController.IsActiveRenderCamera = this.IsShow;
            if (_itemDetails != null) _itemDetails.Set(null, -1);
            if (_inventory != null) _inventory.OnFocusAt += Inventory_OnFocusAt;
            if (_equipment != null) _equipment.OnFocusAt += Equipment_OnFocusAt;
            if (_cleanButton != null) _cleanButton.onClick.AddListener(CleanButton_OnClick);
        }

        public override void Show()
        {
            if (this.IsShow) return;
            base.Show();
            Players.Player.Instance.CameraController.IsActiveRenderCamera = true;
        }
        public override void Hide()
        {
            if (!this.IsShow) return;
            base.Hide();
            Players.Player.Instance.CameraController.IsActiveRenderCamera = false;
        }

        public void SetCreature(ICreature creature)
        {
            if (_creature == creature) return;

            this.Unregister();
            _creature = creature;
            this.Register();
        }

        public virtual void Equip(int index)
        {
            if (_creature == null) return;
            InventorySystem.MoveItemToEquipment(_creature.Inventory.Inventory, _creature.Equipment, index);
        }

        public virtual void Unequip(Game.Equipments.EquipmentType type)
        {
            if (_creature == null) return;
            InventorySystem.MoveEquipmentToInventory(_creature.Equipment, _creature.Inventory.Inventory, type);
        }

        protected virtual void Register() 
        {
            if (_creature == null) return;
            if (_name != null) _name.text = _creature.Information.Name;
            if (_inventory != null)
                _inventory.SetInventory(_creature.Inventory);
            if (_equipment != null) 
                _equipment.SetEquipment(_creature.Equipment);
            if (_creature.Inventory != null)
                _creature.Inventory.Inventory.OnItemChanged += _itemDetails.Inventory_OnItemChanged;
        }
        protected virtual void Unregister() 
        {
            if (_creature == null) return;
            if (_creature.Inventory != null)
                _creature.Inventory.Inventory.OnItemChanged -= _itemDetails.Inventory_OnItemChanged;
        }

        protected virtual void Inventory_OnFocusAt(object sender, int index)
        {
            if (_itemDetails == null) return;
            Item item = _inventory.Controller.Inventory.GetItem(index);
            if (item.IsNull()) return;

            _itemDetails.Set(item, index, isInventory: true);
        }

        protected virtual void Equipment_OnFocusAt(object sender, int index)
        {
            if (_itemDetails == null) return;
            UIItemSlot slot = _equipment.GetSlotByIndex(index);
            if (slot == null) return;
            if (slot.Item == null) return;
            Item item = slot.Item.Item;
            if (item.IsNull()) return;

            _itemDetails.Set(item, index, isInventory: false);
        }

        protected virtual void CleanButton_OnClick()
        {
            if (_inventory == null) return;
            _inventory.Controller.Inventory.CleanAndMerge();
        }

    }
}
