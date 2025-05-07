using UnityEngine;

namespace Asce.Game.Stats
{
    public interface IHasDefense
    {
        public Stat Armor { get; }
        public Stat Resistance { get; }

        public ShieldStat Shield { get; }
    }
}
