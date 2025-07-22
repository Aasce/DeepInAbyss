using Asce.Game.Equipments;
using Asce.Managers.Attributes;
using System;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable, MenuName("Equippable Property")]
    public class EquippableItemProperty : ItemProperty
    {
        [SerializeField] protected EquipmentType _equipmentType = EquipmentType.None;

        public EquipmentType EquipmentType => _equipmentType;
        public override ItemPropertyType PropertyType => ItemPropertyType.Equippable;

        public EquippableItemProperty() : base()
        {
            Name = "Equippable";
        }
    }
}