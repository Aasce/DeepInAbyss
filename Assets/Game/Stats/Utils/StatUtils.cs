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
        public static float CalculateValue(List<StatAgent> agents, out float plat, out float ratio)
        {
            plat = 0f;
            ratio = 1f;

            if (agents == null) return 0f;

            foreach (StatAgent agent in agents)
            {
                if (agent == null) continue;
                switch (agent.ValueType)
                {
                    case StatValueType.Plat:
                        plat += agent.Value;
                        break;
                    case StatValueType.Ratio:
                        ratio *= 1 + agent.Value; // multiplicative stacking
                        break;
                }
            }

            return plat * ratio;
        }

        /// <summary>
        ///     Updates the duration of all active agents
        ///     and removes expired ones.
        /// </summary>
        /// <param name="agents"> Agents list need to update. </param>
        /// <param name="deltaTime"> The elapsed time since the last update. </param>
        /// <returns> Returns true if <paramref name="agents"/> have any agent is removed. </returns>
        public static bool UpdateAgents(List<StatAgent> agents, float deltaTime)
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
        public static bool RemoveAllAgents(List<StatAgent> agents, GameObject author, string reason = null)
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

        /// <summary>
        ///     Removes all agents affecting the <paramref name="agents"/>.
        /// </summary>
        /// <param name="isForceClear">
        ///     If true, all agents are removed regardless of their <see cref="StatAgent.IsClearable"/> flag.
        ///     <br/>
        ///     If false, only agents with <see cref="StatAgent.IsClearable"/> == true (or null-safe) will be removed.
        /// </param>
        /// <returns> Returns true if <paramref name="agents"/> are clear or if an agent is removed. </returns>
        public static bool ClearAgents(List<StatAgent> agents, bool isForceClear = false)
        {
            if (isForceClear)
            {
                agents.Clear();
                return true;
            }

            int count = agents.RemoveAll((agent) => agent?.IsClearable ?? true);
            return count > 0;
        }
    }
}
