using System.Collections.Generic;
using UnityEngine;

namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     Base class for all behavior tree nodes.
    /// </summary>
    public abstract class Node
    {
        private BehaviourTree _tree;
        public virtual BehaviourTree Tree
        {
            get => _tree;
            set => _tree = value;
        }

        public Node()
        {

        }

        public abstract NodeState Tick();
        public abstract List<Node> GetChildren();

        public virtual void Reset()
        {
            // Default implementation does nothing.
            // Override in derived classes if needed.
        }
    }
}
