using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class AllSavePointData
    {
        public List<SavePointData> savePoints = new ();
    }
}