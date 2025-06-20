using Asce.BehaviourTrees;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    /// <summary>
    ///     A leaf node that checks whether a creature exists in the blackboard and is valid for interaction.
    /// </summary>
    public class CreatureCheckerNode : LeafNode
    {
        private string _creatureKey;

        public string CreatureKey
        {
            get => _creatureKey;
            set => _creatureKey = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreatureCheckerNode"/> class.
        /// </summary>
        /// <param name="creatureKey">The key used to retrieve the creature from the blackboard.</param>
        public CreatureCheckerNode(string creatureKey)
        {
            _creatureKey = creatureKey;
        }

        /// <summary>
        ///     Executes the node logic.
        /// </summary>
        /// <returns>
        ///     Returns <see cref="NodeState.Failure"/> if the creature is null, dead, or controlled.
        ///     Otherwise returns <see cref="NodeState.Success"/>.
        /// </returns>
        public override NodeState Tick()
        {
            if (!Tree.Blackboard.TryGet(_creatureKey, out Creature creature)) return NodeState.Failure;
            if (creature == null || creature.IsControled || creature.Status.IsDead) return NodeState.Failure;
            return NodeState.Success;
        }
    }
}