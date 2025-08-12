using Asce.Game.Enviroments;
using Asce.Game.Items;
using Asce.Game.Quests;
using Asce.Managers.Attributes;
using Asce.Managers.UIs;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asce.Game.UIs.Billboards
{
    public class UIBillboardNotice : UIObject, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] protected Outline _outline;
        [SerializeField] protected Image _paperBackground;
        [SerializeField] protected RectTransform _statusTag;
        [SerializeField] protected TextMeshProUGUI _title;
        [SerializeField] protected TextMeshProUGUI _content;
        [SerializeField] protected TextMeshProUGUI _condition;
        [SerializeField] protected TextMeshProUGUI _spoils;

        [Space]
        [SerializeField, Readonly] protected Notice _notice;


        public Notice Notice => _notice;


        public virtual void SetNotice(Notice notice)
        {
            if (_notice == notice) return;

            this.Unregister();
            _notice = notice;
            this.Register();
        }
        public virtual void Refresh()
        {
            if (_notice == null) return;
            this.SetTitle();
            this.SetContent();
            this.SetCondition();
            this.SetSpoils();
            this.SetTag();
        }

        protected virtual void Register()
        {
            if (_notice == null) return;
            this.Refresh();
        }

        protected virtual void Unregister()
        {
            if (_notice == null) return;

        }

        protected virtual void SetTitle()
        {
            if (_title == null) return;
            if (_notice.Quest.IsNull()) return;
            _title.text = _notice.Quest.Information.Name;
        }

        protected virtual void SetContent()
        {
            if (_content == null) return;
            if (_notice.Quest.IsNull()) return;
            _content.text = _notice.Quest.Information.Description;
        }

        protected virtual void SetTag()
        {
            if (_statusTag == null) return;
            if (_notice.Quest.IsNull()) return;
            if (QuestsManager.Instance.ActiveQuests.Contains(_notice.Quest))
            {
                _statusTag.gameObject.SetActive(true);
            }
            else
            {
                _statusTag.gameObject.SetActive(false);
            }
        }

        protected virtual void SetCondition()
        {
            if (_condition == null) return;
            if (_notice.Quest.IsNull()) return;
            string conditionText = string.Empty;
            foreach (SO_QuestCondition questCondition in _notice.Quest.Information.Conditions)
            {
                if (questCondition == null) continue;
                if (questCondition is SO_KillEnemiesQuestCondition killEnemiesQuestCondition)
                {
                    if (killEnemiesQuestCondition.EnemyInformation == null) continue;
                    conditionText += $"- Kill x{killEnemiesQuestCondition.Quantity} {killEnemiesQuestCondition.EnemyInformation.Name}\n";
                    continue;
                }

                if (questCondition is SO_CollectOresQuestCondition collectOresQuestCondition)
                {
                    if (collectOresQuestCondition.OreInformation == null) continue;
                    conditionText += $"- Collect x{collectOresQuestCondition.Quantity} {collectOresQuestCondition.OreInformation.Name}\n";
                    continue;
                }
            }
            _condition.text = conditionText;
        }

        protected virtual void SetSpoils()
        {
            if (_spoils == null) return;
            if (_notice.Quest.IsNull()) return;
            string spoilsText = string.Empty;
            foreach (DroppedSpoilsContainer spoils in _notice.Quest.Information.DroppedSpoils.DroppedSpoils)
            {
                if (spoils == null) continue;
                if (spoils.ItemInformation == null) continue;

                spoilsText += $"x {spoils.QuantityRange.y} {spoils.ItemInformation.Name}\n";
            }
            _spoils.text = string.IsNullOrEmpty(spoilsText) ? "No Spoils" : spoilsText;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(Vector3.one * 1.05f, 0.1f);
            if (_outline != null) _outline.enabled = true;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(Vector3.one, 0.1f);
            if (_outline != null) _outline.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_notice == null) return;
            if (_notice.Quest.IsNull()) return;

            QuestsManager.Instance.AcceptQuest(_notice.Quest);
            this.SetTag();
        }
    }
}
