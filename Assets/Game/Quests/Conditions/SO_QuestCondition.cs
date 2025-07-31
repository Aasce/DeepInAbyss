using UnityEngine;

namespace Asce.Game.Quests
{
    public abstract class SO_QuestCondition : ScriptableObject
    {
        public abstract QuestConditionType ConditionType { get; }
    }
}