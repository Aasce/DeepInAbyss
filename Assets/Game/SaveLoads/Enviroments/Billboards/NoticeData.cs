using Asce.Game.Enviroments;
using Asce.Game.Quests;
using Asce.Managers.SaveLoads;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class NoticeData : SaveData, ISaveData<Notice>, ICreateData<Notice>
    {
        public string name = string.Empty;
        public bool isQuestInActive = false;

        public void Save(in Notice target)
        {
            if (target == null) return;
            if (target.Quest.IsNull()) return;

            isQuestInActive = QuestsManager.Instance.ActiveQuests.Contains(target.Quest);
            name = target.Quest.Information.Name;
        }

        public Notice Create()
        {
            Notice notice = new();
            if (isQuestInActive)
            {
                foreach (Quest q in QuestsManager.Instance.ActiveQuests)
                {
                    if (q.IsNull()) continue;
                    if (q.Information.Name == name)
                    {
                        notice.SetQuest(q);
                        return notice;
                    }
                }
            }

            Quest newQuest = QuestsManager.Instance.CreateQuest(name);
            notice.SetQuest(newQuest);

            return notice;
        }
    }
}