using Asce.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Stats
{
    /// <summary>
    ///     Represents a stat value that can be modified over time by external agents.
    /// </summary>
    [Serializable]
    public class Stat
    {
        [SerializeField] protected float _value;
        [SerializeField] protected List<StatAgent> _agents = new();

        protected float _platValue;
        protected float _ratioValue;

        /// <summary>
        ///     Invoke when the <see cref="Value"/> changes.
        /// </summary>
        public event Action<object, ValueChangedEventArgs> OnValueChanged;


        /// <summary>
        ///     The calculated value of the stat after applying all active agents.
        /// </summary>
        public virtual float Value
        {
            get => _value;
            protected set
            {
                float oldValue = _value;
                _value = value;
                OnValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, _value));
            }
        }

        /// <summary>
        ///     The list of agents currently affecting the stat.
        /// </summary>
        protected List<StatAgent> Agents => _agents;


        /// <summary>
        ///     Initializes a new instance of the <see cref="Stat"/> class.
        /// </summary>
        public Stat() { }


        /// <summary>
        ///     Updates the duration of all active agents, and recalculates <see cref="Value"/> 
        ///     if any agents have expired.
        ///     <br/>
        ///     See <see cref="StatUtils.UpdateAgents"/>
        /// </summary>
        /// <param name="deltaTime"> The elapsed time since the last update. </param>
        public virtual void Update(float deltaTime)
        {
            if (StatUtils.UpdateAgents(Agents, deltaTime))
                this.UpdateValue();
        }

        /// <summary>
        ///     Adds a new agent to the stat using specified parameters.
        /// </summary>
        /// <param name="author"> The source of the agent.</param>
        /// <param name="reason"> The reason the agent is applied. </param>
        /// <param name="value"> The amount the agent affects the stat. </param>
        /// <param name="type"> The type of value (flat or ratio). </param>
        /// <param name="duration"> (Optional) The duration the agent remains active. </param>
        /// <param name="position"> (Optional) The position affect </param>
        /// <returns> The added <see cref="StatAgent"/>. </returns>
        public virtual StatAgent AddAgent(GameObject author, string reason, float value, StatValueType type = StatValueType.Plat, float duration = float.PositiveInfinity, Vector2 position = default)
        {
            StatAgent agent = new StatAgent(author, reason, value, type, duration, position);
            return this.AddAgent(agent);
        }

        /// <summary>
        ///     Adds an existing agent to the stat.
        ///     <br/>
        ///     Recalculates <see cref="Value"/> if <paramref name="agent"/> is added.
        /// </summary>
        /// <param name="agent"> The agent to add. </param>
        /// <returns> The added agent. </returns>
        public virtual StatAgent AddAgent(StatAgent agent)
        {
            if (agent == null) return null;
            Agents.Add(agent);
            this.UpdateValue();

            return agent;
        }

        /// <summary>
        ///     Removes a single agent from the stat that 
        ///     matches the given <paramref name="author"/> and <paramref name="reason"/>.
        /// </summary>
        /// <param name="author"> The source of the agent. </param>
        /// <param name="reason"> The reason of the agent (optional). </param>
        /// <returns> The removed agent, or null if not found. </returns>
        public virtual StatAgent RemoveAgent(GameObject author, string reason = null)
        {
            StatAgent removingAgent = Agents.Find((agent) =>
            {
                if (agent.Author != author) return false;
                if (string.IsNullOrEmpty(reason)) return true;
                if (agent.Reason.Equals(reason)) return true;
                return false;
            });

            return RemoveAgent(removingAgent);
        }

        /// <summary>
        ///     Removes the specified <paramref name="agent"/> from the stat.
        ///     <br/>
        ///     Recalculates <see cref="Value"/> if <paramref name="agent"/> is removed.
        /// </summary>
        /// <param name="agent"> The agent to remove. </param>
        /// <returns> The removed agent, or null if not found. </returns>
        public virtual StatAgent RemoveAgent(StatAgent agent)
        {
            if (agent == null) return null;

            bool isRemoved = Agents.Remove(agent);
            if (isRemoved)
            {
                this.UpdateValue();
                return agent;
            }
            return null;
        }

        /// <summary>
        ///     Removes all agents from the stat that match 
        ///     the given <paramref name="author"/> and <paramref name="reason"/>. 
        ///     See <see cref="StatUtils.RemoveAllAgents"/>
        ///     <br/>
        ///     Recalculates <see cref="Value"/> if any agents is removed
        /// </summary>
        /// <param name="author"> The source of the agents. </param>
        /// <param name="reason"> The reason of the agents (optional). </param>
        public virtual void RemoveAllAgents(GameObject author, string reason = null)
        {
            if (StatUtils.RemoveAllAgents(Agents, author, reason)) 
                this.UpdateValue();
        }

        /// <summary>
        ///     Removes all agents affecting the stat. 
        ///     See <see cref="StatUtils.ClearAgents"/>
        /// </summary>
        public virtual void Clear(bool forceClear = false)
        {
            if (StatUtils.ClearAgents(Agents, forceClear))
                this.UpdateValue();
        }

        /// <summary>
        ///     Recalculates the stat's value based on all currently active agents.
        ///     See <see cref="StatUtils.CalculateValue"/>
        /// </summary>
        protected virtual void UpdateValue() 
            => Value = StatUtils.CalculateValue(Agents, out _platValue, out _ratioValue);

    }
}