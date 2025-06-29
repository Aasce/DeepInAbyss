using Asce.Managers.Attributes;
using System;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable, MenuName("Durabilityable Property")]
    public class DurabilityableItemProperty : ItemProperty
    {
        [SerializeField] private float _maxDurability = 100f;


        public float MaxDurability => _maxDurability;
        public override ItemPropertyType PropertyType => ItemPropertyType.Durabilityable;


        public DurabilityableItemProperty() : base() 
        {
            Name = "Durabilityable";
        }
    }
}