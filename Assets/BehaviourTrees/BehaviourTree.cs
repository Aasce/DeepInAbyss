using System;
using System.Collections.Generic;

namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     Represents the root container for a behavior tree structure,
    ///     managing the root node and a blackboard for shared data.
    /// </summary>
    public class BehaviourTree
    {
        private Node _rootNode;
        private readonly Blackboard _blackboard = new();

        private bool _isTickEnabled = true;

        /// <summary>
        ///     The root node of the behavior tree.
        /// </summary>
        public Node RootNode
        {
            get => _rootNode;
            set
            {
                _rootNode = value;
                if (_rootNode != null) _rootNode.Tree = this;
            }
        }

        /// <summary>
        ///     Shared data container used by all nodes in the tree.
        /// </summary>
        public Blackboard Blackboard => _blackboard;

        /// <summary>
        ///     Controls whether the tree should execute logic when ticked.
        /// </summary>
        public bool IsTickEnabled
        {
            get => _isTickEnabled;
            set => _isTickEnabled = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BehaviourTree"/> class.
        /// </summary>
        /// <param name="rootNode"> The root node to assign. </param>
        /// <param name="defaultBlackBoard"> Optional key-value pairs to initialize the blackboard. </param>
        public BehaviourTree(Node rootNode = null, params KeyValuePair<string, object>[] defaultBlackBoard)
        {
            RootNode = rootNode;
            foreach (KeyValuePair<string, object> kvp in defaultBlackBoard)
            {
                _blackboard.Set(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        ///     Ticks the tree, executing the root node if enabled.
        /// </summary>
        /// <returns>
        ///     The result of the root node tick, or <see cref="NodeState.Running"/> if tick disabled.
        /// </returns>
        public NodeState Tick()
        {
            if (!IsTickEnabled) return NodeState.Running;
            if (RootNode == null) return NodeState.Failure;
            return RootNode.Tick();
        }

        /// <summary>
        ///     Resets the behavior tree and optionally clears the blackboard.
        /// </summary>
        /// <param name="isClearBlackboard"> Whether to clear all blackboard data. </param>
        public void Reset(bool isClearBlackboard = false)
        {
            RootNode?.Reset();
            if (isClearBlackboard) _blackboard.Clear();
        }
    }

}