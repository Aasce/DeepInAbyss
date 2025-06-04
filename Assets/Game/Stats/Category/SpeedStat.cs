using System;

namespace Asce.Game.Stats
{
    [Serializable]
    public class SpeedStat : TimeBasedResourceStat
    {
        public SpeedStat() : base(StatType.Speed)
        {

        }
    }
}
