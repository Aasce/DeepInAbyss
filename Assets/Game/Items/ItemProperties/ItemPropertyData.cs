using UnityEngine;

namespace Asce.Game.Items
{
    public abstract class ItemPropertyData
    {
        [SerializeField] protected ItemProperty _property;

        public ItemProperty Property => _property;

        public ItemPropertyData(ItemProperty property)
        {
            _property = property;
        }
    }
}