using Asce.BehaviourTrees;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    /// <summary>
    ///     A leaf node that moves the creature in random directions, alternating between idle and movement states.
    /// </summary>
    public class RandomMoverNode : LeafNode
    {
        private string _selfKey;
        private bool _isIdle;

        private Cooldown _idleCooldown;
        private Cooldown _moveCooldown;

        private Range _idleRandomTime;
        private Range _moveRandomTime;

        private int _moveDirection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RandomMoverNode"/> class with default idle and move ranges.
        /// </summary>
        /// <param name="selfKey"> Key to retrieve the creature from the blackboard. </param>
        public RandomMoverNode(string selfKey) : this(selfKey, new Range(3f, 5f), new Range(0.5f, 1.5f)) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RandomMoverNode"/> class.
        /// </summary>
        /// <param name="selfKey"> Key to retrieve the creature from the blackboard. </param>
        /// <param name="idleTime"> Range of idle durations. </param>
        /// <param name="moveTime"> Range of movement durations. </param>
        public RandomMoverNode(string selfKey, Range idleTime, Range moveTime)
        {
            _selfKey = selfKey;
            _idleRandomTime = idleTime;
            _moveRandomTime = moveTime;

            _idleCooldown = new Cooldown(_idleRandomTime.RandomValue);
            _moveCooldown = new Cooldown(_moveRandomTime.RandomValue);
        }

        /// <summary>
        ///     Executes the movement behavior.
        ///     Alternates between idle and movement states based on cooldowns.
        /// </summary>
        /// <returns>
        ///     Returns <see cref="NodeState.Failure"/> if the creature or movement component is not found.
        ///     <br/>
        ///     Returns <see cref="NodeState.Running"/> as long as the behavior is active.
        /// </returns>
        public override NodeState Tick()
        {
            if (!Tree.Blackboard.TryGet(_selfKey, out Creature self)) return NodeState.Failure;
            if (self.Action is not IMovable movable) return NodeState.Failure;

            if (_isIdle)
            {
                if (_idleCooldown.IsComplete)
                {
                    _isIdle = false;
                    _moveCooldown.SetBaseTime(_moveRandomTime.RandomValue);
                    _moveDirection = Random.value < 0.5f ? -1 : 1;
                }
                else
                {
                    _idleCooldown.Update(Time.deltaTime);
                    movable.Moving(Vector2.zero);
                }
            }
            else
            {
                if (_moveCooldown.IsComplete)
                {
                    _isIdle = true;
                    _idleCooldown.SetBaseTime(_idleRandomTime.RandomValue);
                    _idleCooldown.Reset();
                }
                else
                {
                    movable.Moving(new Vector2(_moveDirection, 0f));
                    _moveCooldown.Update(Time.deltaTime);
                }
            }

            return NodeState.Running;
        }
    }

}