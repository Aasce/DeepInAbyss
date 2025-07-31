using Asce.Game.Entities;
using UnityEngine;

namespace Asce.Game.Quests
{
    [CreateAssetMenu(menuName = "Asce/Quests/Collect Ores Condition", fileName = "Collect Ores Condition")]
    public class SO_CollectOresQuestCondition : SO_QuestCondition
    {
        [SerializeField] protected SO_EntityInformation _oreInformation;
        [SerializeField, Min(1)] protected int _quantity;

        public override QuestConditionType ConditionType => QuestConditionType.CollectOres;
        public SO_EntityInformation OreInformation => _oreInformation;
        public int Quantity => _quantity;
    }
}