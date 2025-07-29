using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class AllSavePointData : IGroupData<SavePointData>
    {
        public List<SavePointData> savePoints = new ();

        public List<SavePointData> Items => savePoints;
    }
}