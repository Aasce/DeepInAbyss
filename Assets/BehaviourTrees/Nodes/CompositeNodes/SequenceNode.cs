namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A composite node that runs each child in sequence until one fails or all succeed.
    /// </summary>
    public class SequenceNode : CompositeNode
    {
        /// <summary>
        ///     Creates a new <see cref="SequenceNode"/> with the given children.
        /// </summary>
        /// <param name="children"> Child nodes to evaluate in sequence. </param>
        public SequenceNode(params Node[] children) : base(children) { }

        public override NodeState Tick()
        {
            foreach (Node child in Children)
            {
                if (child == null) continue; // Skip null children to avoid exceptions

                NodeState result = child.Tick();
                if (result != NodeState.Success)
                    return result;
            }
            return NodeState.Success;
        }
    }
}
