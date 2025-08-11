using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities.Ores
{
    public class OreStats : EntityStats, IHasOwner<Ore>, IStatsController<SO_OreBaseStats>, IHasHealth, IHasDefense
    {
        [SerializeField] protected HealthGroupStats _healthGroup = new();
        [SerializeField] protected DefenseGroupStats _defenseGroup = new();

        public new Ore Owner
        {
            get => base.Owner as Ore;
            set => base.Owner = value;
        }

        public new SO_OreBaseStats BaseStats => base.BaseStats as SO_OreBaseStats;

        public HealthGroupStats HealthGroup => _healthGroup;
        public DefenseGroupStats DefenseGroup => _defenseGroup;


        public override void LoadBaseStats()
        {
            if (BaseStats == null) return;

            // Health
            HealthGroup.Health.AddAgent(gameObject, baseStatsReason, BaseStats.MaxHealth, StatValueType.Base).ToNotClearable();
            HealthGroup.HealScale.AddAgent(gameObject, baseStatsReason, 1f, StatValueType.Base).ToNotClearable();
            HealthGroup.Load();

            // Defense
            DefenseGroup.Armor.AddAgent(gameObject, baseStatsReason, BaseStats.Armor, StatValueType.Base).ToNotClearable();
            DefenseGroup.Resistance.AddAgent(gameObject, baseStatsReason, BaseStats.Resistance, StatValueType.Base).ToNotClearable();
            DefenseGroup.Shield.AddAgent(gameObject, baseStatsReason, BaseStats.Shield, StatValueType.Base).ToNotClearable();
        }

        public override void UpdateStats(float deltaTime)
        {
            if (!IsStatsUpdating) return;

            HealthGroup.Update(deltaTime);
            DefenseGroup.Update(deltaTime);
        }

        public override void ClearStats(bool isForceClear = false)
        {
            HealthGroup.Clear(isForceClear);
            DefenseGroup.Clear(isForceClear);
        }

        public override void ResetStats()
        {
            this.ClearStats();

            HealthGroup.Reset();
            DefenseGroup.Reset();
        }
    }
}