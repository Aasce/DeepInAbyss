using UnityEngine;
using Asce.Game.Entities;

namespace Asce.Game.Quests
{
    [CreateAssetMenu(menuName = "Asce/Quests/Kill Enemies Condition", fileName = "Kill Enemies Condition")]
    public class SO_KillEnemiesQuestCondition : SO_QuestCondition
    {
        [SerializeField] protected SO_EntityInformation _enemyInformation;
        [SerializeField, Min(1)] protected int _quantity;

        public override QuestConditionType ConditionType => QuestConditionType.KillEnemies;
        public SO_EntityInformation EnemyInformation => _enemyInformation;
        public int Quantity => _quantity;
    }
}