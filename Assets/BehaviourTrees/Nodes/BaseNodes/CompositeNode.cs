using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Asce.BehaviourTrees
{
    public abstract class CompositeNode : Node, ICompositeNode
    {
        private readonly List<Node> _children;

        public override BehaviourTree Tree
        {
            get => base.Tree;
            set
            {
                base.Tree = value;
                foreach (Node child in Children) if (child != null) child.Tree = value;
            }
        }

        public ReadOnlyCollection<Node> Children => _children.AsReadOnly();


        public CompositeNode(params Node[] children)
        {
            _children = children.ToList();
            foreach (Node child in _children)
            {
                if (child == null) continue;
                child.Tree = Tree;
            }
        }

        public override void Reset()
        {
            base.Reset();
            foreach (Node child in _children)
            {
                child?.Reset();
            }
        }


        public void AddChild(Node child)
        {
            if (child == null) return;
            _children.Add(child);
            child.Tree = Tree;
        }

        public void RemoveChild(Node child)
        {
            if (child == null) return;
            if (_children.Remove(child)) child.Tree = null; // Clear the tree reference when removing
        }

        public override List<Node> GetChildren() => new (_children);
    }
}