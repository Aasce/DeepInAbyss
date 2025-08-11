using Asce.BehaviourTrees;
using Asce.Game.Entities.Characters;
using Asce.Game.Entities.Enemies;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    public class EnemyAI : CreatureAI
    {
        [SerializeField] protected LayerMask _targetLayerMask;
        [SerializeField] protected LayerMask _groundLayerMask;

        [Space]
        [SerializeField] protected Range _idleRandomTime = new(4f, 5f);
        [SerializeField] protected Range _moveRandomTime = new(0.5f, 2f);

        [Space]
        [SerializeField] protected float _runningRange = 3f;
        [SerializeField] protected float _attackRange = 1f;

        public new Enemy Creature
        {
            get => base.Creature as Enemy;
            set => base.Creature = value;
        }

        protected override void SetBehaviourBlackboard()
        {
            base.SetBehaviourBlackboard();
            _behaviour.Blackboard.Set<Enemy>("Self", Creature);
            _behaviour.Blackboard.Set<LayerMask>("TargetLayerMask", _targetLayerMask);
            _behaviour.Blackboard.Set<LayerMask>("GroundLayerMask", _groundLayerMask);
            _behaviour.Blackboard.Set<Character>("Target", null);
            _behaviour.Blackboard.Set<float>("RunningRange", _runningRange);
            _behaviour.Blackboard.Set<float>("AttackRange", _attackRange);
        }

        protected override void CreateBehaviour()
        {
            _behaviour.RootNode = new RunUntilFailureNode(
                new CreatureCheckerNode("Self"), // Check if the creature is still alive
                new WaitNode(
                    new TargetFinderNode<Character>("Self", "TargetLayerMask", "GroundLayerMask", "Target"),
                    2f // Find a target per 2 seconds
                ),
                new BranchNode(
                    condition: new IsKeyNotEqualsNode<Character>("Target", null), // Check if the target is not null
                    onTrue: new SequenceNode(
                        // Check the distance to the target, if target not in range, return Node.Failure -> stop Sequence
                        new TargetInViewRadiusCheckerNode("Self", "Target"),
                        
                        new LookingTargetNode("Self", "Target"),
                        new CreatureRunningNode("Self", () =>
                        {
                            Enemy self = _behaviour.Blackboard.Get<Enemy>("Self");
                            Character target = _behaviour.Blackboard.Get<Character>("Target");
                            float runningRange = _behaviour.Blackboard.Get<float>("RunningRange");
                            Vector2 direction = target.transform.position - self.transform.position;
                            return direction.magnitude > runningRange;
                        }),

                        // If the target is in range, execute the following actions. if distance is greater than 3, run towards the target
                        new ActionNode(() =>
                        {
                            Enemy self = _behaviour.Blackboard.Get<Enemy>("Self");
                            Character target = _behaviour.Blackboard.Get<Character>("Target");
                            Vector2 direction = target.transform.position - self.transform.position;

                            Vector2 facingRayPosition = (Vector2)self.transform.position + Vector2.up * self.Status.Height * 0.5f;
                            Vector2 facingRayDirection = Vector2.right * self.Status.FacingDirectionValue;
                            LayerMask groundLayerMask = _behaviour.Blackboard.Get<LayerMask>("GroundLayerMask");

                            RaycastHit2D hit = Physics2D.Raycast(facingRayPosition, facingRayDirection, 1f, groundLayerMask);
                            Debug.DrawRay(facingRayPosition, facingRayDirection, Color.red);


                            float xDeltaDistance = (hit.collider != null) ? hit.point.x - facingRayPosition.x : self.Status.FacingDirectionValue;
                            Vector2 jumpRayPosition = (Vector2)self.transform.position + Vector2.right * xDeltaDistance + Vector2.up * 3f;

                            RaycastHit2D jumpRayHit = Physics2D.Raycast(jumpRayPosition, Vector2.down, 10f, groundLayerMask);
                            Debug.DrawRay(jumpRayPosition, Vector2.down * 10f, Color.green);

                            if (jumpRayHit.distance > 0.01f)
                            {
                                float deltaY = jumpRayHit.point.y - self.transform.position.y;
                                if (deltaY > 0.5f) self.Action.Jumping(true);
                            }
                        }),

                        // If the target is in range, check if the target is close enough to attack, otherwise move towards the target
                        new BranchNode(
                            condition: new CheckRangeNode(
                                () =>
                                {
                                    Enemy self = _behaviour.Blackboard.Get<Enemy>("Self");
                                    Character target = _behaviour.Blackboard.Get<Character>("Target");
                                    Vector2 direction = target.transform.position - self.transform.position;
                                    return direction.magnitude;
                                }, 
                                _behaviour.Blackboard.Get<float>("AttackRange")),
                            onTrue: new ActionNode(() =>
                            {
                                Enemy self = _behaviour.Blackboard.Get<Enemy>("Self");
                                self.Action.Attacking(true);
                                self.Action.Moving(Vector2.zero);
                            }),
                            onFalse: new ActionNode(() =>
                            {
                                Enemy self = _behaviour.Blackboard.Get<Enemy>("Self");
                                Character target = _behaviour.Blackboard.Get<Character>("Target");
                                Vector2 direction = target.transform.position - self.transform.position;
                                self.Action.Moving(new Vector2(direction.x, 0f));
                            })
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
