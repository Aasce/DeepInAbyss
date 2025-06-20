using System.Collections.Generic;

namespace Asce.BehaviourTrees
{
    public abstract class LeafNode : Node
    {
        public override List<Node> GetChildren() => new ();
    }
}