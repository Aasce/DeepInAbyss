using System;
using System.Collections.Generic;

namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A branching behavior tree node that selects one of two actions based on a condition.
    /// </summary>
    public class BranchNode : Node
    {
        private Node _condition;
        private Node _onTrue;
        private Node _onFalse;

        /// <summary>
        ///     Gets or sets the behavior tree this node is attached to.
        ///     Propagates the tree to all children.
        /// </summary>
        public override BehaviourTree Tree
        {
            get => base.Tree;
            set
            {
                base.Tree = value;
                if (_condition != null) _condition.Tree = value;
                if (_onTrue != null) _onTrue.Tree = value;
                if (_onFalse != null) _onFalse.Tree = value;
            }
        }

        /// <summary>
        ///     The condition node to evaluate.
        /// </summary>
        public Node Condition
        {
            get => _condition;
            set
            {
                _condition = value;
                if (_condition != null) _condition.Tree = Tree;
            }
        }

        /// <summary>
        ///     The action to perform if the condition returns <see cref="NodeState.Success"/>.
        /// </summary>
        public Node OnTrue
        {
            get => _onTrue;
            set
            {
                _onTrue = value;
                if (_onTrue != null) _onTrue.Tree = Tree;
            }
        }

        /// <summary>
        ///     The action to perform if the condition returns <see cref="NodeState.Failure"/>.
        /// </summary>
        public Node OnFalse
        {
            get => _onFalse;
            set
            {
                _onFalse = value;
                if (_onFalse != null) _onFalse.Tree = Tree;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BranchNode"/> class.
        /// </summary>
        /// <param name="condition"> The condition node to evaluate. </param>
        /// <param name="onTrue"> The action to execute if the condition succeeds. </param>
        /// <param name="onFalse"> The action to execute if the condition fails. </param>
        public BranchNode(Node condition, Node onTrue = null, Node onFalse = null)
        {
            Condition = condition;
            OnTrue = onTrue;
            OnFalse = onFalse;
        }

        /// <summary>
        ///     Evaluates the condition and runs the corresponding branch.
        /// </summary>
        public override NodeState Tick()
        {
            if (_condition == null) throw new InvalidOperationException("Condition is not set.");
            NodeState result = _condition.Tick();

            if (result == NodeState.Success)
            {
                if (_onTrue == null) return NodeState.Success;
                return _onTrue.Tick();
            }

            if (result == NodeState.Failure)
            {
                if (_onFalse == null) return NodeState.Failure;
                return _onFalse.Tick();
            }

            return NodeState.Running;
        }

        /// <summary>
        ///     Resets all child nodes.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _condition?.Reset();
            _onTrue?.Reset();
            _onFalse?.Reset();
        }

        /// <summary>
        ///     Gets all child nodes (condition, onTrue, onFalse).
        /// </summary>
        public override List<Node> GetChildren()
        {
            List<Node> children = new();
            if (_condition != null) children.Add(_condition);
            if (_onTrue != null) children.Add(_onTrue);
            if (_onFalse != null) children.Add(_onFalse);
            return children;
        }
    }

}