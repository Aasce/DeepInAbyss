using UnityEngine;

namespace Asce.Game.Items
{
    public class DurabilityPropertyData : ItemPropertyData
    {
        [SerializeField] protected float _durability;


        public new DurabilityableItemProperty Property => base.Property as DurabilityableItemProperty;

        public float Durability
        {
            get => _durability;
            set => _durability = value;
        }

        public DurabilityPropertyData(ItemProperty property) : base(property) { }
    }
}