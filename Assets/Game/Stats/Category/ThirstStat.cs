using System;

namespace Asce.Game.Stats
{
    [Serializable]
    public class ThirstStat : TimeBasedResourceStat
    {
        public ThirstStat() : base()
        {
            ChangeInterval.SetBaseTime(10f);
        }
    }
}
