using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class AllChestData : IGroupData<ChestData>
    {
        public List<ChestData> chests = new();

        public List<ChestData> Items => chests;
    }
}