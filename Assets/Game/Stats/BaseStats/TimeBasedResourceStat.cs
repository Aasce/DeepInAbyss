using Asce.Managers;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ReadOnlyCollection<StatAgent> _readOnlyChangeAgents;

        [SerializeField] protected bool _isSelfChangeable = true;
        [SerializeField] protected Cooldown _changeInterval = new(baseChangeInterval);

        protected float _platChangeValue;
        protected float _ratioChangeValue;
        protected float _scaleChangeValue;

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

        public ReadOnlyCollection<StatAgent> ChangeAgents => _readOnlyChangeAgents ??= _changeAgents.AsReadOnly();

        public TimeBasedResourceStat() : base() { }

        /// <summary>
        ///     Updates all stat agents and removes expired ones.
        ///     Updates the cooldown timer and applies change if interval is complete.
        /// </summary>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (StatUtils.UpdateAgents(_changeAgents, deltaTime))
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

        public override void Reset()
        {
            base.Reset();
            this.ClearAgents();
        }

        public virtual StatAgent AddToChangeValue(GameObject author, string reason, float value, StatValueType type = StatValueType.Plat, float duration = float.PositiveInfinity, Vector2 position = default)
        {
            StatAgent agent = new StatAgent(author, reason, value, type, duration, position);
            return this.AddToChangeValue(agent);
        }

        public StatAgent AddToChangeValue(StatAgent agent)
        {
            if (agent == null) return null;
            _changeAgents.Add(agent);
            this.UpdateChangeValue();
            return agent;
        }

        public virtual StatAgent RemoveFromChangeValue(StatAgent agent)
        {
            if (agent == null) return null;

            bool isRemoved = _changeAgents.Remove(agent);
            if (isRemoved)
            {
                this.UpdateChangeValue();
                return agent;
            }
            return null;
        }

        /// <summary>
        ///     Removes all agents that match the given author and reason.
        ///     Recalculates <see cref="_changeAgents"/> if any were removed.
        /// </summary>
        public virtual void RemoveAllFromChangeValue(GameObject author, string reason = null)
        {
            if (StatUtils.RemoveAllAgents(_changeAgents, author, reason))
                this.UpdateChangeValue();
        }

        /// <summary>
        ///     Removes all agents affecting the time-based change.
        /// </summary>
        public virtual void ClearChangeAgents(bool forceClear = false)
        {
            if (StatUtils.ClearAgents(_changeAgents, forceClear))
                this.UpdateChangeValue();
        }

        /// <summary>
        ///     Set the value of the agent that find by <paramref name="match"/> in <see cref="_changeAgents"/>.
        ///     <br/>
        ///     Recalculates if change successfully.
        /// </summary>
        /// <param name="match"> The predicate that defines the conditions of the agent to find. </param>
        /// <param name="value"> The value to be set the agent. </param>
        /// <returns> 
        ///     True if a matching agent was found and the action was invoked. otherwise, false.
        /// </returns>
        public virtual bool SetChangeAgentValue(Predicate<StatAgent> match, float value)
        {
            bool isChanged = StatUtils.SetAgent(_changeAgents, match, (foundAgent) => foundAgent.Value = value);
            if (isChanged) UpdateChangeValue();
            return isChanged;
        }

        /// <summary>
        ///     Set the agent that find by <paramref name="match"/> in <see cref="_changeAgents"/> by <paramref name="setAction"/>
        ///     <br/>
        ///     Recalculates if change successfully.
        /// </summary>
        /// <param name="match"> The predicate that defines the conditions of the agent to find. </param>
        /// <param name="setAction"> The action to perform on the found agent if one is found. </param>
        /// <returns> 
        ///     True if a matching agent was found and the action was invoked. otherwise, false.
        /// </returns>
        public virtual bool SetChangeAgentData(Predicate<StatAgent> match, Action<StatAgent> setAction)
        {
            bool isChanged = StatUtils.SetAgent(_changeAgents, match, setAction);
            if (isChanged) this.UpdateValue();
            return isChanged;
        }


        /// <summary>
        ///     Recalculates the total value applied per interval, based on all agents.
        /// </summary>
        protected virtual void UpdateChangeValue()
            => ChangeValue = StatUtils.CalculateValue(_changeAgents, out _platChangeValue, out _ratioChangeValue, out _scaleChangeValue);
    }
}
