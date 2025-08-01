using Asce.Game.Items;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class AllItemObjectsData : SaveData
    {
        public List<ItemObjectData> itemObjects = new();

        public AllItemObjectsData() 
        {
            foreach (ItemObject itemObject in ItemObjectsManager.Instance.Pool.Activities)
            {
                if (itemObject == null) continue;
                ItemObjectData data = new ItemObjectData();
                data.Save(itemObject);
                itemObjects.Add(data);
            }
        }

        public void Load()
        {
            foreach (ItemObjectData data in itemObjects)
            {
                if (data == null) continue;
                data.Create();
            }
        }
    }
}