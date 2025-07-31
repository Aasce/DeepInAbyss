using Asce.Game.Quests;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Asce.Managers.Utils;

namespace Asce.Game.UIs.Quests
{
    public class UIQuest : UIObject, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, Readonly] protected UIQuestsInformation _controller;

        [SerializeField] protected Image _icon;
        [SerializeField] protected Image _background;
        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected RectTransform _conditionHolder;

        protected Quest _quest;
        protected List<UIQuestConditionState> _uiConditions = new();

        public UIQuestsInformation Controller
        {
            get => _controller;
            set => _controller = value;
        }

        public Quest Quest => _quest;

        public void Set(Quest quest)
        {
            if (_quest == quest) return;
            this.Unregister();
            _quest = quest;
            this.Register();
        }

        protected void Register()
        {
            if(_quest.IsNull()) return;
            if(_controller == null) return;
            if (_icon != null) _icon.sprite = _quest.Information.Icon;
            if (_name != null) _name.text = _quest.Information.Name;
            if (_conditionHolder != null)
            {
                foreach (QuestConditionState state in _quest.ConditionStates)
                {
                    if (state == null) continue;
                    UIQuestConditionState uiQuestCondition = _controller.UIQuestConditionPool.Activate();
                    if (uiQuestCondition == null) continue;

                    uiQuestCondition.RectTransform.SetParent(_conditionHolder);
                    uiQuestCondition.Set(state);
                    _uiConditions.Add(uiQuestCondition);
                }
            }
        }

        protected void Unregister()
        {
            if (_quest.IsNull()) return;
            if (_controller == null) return;
            foreach (UIQuestConditionState uiCondition in _uiConditions)
            {
                if (uiCondition == null) continue;
                _controller.UIQuestConditionPool.Deactivate(uiCondition);
            }
            _uiConditions.Clear();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            QuestsManager.Instance.CompleteQuest(_quest);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_quest.IsNull()) return;
            if (_name != null)
            {
                if (_quest.IsComplete()) _name.DOColor(Color.yellow, 0.25f);
                else _name.DOColor(Color.cyan, 0.25f);
            }
            if (_background != null)
            {
                _background.DOColor(ColorUtils.Grayscale(0.5f, 1f), 0.25f);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_quest.IsNull()) return;
            if (_name != null)
            {
                _name.DOColor(Color.white, 0.25f);
            }
            if (_background != null)
            {
                _background.DOColor(ColorUtils.Grayscale(0.5f, 0.75f), 0.25f);
            }
        }
    }
}
