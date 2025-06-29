using Asce.Managers.Attributes;
using System;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable, MenuName("Enchantable Property")]
    public class EnchantableItemProperty : ItemProperty
    {
        

        public override ItemPropertyType PropertyType => ItemPropertyType.Enchantable;

        public EnchantableItemProperty() : base()
        {
            Name = "Enchantable";
        }
    }
}