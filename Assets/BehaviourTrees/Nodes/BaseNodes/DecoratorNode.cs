using System.Collections.Generic;

namespace Asce.BehaviourTrees
{
    public abstract class DecoratorNode : Node, IDecoratorNode
    {
        private Node _child;

        public override BehaviourTree Tree 
        { 
            get => base.Tree; 
            set
            {
                base.Tree = value;
                if (Child != null) Child.Tree = value;
            } 
        }

        public Node Child 
        { 
            get => _child;
            set
            {
                _child = value;
                if (_child != null) _child.Tree = Tree;
            }
        }

        public DecoratorNode(Node child)
        {
            Child = child;
        }

        public override void Reset()
        {
            base.Reset();
            Child?.Reset();
        }

        public override List<Node> GetChildren() => Child != null ? new List<Node> { Child } : new List<Node>();
    }
}