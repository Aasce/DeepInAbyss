using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Items;
using Asce.Game.Players;
using Asce.Managers;
using Asce.Managers.SaveLoads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Asce.Game.Quests
{
    public class QuestsManager : MonoBehaviourSingleton<QuestsManager>, IReceiveData<List<Quest>>
    {
        [SerializeField] protected SO_QuestsData _questsData;
        [SerializeField] protected List<Quest> _activeQuests = new();
        protected ReadOnlyCollection<Quest> _readonlyActiveQuests;

        public event Action<object, Quest> OnQuestAdded;
        public event Action<object, Quest> OnQuestRemoved;
        public event Action<object, Quest> OnQuestComplete;

        public SO_QuestsData QuestsData => _questsData;
        public ReadOnlyCollection<Quest> ActiveQuests => _readonlyActiveQuests ??= _activeQuests.AsReadOnly();

        public void AcceptQuest(string questName)
        {
            Quest quest = this.CreateQuest(questName);
            this.AcceptQuest(quest);
        }

        public void AcceptQuest(Quest quest)
        {
            if (quest.IsNull()) return; 
            if (_activeQuests.Contains(quest))
            {
                return;
            }

            quest.RegisterEvent();

            _activeQuests.Add(quest);
            OnQuestAdded?.Invoke(this, quest);
        }

        public void UnacceptQuest(Quest quest)
        {
            if (quest.IsNull()) return;
            if (!_activeQuests.Remove(quest)) return;

            OnQuestRemoved?.Invoke(this, quest);
        }


        public void CompleteQuest(string questName)
        {
            if (string.IsNullOrEmpty(questName)) return;
            Quest quest = _activeQuests.Find(q => !q.IsNull() && q.Information.Name == questName);
            this.CompleteQuest(quest);
        }

        public void CompleteQuest(Quest quest)
        {
            if (quest.IsNull()) return;
            if (Player.Instance.ControlledCreature == null) return;
            if (!_activeQuests.Contains(quest)) return;

            if (quest.IsComplete())
            {
                List<Item> dropped = DroppedSpoilsSystem.GetDroppedItems(quest.Information.DroppedSpoils);
                if (Player.Instance.ControlledCreature.Inventory is CreatureInventory inventory)
                {
                    List<Item> remainings = inventory.Inventory.AddItems(dropped);
                    ItemObjectsManager.Instance.Spawns(remainings, Player.Instance.ControlledCreature.gameObject.transform.position);
                }

                quest.UnregisterEvent();
                OnQuestComplete?.Invoke(this, quest);
                this.UnacceptQuest(quest);
            }
        }

        public Quest CreateQuest(string questName)
        {
            if (string.IsNullOrEmpty(questName)) return null;
            if (QuestsData == null) return null;

            SO_QuestInformation information = QuestsData.GetQuestByName(questName);
            return this.CreateQuest(information);
        }

        public Quest CreateRandomQuest()
        {
            if (QuestsData == null) return null;
            if (QuestsData.Data.Count == 0) return null;

            int random = UnityEngine.Random.Range(0, _questsData.Data.Count);

            SO_QuestInformation information = QuestsData.Data[random];
            return this.CreateQuest(information);
        }

        public Quest CreateQuest(SO_QuestInformation information)
        {
            if (information == null) return null;

            Quest quest = new(information);
            return quest;
        }

        void IReceiveData<List<Quest>>.Receive(List<Quest> data)
        {
            if (data == null) return;
            foreach (Quest quest in data)
            {
                if (quest.IsNull()) continue;
                this.AcceptQuest(quest);
            }
        }
    }
}
