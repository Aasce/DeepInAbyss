using Asce.Game.Enviroments;
using Asce.Game.Items;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class ChestData : SaveData, ISaveData<Chest>, ILoadData<Chest>
    {
        public string id;
        public List<ItemData> inventory = new();

        public void Save(in Chest target)
        {
            this.id = target.ID;

            // Set Inventory
            var items = target.Inventory.Items;
            foreach (var item in items)
            {
                ItemData itemData = new(item);
                inventory.Add(itemData);
            }
        }

        public bool Load(Chest target)
        {
            if (target == null) return false;

            List<Item> items = new();
            foreach (ItemData itemData in this.inventory)
            {
                Item item = itemData.Create();
                items.Add(item);
            }
            target.Inventory.Load(items);
            return true;
        }
    }
}