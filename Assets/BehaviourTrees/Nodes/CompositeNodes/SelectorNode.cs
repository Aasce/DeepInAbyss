namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A composite node that runs each child until one succeeds or all fail.
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        /// <summary>
        ///     Creates a new <see cref="SelectorNode"/> with the given children.
        /// </summary>
        /// <param name="children"> Child nodes to evaluate in order. </param>
        public SelectorNode(params Node[] children) : base(children) { }

        public override NodeState Tick()
        {
            foreach (Node child in Children)
            {
                if (child == null) continue; // Skip null children to avoid exceptions

                NodeState result = child.Tick();
                if (result != NodeState.Failure)
                    return result;
            }
            return NodeState.Failure;
        }
    }
}
