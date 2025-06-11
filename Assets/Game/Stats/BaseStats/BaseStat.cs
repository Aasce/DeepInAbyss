using Asce.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.Stats
{

    /// <summary>
    ///     Represents a stat value that can be modified over time by external agents.
    /// </summary>
    [Serializable]
    public abstract class BaseStat<T> where T : StatAgent
    {
        [SerializeField] protected StatType _statType = StatType.None;
        [SerializeField] protected float _value;
        [SerializeField] protected List<T> _agents = new();
        private ReadOnlyCollection<T> _readOnlyAgents;

        protected float _platValue;
        protected float _ratioValue;
        protected float _scaleValue;

        /// <summary>
        ///     Invoke when the <see cref="Value"/> changes.
        /// </summary>
        public event Action<object, ValueChangedEventArgs> OnValueChanged;

        public virtual StatType StatType 
        {
            get => _statType;
            set => _statType = value;
        }

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

        public ReadOnlyCollection<T> Agents => _readOnlyAgents ??= _agents.AsReadOnly();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Stat"/> class.
        /// </summary>
        public BaseStat() { }
        public BaseStat(StatType type) 
        { 
            StatType = type;
        }


        /// <summary>
        ///     Updates the duration of all active agents, and recalculates <see cref="Value"/> 
        ///     if any agents have expired.
        ///     <br/>
        ///     See <see cref="StatUtils.UpdateAgents"/>
        /// </summary>
        /// <param name="deltaTime"> The elapsed time since the last update. </param>
        public virtual void Update(float deltaTime)
        {
            if (StatUtils.UpdateAgents(_agents, deltaTime))
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
        public virtual T AddAgent(GameObject author, string reason, float value, StatValueType type = StatValueType.Plat, float duration = float.PositiveInfinity, Vector2 position = default)
        {
            T agent = CreateAgent(author, reason, value, type, duration, position);
            return this.AddAgent(agent);
        }

        /// <summary>
        ///     Adds an existing agent to the stat.
        ///     <br/>
        ///     Recalculates <see cref="Value"/> if <paramref name="agent"/> is added.
        /// </summary>
        /// <param name="agent"> The agent to add. </param>
        /// <returns> The added agent. </returns>
        public virtual T AddAgent(T agent)
        {
            if (agent == null) return null;
            _agents.Add(agent);
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
            T removingAgent = _agents.Find((agent) =>
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
        public virtual T RemoveAgent(T agent)
        {
            if (agent == null) return null;

            bool isRemoved = _agents.Remove(agent);
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
            if (StatUtils.RemoveAllAgents(_agents, author, reason))
                this.UpdateValue();
        }

        public virtual T FindAgents(GameObject author, string reason = null)
        {
            return StatUtils.FindAgent(_agents, author, reason);
        }

        /// <summary>
        ///     Removes all agents affecting the stat. 
        ///     See <see cref="StatUtils.ClearAgents"/>
        /// </summary>
        public virtual void Clear(bool forceClear = false)
        {
            this.ClearAgents(forceClear);
        }

        /// <summary>
        ///     Removes all agents affecting the base stats.
        ///     See <see cref="StatUtils.ClearAgents"/>
        /// </summary>
        /// <param name="forceClear"></param>
        public void ClearAgents(bool forceClear = false)
        {
            if (StatUtils.ClearAgents(_agents, forceClear))
                this.UpdateValue();
        }

        public virtual void Reset()
        {
            this.ClearAgents();
        }

        /// <summary>
        ///     Set the value of the agent that find by <paramref name="match"/> in <see cref="_agents"/>.
        ///     <br/>
        ///     Recalculates if change successfully.
        /// </summary>
        /// <param name="match"> The predicate that defines the conditions of the agent to find. </param>
        /// <param name="value"> The value to be set the agent. </param>
        /// <returns> 
        ///     True if a matching agent was found and the action was invoked. otherwise, false.
        /// </returns>
        public virtual bool SetAgentValue (Predicate<T> match, float value)
        {
            bool isChanged = StatUtils.SetAgent(_agents, match, (foundAgent) => foundAgent.Value = value);
            if (isChanged) this.UpdateValue();
            return isChanged;
        }

        /// <summary>
        ///     Set the agent that find by <paramref name="match"/> in <see cref="_agents"/> by <paramref name="setAction"/>
        ///     <br/>
        ///     Recalculates if change successfully.
        /// </summary>
        /// <param name="match"> The predicate that defines the conditions of the agent to find. </param>
        /// <param name="setAction"> The action to perform on the found agent if one is found. </param>
        /// <returns> 
        ///     True if a matching agent was found and the action was invoked. otherwise, false.
        /// </returns>
        public virtual bool SetAgentData(Predicate<T> match, Action<T> setAction)
        {
            bool isChanged = StatUtils.SetAgent(_agents, match, setAction);
            if (isChanged) this.UpdateValue();
            return isChanged;
        }

        /// <summary>
        ///     Recalculates the stat's value based on all currently active agents.
        ///     See <see cref="StatUtils.CalculateValue"/>
        /// </summary>
        protected virtual void UpdateValue()
            => Value = StatUtils.CalculateValue(_agents, out _platValue, out _ratioValue, out _scaleValue);

        protected abstract T CreateAgent(GameObject author, string reason, float value, StatValueType type, float duration, Vector2 position);
    }
}