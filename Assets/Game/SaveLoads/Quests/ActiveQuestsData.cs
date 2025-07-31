using Asce.Game.Quests;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class ActiveQuestsData : SaveData
    {
        public List<QuestData> quests = new();


        public ActiveQuestsData() 
        {
            foreach(Quest quest in QuestsManager.Instance.ActiveQuests)
            {
                if (quest == null) continue;
                QuestData data = new();
                data.Save(quest);
                this.quests.Add(data);
            }
        }


        public void Load()
        {
            List<Quest> quests = new();
            foreach(QuestData data in this.quests)
            {
                if (data == null) continue;
                Quest quest = data.Create();
                quests.Add(quest);
            }

            if (QuestsManager.Instance is IReceiveData<List<Quest>> receiver)
            {
                receiver.Receive(quests);
            }
        }
    }
}