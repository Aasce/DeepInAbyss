using System;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable]
    public class StackPropertyData : ItemPropertyData
    {
        [SerializeField] protected int _quantity = 0;

        public new StackableItemProperty Property => base.Property as StackableItemProperty;


        public int Quantity
        { 
            get => _quantity;
            set
            {
                int maxStack = Property != null ? Property.MaxStack : 1;
                _quantity = Mathf.Clamp(value, 0, maxStack);
            }
        }

        public StackPropertyData(ItemProperty property) : base(property) { }
    }
}