using System;

namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A leaf node that performs an action and returns a state.
    /// </summary>
    public class ActionNode : LeafNode
    {
        protected Action _action;

        public Action Action
        {
            get => _action;
            set => _action = value;
        }

        /// <summary>
        ///     Creates a new <see cref="ActionNode"/> with the specified action function.
        /// </summary>
        /// <param name="action"> The action to perform. </param>
        public ActionNode(Action action = null)
        {
            _action = action;
        }

        public override NodeState Tick()
        {
            if (_action == null) return NodeState.Failure;
            _action.Invoke();

            return NodeState.Success;
        }
    }
}
