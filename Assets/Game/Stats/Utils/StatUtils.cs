using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Stats
{
    public static class StatUtils
    {
        /// <summary>
        ///     Calculates the <paramref name="agents"/>'s value based on all currently active agents.
        /// </summary>
        /// <param name="agents"></param>
        /// <param name="plat"> Total plat value </param>
        /// <param name="ratio"> Total ratio value, by multiplicative stacking </param>
        /// <returns> Returns total value of <paramref name="agents"/> </returns>
        public static float CalculateValue<T>(ICollection<T> agents, out float plat, out float ratio, out float scale) where T : StatAgent
        {
            plat = 0f;
            ratio = 1f;
            scale = 1f;

            if (agents == null) return 0f;

            foreach (T agent in agents)
            {
                if (agent == null) continue;
                switch (agent.ValueType)
                {
                    case StatValueType.Base:
                    case StatValueType.Plat:
                        plat += agent.Value;
                        break;

                    case StatValueType.Ratio:
                        ratio *= 1 + agent.Value; // multiplicative stacking
                        break;

                    case StatValueType.Scale:
                        scale *= agent.Value;
                        break;
                }
            }

            return plat * ratio * scale;
        }

        /// <summary>
        ///     Updates the duration of all active agents
        ///     and removes expired ones.
        /// </summary>
        /// <param name="agents"> Agents list need to update. </param>
        /// <param name="deltaTime"> The elapsed time since the last update. </param>
        /// <returns> Returns true if <paramref name="agents"/> have any agent is removed. </returns>
        public static bool UpdateAgents<T>(IList<T> agents, float deltaTime) where T : StatAgent
        {
            bool updated = false;
            for (int i = agents.Count - 1; i >= 0; i--)
            {
                agents[i].Duration.Update(deltaTime);
                if (agents[i].Duration.IsComplete)
                {
                    agents.RemoveAt(i);
                    updated = true;
                }
            }

            return updated;
        }


        /// <summary>
        ///     Removes all agents from the stat that match 
        ///     the given <paramref name="author"/> and <paramref name="reason"/>.
        /// </summary>
        /// <param name="agents"> Agents list need to remove. </param>
        /// <param name="author"> The source of the agents. </param>
        /// <param name="reason"> The reason of the agents (optional). </param>
        /// <returns> Returns true if <paramref name="agents"/> have any agent is removed. </returns>
        public static bool RemoveAllAgents<T>(List<T> agents, GameObject author, string reason = null) where T : StatAgent
        {
            int count = agents.RemoveAll((agent) =>
            {
                if (agent == null) return true; // Remove if agent is null
                if (agent.Author != author) return false; // Keep if the author doesn't match
                if (string.IsNullOrEmpty(reason)) return true; // If no specific reason is given, remove agent with matching author
                if (agent.Reason.Equals(reason)) return true; // Remove if the reason matches
                return false; // Otherwise, keep the agent
            });

            return count > 0;
        }

        public static T FindAgent<T>(List<T> agent,  GameObject author, string reason = null) where T : StatAgent
        {
            if (agent == null || agent.Count == 0) return null;
            return agent.Find((agent) =>
            {
                if (agent == null) return false; 
                if (agent.Author != author) return false; // Not the agent to find because the author doesn't match
                if (string.IsNullOrEmpty(reason)) return true; // If no specific reason is given and matching author
                if (agent.Reason.Equals(reason)) return true; // Reason matches
                return false; // Otherwise, not this agent
            });
        }


        /// <summary>
        ///     Removes all agents affecting the <paramref name="agents"/>.
        /// </summary>
        /// <param name="isForceClear">
        ///     If true, all agents are removed regardless of their <see cref="StatAgent.IsClearable"/> flag.
        ///     <br/>
        ///     If false, only agents with <see cref="StatAgent.IsClearable"/> == true (or null-safe) will be removed.
        /// </param>
        /// <returns> Returns true if <paramref name="agents"/> are clear or if an agent is removed. </returns>
        public static bool ClearAgents<T>(List<T> agents, bool isForceClear = false) where T : StatAgent
        {
            if (isForceClear)
            {
                agents.Clear();
                return true;
            }

            // remove if type is not Base
            int count = agents.RemoveAll((agent) => agent == null || (agent.IsClearable && agent.ValueType != StatValueType.Base));
            return count > 0;
        }

        /// <summary>
        ///     Finds an agent in the list that matches the specified condition and applies a given action to it.
        /// </summary>
        /// <typeparam name="T"> The type of agent, which must inherit from <see cref="StatAgent"/>. </typeparam>
        /// <param name="agents"> The list of agents to search. </param>
        /// <param name="match"> The predicate that defines the conditions of the agent to find. </param>
        /// <param name="setAction"> The action to perform on the found agent if one is found. </param>
        /// <returns> 
        ///     True if a matching agent was found and the action was invoked. otherwise, false.
        /// </returns>
        public static bool SetAgent<T>(List<T> agents, Predicate<T> match, Action<T> setAction) where T : StatAgent
        {
            T foundAgent = agents.Find(match);
            if (foundAgent != null)
            {
                setAction?.Invoke(foundAgent);
                return true;
            }

            return false;
        }

    }
}
