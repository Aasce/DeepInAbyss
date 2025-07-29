using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class AllBillboardData : IGroupData<BillboardData>
    {
        public List<BillboardData> billboards = new();

        public List<BillboardData> Items => billboards;
    }
}