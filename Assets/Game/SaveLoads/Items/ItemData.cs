using Asce.Game.Items;
using Asce.Managers.SaveLoads;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class ItemData : SaveData, ISaveData<Item>, ICreateData<Item>
    {
        public string name;
        public int quantity;
        public float durability;

        // public List<ItemEnchantData> enchants = new();

        public ItemData() { }
        public ItemData(Item item) 
        {
            this.Save(item);
        }

        public void Save(in Item target)
        {
            bool isItemNull = target.IsNull();
            name = isItemNull ? string.Empty : target.Information.Name;
            quantity = isItemNull ? 0 : target.GetQuantity();
            durability = isItemNull ? 0 : target.GetDurability();
        }

        public Item Create()
        {
            SO_ItemInformation information = ItemObjectsManager.Instance.ItemData.GetItemByName(this.name);
            if (information == null) return null;

            Item item = new(information);

            item.SetQuantity(this.quantity);
            item.SetDurability(this.durability);
            return item;
        }
    }
}