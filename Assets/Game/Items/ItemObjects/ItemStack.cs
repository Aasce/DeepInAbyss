using UnityEngine;

namespace Asce.Game.Items
{
    [SerializeField]
    public struct ItemStack
    {
        [SerializeField] private string _name;
        [SerializeField] private int _quantity;

        public string Name
        {
            readonly get => _name;
            set => _name = value;
        }
        public int Quantity
        {
            readonly get => _quantity;
            set => _quantity = value;
        }

        public ItemStack(string name, int quantity)
        {
            _name = name;
            _quantity = quantity;
        }

        public readonly SO_ItemInformation GetItemInfo() => ItemObjectsManager.Instance.ItemData.GetItemByName(Name);
    }
}