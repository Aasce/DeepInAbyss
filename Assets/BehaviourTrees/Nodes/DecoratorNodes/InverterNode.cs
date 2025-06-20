using System;

namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A decorator node that inverts the result of its child.
    /// </summary>
    public class InverterNode : DecoratorNode
    {
        /// <summary>
        ///     Creates a new <see cref="InverterNode"/> with the specified child node.
        /// </summary>
        /// <param name="child"> The node whose result should be inverted. </param>
        public InverterNode(Node child = null) : base(child) { }

        public override NodeState Tick()
        {
            if (Child == null) throw new InvalidOperationException("Child node is not set.");

            NodeState result = Child.Tick();
            return result switch
            {
                NodeState.Success => NodeState.Failure,
                NodeState.Failure => NodeState.Success,
                _ => NodeState.Running
            };
        }
    }
}
