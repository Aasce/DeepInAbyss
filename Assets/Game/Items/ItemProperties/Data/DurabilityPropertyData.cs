using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.Items
{
    public class DurabilityPropertyData : ItemPropertyData
    {
        [SerializeField] protected float _durability;
        public Action<object, ValueChangedEventArgs<float>> OnDurabilityChanged;

        public new DurabilityableItemProperty Property => base.Property as DurabilityableItemProperty;

        public float Durability
        {
            get => _durability;
            set
            {
                float maxDurability = Property != null ? Property.MaxDurability : 0f;
                float oldDurability = _durability;
                _durability = Mathf.Clamp(value, 0, maxDurability);
                OnDurabilityChanged?.Invoke(this, new ValueChangedEventArgs<float>(oldDurability, _durability));
            }
        }

        public DurabilityPropertyData(ItemProperty property) : base(property) { }
    }
}