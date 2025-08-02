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
        [SerializeField] protected Stat _changeStat = new();

        [SerializeField] protected bool _isSelfChangeable = true;
        [SerializeField] protected Cooldown _changeInterval = new(baseChangeInterval);

        protected float _platChangeValue;
        protected float _ratioChangeValue;
        protected float _scaleChangeValue;

        public Stat ChangeStat => _changeStat;

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

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResourceStat"/> class.
        /// </summary>
        public TimeBasedResourceStat() : base() { }
        public TimeBasedResourceStat(StatType type) : base(type) { }

        /// <summary>
        ///     Updates all stat agents and removes expired ones.
        ///     Updates the cooldown timer and applies change if interval is complete.
        /// </summary>
        public virtual void Update(float deltaTime)
        {
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
            this.AddToCurrentValue(null, "time-based change", ChangeStat.Value, StatValueType.Plat);
        }

        /// <summary>
        ///     Removes all stat agents, including those affecting the time-based change.
        /// </summary>
        public override void Clear(bool forceClear = false)
        {
            base.Clear(forceClear);
            _changeStat.Clear(forceClear);
        }

        public override void Reset()
        {
            base.Reset();
            this.ClearAgents();
        }
    }
}
