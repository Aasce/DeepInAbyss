using Asce.Game.Enviroments;
using Asce.Managers.SaveLoads;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class ChestData : SaveData, ISaveData<Chest>, ILoadData<Chest>
    {
        public string id;
        public InventoryData inventory = new();

        public void Save(in Chest target)
        {
            this.id = target.ID;
            inventory.Save(target.Inventory);
        }

        public bool Load(Chest target)
        {
            if (target == null) return false;
            inventory.Load(target.Inventory);
            return true;
        }
    }
}