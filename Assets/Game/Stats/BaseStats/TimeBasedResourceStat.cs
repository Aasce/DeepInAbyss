using Asce.Managers;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Stats
{
    /// <summary>
    ///     A ResourceStat whose <see cref="ResourceStat.CurrentValue"/> changes over time,
    ///     based on a configurable interval and change value.
    ///     <br/>
    ///     Used for stats like Hunger, Thirst, or Stamina regeneration.
    /// </summary>
    [Serializable]
    public class TimeBasedResourceStat : ResourceStat
    {
        public static readonly float baseChangeInterval = 1f;

        [Header("Time Based Stat")]
        [SerializeField] protected float _changeValue;
        [SerializeField] protected List<StatAgent> _changeAgents = new();

        [SerializeField] protected bool _isSelfChangeable = true;
        [SerializeField] protected Cooldown _changeInterval = new(baseChangeInterval);

        protected float _platChangeValue;
        protected float _ratioChangeValue;

        /// <summary>
        ///     Invoked when the <see cref="ChangeValue"/> changes.
        /// </summary>
        public event Action<object, ValueChangedEventArgs> OnChangeValueChanged;

        /// <summary>
        ///     The final value applied periodically to <see cref="ResourceStat.CurrentValue"/>.
        /// </summary>
        public virtual float ChangeValue
        {
            get => _changeValue;
            set
            {
                float oldValue = _changeValue;
                _changeValue = value;
                OnChangeValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, _changeValue));
            }
        }

        /// <summary>
        ///     If true, the change will be applied when ChangeInterval completes. 
        ///     Otherwise, the change will not be applied.
        /// </summary>
        public virtual bool IsSelfChangeable
        {
            get => _isSelfChangeable;
            set => _isSelfChangeable = value;
        }

        /// <summary>
        ///     The interval at which <see cref="ApplyChange"/> is invoked.
        /// </summary>
        public Cooldown ChangeInterval => _changeInterval;

        protected List<StatAgent> ChangeAgents => _changeAgents;

        public TimeBasedResourceStat() : base() { }

        /// <summary>
        ///     Updates all stat agents and removes expired ones.
        ///     Updates the cooldown timer and applies change if interval is complete.
        /// </summary>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (StatUtils.UpdateAgents(ChangeAgents, deltaTime))
                this.UpdateChangeValue();

            ChangeInterval.Update(deltaTime);
            if (IsSelfChangeable && ChangeInterval.IsComplete)
            {
                this.ApplyChange();
                ChangeInterval.Reset();
            }
        }

        /// <summary>
        ///     Applies the time-based change to <see cref="CurrentValue"/>.
        ///     Can be overridden to define specific logic (e.g., clamp).
        /// </summary>
        public virtual void ApplyChange()
        {
            this.AddToCurrentValue(null, "time-based change", ChangeValue, StatValueType.Plat, isAddToAgent: false);
        }

        /// <summary>
        ///     Removes all stat agents, including those affecting the time-based change.
        /// </summary>
        public override void Clear(bool forceClear = false)
        {
            base.Clear(forceClear);
            this.ClearChangeAgents(forceClear);
        }

        public virtual StatAgent AddToChangeValue(GameObject author, string reason, float value, StatValueType type = StatValueType.Plat, float duration = float.PositiveInfinity, Vector2 position = default)
        {
            StatAgent agent = new StatAgent(author, reason, value, type, duration, position);
            return this.AddToChangeValue(agent);
        }

        public StatAgent AddToChangeValue(StatAgent agent)
        {
            if (agent == null) return null;
            ChangeAgents.Add(agent);
            this.UpdateChangeValue();
            return agent;
        }

        public virtual StatAgent RemoveFromChangeValue(StatAgent agent)
        {
            if (agent == null) return null;

            bool isRemoved = ChangeAgents.Remove(agent);
            if (isRemoved)
            {
                this.UpdateChangeValue();
                return agent;
            }
            return null;
        }

        /// <summary>
        ///     Removes all agents that match the given author and reason.
        ///     Recalculates <see cref="ChangeValue"/> if any were removed.
        /// </summary>
        public virtual void RemoveAllFromChangeValue(GameObject author, string reason = null)
        {
            if (StatUtils.RemoveAllAgents(ChangeAgents, author, reason))
                this.UpdateChangeValue();
        }

        /// <summary>
        ///     Removes all agents affecting the time-based change.
        /// </summary>
        public virtual void ClearChangeAgents(bool forceClear = false)
        {
            if (StatUtils.ClearAgents(ChangeAgents, forceClear))
                this.UpdateChangeValue();
        }

        /// <summary>
        ///     Recalculates the total value applied per interval, based on all agents.
        /// </summary>
        protected virtual void UpdateChangeValue()
            => ChangeValue = StatUtils.CalculateValue(ChangeAgents, out _platChangeValue, out _ratioChangeValue);
    }
}
