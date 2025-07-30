using Asce.Game.Inventories;
using Asce.Game.Items;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class InventoryData : SaveData, ISaveData<Inventory>, ILoadData<Inventory>
    {
        public List<ItemData> items = new();


        public void Save(in Inventory inventory)
        {
            if (inventory == null) return;
            foreach (var item in inventory.Items)
            {
                ItemData itemData = new(item);
                this.items.Add(itemData);
            }
        }

        public bool Load(Inventory inventory)
        {
            if (inventory == null) return false;
            List<Item> items = new();
            foreach (ItemData itemData in this.items)
            {
                Item item = itemData.Create();
                items.Add(item);
            }
            inventory.Load(items);

            return true;
        }
    }
}