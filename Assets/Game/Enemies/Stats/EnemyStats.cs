using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyStats : CreatureStats, IHasOwner<Enemy>, IStatsController<SO_EnemyBaseStats>
    {
        [SerializeField] protected JumpForceStat _jumpForce = new();

        public new Enemy Owner
        {
            get => base.Owner as Enemy;
            set => base.Owner = value;
        }

        public new SO_EnemyBaseStats BaseStats => base.BaseStats as SO_EnemyBaseStats;
        public JumpForceStat JumpForce => _jumpForce;

        public override void LoadBaseStats()
        {
            base.LoadBaseStats();

            JumpForce.AddAgent(gameObject, baseStatsReason, BaseStats.JumpForce, StatValueType.Base).ToNotClearable();
        }

        public override void UpdateStats(float deltaTime)
        {
            base.UpdateStats(deltaTime);

            JumpForce.Update(deltaTime);
        }

        public override void ClearStats(bool isForceClear = false)
        {
            base.ClearStats(isForceClear);
            JumpForce.Clear(isForceClear);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            JumpForce.Reset();
        }
    }
}