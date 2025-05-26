using Asce.Game.Stats;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureStats : MonoBehaviour, IHasOwner<Creature>, IStatsController<SO_CreatureBaseStats>, IHasSurvivalStats, IHasCombatStats, IHasUtilitiesStats
    {
        [SerializeField, HideInInspector] private Creature _owner;
        [SerializeField] private SO_CreatureBaseStats _baseStats;

        [Header("Survival")]
        [SerializeField] protected HealthStat _health = new();
        [SerializeField] protected StaminaStat _stamina = new();
        [SerializeField] protected HungerStat _hunger = new();
        [SerializeField] protected ThirstStat _thirst = new();
        [SerializeField] protected BreathStat _breath = new();

        [Header("Combat")]
        [SerializeField] protected Stat _strength = new();
        [SerializeField] protected Stat _armor = new();
        [SerializeField] protected Stat _resistance = new();
        [SerializeField] protected ShieldStat _shield = new();

        [Header("Utilities")]
        [SerializeField] protected Stat _speed = new();
        [SerializeField] protected ViewRadiusStat _viewRadius = new();

        /// <summary>
        ///     Reference to the creature that owns this stats controller.
        /// </summary>
        public virtual Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public virtual SO_CreatureBaseStats BaseStats => _baseStats;

        public HealthStat Health => _health;
        public StaminaStat Stamina => _stamina;
        public HungerStat Hunger => _hunger;
        public ThirstStat Thirst => _thirst;
        public BreathStat Breath => _breath;

        public Stat Strength => _strength;
        public Stat Armor => _armor;
        public Stat Resistance => _resistance;
        public ShieldStat Shield => _shield;

        public Stat Speed => _speed;
        public ViewRadiusStat ViewRadius => _viewRadius;

        protected virtual void Awake()
        {
            if (transform.LoadComponent(out _owner))
            {
                Owner.Stats = this;
            }
        }

        protected virtual void Start()
        {
            this.LoadBaseStats();
        }

        protected virtual void Update()
        {
            this.UpdateStats(Time.deltaTime);
        }


        public virtual void LoadBaseStats()
        {
            if (BaseStats == null) return;

            Health.AddAgent(gameObject, "base stats", BaseStats.MaxHealth, StatValueType.Plat).ToNotClearable();
            Health.AddToChangeValue(gameObject, "base stats", BaseStats.HealthRegen, StatValueType.Plat).ToNotClearable();

            Stamina.AddAgent(gameObject, "base stats", BaseStats.MaxStamina, StatValueType.Plat).ToNotClearable();
            Stamina.AddToChangeValue(gameObject, "base stats", BaseStats.StaminaRegen, StatValueType.Plat).ToNotClearable();

            Hunger.AddAgent(gameObject, "base stats", BaseStats.MaxHunger, StatValueType.Plat).ToNotClearable();
            Hunger.AddToChangeValue(gameObject, "base stats", -BaseStats.Hungry, StatValueType.Plat).ToNotClearable();

            Thirst.AddAgent(gameObject, "base stats", BaseStats.MaxThirst, StatValueType.Plat).ToNotClearable();
            Thirst.AddToChangeValue(gameObject, "base stats", -BaseStats.Thirsty, StatValueType.Plat).ToNotClearable();

            Breath.AddAgent(gameObject, "base stats", BaseStats.MaxBreath, StatValueType.Plat).ToNotClearable();
            Breath.AddToChangeValue(gameObject, "base stats", BaseStats.Breathe, StatValueType.Plat).ToNotClearable();


            Strength.AddAgent(gameObject, "base stats", BaseStats.Strength, StatValueType.Plat).ToNotClearable();
            Armor.AddAgent(gameObject, "base stats", BaseStats.Armor, StatValueType.Plat).ToNotClearable();
            Resistance.AddAgent(gameObject, "base stats", BaseStats.Resistance, StatValueType.Plat).ToNotClearable();

            Speed.AddAgent(gameObject, "base stats", BaseStats.Speed, StatValueType.Plat).ToNotClearable();
            ViewRadius.AddAgent(gameObject, "base stats", BaseStats.ViewRadius, StatValueType.Plat).ToNotClearable();
        }

        public virtual void UpdateStats(float deltaTime)
        {
            Health.Update(deltaTime);
            Stamina.Update(deltaTime);
            Hunger.Update(deltaTime);
            Thirst.Update(deltaTime);
            Breath.Update(deltaTime);

            Strength.Update(deltaTime);
            Armor.Update(deltaTime);
            Resistance.Update(deltaTime);
            Shield.Update(deltaTime);

            Speed.Update(deltaTime);
            ViewRadius.Update(deltaTime);
        }

        public virtual void ResetStats(bool isForceClear = false)
        {
            Health.Clear(isForceClear);
            Stamina.Clear(isForceClear);
            Hunger.Clear(isForceClear);
            Thirst.Clear(isForceClear);
            Breath.Clear(isForceClear);

            Strength.Clear(isForceClear);
            Armor.Clear(isForceClear);    
            Resistance.Clear(isForceClear);
            Shield.Clear(isForceClear);

            Speed.Clear(isForceClear);
            ViewRadius.Clear(isForceClear);
        }
    }
}