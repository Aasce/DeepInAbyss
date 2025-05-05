using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreaturePhysicController : MonoBehaviour
    {
        private readonly float _moveForceScale = 10f;
        private readonly float _jumpForceScale = 2f;

        [SerializeField] protected Collider2D _body;
        [SerializeField] protected Rigidbody2D _rb;

        [Header("Ground")]
        [SerializeField] protected bool _isGrounded;
        [SerializeField] protected Transform _groundCheck;
        [SerializeField] protected float _groundCheckRadius = 0.2f;
        [SerializeField] protected LayerMask _groundLayer;

        [Header("Gravity")]
        [SerializeField] protected bool _isUseGravity = true;
        [SerializeField] protected float _garvityScale = 1.0f;


        public Collider2D Body => _body;
        public Rigidbody2D Rigidbody => _rb;


        public bool IsGrounded 
        { 
            get => _isGrounded; 
            protected set => _isGrounded = value;
        }

        public bool IsOnAir => !IsGrounded;


        protected virtual void Update()
        {
            // Check if the character is touching the ground
            IsGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
        }


        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {

        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {

        }

        /// <summary>
        ///     Enables or disables gravity on the Rigidbody by setting its gravity scale.
        /// </summary>
        /// <param name="isUse">
        ///     If true, gravity is applied using the predefined <paramref name="_garvityScale"/>
        ///     <br/>
        ///     If false, gravity is disabled (gravity scale set to 0).
        /// </param>
        public virtual void UseGravity(bool isUse = true)
        {
            if (Rigidbody == null) return;
            Rigidbody.gravityScale = isUse ? _garvityScale : 0f;
        }

        /// <summary>
        ///     Limit each direction of velocity according to the passed parameter. 
        ///     <br/>
        ///     If that direction is null, that direction of velocity will be kept unchanged.
        /// </summary>
        public virtual void ClampVelocity(float? xClamp = null, float? yClamp = null)
        {
            Vector2 velocity = Rigidbody.linearVelocity;

            if (xClamp != null && Mathf.Abs(velocity.x) > xClamp)
            {
                velocity.x = (float)xClamp * Mathf.Sign(velocity.x);
            }

            if (yClamp != null && Mathf.Abs(velocity.y) > yClamp)
            {
                velocity.y = (float)yClamp * Mathf.Sign(velocity.y);
            }

            Rigidbody.linearVelocity = velocity;
        }

        /// <summary>
        ///     Sets the velocity of the Rigidbody on the X and/or Y axis. 
        ///     If a value is not provided for one of the axes, the current velocity of that axis is retained.
        /// </summary>
        /// <param name="x"> 
        ///     Optional X-axis velocity. If null, keeps the current X velocity.
        /// </param>
        /// <param name="y"> 
        ///     Optional Y-axis velocity. If null, keeps the current Y velocity.
        /// </param>
        public virtual void SetVelocity(float? x = null, float? y = null)
        {
            float xVelocity = x ?? Rigidbody.linearVelocityX;
            float yVelocity = y ?? Rigidbody.linearVelocityY;
            this.SetVelocity(new Vector2(xVelocity, yVelocity));
        }

        /// <summary>
        ///     Sets the velocity of the Rigidbody using a 2D vector.
        /// </summary>
        /// <param name="velocity"> The new velocity to apply to the Rigidbody. </param>
        public virtual void SetVelocity(Vector2 velocity)
        {
            Rigidbody.linearVelocity = velocity;
        }

        /// <summary>
        ///     Move character by <paramref name="direction"/> after calculating the force
        /// </summary>
        /// <param name="direction"></param>
        public virtual void MoveByDirection(Vector2 direction)
        {
            Vector2 tranfer = direction * Rigidbody.mass * _moveForceScale;
            Rigidbody.AddForce(tranfer, ForceMode2D.Force);
        }

        /// <summary>
        ///     Add force to Rigidbody by ForceMode2D.Impulse
        /// </summary>
        /// <param name="force"></param>
        public virtual void AddForce(Vector2 force)
        {
            Vector2 finalForce = force * Rigidbody.mass * _jumpForceScale;
            Rigidbody.AddForce(finalForce, ForceMode2D.Impulse);
        }
    }
}
