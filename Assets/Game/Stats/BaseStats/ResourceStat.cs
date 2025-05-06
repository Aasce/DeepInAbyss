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
        public readonly float affectDuration = 10f;

        [Header("ResourceStat")]
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
        ///     A list of active <see cref="StatAgent"/>s currently affecting the <see cref="CurrentValue"/>.
        /// </summary>
        protected List<StatAgent> CurrentAgents => _currentAgents;


        /// <summary>
        ///     Updates all stat agents and removes expired ones.
        ///     <br/>
        ///     override from <see cref="Stat.Update(float)"/>
        /// </summary>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = CurrentAgents.Count - 1; i >= 0; i--)
            {
                CurrentAgents[i].Duration.Update(deltaTime);
                if (CurrentAgents[i].Duration.IsComplete)
                {
                    CurrentAgents.RemoveAt(i);
                }
            }
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
        /// <param name="type"> The type of value (flat or ratio). </param>
        public virtual void AddToCurrentValue(GameObject author, string reason, float value, StatValueType type = StatValueType.Plat)
        {
            CurrentAgents.Add(new StatAgent(author, reason, value, type, affectDuration));
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
            // The value is the difference between the value to be changed and the current value
            CurrentAgents.Add(new StatAgent(author, reason, value - CurrentValue, affectDuration));
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
