using Asce.Game.Combats;
using Asce.Game.Stats;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureStats : MonoBehaviour, IHasOwner<Creature>, IStatsController<SO_CreatureBaseStats>, IHasSurvivalStats, IHasCombatStats, IHasUtilitiesStats
    {
        public static readonly string baseStatsReason = "base stats";

        [SerializeField, Readonly] private Creature _owner;
        [SerializeField] private SO_CreatureBaseStats _baseStats;

        [Space]
        [SerializeField] protected bool _isStatsUpdating = true;

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
        public virtual Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }
        public virtual bool IsDead => Owner.Status.IsDead;

        public virtual SO_CreatureBaseStats BaseStats => _baseStats;

        public virtual bool IsStatsUpdating
        {
            get => _isStatsUpdating;
            set => _isStatsUpdating = value;
        }

        public HealthGroupStats HealthGroup => _healthGroup;
        public StaminaStat Stamina => _stamina;
        public SustenanceGroupStats SustenanceGroup => _sustenanceGroup;

        public Stat Strength => _strength;
        public DefenseGroupStats DefenseGroup => _defenseGroup;

        public Stat Speed => _speed;
        public ViewRadiusStat ViewRadius => _viewRadius;


        protected virtual void Reset()
        {
            if (transform.LoadComponent(out _owner))
            {
                Owner.Stats = this;
            }
        }

        protected virtual void Start()
        {
            this.LoadBaseStats();
            Owner.Status.OnDeath += Owner_OnDeath;
            Owner.Status.OnRevive += Owner_OnRevive;
        }

        protected virtual void Update()
        {
            this.UpdateStats(Time.deltaTime);
        }


        public virtual void LoadBaseStats()
        {
            if (BaseStats == null) return;

            // Health
            HealthGroup.Health.AddAgent(gameObject, baseStatsReason, BaseStats.MaxHealth, StatValueType.Base).ToNotClearable();
            HealthGroup.Health.ChangeStat.AddAgent(gameObject, baseStatsReason, BaseStats.HealthRegen, StatValueType.Base).ToNotClearable();

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

        public virtual void UpdateStats(float deltaTime)
        {
            if (!IsStatsUpdating) return;
            
            HealthGroup.Update(deltaTime);
            Stamina.Update(deltaTime);
            SustenanceGroup.Update(deltaTime);

            DefenseGroup.Update(deltaTime);
        }

        public virtual void ClearStats(bool isForceClear = false)
        {
            HealthGroup.Clear(isForceClear);
            Stamina.Clear(isForceClear);
            SustenanceGroup.Clear(isForceClear);

            Strength.Clear(isForceClear);
            DefenseGroup.Clear(isForceClear);

            Speed.Clear(isForceClear);
            ViewRadius.Clear(isForceClear);
        }

        public virtual void ResetStats()
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

        protected virtual void Owner_OnDeath(object sender)
        {
            IsStatsUpdating = false;
        }

        protected virtual void Owner_OnRevive(object sender)
        {
            IsStatsUpdating = true;
            this.ResetStats();
        }

    }
}