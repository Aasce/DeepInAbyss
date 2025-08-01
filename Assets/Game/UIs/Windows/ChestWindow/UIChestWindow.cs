using Asce.Game.Enviroments;
using Asce.Game.Inventories;
using Asce.Game.UIs.Inventories;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Chests
{
    public class UIChestWindow : UIWindow
    {
        [Header("Chest Window")]
        [SerializeField, Readonly] protected UIInventory _inventory;
        [SerializeField] protected Button _cleanButton;
        [SerializeField] protected Button _lootButton;

        protected Chest _chest;


        public UIInventory Inventory => _inventory;


        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _inventory);
        }
        protected override void Start()
        {
            base.Start();
            if (_cleanButton != null) _cleanButton.onClick.AddListener(CleanButton_OnClick);
            if (_lootButton != null) _lootButton.onClick.AddListener(LootButton_OnClick);
        }

        public override void Show()
        {
            if (_chest == null) return;
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
            this.SetChest(null);
        }

        public void SetChest(Chest chest)
        {
            if (_chest == chest) return;

            this.Unregister();
            _chest = chest;
            this.Register();
        }

        protected virtual void Register()
        {
            if (_chest == null) return;
            if (_inventory != null)
                _inventory.SetInventory(_chest);

            _title.SetText(_chest.name);
            _chest.IsOpened = true;
        }

        protected virtual void Unregister()
        {
            if (_chest == null) return;
            _chest.IsOpened = false;
        }

        protected virtual void CleanButton_OnClick()
        {
            if (_inventory == null) return;
            _inventory.Controller.Inventory.CleanAndMerge();
        }

        protected virtual void LootButton_OnClick()
        {
            if (_inventory == null) return;
            if (_chest == null) return;
            if (_chest.Opener == null) return;
            if (_chest.Opener.Inventory == null) return;

            InventorySystem.LootAll(_chest.Inventory, _chest.Opener.Inventory.Inventory);
        }

    }
}
