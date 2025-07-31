using Asce.Game.Quests;
using Asce.Managers.UIs;
using TMPro;
using UnityEngine;

namespace Asce.Game.UIs.Quests
{
    public class UIQuestConditionState : UIObject
    {
        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected TextMeshProUGUI _progress;

        protected QuestConditionState _conditionState;


        public QuestConditionState ConditionState => _conditionState;


        public void Set(QuestConditionState state)
        {
            if (_conditionState == state) return;
            this.Unregister();
            _conditionState = state;
            this.Register();
        }

        protected void Register()
        {
            if (_conditionState == null) return;
            if (_name != null) _name.text = _conditionState.Information.Name;
            this.SetProgress();

            _conditionState.OnCurrentQuantityChanged += ConditionState_OnCurrentQuantityChanged;
        }

        protected void Unregister()
        {
            if (_conditionState == null) return;
            _conditionState.OnCurrentQuantityChanged -= ConditionState_OnCurrentQuantityChanged;
        }

        protected void ConditionState_OnCurrentQuantityChanged(object sender, Managers.ValueChangedEventArgs<int> args)
        {
            this.SetProgress();
        }

        protected void SetProgress()
        {
            if (_progress != null)
            {
                if (_conditionState.Information is SO_KillEnemiesQuestCondition killEnemiesCondition)
                {
                    _progress.text = $"{_conditionState.CurrentQuantity}/{killEnemiesCondition.Quantity}";
                }
                else if (_conditionState.Information is SO_CollectOresQuestCondition collectOresCondition)
                {
                    _progress.text = $"{_conditionState.CurrentQuantity}/{collectOresCondition.Quantity}";
                }
                else if (_conditionState.Information is SO_ReachLocationQuestCondition reachLocationCondition)
                {
                    _progress.text = $"{_conditionState.CurrentDistance:#:#}";
                }
            }
        } 
    }
}
