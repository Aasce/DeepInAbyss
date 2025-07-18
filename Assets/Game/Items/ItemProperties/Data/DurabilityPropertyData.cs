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
            set
            {
                float maxDurability = Property != null ? Property.MaxDurability : 0f;
                _durability = Mathf.Clamp(value, 0, maxDurability);
            }
        }

        public DurabilityPropertyData(ItemProperty property) : base(property) { }
    }
}