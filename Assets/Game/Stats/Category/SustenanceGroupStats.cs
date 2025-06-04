using System;
using UnityEngine;

namespace Asce.Game.Stats
{
    /// <summary>
    ///     Represents a group of sustenance-related stats for an entity, including hunger, thirst, and breath.
    /// </summary>
    [Serializable]
    public class SustenanceGroupStats : IGroupStats 
    {
        [Tooltip("The stat representing the entity's hunger level, which decreases over time.")]
        [SerializeField] protected TimeBasedResourceStat _hunger = new(StatType.Hunger);

        [Tooltip("The stat representing the entity's thirst level, which decreases over time.")]
        [SerializeField] protected TimeBasedResourceStat _thirst = new(StatType.Thirst);

        [Tooltip("The stat representing the entity's breath level, which may decrease or recover depending on the environment.")]
        [SerializeField] protected TimeBasedResourceStat _breath = new(StatType.Breath);


        /// <summary>
        ///     Gets the hunger stat.
        /// </summary>
        public TimeBasedResourceStat Hunger => _hunger;


        /// <summary>
        ///     Gets the thirst stat.
        /// </summary>
        public TimeBasedResourceStat Thirst => _thirst;

        /// <summary>
        ///     Gets the breath stat.
        /// </summary>
        public TimeBasedResourceStat Breath => _breath;

        public SustenanceGroupStats()
        {
            _hunger.ChangeInterval.SetBaseTime(10f);
            _thirst.ChangeInterval.SetBaseTime(10f);
        }

        public virtual void Update(float deltaTime)
        {
            Hunger.Update(deltaTime);
            Thirst.Update(deltaTime);
            Breath.Update(deltaTime);
        }

        public virtual void Clear(bool isForceClear = false)
        {
            Hunger.Clear(isForceClear);
            Thirst.Clear(isForceClear);
            Breath.Clear(isForceClear);
        }

        public virtual void Reset()
        {
            Hunger.Reset();
            Thirst.Reset();
            Breath.Reset();
        }
    }
}