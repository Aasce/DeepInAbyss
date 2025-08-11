using Asce.BehaviourTrees;
using System;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    public class CreatureRunningNode : LeafNode
    {
        private string _selfKey;
        private Func<bool> _checkRun;


        public CreatureRunningNode(string selfKey, Func<bool> check = null)
        {
            _selfKey = selfKey;
            _checkRun = check;
        }

        public override NodeState Tick()
        {
            if (!Tree.Blackboard.TryGet(_selfKey, out Creature self)) return NodeState.Failure;
            if (self.Action is not IRunnable runnable) return NodeState.Failure;

            bool isRun = _checkRun?.Invoke() ?? true;
            runnable.Running(isRun);
            return NodeState.Success;
        }
    }
}