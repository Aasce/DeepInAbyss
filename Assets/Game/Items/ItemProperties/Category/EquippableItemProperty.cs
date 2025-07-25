using Asce.Game.Equipments;
using Asce.Game.Equipments.Events;
using Asce.Managers.Attributes;
using System;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable, MenuName("Equippable Property")]
    public class EquippableItemProperty : ItemProperty
    {
        [SerializeField] protected EquipmentType _equipmentType = EquipmentType.None;
        [SerializeField] protected EquipEvent _equipEvent;

        public EquipmentType EquipmentType => _equipmentType;
        public override ItemPropertyType PropertyType => ItemPropertyType.Equippable;
        public EquipEvent EquipEvent => _equipEvent;

        public EquippableItemProperty() : base()
        {
            Name = "Equippable";
        }
    }
}