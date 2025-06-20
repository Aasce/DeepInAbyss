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
        private string _selfkey;
        private string _targetKey;
        private string _targetLayerMaskKey;

        /// <summary>
        ///     Key used to retrieve the "self" creature.
        /// </summary>
        public string SelfKey
        {
            get => _selfkey;
            set => _selfkey = value;
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
        /// <param name="layerMaskKey"> Key to retrieve the layer mask for target search. </param>
        /// <param name="targetKey"> Key to store the found target (default is "Target"). </param>
        public TargetFinderNode(string selfKey, string layerMaskKey, string targetKey = "Target")
        {
            _selfkey = selfKey;
            _targetLayerMaskKey = layerMaskKey;
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
            if (!Tree.Blackboard.TryGet(_selfkey, out Creature self)) return NodeState.Failure;
            if (!Tree.Blackboard.TryGet(_targetLayerMaskKey, out LayerMask targetLayerMask)) return NodeState.Failure;

            T target = self.FindTarget<T>(targetLayerMask);
            Tree.Blackboard.Set(_targetKey, target);
            return NodeState.Success;
        }
    }

}