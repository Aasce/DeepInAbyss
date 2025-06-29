using Asce.Game.Crafts;
using Asce.Managers.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable, MenuName("Craftable Property")]
    public class CraftableItemProperty : ItemProperty
    {
        [SerializeField] private List<Ingredient> _ingredients = new();


        public List<Ingredient> Ingredients => _ingredients;
        public override ItemPropertyType PropertyType => ItemPropertyType.Craftable;


        public CraftableItemProperty () : base () 
        {
            Name = "Craftable";
        }
    }
}