using UnityEngine;

namespace Asce.Game.Entities
{
    public class Character : Creature, IControllableCharacter, IMovable, IWalkable, IClimbale, IJumpable
    {
        public new CharacterColliderController Collider => base.Collider as CharacterColliderController;

        public bool IsClimbing { get; set; }

        public override void Move(Vector2 direction)
        {
            if (Collider?.Rigidbody == null) return;

            if (!IsMovable) return;

            if (Collider.IsClimbable)
            {
                // If movement is vertical, switch to climbing mode
                if (direction.normalized.y != 0)
                {
                    Collider.Rigidbody.gravityScale = 0; // Disable gravity while climbing
                    IsClimbing = true;
                }
            }
            else
            {
                Collider.Rigidbody.gravityScale = 1; // Restore gravity when not climbing
                IsClimbing = false;
            }

            // Perform climbing or walking based on the state
            if (IsClimbing)
                this.Climb(direction.normalized);
            else
                this.Walk(direction.normalized.x);
        }

        public void Walk(float direction)
        {
            // Only move horizontally if the character is grounded
            if (!Collider.IsGrounded) return;
            Collider.Rigidbody.linearVelocityX = direction * 10f;
        }

        public void Climb(Vector2 direction)
        {
            // Move in both X and Y while climbing
            if (!Collider.IsClimbable) return;
            Collider.Rigidbody.linearVelocity = direction * 4f;
        }

        public void Jump()
        {
            // Only jump if grounded and movable
            if (Collider?.Rigidbody == null) return;

            if (!IsMovable) return;
            if (!Collider.IsGrounded) return;

            Collider.Rigidbody.AddForce(Vector2.up * 4f, ForceMode2D.Impulse);
        }
    }
}