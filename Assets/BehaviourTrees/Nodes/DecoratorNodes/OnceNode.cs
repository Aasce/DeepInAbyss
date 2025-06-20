using System;

namespace Asce.BehaviourTrees
{
    public class OnceNode : DecoratorNode
    {
        protected bool _isExecuted = false;

        public OnceNode(Node child = null) : base(child) { }

        /// <summary>
        ///     A node that runs its action only once and returns Success on the first run.
        /// </summary>
        public override NodeState Tick()
        {
            if (Child == null) throw new InvalidOperationException("Child node is not set.");
            if (_isExecuted) return NodeState.Success;

            NodeState result = Child.Tick();
            if (result == NodeState.Success || result == NodeState.Failure)
            {
                _isExecuted = true;
            }

            return result;
        }

        public override void Reset() => _isExecuted = false;
    }
}