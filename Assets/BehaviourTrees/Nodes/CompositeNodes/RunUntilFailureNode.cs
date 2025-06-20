namespace Asce.BehaviourTrees
{
    public class RunUntilFailureNode : CompositeNode
    {
        public RunUntilFailureNode(params Node[] children) : base(children) { }

        public override NodeState Tick()
        {
            bool anyRunning = false;
            foreach (Node child in Children)
            {
                if (child == null) continue; // Skip null children to avoid exceptions

                NodeState result = child.Tick();
                if (result == NodeState.Failure) return result;
                if (result == NodeState.Running) anyRunning = true;
            }
            return anyRunning ? NodeState.Running : NodeState.Success;
        }
    }
}
