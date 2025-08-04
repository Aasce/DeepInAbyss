using Asce.Game.Items;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Inventories
{
    [CreateAssetMenu(menuName = "Asce/Items/Default Inventory Items", fileName = "Default Inventory Items")]
    public class SO_DefaultInventoryItems : ScriptableObject
    {
        [SerializeField] protected List<InventoryItemContainer> _items = new();
        protected ReadOnlyCollection<InventoryItemContainer> _readonlyItems;

        public ReadOnlyCollection<InventoryItemContainer> Items => _readonlyItems ??= _items.AsReadOnly();

        protected virtual void OnEnable()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                InventoryItemContainer item = _items[i];
                if (item == null) item = new();
                if (item.Information == null) item.Name = "None";
                else item.Name = item.Information.Name;
            }
        }
    }

    [System.Serializable]
    public class InventoryItemContainer 
    {
        [SerializeField, HideInInspector] protected string _name;

        [SerializeField] protected SO_ItemInformation _information;
        [SerializeField, Min(0)] protected int _quantity = 1;
        [SerializeField, Min(0f)] protected float _durationRatio = 1f;

        public string Name { get => _name; set => _name = value; }
        public SO_ItemInformation Information => _information;
        public int Quantity => _quantity;
        public float DurationRatio => _durationRatio;

        public Item CreateItem()
        {
            if (_information == null) return null;

            Item item = new(Information);
            item.SetQuantity(Quantity);
            item.SetDurability(Information.GetMaxDurability() * DurationRatio);

            return item;
        }
    }
}
