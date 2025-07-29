using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class AllHiddenAreaData : IGroupData<HiddenAreaData>
    {
        public List<HiddenAreaData> savePoints = new();

        public List<HiddenAreaData> Items => savePoints;
    }
}