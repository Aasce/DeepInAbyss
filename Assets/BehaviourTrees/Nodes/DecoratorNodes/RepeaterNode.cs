using System;

namespace Asce.BehaviourTrees
{
    /// <summary> 
    ///     A node that repeats its child a set number of times or infinitely. 
    /// </summary>
    public class RepeaterNode : DecoratorNode
    {
        protected int _repeatCount;
        protected int _currentCount;


        public int RepeatCount
        {
            get => _repeatCount;
            set => _repeatCount = value;
        }

        public int CurrentCount
        {
            get => _currentCount;
            set => _currentCount = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RepeaterNode"/> class.
        ///     <br/>
        ///     Repeats child node execution until it fails or count is reached.
        /// </summary>
        /// <param name="child"> The node to repeat. </param>
        /// <param name="repeatCount"> Number of times to repeat. Use -1 for infinite. </param>
        public RepeaterNode(Node child = null, int repeatCount = -1) : base(child)
        {
            _repeatCount = repeatCount;
        }

        public override NodeState Tick()
        {
            if (Child == null) throw new InvalidOperationException("Child node is not set.");
            NodeState result = Child.Tick();

            if (result == NodeState.Failure)
                return NodeState.Failure;

            if (_repeatCount > 0)
            {
                _currentCount++;
                if (_currentCount >= _repeatCount)
                {
                    _currentCount = 0;
                    return NodeState.Success;
                }
            }

            // _repeatCount < 0 -> -1 means infinite repetition
            return NodeState.Running;
        }
    }
}