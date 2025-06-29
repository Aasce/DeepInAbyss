using Asce.Managers.Attributes;
using System;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable, MenuName("Stackable Property")]
    public class StackableItemProperty : ItemProperty
    {
        [SerializeField, Min(1)] private int _maxStack = 1;

        public int MaxStack => _maxStack;

        public override ItemPropertyType PropertyType => ItemPropertyType.Stackable;

        public StackableItemProperty() : base() 
        {
            Name = "Stackable";
        }
    }
}