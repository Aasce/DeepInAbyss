using Asce.Game.Quests;
using Asce.Managers.Pools;
using Asce.Managers.UIs;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Quests
{
    public class UIQuestsInformation : UIObject
    {
        [SerializeField] protected Button _toggleButton;
        [SerializeField] protected RectTransform _contentHolder;

        [Space]
        [SerializeField] protected Pool<UIQuest> _uiQuestsPool = new();
        [SerializeField] protected Pool<UIQuestConditionState> _uiQuestConditionPool = new();


        public Pool<UIQuestConditionState> UIQuestConditionPool => _uiQuestConditionPool;


        protected virtual void Start()
        {
            this.SyncQuests();
            QuestsManager.Instance.OnQuestAdded += QuestManager_OnQuestAdded;
            QuestsManager.Instance.OnQuestRemoved += QuestManager_OnQuestRemoved;
            if (_toggleButton != null) _toggleButton.onClick.AddListener(ToggleButton_OnClick);
        }


        protected virtual void SyncQuests()
        {
            foreach(Quest quest in QuestsManager.Instance.ActiveQuests)
            {
                if (quest.IsNull()) continue;

                UIQuest uiQuest = _uiQuestsPool.Activate();
                if (uiQuest == null) continue;

                uiQuest.Controller = this;
                uiQuest.Set(quest);
            }
        }


        protected virtual void QuestManager_OnQuestAdded(object sender, Quest addedQuest)
        {
            if (addedQuest.IsNull()) return;
            UIQuest uiQuest = _uiQuestsPool.Activate();
            if (uiQuest == null) return;

            uiQuest.Controller = this;
            uiQuest.Set(addedQuest);
        }

        protected virtual void QuestManager_OnQuestRemoved(object sender, Quest removedQuest)
        {
            if (removedQuest.IsNull()) return;
            _uiQuestsPool.DeactivateMatch((uiQuest) => 
            {
                if (uiQuest == null) return true;
                return uiQuest.Quest == removedQuest;
            });
        }

        protected virtual void ToggleButton_OnClick()
        {
            if (_contentHolder == null) return;
            _contentHolder.gameObject.SetActive(!_contentHolder.gameObject.activeSelf);
        }
    }
}
