using Asce.Managers;
using System;
using System.Collections.Generic;
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
        [SerializeField] protected List<StatAgent> _currentAgents = new();

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
        public float Ratio => Value <= 0f ? 0f:  CurrentValue / Value;

        /// <summary>
        ///     True if CurrentValue greater than or equals Value.
        /// </summary>
        public bool IsFull => CurrentValue >= Value;

        /// <summary>
        ///     True if CurrentValue less than or equals zero.
        /// </summary>
        public bool IsEmpty => CurrentValue <= 0f;


        /// <summary>
        ///     A list of active <see cref="StatAgent"/>s currently affecting the <see cref="CurrentValue"/>.
        /// </summary>
        protected List<StatAgent> CurrentAgents => _currentAgents;


        /// <summary>
        ///     Initializes a new instance of the <see cref="ResourceStat"/> class.
        /// </summary>
        public ResourceStat() : base() { }


        /// <summary>
        ///     Updates all stat agents and removes expired ones.
        ///     <br/>
        ///     override from <see cref="Stat.Update(float)"/>
        ///     <br/>
        ///     See <see cref="StatUtils.UpdateAgents"/>
        /// </summary>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            StatUtils.UpdateAgents(CurrentAgents, deltaTime);
        }


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
            CurrentAgents.Clear();
        }

        /// <summary>
        ///     Adds a value to <see cref="CurrentValue"/>, 
        ///     creating a temporary agent that lasts for <see cref="affectDuration"/>.
        /// </summary>
        /// <param name="author"> The source of the change. </param>
        /// <param name="reason"> The reason for the change. </param>
        /// <param name="value"> The value to add. </param>
        /// <param name="type"> (Optionals) The type of value (flat or ratio). </param>
        /// <param name="isAddToAgent"> (Optionals) If true, create agent and and to <see cref="CurrentAgents"/> </param>
        public virtual void AddToCurrentValue(GameObject author, string reason, float value, StatValueType type = StatValueType.Plat, bool isAddToAgent = true)
        {
            if (isAddToAgent) 
                CurrentAgents.Add(new StatAgent(author, reason, value, type, baseAffectDuration));

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
        /// <param name="isAddToAgent"> (Optionals) If true, create agent and and to <see cref="CurrentAgents"/> </param>
        public virtual void SetCurrentValue(GameObject author, string reason, float value, bool isAddToAgent = true)
        {
            // The value is the difference between the value to be changed and the current value
            if (isAddToAgent) 
                CurrentAgents.Add(new StatAgent(author, reason, value - CurrentValue, baseAffectDuration));

            CurrentValue = value;
        }

        /// <summary>
        ///     Sets <see cref="CurrentValue"/> to zero.
        /// </summary>
        /// <param name="author"> The source of the change. </param>
        /// <param name="reason"> The reason for the change. </param>
        public virtual void ToEmpty(GameObject author, string reason) => this.SetCurrentValue(author, reason, 0f);

        /// <summary>
        ///     Sets <see cref="CurrentValue"/> to the max <see cref="Value"/>.
        /// </summary>
        /// <param name="author"> The source of the change. </param>
        /// <param name="reason"> The reason for the change. </param>
        public virtual void ToFull(GameObject author, string reason) => this.SetCurrentValue(author, reason, Value);

    }
}
