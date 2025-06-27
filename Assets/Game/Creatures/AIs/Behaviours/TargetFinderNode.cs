using Asce.BehaviourTrees;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    /// <summary>
    ///     A leaf node that searches for a target of type <typeparamref name="T"/> in the vicinity,
    ///     and stores it in the blackboard.
    /// </summary>
    /// <typeparam name="T"> The type of creature to find.< /typeparam>
    public class TargetFinderNode<T> : LeafNode where T : ICreature
    {
        private string _selfKey;
        private string _targetKey;
        private string _targetLayerMaskKey;
        private string _groundLayerMaskKey;

        /// <summary>
        ///     Key used to retrieve the "self" creature.
        /// </summary>
        public string SelfKey
        {
            get => _selfKey;
            set => _selfKey = value;
        }

        /// <summary>
        ///     Key used to store the target found.
        /// </summary>
        public string TargetKey
        {
            get => _targetKey;
            set => _targetKey = value;
        }

        /// <summary>
        ///     Key used to retrieve the target layer mask.
        /// </summary>
        public string TargetLayerMaskKey
        {
            get => _targetLayerMaskKey;
            set => _targetLayerMaskKey = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetFinderNode{T}"/> class.
        /// </summary>
        /// <param name="selfKey"> Key to retrieve the self creature from the blackboard. </param>
        /// <param name="targetLayerMaskKey"> Key to retrieve the layer mask for target search. </param>
        /// <param name="targetKey"> Key to store the found target (default is "Target"). </param>
        public TargetFinderNode(string selfKey, string targetLayerMaskKey, string groundLayerMaskKey, string targetKey = "Target")
        {
            _selfKey = selfKey;
            _targetLayerMaskKey = targetLayerMaskKey;
            _groundLayerMaskKey = groundLayerMaskKey;
            _targetKey = targetKey;
        }

        /// <summary>
        ///     Executes the target-finding logic.
        /// </summary>
        /// <returns>
        ///     Returns <see cref="NodeState.Failure"/> if self or layer mask not found.
        ///     Otherwise stores the found target and returns <see cref="NodeState.Success"/>.
        /// </returns>
        public override NodeState Tick()
        {
            if (!Tree.Blackboard.TryGet(_selfKey, out Creature self)) return NodeState.Failure;
            if (!Tree.Blackboard.TryGet(_targetLayerMaskKey, out LayerMask targetLayerMask)) return NodeState.Failure;
            LayerMask groundLayerMask = Tree.Blackboard.Get<LayerMask>(_groundLayerMaskKey);

            T target = self.FindTarget<T>(targetLayerMask, groundLayerMask);
            Tree.Blackboard.Set(_targetKey, target);
            return NodeState.Success;
        }
    }

}