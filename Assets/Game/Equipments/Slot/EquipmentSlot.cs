using Asce.Game.Entities;
using Asce.Game.Items;
using Asce.Managers;
using Asce.Managers.SaveLoads;
using System;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class EquipmentSlot : GameComponent, IEquipmentSlot, IReceiveData<Item>
    {
        // Ref
        [SerializeField] protected IEquipmentController _equipmentOwner;
        [SerializeField] protected Item _equipmentItem;

        public event Action<object, ValueChangedEventArgs<Item>> OnEquipmentChanged;

        public IEquipmentController EquipmentOwner
        {
            get => _equipmentOwner;
            set => _equipmentOwner = value;
        }

        public Item EquipmentItem
        {
            get => _equipmentItem;
            protected set
            {
                if (_equipmentItem == value) return;
                Item oldItem = _equipmentItem;
                _equipmentItem = value;
                OnEquipmentChanged?.Invoke(this, new ValueChangedEventArgs<Item>(oldItem, _equipmentItem));
            }
        }

        public virtual void AddEquipment(Item item)
        {
            if (_equipmentItem == item) return;
            this.Unregister();
            EquipmentItem = item;
            this.Register();
        }

        public virtual Item RemoveEquipment()
        {
            this.Unregister();
            Item item = EquipmentItem;
            EquipmentItem = null;
            return item;
        }

        protected virtual void Register()
        {
            if (_equipmentOwner == null) return;
            if (_equipmentItem.IsNull()) return;
            if (!_equipmentItem.Information.HasProperty(ItemPropertyType.Equippable)) return;
            if (_equipmentItem.Information.GetPropertyByType(ItemPropertyType.Equippable) is not EquippableItemProperty equippable) return;
            if (equippable.EquipEvent != null) equippable.EquipEvent.OnEquip(_equipmentOwner.Owner);
        }


        protected virtual void Unregister()
        {
            if (_equipmentOwner == null) return;
            if (_equipmentItem.IsNull()) return;
            if (!_equipmentItem.Information.HasProperty(ItemPropertyType.Equippable)) return;
            if (_equipmentItem.Information.GetPropertyByType(ItemPropertyType.Equippable) is not EquippableItemProperty equippable) return;
            if (equippable.EquipEvent != null) equippable.EquipEvent.OnUnequip(_equipmentOwner.Owner);
        }


        void IReceiveData<Item>.Receive(Item item)
        {
            if (item.IsNull()) return;
            EquipmentItem = item;
            this.Register();
        }
    }
}