using Asce.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Stats
{
    /// <summary>
    ///     A stat that represents a resource with a modifiable current value, 
    ///     such as health, stamina,...
    /// </summary>
    [Serializable]
    public class ResourceStat : Stat
    {
        /// <summary>
        ///     The default duration (in seconds) that a value-affecting agent remains active.
        /// </summary>
        public static readonly float baseAffectDuration = 10f;

        [Header("Resource Stat")]
        [SerializeField] protected float _currentValue;

        private ReadOnlyCollection<StatAgent> _readOnlyCurrentAgents;

        /// <summary>
        ///     Invoked when the <see cref="CurrentValue"/> changes.
        /// </summary>
        public event Action<object, ValueChangedEventArgs> OnCurrentValueChanged;


        /// <summary>
        ///     The max value of the resource stat.
        /// </summary>
        public override float Value 
        { 
            get => base.Value; 
            protected set
            {
                base.Value = value;
                CurrentValue = CurrentValue;
            }
        }

        /// <summary>
        ///     The current value of the resource.
        /// </summary>
        public float CurrentValue
        {
            get => _currentValue;
            protected set
            {
                float newValue = Mathf.Clamp(value, 0f, Value);
                if (_currentValue == newValue) return;

                float oldValue = _currentValue;
                _currentValue = newValue;
                OnCurrentValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, _currentValue));
            }
        }

        /// <summary>
        ///     The normalized value of the resource (<see cref="CurrentValue"/> / <see cref="Value"/>), clamped between 0 and 1.
        /// </summary>
        public float Ratio => Value <= 0f ? 0f : CurrentValue / Value;

        /// <summary>
        ///     True if Value greater than or equals zero and CurrentValue greater than or equals Value.
        /// </summary>
        public bool IsFull => Value >= 0 && CurrentValue >= Value;

        /// <summary>
        ///     True if Value greater than or equals zero and CurrentValue less than or equals zero.
        /// </summary>
        public bool IsEmpty => Value >= 0 && CurrentValue <= 0f;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResourceStat"/> class.
        /// </summary>
        public ResourceStat() : base() { }
        public ResourceStat(StatType type) : base(type) { }

        /// <summary>
        ///     Adds a new <see cref="StatAgent"/> and applies its effect to <see cref="CurrentValue"/>.
        ///     <br/>
        ///     override from <see cref="Stat.AddAgent(StatAgent)"/>
        /// </summary>
        /// <param name="agent"> The agent to apply. </param>
        /// <returns> The applied agent. </returns>
        public override StatAgent AddAgent(StatAgent agent)
        {
            if (agent == null) return null;

            float oldValue = Value;
            base.AddAgent(agent);

            float deltaValue = Value - oldValue;
            this.AddToCurrentValue(agent.Author, agent.Reason, Mathf.Max(0f, deltaValue));
            return agent;
        }

        /// <summary>
        ///     Removes all agents affecting the stat and the current stat. 
        ///     See <see cref="StatUtils.ClearAgents"/>
        /// </summary>
        public override void Clear(bool forceClear = false)
        {
            base.Clear(forceClear);
        }

        public override void Reset()
        {
            base.Reset();

            this.ToFull(null, "Reset", false);
        }

        /// <summary>
        ///     Adds a value to <see cref="CurrentValue"/>, 
        ///     creating a temporary agent that lasts for <see cref="affectDuration"/>.
        /// </summary>
        /// <param name="author"> The source of the change. </param>
        /// <param name="reason"> The reason for the change. </param>
        /// <param name="value"> The value to add. </param>
        /// <param name="type"> (Optionals) The type of value (flat or ratio). </param>
        public virtual void AddToCurrentValue(GameObject author, string reason, float value, StatValueType type = StatValueType.Plat)
        {
            CurrentValue = type switch
            {
                StatValueType.Plat => CurrentValue + value,
                StatValueType.Ratio => CurrentValue + Value * value,
                _ => 0f,
            };
        }

        /// <summary>
        ///     Sets <see cref="CurrentValue"/> directly to the given value.
        /// </summary>
        /// <param name="author"> The source of the change. </param>
        /// <param name="reason"> The reason for the change. </param>
        /// <param name="value"> The new value to set. </param>
        public virtual void SetCurrentValue(GameObject author, string reason, float value)
        {
            CurrentValue = value;
        }

        /// <summary>
        ///     Sets <see cref="CurrentValue"/> to zero.
        ///     <br/>
        ///     see <see cref="SetCurrentValue"/>
        /// </summary>
        /// <param name="author"> The source of the change. </param>
        /// <param name="reason"> The reason for the change. </param>
        /// <param name="isAddToAgent"> (Optionals) If true, create agent and and to <see cref="_currentAgents"/> </param>
        public virtual void ToEmpty(GameObject author, string reason, bool isAddToAgent = true) => this.SetCurrentValue(author, reason, 0f);

        /// <summary>
        ///     Sets <see cref="CurrentValue"/> to the max <see cref="Value"/>.
        ///     <br/>
        ///     see <see cref="SetCurrentValue"/>
        /// </summary>
        /// <param name="author"> The source of the change. </param>
        /// <param name="reason"> The reason for the change. </param>
        /// <param name="isAddToAgent"> (Optionals) If true, create agent and and to <see cref="_currentAgents"/> </param>
        public virtual void ToFull(GameObject author, string reason, bool isAddToAgent = true) => this.SetCurrentValue(author, reason, Value);

    }
}
