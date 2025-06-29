using System;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable]
    public abstract class ItemProperty
    {
        [SerializeField, HideInInspector] protected string _name = string.Empty;

        public string Name
        {
            get => _name;
            protected set => _name = value;
        }

        public abstract ItemPropertyType PropertyType { get; }

        public ItemProperty() 
        {
            Name = this.GetType().ToString();
        }
    }
}