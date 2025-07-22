using Asce.Game.Items;
using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class EquipmentSlot : GameComponent, IEquipmentSlot
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

        public virtual void RemoveEquipment()
        {
            this.Unregister();
            EquipmentItem = null;
        }

        protected virtual void Register()
        {
            if (_equipmentItem == null) return;

        }


        protected virtual void Unregister()
        {
            if (_equipmentItem == null) return;


        }


    }
}