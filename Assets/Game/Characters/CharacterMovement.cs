using UnityEngine;

namespace Asce.Game.Entities
{
    /// <summary>
    ///     Handles 2D character movement including walking, climbing, and jumping.
    ///     <br/>
    ///     Inherits from <see cref="CreatureMovement"/> 
    ///     and implements <see cref="IMovable"/>, <see cref="IWalkable"/>, <see cref="IClimbale"/>, and <see cref="IJumpable"/>.
    /// </summary>
    public class CharacterMovement : CreatureMovement, IHasOwner<Character>, IMovable, IWalkable, IClimbale, IJumpable
    {
        [SerializeField] private bool _isClimbing;
        [SerializeField] private bool _canJump = true;
        [SerializeField] private float _jumbCooldown = 0.2f;

        [SerializeField] private float _walkSpeedScale = 1f;
        [SerializeField] private float _airSpeedScale = 1f;
        [SerializeField] private float _climbSpeedScale = .5f;

        protected float _speedScale;

        /// <summary>
        ///     Reference to the character that owns this movement controller.
        /// </summary>
        public new Character Owner 
        { 
            get => base.Owner as Character; 
            set => base.Owner = value; 
        }

        /// <summary>
        ///     The current movement speed applied to the character, depending on the state (walking, climbing, etc.).
        /// </summary>
        public float Speed { get; protected set; }

        /// <summary>
        ///     The current jump force applied to the character.
        /// </summary>
        public float JumpForce { get; protected set; }

        /// <summary>
        ///     Whether the character is currently in climbing state.
        /// </summary>
        public bool IsClimbing 
        { 
            get => _isClimbing;
            set => _isClimbing = value; 
        }

        /// <summary>
        ///     Whether the character is allowed to jump.
        /// </summary>
        public bool CanJump 
        {
            get => _canJump; 
            set => _canJump = value; 
        }

        /// <summary>
        ///     Cooldown duration (in seconds) before the character can jump again.
        /// </summary>
        public float JumpCooldown 
        { 
            get => _jumbCooldown;
            set => _jumbCooldown = value; 
        }


        protected override void Update()
        {
            base.Update();
            this.StatsHandle();
            this.ClimbHandle();
        }


        /// <summary>
        ///     Moves the character in a given direction.
        ///     <br/>
        ///     Enables climbing if vertical movement is requested and climbing is possible.
        /// </summary>
        /// <param name="direction"> The desired movement direction. </param>
        public override void Move(Vector2 direction)
        {
            if (Owner?.PhysicController?.Rigidbody == null) return;

            if (Owner.PhysicController.IsClimbable)
            {
                // If movement is vertical, switch to climbing mode
                if (direction.normalized.y != 0)
                {
                    Owner.PhysicController.UseGravity(false);
                    IsClimbing = true;
                }
            }

            // Perform climbing or walking based on the state
            if (IsClimbing)
                Climb(direction.normalized);
            else
                Walk(direction.normalized.x);
        }

        /// <summary>
        ///     Moves the character horizontally at the current speed.
        /// </summary>
        /// <param name="direction"> The horizontal input direction (-1 for left, 1 for right). </param>
        public void Walk(float direction)
        {
            if (Owner?.PhysicController?.Rigidbody == null) return;

            // Only move horizontally
            Owner.PhysicController.MoveByDirection(Vector2.right * direction * Speed);
            Owner.PhysicController.ClampVelocity(xClamp: Speed);
        }

        /// <summary>
        ///     Moves the character in both horizontal and vertical directions while climbing.
        /// </summary>
        /// <param name="direction"> The normalized climbing direction. </param>
        public void Climb(Vector2 direction)
        {
            if (Owner?.PhysicController?.Rigidbody == null) return;
            if (!Owner.PhysicController.IsClimbable) return;

            // Move in both X and Y while climbing
            Owner.PhysicController.SetVelocity(direction.normalized * Speed);
        }

        /// <summary>
        ///     Makes the character jump if grounded or climbing and if jumping is allowed.
        ///     Resets vertical velocity before applying the jump force.
        /// </summary>
        public void Jump()
        {
            if (Owner?.PhysicController?.Rigidbody == null) return;
            if (!IsMovable) return;
            if (!CanJump) return;

            // Only jump if grounded or climbing
            if (Owner.PhysicController.IsGrounded || IsClimbing)
            {
                Owner.PhysicController.SetVelocity(y: 0f);
                Owner.PhysicController.AddForce(Vector2.up * JumpForce);
                IsClimbing = false;

                CanJump = false;
                Invoke(nameof(ResetJump), JumpCooldown); // Character cant jump after Cooldown
            }
        }

        /// <summary>
        ///     Resets the jump availability after the jump cooldown period ends.
        /// </summary>
        public void ResetJump()
        {
            CanJump = true;
        }

        /// <summary>
        ///     Sets the appropriate movement speed based on Speed Stat 
        ///     and based on whether the character is climbing, in the air, or grounded.
        ///     <br/>
        ///     Sets the jump force base on JumpForce Stat
        /// </summary>
        protected virtual void StatsHandle()
        {
            if (IsClimbing) _speedScale = _climbSpeedScale;
            else if (Owner.PhysicController.IsOnAir) _speedScale = _airSpeedScale;
            else _speedScale = _walkSpeedScale;

            Speed = Owner?.Stats?.Speed?.Value ?? 5f * _speedScale;
            JumpForce = Owner?.Stats?.JumpForce?.Value ?? 7.5f;
        }

        /// <summary>
        ///     Handles the transition out of climbing state when the character is no longer near a climbable surface
        ///     or is standing on the ground.
        /// </summary>
        protected virtual void ClimbHandle()
        {
            // Character can't climb if not climbable or standing on ground.
            if (!Owner.PhysicController.IsClimbable || Owner.PhysicController.IsGrounded)
            {
                Owner.PhysicController.UseGravity(true); // Restore gravity
                IsClimbing = false;
            }
        }
    }
}
