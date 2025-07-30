using Asce.Game.Entities;
using System;
using UnityEngine;

namespace Asce.Game.Stats
{
    /// <summary>
    ///     Represents a group of health-related stats for an entity, including health and healing scale.
    /// </summary>
    [Serializable]
    public class HealthGroupStats : IGroupStats
    {
        private readonly string _healScaleAffectReason = "heal scale affect";

        [Tooltip("The health stat that represents the entity's health.")]
        [SerializeField] protected TimeBasedResourceStat _health = new(StatType.Health);

        [Tooltip("The stat that scales incoming healing effects.")]
        [SerializeField] protected Stat _healScale = new(StatType.HealthScale);

        /// <summary>
        ///     Event triggered whenever a healing action occurs.
        /// </summary>
        public event Action<object> OnHealing;

        public TimeBasedResourceStat Health => _health;
        public Stat HealScale => _healScale;
        protected StatAgent _healScaleAgent;

        public HealthGroupStats() 
        {
            HealScale.OnValueChanged += HealScale_OnValueChanged;
        }

        public virtual void Load()
        {
            _healScaleAgent = Health.ChangeStat.AddAgent(null, _healScaleAffectReason, HealScale.Value, StatValueType.Scale);
            _healScaleAgent.ToNotClearable();
        }

        public virtual void Update(float deltaTime)
        {
            Health.Update(deltaTime);
        }

        public virtual void Clear(bool isForceClear = false)
        {
            Health.Clear(isForceClear);
            HealScale.Clear(isForceClear);
        }

        public virtual void Reset()
        {
            Health.Reset();
            HealScale.Reset();
        }

        /// <summary>
        ///     Applies healing to the health stat, scaled by the current value of <see cref="HealScale"/>.
        /// </summary>
        /// <param name="healer"> The entity performing the healing. </param>
        /// <param name="reason"> A description or tag for the healing action. </param>
        /// <param name="healAmount"> The base amount of healing to apply. </param>
        /// <param name="type"> The type of value used when applying healing (flat or ratio). </param>
        /// <returns>
        ///     Returns final heal value.
        /// </returns>
        public virtual float Heal(IEntity healer, string reason, float healAmount, StatValueType type = StatValueType.Plat)
        {
            float heal = healAmount * HealScale.Value;
            Health.AddToCurrentValue(healer.gameObject, reason, heal, type);
            OnHealing?.Invoke(this);
            return heal;
        }

        /// <summary>
        ///     Callback method that responds to changes in the <see cref="HealScale"/> value.
        ///     <br/>
        ///     Updates the corresponding effect on the health stat.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="args"> Details of the value change event. </param>
        protected virtual void HealScale_OnValueChanged(object sender, Managers.ValueChangedEventArgs args)
        {
            if (_healScaleAgent == null)
            {
                _healScaleAgent = Health.ChangeStat.AddAgent(null, _healScaleAffectReason, HealScale.Value, StatValueType.Scale);
                _healScaleAgent.ToNotClearable();
                return;
            }

            _healScaleAgent.Value = HealScale.Value;
            _health.UpdateValue();
        }

    }
}