using UnityEngine;

namespace Asce.Game.Quests
{
    [System.Serializable]
    public class QuestCondition 
    {
        [SerializeField] protected QuestConditionType _conditionType;

        [Header("Kill, Collect")]
        [SerializeField] protected string _targetName;
        [SerializeField, Min(1)] protected int _targetQuantity;

        [Header("Reach Location")]
        [SerializeField] protected Vector2 _position;
        [SerializeField] protected float _distance;

        [Header("Talk to NPC")]
        [SerializeField] protected string _npcName;


        public QuestConditionType ConditionType => _conditionType;

        public string TargetName => _targetName;
        public int TargetQuantity => _targetQuantity;
        
        public Vector2 Position => _position;
        public float Distance => _distance;

        public string NPCName => _npcName;
    }
}
