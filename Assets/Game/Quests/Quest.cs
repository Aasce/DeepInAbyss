using Asce.Game.Players;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Quests
{
    [System.Serializable]
    public class Quest
    {
        [SerializeField] protected SO_QuestInformation _information;
        [SerializeField] protected List<QuestConditionState> _conditionStates = new();

        public SO_QuestInformation Information => _information;
        public List<QuestConditionState> ConditionStates => _conditionStates;


        public Quest() : this(null) { }
        public Quest(SO_QuestInformation information) 
        {
            this.SetInformation(information);
        }

        public void SetInformation(SO_QuestInformation information)
        {
            _information = information;
            if (_information == null) return;
            _conditionStates.Clear();
            foreach(SO_QuestCondition condition in _information.Conditions)
            {
                if (condition == null) continue;
                QuestConditionState state = new(condition);
                _conditionStates.Add(state);
            }
        }

        public bool IsComplete()
        {
            foreach (QuestConditionState state in _conditionStates)
            {
                if (state == null) continue;
                bool isConditionComplete = state.Check();
                if (!isConditionComplete) return false; // Exits condition not complete
            }
            return true;
        }

        public void RegisterEvent()
        {
            if (Player.Instance.ControlledCreature == null) return;
            foreach (var conditionState in this.ConditionStates)
            {
                if (conditionState.Information is SO_KillEnemiesQuestCondition)
                {
                    Player.Instance.ControlledCreature.OnAfterSendDamage += conditionState.HandleKillEnemies;
                    continue;
                }

                if (conditionState.Information is SO_CollectOresQuestCondition)
                {
                    Player.Instance.ControlledCreature.OnAfterSendDamage += conditionState.HandleCollectOres;
                    continue;
                }
            }
        }

        public void UnregisterEvent()
        {
            if (Player.Instance.ControlledCreature == null) return;
            foreach (var conditionState in this.ConditionStates)
            {
                if (conditionState.Information is SO_KillEnemiesQuestCondition)
                {
                    Player.Instance.ControlledCreature.OnAfterSendDamage -= conditionState.HandleKillEnemies;
                    continue;
                }

                if (conditionState.Information is SO_CollectOresQuestCondition)
                {
                    Player.Instance.ControlledCreature.OnAfterSendDamage -= conditionState.HandleCollectOres;
                    continue;
                }
            }
        }
    }
}
