using Asce.Game.Entities;
using Asce.Game.Enviroments;
using Asce.Game.Equipments;
using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Game.UIs.ContextMenus;
using Asce.Game.UIs.Inventories;
using Asce.Managers;
using Asce.Managers.Pools;
using Asce.Managers.UIs;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Asce.Game.UIs.Equipments
{
    public class UIEquipment : UIObject, IUISlotController, IUIEquipment
    {
        [SerializeField] protected UIItemSlot _headSlot;
        [SerializeField] protected UIItemSlot _chestSlot;
        [SerializeField] protected UIItemSlot _legsSlot;
        [SerializeField] protected UIItemSlot _feetSlot;

        [SerializeField] protected UIItemSlot _backpackSlot;
        [SerializeField] protected UIItemSlot _weaponSlot;

        [Space]
        [SerializeField] protected Pool<UIItem> _itemsPool = new();

        protected Dictionary<EquipmentType, UIItemSlot> _slots = new();

        protected IEquipmentController _controller;
        protected UIItemContextMenu _itemContextMenu;

        protected bool _isInitialized = false;

        public event Action<object, int> OnFocusAt;

        public IEquipmentController Controller => _controller;

        protected virtual void Awake()
        {
            this.Init();
        }
        protected virtual void Start()
        {
            _itemContextMenu = UIScreenCanvasManager.Instance.ContextMenusController.GetMenu<UIItemContextMenu>();

            if (_itemContextMenu != null)
            {
                this.OnHide += (sender) => _itemContextMenu.Hide();
            }
        }

        public virtual void SetEquipment(IEquipmentController equipmentController)
        {
            this.Init();
            if (_controller == equipmentController) return;

            this.Unregister();
            _controller = equipmentController;
            this.Register();
        }

        public virtual UIItemSlot GetSlotByIndex(int index)
        {
            return _slots.Values.First((slot) => slot != null && slot.Index == index);
        }

        public void FocusAt(int index)
        {
            OnFocusAt?.Invoke(this, index);
        }

        public void ShowMenuContextAt(int index)
        {
            if (_itemContextMenu == null) return;

            UIItemSlot slot = GetSlotByIndex(index);
            if (slot == null) return;
            if (slot.Item == null) return;

            Item item = slot.Item.Item;
            if (item.IsNull())
            {
                _itemContextMenu.Index = -1;
                _itemContextMenu.Hide();
                return;
            }

            Vector2 position = slot.RectTransform.position;
            position.x -= slot.RectTransform.sizeDelta.x * 0.5f;
            position.y += slot.RectTransform.sizeDelta.y * 0.5f;
            _itemContextMenu.RectTransform.position = position;

            _itemContextMenu.Index = index;
            _itemContextMenu.Set(item);
            _itemContextMenu.Show();
        }

        public virtual void DropItemAt(int index, int quantity = -1)
        {
            if (_controller == null) return;
        }

        public virtual void BeginDragItem(UIItem sender, PointerEventData eventData)
        {
            if (sender.UISlot == null || eventData.button == PointerEventData.InputButton.Right)
                return;

            Item item = sender.Item;
            if (item.IsNull()) return;

            // Elevate UIItem in the hierarchy
            sender.transform.SetParent(UIScreenCanvasManager.Instance.Canvas.transform);
            sender.CanvasGroup.blocksRaycasts = false;
            sender.IsDragging = true;
        }

        public virtual void EndDragItem(UIItem sender, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                return;

            sender.IsDragging = false;
            sender.CanvasGroup.blocksRaycasts = true;

            GameObject target = eventData.pointerEnter;
            UIItemSlot targetSlot = target != null ? target.GetComponentInParent<UIItemSlot>() : null;
            UIItemSlot originSlot = sender.UISlot;

            int fromIndex = originSlot != null ? originSlot.Index : -1;
            int toIndex = targetSlot != null ? targetSlot.Index : -1;
            HandleNormalEndDrag(sender, fromIndex, target, originSlot, targetSlot);

            if (_itemContextMenu != null) _itemContextMenu.Hide();
        }

        private void HandleNormalEndDrag(
           UIItem sender,
           int fromIndex,
           GameObject target,
           UIItemSlot originSlot,
           UIItemSlot targetSlot)
        {
            // Dropped outside of any slot -> drop the item to the ground
            if (target == null)
            {
                DropItemAt(fromIndex);
                if (originSlot != null) originSlot.ResetItemPosition();
                return;
            }

            // Invalid drop target -> return the item to the original slot
            if (targetSlot == null)
            {
                if (originSlot != null) originSlot.ResetItemPosition();
                return;
            }

            // Dropped into another inventory
            if (targetSlot.Inventory != null && targetSlot.Inventory != (IUISlotController)this)
            {
                originSlot.ResetItemPosition();
                EquipmentType type = _slots.GetKeyByValue(originSlot);
                if (targetSlot.Inventory is IUIInventory uIInventory)
                {
                    InventorySystem.MoveEquipmentToInventory(
                        _controller,
                        uIInventory.Controller.Inventory,
                        type,
                        targetSlot.Index);
                }
                return;
            }
        }
        protected virtual void Register()
        {
            if (_controller == null) return;

            if (_controller is IHasHeadSlot hasHead) 
            {
                this.UpdateSlot(EquipmentType.Helmet, hasHead.HeadSlot.EquipmentItem);
                hasHead.HeadSlot.OnEquipmentChanged += HeadSlot_OnEquipmentChanged;
            }
            else if (_slots.TryGetValue(EquipmentType.Helmet, out UIItemSlot slot)) slot.Hide();
            
            if (_controller is IHasChestSlot hasChest)
            {
                this.UpdateSlot(EquipmentType.Chest, hasChest.ChestSlot.EquipmentItem);
                hasChest.ChestSlot.OnEquipmentChanged += ChestSlot_OnEquipmentChanged;
            }
            else if (_slots.TryGetValue(EquipmentType.Chest, out UIItemSlot slot)) slot.Hide();
            
            if (_controller is IHasLegsSlot hasLegs)
            {
                this.UpdateSlot(EquipmentType.Legging, hasLegs.LegsSlot.EquipmentItem);
                hasLegs.LegsSlot.OnEquipmentChanged += LegsSlot_OnEquipmentChanged;
            }
            else if (_slots.TryGetValue(EquipmentType.Legging, out UIItemSlot slot)) slot.Hide();
            
            if (_controller is IHasFeetsSlot hasFeets)
            {
                this.UpdateSlot(EquipmentType.Boots, hasFeets.FeetsSlot.EquipmentItem);
                hasFeets.FeetsSlot.OnEquipmentChanged += FeetsSlot_OnEquipmentChanged;
            }
            else if (_slots.TryGetValue(EquipmentType.Boots, out UIItemSlot slot)) slot.Hide();
            
            if (_controller is IHasBackpackSlot hasBackpack)
            {
                this.UpdateSlot(EquipmentType.Backpack, hasBackpack.BackpackSlot.EquipmentItem);
                hasBackpack.BackpackSlot.OnEquipmentChanged += BackpackSlot_OnEquipmentChanged;
            }
            else if (_slots.TryGetValue(EquipmentType.Backpack, out UIItemSlot slot)) slot.Hide();
            
            if (_controller is IHasWeaponSlot hasWeapon)
            {
                this.UpdateSlot(EquipmentType.Weapon, hasWeapon.WeaponSlot.EquipmentItem);
                hasWeapon.WeaponSlot.OnEquipmentChanged += WeaponSlot_OnEquipmentChanged;
            }
            else if (_slots.TryGetValue(EquipmentType.Weapon, out UIItemSlot slot)) slot.Hide();
            
        }
        protected virtual void Unregister()
        {
            if (_controller == null) return;

            if (_controller is IHasHeadSlot hasHead) hasHead.HeadSlot.OnEquipmentChanged -= HeadSlot_OnEquipmentChanged;
            if (_controller is IHasChestSlot hasChest) hasChest.ChestSlot.OnEquipmentChanged -= ChestSlot_OnEquipmentChanged;
            if (_controller is IHasLegsSlot hasLegs) hasLegs.LegsSlot.OnEquipmentChanged -= LegsSlot_OnEquipmentChanged;
            if (_controller is IHasFeetsSlot hasFeets) hasFeets.FeetsSlot.OnEquipmentChanged -= FeetsSlot_OnEquipmentChanged;
            if (_controller is IHasBackpackSlot hasBackpack) hasBackpack.BackpackSlot.OnEquipmentChanged -= BackpackSlot_OnEquipmentChanged;
            if (_controller is IHasWeaponSlot hasWeapon) hasWeapon.WeaponSlot.OnEquipmentChanged -= WeaponSlot_OnEquipmentChanged;
        }

        protected virtual void Init()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            this.LoadSlot(0, EquipmentType.Helmet, _headSlot);
            this.LoadSlot(1, EquipmentType.Chest, _chestSlot);
            this.LoadSlot(2, EquipmentType.Legging, _legsSlot);
            this.LoadSlot(3, EquipmentType.Boots, _feetSlot);
            this.LoadSlot(4, EquipmentType.Backpack, _backpackSlot);
            this.LoadSlot(5, EquipmentType.Weapon, _weaponSlot);
        }

        protected virtual void LoadSlot(int index, EquipmentType type, UIItemSlot slot)
        {
            if (slot == null) return;
            if (_slots.ContainsKey(type)) return;

            slot.Inventory = this;
            slot.Index = index;
            _slots[type] = slot;
        }

        protected virtual void UpdateSlot(EquipmentType type, Item item)
        {
            if (!_slots.TryGetValue(type, out UIItemSlot uiSlot)) return;
            uiSlot.Show();

            if (item.IsNull())
            {
                if (!uiSlot.IsContainItem) return;

                // Remove current item if exists
                _itemsPool.Deactivate(uiSlot.Item);
                uiSlot.SetItem(null);
                return;
            }

            if (!uiSlot.IsContainItem)
            {
                // Activate new UI item and bind
                UIItem uiItem = _itemsPool.Activate();
                if (uiItem == null)
                {
                    uiSlot.SetItem(null);
                    return;
                }

                uiItem.SetItem(item);
                uiSlot.SetItem(uiItem);
            }
            else
            {
                // Update existing UI item
                uiSlot.Item.SetItem(item);
                uiSlot.ResetItemPosition();
            }
        }

        protected virtual void HeadSlot_OnEquipmentChanged(object sender, ValueChangedEventArgs<Item> args)
        {
            UpdateSlot(EquipmentType.Helmet, args.NewValue);
        }
        protected virtual void ChestSlot_OnEquipmentChanged(object sender, ValueChangedEventArgs<Item> args)
        {
            UpdateSlot(EquipmentType.Chest, args.NewValue);
        }
        protected virtual void LegsSlot_OnEquipmentChanged(object sender, ValueChangedEventArgs<Item> args)
        {
            UpdateSlot(EquipmentType.Legging, args.NewValue);
        }
        protected virtual void FeetsSlot_OnEquipmentChanged(object sender, ValueChangedEventArgs<Item> args)
        {
            UpdateSlot(EquipmentType.Boots, args.NewValue);
        }
        protected virtual void BackpackSlot_OnEquipmentChanged(object sender, ValueChangedEventArgs<Item> args)
        {
            UpdateSlot(EquipmentType.Backpack, args.NewValue);
        }
        protected virtual void WeaponSlot_OnEquipmentChanged(object sender, ValueChangedEventArgs<Item> args)
        {
            UpdateSlot(EquipmentType.Weapon, args.NewValue);
        }


    }
}
