using Asce.BehaviourTrees;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    public class TargetFollowerNode : LeafNode
    {
        private string _selfKey;
        private string _targetKey;


        public string SelfKey
        {
            get => _selfKey;
            set => _selfKey = value;
        }
        public string TargetKey
        {
            get => _targetKey;
            set => _targetKey = value;
        }

        public TargetFollowerNode(string selfKey, string targetKey)
        {
            _selfKey = selfKey;
            _targetKey = targetKey;
        }

        public override NodeState Tick()
        {
            if (!Tree.Blackboard.TryGet(_selfKey, out Creature self)) return NodeState.Failure;
            if (!Tree.Blackboard.TryGet(_targetKey, out Creature target)) return NodeState.Failure;



            return NodeState.Success;
        }
    }
}