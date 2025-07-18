using Asce.Game.Crafts;
using Asce.Managers.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable, MenuName("Craftable Property")]
    public class CraftableItemProperty : ItemProperty
    {
        [SerializeField] private List<Ingredient> _ingredients = new();
        protected ReadOnlyCollection<Ingredient> _readonlyIngredients;

        public ReadOnlyCollection<Ingredient> Ingredients => _readonlyIngredients ??= _ingredients.AsReadOnly();
        public override ItemPropertyType PropertyType => ItemPropertyType.Craftable;


        public CraftableItemProperty () : base () 
        {
            Name = "Craftable";
        }
    }
}