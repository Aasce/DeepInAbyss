using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureStats : EntityStats, IHasOwner<Creature>, IStatsController<SO_CreatureBaseStats>, IHasSurvivalStats, IHasStamina, IHasCombatStats, IHasUtilitiesStats
    {
        [Header("Survival")]
        [SerializeField] protected HealthGroupStats _healthGroup = new();
        [SerializeField] protected StaminaStat _stamina = new();
        [SerializeField] protected SustenanceGroupStats _sustenanceGroup = new();

        [Header("Combat")]
        [SerializeField] protected StrengthStat _strength = new();
        [SerializeField] protected DefenseGroupStats _defenseGroup = new();

        [Header("Utilities")]
        [SerializeField] protected SpeedStat _speed = new();
        [SerializeField] protected ViewRadiusStat _viewRadius = new();

        /// <summary>
        ///     Reference to the creature that owns this stats controller.
        /// </summary>
        public new Creature Owner
        {
            get => base.Owner as Creature;
            set => base.Owner = value;
        }

        public new SO_CreatureBaseStats BaseStats => base.BaseStats as SO_CreatureBaseStats;


        public HealthGroupStats HealthGroup => _healthGroup;
        public StaminaStat Stamina => _stamina;
        public SustenanceGroupStats SustenanceGroup => _sustenanceGroup;

        public Stat Strength => _strength;
        public DefenseGroupStats DefenseGroup => _defenseGroup;

        public Stat Speed => _speed;
        public ViewRadiusStat ViewRadius => _viewRadius;


        public override void LoadBaseStats()
        {
            if (Owner.IsLoaded) return;
            if (BaseStats == null) return;

            // Health
            HealthGroup.Health.AddAgent(gameObject, baseStatsReason, BaseStats.MaxHealth, StatValueType.Base).ToNotClearable();
            HealthGroup.Health.ChangeStat.AddAgent(gameObject, baseStatsReason, BaseStats.HealthRegen, StatValueType.Base).ToNotClearable();
            HealthGroup.HealScale.AddAgent(gameObject, baseStatsReason, 1f, StatValueType.Base).ToNotClearable();
            HealthGroup.Load();

            // Stamina
            Stamina.AddAgent(gameObject, baseStatsReason, BaseStats.MaxStamina, StatValueType.Base).ToNotClearable();
            Stamina.ChangeStat.AddAgent(gameObject, baseStatsReason, BaseStats.StaminaRegen, StatValueType.Base).ToNotClearable();

            // Sustenance
            SustenanceGroup.Hunger.AddAgent(gameObject, baseStatsReason, BaseStats.MaxHunger, StatValueType.Base).ToNotClearable();
            SustenanceGroup.Hunger.ChangeStat.AddAgent(gameObject, baseStatsReason, -BaseStats.Hungry, StatValueType.Base).ToNotClearable();

            SustenanceGroup.Thirst.AddAgent(gameObject, baseStatsReason, BaseStats.MaxThirst, StatValueType.Base).ToNotClearable();
            SustenanceGroup.Thirst.ChangeStat.AddAgent(gameObject, baseStatsReason, -BaseStats.Thirsty, StatValueType.Base).ToNotClearable();

            SustenanceGroup.Breath.AddAgent(gameObject, baseStatsReason, BaseStats.MaxBreath, StatValueType.Base).ToNotClearable();
            SustenanceGroup.Breath.ChangeStat.AddAgent(gameObject, baseStatsReason, BaseStats.Breathe, StatValueType.Base).ToNotClearable();

            // Strength
            Strength.AddAgent(gameObject, baseStatsReason, BaseStats.Strength, StatValueType.Base).ToNotClearable();

            // Defense
            DefenseGroup.Armor.AddAgent(gameObject, baseStatsReason, BaseStats.Armor, StatValueType.Base).ToNotClearable();
            DefenseGroup.Resistance.AddAgent(gameObject, baseStatsReason, BaseStats.Resistance, StatValueType.Base).ToNotClearable();

            // Utilities
            Speed.AddAgent(gameObject, baseStatsReason, BaseStats.Speed, StatValueType.Base).ToNotClearable();
            ViewRadius.AddAgent(gameObject, baseStatsReason, BaseStats.ViewRadius, StatValueType.Base).ToNotClearable();
        }

        public override void UpdateStats(float deltaTime)
        {
            if (!IsStatsUpdating) return;
            
            HealthGroup.Update(deltaTime);
            Stamina.Update(deltaTime);
            SustenanceGroup.Update(deltaTime);

            DefenseGroup.Update(deltaTime);
        }

        public override void ClearStats(bool isForceClear = false)
        {
            HealthGroup.Clear(isForceClear);
            Stamina.Clear(isForceClear);
            SustenanceGroup.Clear(isForceClear);

            Strength.Clear(isForceClear);
            DefenseGroup.Clear(isForceClear);

            Speed.Clear(isForceClear);
            ViewRadius.Clear(isForceClear);
        }

        public override void ResetStats()
        {
            this.ClearStats();
            HealthGroup.Reset();
            Stamina.Reset();
            SustenanceGroup.Reset();

            Strength.Reset();
            DefenseGroup.Reset();

            Speed.Reset();
            ViewRadius.Reset();
        }

        /// <summary>
        /// Try to consume stamina before performing an action.
        /// </summary>
        /// <param name="requiredStamina">Stamina required to perform the action</param>
        /// <returns>True if enough stamina and deducted, otherwise false</returns>
        public virtual bool TryConsumeStamina(float requiredStamina)
        {
            if (Stamina.CurrentValue < requiredStamina)
                return false;

            Stamina.AddToCurrentValue(null, "self deduct", -requiredStamina);
            Stamina.ChangeInterval.CurrentTime = 2f;
            return true;
        }

    }
}