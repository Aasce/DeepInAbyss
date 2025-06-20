using System;

namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A leaf node that performs an action and returns a state.
    /// </summary>
    public class DelegateNode : LeafNode
    {
        protected Func<NodeState> _function;

        public Func<NodeState> Function
        {
            get => _function;
            set => _function = value;
        }

        /// <summary>
        ///     Creates a new <see cref="DelegateNode"/> with the specified action function.
        /// </summary>
        /// <param name="action"> The action to perform. </param>
        public DelegateNode(Func<NodeState> action = null)
        {
            _function = action;
        }

        public override NodeState Tick()
        {
            if (_function == null) throw new InvalidOperationException("Function is not set.");

            return _function.Invoke();
        }
    }
}
