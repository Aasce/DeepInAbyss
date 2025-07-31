using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Entities.Ores;
using UnityEngine;

namespace Asce.Game.Quests
{
    [System.Serializable]
    public class QuestConditionState
    {
        [SerializeField] protected SO_QuestCondition _conditionInformation;

        [Space]
        [SerializeField] protected int _currentQuantity;
        [SerializeField] protected float _currentDistance;

        public SO_QuestCondition Information => _conditionInformation;
        public int CurrentQuantity
        {
            get => _currentQuantity;
            set => _currentQuantity = value;
        }

        public float CurrentDistance
        {
            get => _currentDistance;
            set => _currentDistance = value;
        }

        public QuestConditionState() : this(null) { }
        public QuestConditionState(SO_QuestCondition conditionInformation)
        {
            _conditionInformation = conditionInformation;
        }

        public bool Check()
        {
            return _conditionInformation switch
            {
                SO_KillEnemiesQuestCondition killEnemiesCondition => _currentQuantity >= killEnemiesCondition.Quantity,
                SO_CollectOresQuestCondition collectOresCondition => _currentQuantity >= collectOresCondition.Quantity,
                SO_ReachLocationQuestCondition reachLocationCondition => _currentDistance >= reachLocationCondition.Distance,
                _ => false
            };
        }

        public void HandleKillEnemies(object sender, DamageContainer damageContainer)
        {
            if (damageContainer == null) return;
            if (_conditionInformation is not SO_KillEnemiesQuestCondition killCondition) return;
            if (damageContainer.Receiver is not ICreature creature) return;
            if (creature.Information != killCondition.EnemyInformation) return;

            if (creature.Status.IsDead)
            {
                CurrentQuantity++;
            }
        }

        public void HandleCollectOres(object sender, DamageContainer damageContainer)
        {
            if (damageContainer == null) return;
            if (_conditionInformation is not SO_CollectOresQuestCondition collectCondition) return;
            if (damageContainer.Receiver is not IOre ore) return;
            if (ore.Information != collectCondition.OreInformation) return;

            if (ore.Status.IsDead)
            {
                CurrentQuantity++;
            }
        }
    }
}
