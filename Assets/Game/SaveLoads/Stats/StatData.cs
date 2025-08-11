using Asce.Game.Stats;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class StatData : SaveData, ISaveData<Stat>, ILoadData<Stat>
    {
        public StatType type = StatType.None;
        public List<StatAgentData> statAgents = new();
        public List<StatAgentData> changeStatAgents = new();
        public float currentValue;

        public void Save(in Stat target)
        {
            if (target == null) return;
            type = target.StatType;

            this.SaveAgents(target, statAgents);
            if (target is ResourceStat resourceTarget) currentValue = resourceTarget.CurrentValue;
            if (target is TimeBasedResourceStat timeBasedTarget) this.SaveAgents(timeBasedTarget.ChangeStat, changeStatAgents);
        }

        public bool Load(Stat target)
        {
            if (target == null) return false;
            target.StatType = type;

            this.LoadAgents(target, statAgents);
            if (target is ResourceStat resourceTarget) resourceTarget.SetCurrentValue(null, "", currentValue);
            if (target is TimeBasedResourceStat timeBasedTarget) this.LoadAgents(timeBasedTarget.ChangeStat, changeStatAgents);

            return true;
        }

        private void SaveAgents(in Stat target, List<StatAgentData> statAgents)
        {
            foreach (StatAgent agent in target.Agents)
            {
                if (agent == null) continue;
                if (agent.Reason.Contains("self")) continue; // Not save self agent
                if (agent.Reason.Contains("base")) continue; // Not save base agent
                if (agent.Reason.Contains("effect")) continue; // Not save status effect agent
                if (agent.Reason.Contains("equipment")) continue; // Not save equipment agent
                StatAgentData data = new();
                data.Save(agent);
                statAgents.Add(data);
            }
        }

        private void LoadAgents(Stat target, List<StatAgentData> statAgents)
        {
            foreach (StatAgentData data in statAgents)
            {
                if (data == null) continue;
                StatAgent agent = data.Create();

                target.AddAgent(agent);
            }
        }
    }
}