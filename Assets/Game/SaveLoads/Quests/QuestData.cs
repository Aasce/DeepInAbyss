using Asce.Game.Quests;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class QuestData : SaveData, ISaveData<Quest>, ICreateData<Quest>
    {
        public string name;
        public List<QuestConditionStateData> conditionStates = new();

        public void Save(in Quest target)
        {
            if (target.IsNull()) return;
            name = target.Information.Name;

            foreach (QuestConditionState state in target.ConditionStates) 
            {
                QuestConditionStateData conditionStateData = new();
                conditionStateData.Save(state);

                conditionStates.Add(conditionStateData);
            }
        }

        public Quest Create()
        {
            Quest target = QuestsManager.Instance.CreateQuest(name);
            if (target == null) return null;

            foreach (QuestConditionState state in target.ConditionStates)
            {
                if (state == null) continue;
                if (state.Information == null) continue;
                QuestConditionStateData foundStateData = conditionStates.Find(stateData => stateData != null && stateData.name == state.Information.name);
                if (foundStateData == null) continue;
                foundStateData.Load(state);
            }

            return target;
        }

    }
}