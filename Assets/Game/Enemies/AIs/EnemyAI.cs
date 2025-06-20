using Asce.BehaviourTrees;
using Asce.Game.Entities.Enemies;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    public class EnemyAI : CreatureAI
    {
        [SerializeField] protected LayerMask _targetLayerMask;
        [SerializeField] protected Range _idleRandomTime = new(4f, 5f);
        [SerializeField] protected Range _moveRandomTime = new(0.5f, 2f);

        public new Enemy Creature
        {
            get => base.Creature as Enemy;
            set => base.Creature = value;
        }

        protected override void CreateBehaviour()
        {
            _behaviour.Blackboard.Set<Creature>("Self", Creature);
            _behaviour.Blackboard.Set<LayerMask>("TargetLayerMask", _targetLayerMask);
            _behaviour.Blackboard.Set<Character>("Target", null);
            _behaviour.Blackboard.Set<Vector2>("DirectionToTarget", default);

            _behaviour.RootNode = new RunUntilFailureNode(
                new CreatureCheckerNode("Self"), // Check if the creature is still alive
                new WaitNode(
                    new TargetFinderNode<Character>("Self", "TargetLayerMask", "Target"),
                    2f // Find a target per 2 seconds
                ),
                new BranchNode(
                    condition: new IsKeyNotEqualsNode<Character>("Target", null), // Check if the target is not null
                    onTrue: new SequenceNode(
                        // Check the distance to the target, if target not in range, return Node.Failure -> stop Sequence
                        new CheckRangeNode(() =>
                        {
                            Character target = _behaviour.Blackboard.Get<Character>("Target");
                            Vector2 direction = target.transform.position - transform.position;
                            _behaviour.Blackboard.Set("DirectionToTarget", direction);

                            return direction.magnitude;
                        }, range: _behaviour.Blackboard.Get<Enemy>("Self").Stats.ViewRadius.Value),

                        // If the target is in range, execute the following actions. if distance is greater than 3, run towards the target
                        new ActionNode(() =>
                        {
                            Enemy self = _behaviour.Blackboard.Get<Enemy>("Self");
                            self.Action.Looking(true, _behaviour.Blackboard.Get<Character>("Target").transform.position);
                            self.Action.Running(_behaviour.Blackboard.Get<Vector2>("DirectionToTarget").magnitude > 3f);
                        }),

                        // If the target is in range, check if the target is close enough to attack, otherwise move towards the target
                        new BranchNode(
                            condition: new CheckRangeNode(() => _behaviour.Blackboard.Get<Vector2>("DirectionToTarget").magnitude, 1.5f),
                            onTrue: new ActionNode(() =>
                            {
                                Enemy self = _behaviour.Blackboard.Get<Enemy>("Self");
                                self.Action.Attacking(true);
                                self.Action.Moving(Vector2.zero);
                            }),
                            onFalse: new ActionNode(() => _behaviour.Blackboard.Get<Enemy>("Self").Action.Moving(new Vector2(_behaviour.Blackboard.Get<Vector2>("DirectionToTarget").x, 0f)))
                        )
                    ),

                    // If the target is null, move randomly
                    onFalse: new SequenceNode(
                        new ActionNode(() => _behaviour.Blackboard.Get<Enemy>("Self").Action.Running(false)),
                        new RandomMoverNode("Self", _idleRandomTime, _moveRandomTime)
                    )
                )
            );
        }
    }
}
