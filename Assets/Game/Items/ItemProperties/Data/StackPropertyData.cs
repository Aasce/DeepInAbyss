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
            set => _quantity = value;
        }

        public StackPropertyData(ItemProperty property) : base(property) { }
    }
}