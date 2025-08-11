using Asce.BehaviourTrees;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    public class LookingTargetNode : LeafNode
    {
        private string _selfKey;
        private string _targetKey;


        public LookingTargetNode(string selfKey, string targetKey)
        {
            _selfKey = selfKey;
            _targetKey = targetKey;
        }

        public override NodeState Tick()
        {
            if (!Tree.Blackboard.TryGet(_selfKey, out Creature self)) return NodeState.Failure;
            if (!Tree.Blackboard.TryGet(_targetKey, out ICreature target)) return NodeState.Failure;
            if (self.Action is not ILookable lookable) return NodeState.Failure;

            lookable.Looking(true, target.gameObject.transform.position);
            return NodeState.Success;
        }
    }
}