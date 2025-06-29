using System;
using UnityEngine;

namespace Asce.Game.Crafts
{
    [Serializable]
    public class Ingredient
    {
        [SerializeField] protected string _name = string.Empty;
        [SerializeField] protected int _quantity = 0;

        public string Name => _name;
        public int Quantity => _quantity;
    }
}