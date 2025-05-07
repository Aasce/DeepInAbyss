using System;

namespace Asce.Game.Stats
{
    [Serializable]
    public class HungerStat : TimeBasedResourceStat
    {
        public HungerStat() : base() 
        {
            ChangeInterval.SetBaseTime(10f);
        }
    }
}
