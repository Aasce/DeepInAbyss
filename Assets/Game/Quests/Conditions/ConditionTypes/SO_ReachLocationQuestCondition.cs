using UnityEngine;

namespace Asce.Game.Quests
{
    [CreateAssetMenu(menuName = "Asce/Quests/Reach Location Condition", fileName = "Reach Location Condition")]
    public class SO_ReachLocationQuestCondition : SO_QuestCondition
    {
        [SerializeField] protected string _locationName;
        [SerializeField] protected Vector2 _position;
        [SerializeField] protected float _distance;

        public override QuestConditionType ConditionType => QuestConditionType.ReachLocation;

        public string LocationName => _locationName;
        public Vector2 Position => _position;
        public float Distance => _distance;
    }
}