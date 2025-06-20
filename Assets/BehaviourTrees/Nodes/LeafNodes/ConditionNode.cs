using System;

namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A leaf node that checks a condition and returns Success or Failure.
    /// </summary>
    public class ConditionNode : LeafNode
    {
        protected Func<bool> _condition;


        public Func<bool> Condition
        {
            get => _condition;
            set => _condition = value;
        }

        /// <summary>
        ///     Creates a new <see cref="ConditionNode"/> with the specified condition function.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        public ConditionNode(Func<bool> condition = null)
        {
            _condition = condition;
        }

        public override NodeState Tick()
        {
            if (_condition == null) throw new InvalidOperationException("Condition is not set.");

            return _condition.Invoke() ? NodeState.Success : NodeState.Failure;
        }

    }
}

