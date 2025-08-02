using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Items
{
    [CreateAssetMenu(menuName = "Asce/Items/Shop Item", fileName = "Shop Item")]
    public class SO_ShopItems : ScriptableObject
    {
        [SerializeField] protected List<ShopItem> _items = new();
        public ReadOnlyCollection<ShopItem> _readonlyItems;

        public ReadOnlyCollection<ShopItem> Items => _readonlyItems ??= _items.AsReadOnly();

        protected virtual void OnEnable()
        {
            foreach(ShopItem item in _items)
            {
                if (item == null) continue;
                if (item.Item == null) continue;
                item.Name = item.Item.Name;
            }   
        }
    }


    [System.Serializable]
    public class ShopItem
    {
        [SerializeField, HideInInspector] protected string _name;
        [SerializeField] protected SO_ItemInformation _item;
        [SerializeField] protected int _quantity;

        [Space]
        [SerializeField] protected List<ShopItemCost> _costs = new();
        
        public string Name { get => _name; set => _name = value; }
        public SO_ItemInformation Item => _item;
        public int Quantity => _quantity;
        
        public List<ShopItemCost> Costs => _costs;       
    }

    [System.Serializable]
    public class ShopItemCost
    {
        [SerializeField] protected SO_ItemInformation _costType;
        [SerializeField] protected int _cost;

        public SO_ItemInformation CostType => _costType;
        public int Cost => _cost;
    }
}
