using Asce.Managers.Attributes;
using System;
using UnityEngine;

namespace Asce.Game.Items
{
    [Serializable, MenuName("Usable Property")]
    public class UsableItemProperty : ItemProperty
    {
        [SerializeField] protected ItemUseCostType _costType = ItemUseCostType.None;
        [SerializeField] protected int _quantityCost = 1;
        [SerializeField] protected float _durabilityCost = 0f;

        [Space]
        [SerializeField] protected UseEvent _useEvent;
        
        public override ItemPropertyType PropertyType => ItemPropertyType.Usable;

        public ItemUseCostType CostType => _costType;
        public int QuantityCost => _quantityCost;
        public float DurabilityCost => _durabilityCost;

        public UseEvent UseEvent => _useEvent;

        public UsableItemProperty() : base()
        {
            Name = "Usable";
        }
    }

    public enum ItemUseCostType
    {
        None = 0,
        RemoveQuantity = 1,
        DeductDurability = 2,
    }
}