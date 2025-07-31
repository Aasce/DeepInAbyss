using Asce.Game.Quests;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class QuestConditionStateData : SaveData, ISaveData<QuestConditionState>, ILoadData<QuestConditionState>
    {
        public string name;
        public int quantity;


        public void Save(in QuestConditionState target)
        {
            if (target == null) return;
            quantity = target.CurrentQuantity;
            name = target.Information.name;
        }

        public bool Load(QuestConditionState target)
        {
            if (target == null) return false;
            target.CurrentQuantity = quantity;

            return true;
        }
    }
}