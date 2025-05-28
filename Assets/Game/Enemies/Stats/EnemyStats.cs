using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyStats : CreatureStats, IHasOwner<Enemy>, IStatsController<SO_EnemyBaseStats>
    {
        public new Enemy Owner
        {
            get => base.Owner as Enemy;
            set => base.Owner = value;
        }

        public new SO_EnemyBaseStats BaseStats => base.BaseStats as SO_EnemyBaseStats;
    }
}