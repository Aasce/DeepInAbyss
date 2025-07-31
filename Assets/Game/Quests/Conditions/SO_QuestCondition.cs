using UnityEngine;

namespace Asce.Game.Quests
{
    public abstract class SO_QuestCondition : ScriptableObject
    {
        [SerializeField] protected string _name;

        public abstract QuestConditionType ConditionType { get; }

        public string Name => _name;

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(_name)) _name = name;
        }
    }
}