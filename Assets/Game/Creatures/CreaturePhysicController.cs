using Asce.Game.Enviroments;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreaturePhysicController : MonoBehaviour, IHasOwner<Creature>
    {
        #region Fields ---------------------------------------------------------------------
        [SerializeField] private Creature _owner;                                   // the creature that owns this controller

        [Header("Reference")]
        [SerializeField] protected Collider2D _bodyCollider;
        [SerializeField] protected Rigidbody2D _rigidbody;

        protected float _weight = 50.0f;

        [Header("Physic")]
        [SerializeField] protected float _groundDrag = 10.0f;                         // braking acceleration (from movement to still) while on ground
        [SerializeField] protected float _airDrag = 0.75f;                            // braking acceleration (from movement to still) while in air
        [SerializeField] protected float _gravityScale = 1f;
        protected float _currentGravityScale;
        protected Vector2 _currentVelocity;

        protected Vector2 _colliderSize = new(0.375f, 1.3125f);
        protected Vector2 _colliderOffset = new(0.0f, 1.15625f);

        [Header("Ground")]
        [SerializeField] protected LayerMask _groundCheckLayerMask;
        protected bool _isGrounded;
        protected float _groundYPosition;

        /// <summary>
        ///  The velocity y threshold to triger the OnLand event
        /// </summary>
        protected readonly float _landEventThreshold = 3.0f;
        protected virtual float GroundRaycastDistance => 18 * Constants.PIXEL_SIZE;

        // Ground lift
        protected float _groundLiftSpeed;

        // Sliding
        private readonly float _slideMaxSpeed = 5.0f; // the maxSpeed speed the character slide down a slope it can not stand on
        protected bool _isSliding;
        protected float _slideSpeed;
        protected Vector2 _slideVelocity;

        // Standing colliders
        protected Cooldown _getDownPlatformCooldown = new(0.1f);

        protected bool _isStandingOnPlatform; // is the character standing on a one-way platform
        protected List<Collider2D> _standingColliders = new(); // collider the character is standing on
        protected List<Vector2> _standingPosList = new();
        protected HashSet<Collider2D> _ignoreColliders = new();
        protected HashSet<Collider2D> _checkList = new();

        [Header("Surface")]
        [Tooltip("the maxSpeed angle of a slope the character can stand on, if the slope angle is larger than this, the character will slide down the slope")]
        [SerializeField] protected float _surfaceAngleLimit = 46.0f;

        protected Vector2 _surfaceNormal;
        protected Vector2 _surfaceDirection;
        protected Vector2 _surfaceDirectionDown;
        protected float _surfaceAngle;
        protected float _surfaceAngleForward;
        protected float _surfaceSpeedMultiply = 1.0f;

        // Others
        protected bool _isDead;
        protected FacingType _facingDirection = FacingType.None;
        protected bool _isUpdateCollider;

        // Timer
        protected float _airTime;

        #endregion

        #region - EVENTS -

        public event Action<object> OnLand;

        #endregion

        #region Properties -----------------------------------------------------------------
        public Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public virtual Collider2D BodyCollider
        {
            get => _bodyCollider;
            protected set => _bodyCollider = value;
        }

        public Rigidbody2D Rigidbody => _rigidbody;


        /// <summary>
        ///     Is the character dead? if dead, plays dead animation and disable control
        /// </summary>
        public bool IsDead
        {
            get => _isDead;
            set
            {
                if (_isDead == value) return;
                _isDead = value;
                Owner.View.IsDead = value;
            }
        }

        public FacingType FacingDirection
        {
            get => _facingDirection;
            set => _facingDirection = value;
        }

        protected bool IsUpdateCollider
        {
            get => _isUpdateCollider;
            set => _isUpdateCollider = value;
        }

        /// <summary>
        ///     Returns the Collider2D that the Creature is standing on.
        /// </summary>
        public Collider2D StandingCollider => _standingColliders.Count <= 0 ? null : _standingColliders[0];

        /// <summary>
        ///     raycast parameters for ground check
        /// </summary>
        public virtual Vector2 GroundRaycastMidPosition
        {
            get
            {
                Vector2 pos = BodyCollider.bounds.center;
                pos.y = transform.position.y + GroundRaycastDistance - Constants.PIXEL_SIZE;
                return pos;
            }
        }
        /// <summary>
        ///     raycast parameters for ground check
        /// </summary>
        public virtual Vector2 GroundRaycastFrontPosition => GroundRaycastMidPosition + new Vector2(Constants.PIXEL_SIZE * 6f * (int)FacingDirection, 0.0f);

        /// <summary>
        ///     raycast parameters for ground check
        /// </summary>
        public virtual Vector2 GroundRaycastBackPosition => GroundRaycastMidPosition + new Vector2(Constants.PIXEL_SIZE * 6f * -(int)FacingDirection, 0.0f);



        /// <summary>
        ///     Is the character on ground
        /// </summary>
        public virtual bool IsGrounded
        {
            get => _isGrounded;
            protected set
            {
                if (_isGrounded == value) return;
                _isGrounded = value;

                //ground event
                if (_isGrounded && _currentVelocity.y < -_landEventThreshold) OnLand?.Invoke(this);
            }
        }

        public virtual bool IsStandingOnPlatform
        {
            get => _isStandingOnPlatform;
            protected set => _isStandingOnPlatform = value;
        }

        public bool IsSliding
        {
            get => _isSliding;
            protected set => _isSliding = value;
        }

        public Vector2 SlideVelocity
        {
            get => _slideVelocity;
            protected set => _slideVelocity = value;
        }

        public float GroundLiftSpeed
        {
            get => _groundLiftSpeed;
            protected set => _groundLiftSpeed = value;
        }

        public Vector2 SurfaceDirection
        {
            get => _surfaceDirection;
            protected set => _surfaceDirection = value;
        }
        public Vector2 SurfaceNormal
        {
            get => _surfaceNormal;
            protected set => _surfaceNormal = value;
        }
        public float SurfaceAngle
        {
            get => _surfaceAngle;
            protected set => _surfaceAngle = value;
        }
        public float SurfaceAngleForward
        {
            get => _surfaceAngleForward;
            protected set => _surfaceAngleForward = value;
        }

        public virtual bool IsInAir => !IsGrounded;
        public float AirTime
        {
            get => _airTime;
            protected set => _airTime = value;
        }

        #endregion


        #region - METHODS -

        #region - UNITY METHODS -
        protected virtual void Awake()
        {

        }
        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            this.UpdateTime(Time.deltaTime);
            this.UpdateCollider();
        }

        protected virtual void FixedUpdate()
        {
            this.HandleGround();
            this.GroundLift();
            this.HandleSpeedAndAcceleration();
        }

        #endregion

        /// <summary>
        ///     Keep creature above ground
        /// </summary>
        protected virtual void GroundLift()
        {
            // Not on ground, do nothing
            if (!CheckIsGroundLift()) return;

            //the speed to lift the character on ground
            GroundLiftSpeed = Mathf.Max(0.0f, _groundYPosition - transform.position.y) * 7.5f;

            //apply force to standing rigidbody
            Vector2 force = _weight * Physics2D.gravity;
            this.ApplyForceToStandingColliders(force);
        }

        /// <summary>
        ///     Check if the creature is on the ground to lift
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckIsGroundLift()
        {
            if (!IsGrounded) return false;
            return true;
        }

        /// <summary>
        ///     Check if the creature is on ground
        /// </summary>
        protected virtual void HandleGround()
        {
            //get raycast results
            RaycastHit2D leftRaycastHit = gameObject.Raycast(GroundRaycastFrontPosition, Vector2.down, GroundRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
            RaycastHit2D midRaycastHit = gameObject.Raycast(GroundRaycastMidPosition, Vector2.down, GroundRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
            RaycastHit2D rightRraycastHit = gameObject.Raycast(GroundRaycastBackPosition, Vector2.down, GroundRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);

            bool isHit = leftRaycastHit.collider || midRaycastHit.collider || rightRraycastHit.collider;
            _groundYPosition = Mathf.Max(leftRaycastHit.point.y, midRaycastHit.point.y, rightRraycastHit.point.y);

            this.SetStandingCollider(leftRaycastHit, midRaycastHit, rightRraycastHit);

            //check grounded
            if (isHit && _groundYPosition >= transform.position.y - Constants.PIXEL_SIZE)
            {
                IsGrounded = true;
                this.RevertIgnoredPlatforms();
            }
            else IsGrounded = false;

            _currentGravityScale = IsGrounded ? 0.0f : _gravityScale;

            this.HandleSurface(leftRaycastHit, midRaycastHit, rightRraycastHit);
        }

        /// <summary>
        ///     Set the standing collider and position list, and check if the Creature is standing on a platform.
        /// </summary>
        /// <param name="raycastHits"></param>
        protected virtual void SetStandingCollider(params RaycastHit2D[] raycastHits)
        {
            _standingPosList.Clear();
            _standingColliders.Clear();

            foreach (RaycastHit2D hit in raycastHits)
            {
                if (hit.collider == null) continue; // Skip if no collider
                
                _standingColliders.Add(hit.collider);
                _standingPosList.Add(hit.point);
            }

            IsStandingOnPlatform = this.CheckStandingOnPlatforms();
        }

        /// <summary>
        ///     Check creature is on platforms or not.
        /// </summary>
        protected virtual bool CheckStandingOnPlatforms()
        {
            if (_standingColliders == null || _standingColliders.Count <= 0) return false;

            bool isStandingOnPlatform = _standingColliders[0].gameObject.TryGetComponent<Platform>(out _);
            for (int i = 1; i < _standingColliders.Count; i++)
            {
                isStandingOnPlatform = isStandingOnPlatform && _standingColliders[i].gameObject.TryGetComponent<Platform>(out _);
            }

            return isStandingOnPlatform;
        }

        /// <summary>
        ///     Revert the ignored platforms, so the creature can stand on them again.
        /// </summary>
        /// <param name="colliders"></param>
        protected virtual void RevertIgnoredPlatforms()
        {
            _checkList.Clear();
            _checkList.Add(gameObject.Raycast(GroundRaycastFrontPosition, Vector2.down, GroundRaycastDistance, _groundCheckLayerMask, isIgnorePlatform: false).collider);
            _checkList.Add(gameObject.Raycast(GroundRaycastMidPosition, Vector2.down, GroundRaycastDistance, _groundCheckLayerMask, isIgnorePlatform: false).collider);
            _checkList.Add(gameObject.Raycast(GroundRaycastBackPosition, Vector2.down, GroundRaycastDistance, _groundCheckLayerMask, isIgnorePlatform: false).collider);

            _ignoreColliders.RemoveWhere(collider => !_checkList.Contains(collider));
        }

        protected virtual void HandleSurface(params RaycastHit2D[] raycastHits)
        {
            SurfaceNormal = CalculateSurfaceNormal(raycastHits);

            SurfaceDirection = Vector2Utils.RotateVector(SurfaceNormal, -90.0f * (int)FacingDirection);
            if (SurfaceDirection.y > 0) _surfaceDirectionDown = -SurfaceDirection;
            else _surfaceDirectionDown = SurfaceDirection;

            SurfaceAngle = Vector2.Angle(SurfaceNormal, Vector2.up);
            SurfaceAngleForward = Vector2.Angle(SurfaceDirection, Vector2.up);

            // cancel velocity parts that is perpendicular to ground surface
            if (IsGrounded)
            {
                float dot = Vector2.Dot(_currentVelocity, SurfaceNormal);
                if (dot < 0f) _currentVelocity -= dot * SurfaceNormal;
            }
        }

        protected virtual Vector2 CalculateSurfaceNormal(params RaycastHit2D[] raycastHits)
        {
            if (!IsGrounded) return Vector2.up;
            if (raycastHits.Length == 0) return Vector2.up;

            Vector2 normal = Vector2.up;
            float minAngle = 90.0f;
            
            foreach (RaycastHit2D hit in raycastHits)
            {
                if (!hit.collider) continue; // Skip if no collider
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    normal = hit.normal;
                }                
            }
            
            return normal;
        }

        /// <summary>
        ///    Ignore the standing platforms, so the creature can fall through them.
        /// </summary>
        /// <param name="colliders"></param>
        protected virtual void IgnoreStandingPlatforms()
        {
            foreach (Collider2D collider in _standingColliders)
            {
                if (collider == null) continue; // Skip if no collider
                // if (_ignoreColliders.Contains(collider)) continue;
                if (collider.gameObject.TryGetComponent<Platform>(out _)) _ignoreColliders.Add(collider);
            }
        }

        /// <summary>
        ///     Apply force to the standing colliders.
        /// </summary>
        /// <param name="force"></param>
        protected virtual void ApplyForceToStandingColliders(Vector2 force)
        {
            if (_standingColliders == null || _standingColliders.Count <= 0) return;

            Vector2 finalForce = force / _standingColliders.Count;
            for (int i = 0; i < _standingColliders.Count; i++)
            {
                Collider2D collider = _standingColliders[i];
                Vector2 position = _standingPosList[i];

                if (collider == null) continue; // Skip if no collider
                if (collider.attachedRigidbody) collider.attachedRigidbody.AddForceAtPosition(finalForce, position);
            }
        }

        /// <summary>
        ///     Slide down slope if the angle is too steep
        /// </summary>
        protected virtual void SlideDownOnSlope(float deltaTime)
        {
            this.HandleSlideDown();

            if (IsSliding == false)
            {
                _slideSpeed = Mathf.Lerp(_slideSpeed, 0.0f, 2.0f * Time.deltaTime);
                return;
            }

            float targetSlideSpeed = Mathf.Lerp(0.0f, 1.0f, SurfaceAngle / 90.0f) * _slideMaxSpeed;
            _slideSpeed = Mathf.Lerp(_slideSpeed, targetSlideSpeed, 2.0f * Time.deltaTime);
            SlideVelocity = Mathf.Lerp(0.0f, 1.0f, SurfaceAngle / 90.0f) * _slideSpeed * _surfaceDirectionDown;
            transform.position += (Vector3)SlideVelocity * deltaTime;
        }

        protected virtual void HandleSlideDown()
        {
            IsSliding = true;

            if (!IsGrounded) IsSliding = false;
            if (SurfaceAngle < _surfaceAngleLimit) IsSliding = false;
        }

        protected virtual void UpdateTime(float deltaTime)
        {
            //air timer
            if (IsInAir) AirTime += deltaTime;
            else AirTime = 0.0f;
        }

        protected virtual void UpdateFacing(float moveDirection, Vector2 targetPosition)
        {
            // Look at target
            if (Owner.View.IsLookingAtTarget)
            {
                FacingDirection = Owner.View.LookAtTargetFacing(targetPosition, FacingDirection);
                return;
            }

            // Move
            if (Mathf.Abs(moveDirection) > Constants.MOVE_THRESHOLD)
            {
                FacingDirection = (FacingType)Mathf.RoundToInt(Mathf.Sign(moveDirection));
                return;
            }
        }

        protected virtual void HandleSpeedAndAcceleration()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void UpdateCollider()
        {

        }
        #endregion
    }
}