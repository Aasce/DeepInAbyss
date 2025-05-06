using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureStats : MonoBehaviour, IStatsController<SO_CreatureBaseStats>
    {
        [SerializeField] private SO_CreatureBaseStats _baseStats;

        [Header("Survival")]
        [SerializeField] protected ResourceStat _health = new();
        [SerializeField] protected ResourceStat _stamina = new();
        [SerializeField] protected ResourceStat _hunger = new();
        [SerializeField] protected ResourceStat _thirsty = new();

        [Header("Combat")]
        [SerializeField] protected Stat _strength = new();
        [SerializeField] protected Stat _armor = new();
        [SerializeField] protected Stat _resistance = new();

        [Header("Utilities")]
        [SerializeField] protected Stat _speed = new();


        public virtual SO_CreatureBaseStats BaseStats => _baseStats;

        public ResourceStat Health => _health;
        public ResourceStat Stamina => _stamina;
        public ResourceStat Hunger => _hunger;
        public ResourceStat Thirsty => _thirsty;

        public Stat Strength => _strength;
        public Stat Armor => _armor;
        public Stat Resistance => _resistance;

        public Stat Speed => _speed;


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

            Health.AddAgent(gameObject, "base stats", BaseStats.MaxHealth, StatValueType.Plat);
            Stamina.AddAgent(gameObject, "base stats", BaseStats.MaxStamina, StatValueType.Plat);
            Hunger.AddAgent(gameObject, "base stats", BaseStats.MaxHunger, StatValueType.Plat);
            Thirsty.AddAgent(gameObject, "base stats", BaseStats.MaxThirsty, StatValueType.Plat);

            Strength.AddAgent(gameObject, "base stats", BaseStats.Strength, StatValueType.Plat);
            Armor.AddAgent(gameObject, "base stats", BaseStats.Armor, StatValueType.Plat);
            Resistance.AddAgent(gameObject, "base stats", BaseStats.Resistance, StatValueType.Plat);


            Speed.AddAgent(gameObject, "base stats", BaseStats.Speed, StatValueType.Plat);
        }

        public virtual void UpdateStats(float deltaTime)
        {
            Health.Update(deltaTime);
            Stamina.Update(deltaTime);
            Hunger.Update(deltaTime);
            Thirsty.Update(deltaTime);

            Strength.Update(deltaTime);
            Armor.Update(deltaTime);
            Resistance.Update(deltaTime);

            Speed.Update(deltaTime);
        }

    }
}