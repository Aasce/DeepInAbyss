using Asce.Game.Enviroments;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreaturePhysicController : MonoBehaviour, IHasOwner<Creature>
    {
        #region - FIELDS -

        [Header("Reference")]
        [SerializeField, Readonly] private Creature _owner;                                   // the creature that owns this controller
        [SerializeField, Readonly] protected Rigidbody2D _rigidbody;
        [SerializeField] protected Collider2D _bodyCollider;

        [Header("Physic")]
        [SerializeField] protected float _weight = 50.0f;
        [SerializeField] protected float _groundDrag = 10.0f;                         // braking acceleration (from movement to still) while on ground
        [SerializeField] protected float _airDrag = 0.75f;                            // braking acceleration (from movement to still) while in air
        [SerializeField] protected float _gravityScale = 1f;
        public float currentGravityScale;
        public Vector2 currentVelocity;

        [Header("Ground")]
        [SerializeField] protected LayerMask _groundCheckLayerMask;
        protected bool _isGrounded;
        protected float _groundYPosition;

        // Ground lift
        [SerializeField] protected float _groundLiftMaxSpeed = 7.5f;
        protected float _groundLiftSpeed;

        /// <summary>
        ///     The velocity y threshold to triger the OnLand event
        /// </summary>
        protected readonly float _landEventThreshold = 3.0f;

        [Header("Sliding")]
        [SerializeField] private float _slideMaxSpeed = 5.0f; // the maxSpeed speed the character slide down a slope it can not stand on
        protected bool _isSliding;
        protected float _slideSpeed;
        protected Vector2 _slideVelocity;

        // Standing colliders
        protected bool _isGetDownPlatform = false;
        protected Cooldown _getDownPlatformCooldown = new(0.1f);

        protected bool _isStandingOnPlatform; // is the character standing on a one-way platform
        protected readonly List<Collider2D> _standingColliders = new(); // collider the character is standing on
        protected readonly List<Vector2> _standingPosList = new();
        protected readonly HashSet<Collider2D> _ignoreColliders = new();
        protected readonly HashSet<Collider2D> _checkList = new();

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
        protected bool _isUpdateCollider;

        // Timer
        protected float _airTime;

        #endregion

        #region - EVENTS -

        public event Action<object> OnLand;

        #endregion

        #region - PROPERTIES -
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

        public virtual float Weight
        {
            get => _weight;
            set => _weight = value;
        }

        #region - GROUND PROPERTIES -

        /// <summary>
        ///     Is the character on ground
        /// </summary>
        public virtual bool IsGrounded
        {
            get => _isGrounded;
            set
            {
                if (_isGrounded == value) return;
                _isGrounded = value;

                //ground event
                if (_isGrounded && currentVelocity.y < -_landEventThreshold) OnLand?.Invoke(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual float GroundRaycastDistance => Constants.PIXEL_SIZE * 25f;

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
        public virtual Vector2 GroundRaycastFrontPosition => GroundRaycastMidPosition + new Vector2(Constants.PIXEL_SIZE * 8f * Owner.Status.FacingDirectionValue, 0.0f);

        /// <summary>
        ///     raycast parameters for ground check
        /// </summary>
        public virtual Vector2 GroundRaycastBackPosition => GroundRaycastMidPosition + new Vector2(Constants.PIXEL_SIZE * 8f * -Owner.Status.FacingDirectionValue, 0.0f);

        /// <summary>
        ///     
        /// </summary>
        public float GroundLiftSpeed
        {
            get => _groundLiftSpeed;
            protected set => _groundLiftSpeed = value;
        }
        public float GroundLiftMaxSpeed
        {
            get => _groundLiftMaxSpeed;
            protected set => _groundLiftMaxSpeed = value;
        }

        #endregion

        #region - SLIDING PROPERTIES -
        public bool IsSliding
        {
            get => _isSliding;
            protected set => _isSliding = value;
        }

        public float SlideSpeed
        {
            get => _slideSpeed;
            protected set => _slideSpeed = value;
        }

        public float SlideMaxSpeed => _slideMaxSpeed;
        

        public Vector2 SlideVelocity
        {
            get => _slideVelocity;
            protected set => _slideVelocity = value;
        }

        #endregion

        #region - GET DOWN PLATFORM -
        public bool IsGetDownPlatform
        {
            get => _isGetDownPlatform;
            set => _isGetDownPlatform = value;
        }
        #endregion

        #region - STANDING COLLIDER PROPERTIES -
        public virtual bool IsStandingOnPlatform
        {
            get => _isStandingOnPlatform;
            protected set => _isStandingOnPlatform = value;
        }

        /// <summary>
        ///     Returns the Collider2D that the Creature is standing on.
        /// </summary>
        public Collider2D StandingCollider => _standingColliders.Count <= 0 ? null : _standingColliders[0];
        #endregion

        #region - SURFACE PROPERTIES -
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
        public Vector2 SurfaceDirectionDown
        {
            get => _surfaceDirectionDown;
            protected set => _surfaceDirectionDown = value;
        }
        public float SurfaceSpeedMultiply
        {
            get => _surfaceSpeedMultiply;
            set => _surfaceSpeedMultiply = value;
        }
        #endregion

        public bool IsUpdateCollider
        {
            get => _isUpdateCollider;
            set => _isUpdateCollider = value;
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
        protected virtual void Reset()
        {
            transform.LoadComponent(out _rigidbody);
            if (transform.LoadComponent(out _owner))
            {
                Owner.PhysicController = this;
            }
        }

        protected virtual void Awake()
        {

        }
        protected virtual void Start()
        {
            Owner.Status.OnDeath += Status_OnDeath;
            Owner.Status.OnRevive += Status_OnRevive;
        }

        protected virtual void Update()
        {
            this.UpdateTime(Time.deltaTime);
            this.UpdateCollider();
        }

        protected virtual void FixedUpdate()
        {
            currentVelocity = Rigidbody.linearVelocity - Vector2.up * GroundLiftSpeed;
            currentGravityScale = _gravityScale;

            this.PhysicUpdate(Time.fixedDeltaTime);
            Owner.Action.HandlePhysic(Time.fixedDeltaTime);

            Rigidbody.gravityScale = currentGravityScale;
            Rigidbody.linearVelocity = currentVelocity + Vector2.up * GroundLiftSpeed;
        }

        #endregion

        #region - UPDATE AND HANDLE METHODS -

        protected virtual void PhysicUpdate(float deltaTime)
        {
            this.HandleGround();
            this.GroundLift();
            this.HandleSpeedAndAcceleration();
            this.SlideDownOnSlope(deltaTime);
        }

        /// <summary>
        ///     Check if the creature is on ground
        /// </summary>
        protected virtual void HandleGround()
        {
            if (Owner.Status.IsDead) return;

            //get raycast results
            RaycastHit2D leftRaycastHit = gameObject.Raycast(GroundRaycastFrontPosition, Vector2.down, GroundRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
            RaycastHit2D midRaycastHit = gameObject.Raycast(GroundRaycastMidPosition, Vector2.down, GroundRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
            RaycastHit2D rightRraycastHit = gameObject.Raycast(GroundRaycastBackPosition, Vector2.down, GroundRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);

            this.SetStandingCollider(leftRaycastHit, midRaycastHit, rightRraycastHit);
            this.SetGroundYPosition(leftRaycastHit, midRaycastHit, rightRraycastHit);

            //check grounded
            bool isHit = leftRaycastHit.collider || midRaycastHit.collider || rightRraycastHit.collider;
            if (isHit && _groundYPosition >= transform.position.y - Constants.PIXEL_SIZE)
            {
                IsGrounded = true;
                this.RevertIgnoredPlatforms();
            }
            else IsGrounded = false;

            currentGravityScale = IsGrounded ? 0.0f : _gravityScale;

            this.HandleSurface(leftRaycastHit, midRaycastHit, rightRraycastHit);

            // If the character is grounded, remove the velocity component that is pushing into the ground
            if (IsGrounded)
            {
                float dot = Vector2.Dot(currentVelocity, SurfaceNormal); // Project the velocity onto the surface normal

                // If the projection indicates movement into the surface, cancel it
                if (dot < 0f) currentVelocity -= dot * SurfaceNormal;
            }
        }

        /// <summary>
        ///     Handle sliding down slopes.
        /// </summary>
        protected virtual void HandleSlideDown()
        {
            IsSliding = true;

            if (!IsGrounded) IsSliding = false;
            if (SurfaceAngle < _surfaceAngleLimit) IsSliding = false;
        }

        /// <summary>
        ///     Handles surface-related calculations based on raycast hits.
        ///     Computes surface normal, direction, angle, and adjusts velocity if grounded.
        /// </summary>
        /// <param name="raycastHits"> Array of raycast hit results used to determine surface information. </param>
        protected virtual void HandleSurface(params RaycastHit2D[] raycastHits)
        {
            SurfaceNormal = CalculateSurfaceNormal(raycastHits); // Calculate the average surface normal from the given raycast hits

            // Determine the surface direction by rotating the normal -90 degrees 
            // (clockwise or counter-clockwise depending on the character's facing direction)
            SurfaceDirection = Vector2Utils.RotateVector(SurfaceNormal, -90.0f * Owner.Status.FacingDirectionValue);

            // If the surface direction is pointing upward, invert it to get the downward direction
            if (SurfaceDirection.y > 0) SurfaceDirectionDown = -SurfaceDirection;
            else SurfaceDirectionDown = SurfaceDirection;

            SurfaceAngle = Vector2.Angle(SurfaceNormal, Vector2.up); // Calculate the angle between the surface normal and the upward direction
            SurfaceAngleForward = Vector2.Angle(SurfaceDirection, Vector2.up); // Calculate the angle between the surface movement direction and the upward direction
        }

        /// <summary>
        ///     Handle the speed and acceleration of the creature.
        /// </summary>
        protected virtual void HandleSpeedAndAcceleration()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void UpdateCollider()
        {

        }

        /// <summary>
        ///     Update the time
        /// </summary>
        /// <param name="deltaTime"></param>
        protected virtual void UpdateTime(float deltaTime)
        {
            //air timer
            if (IsInAir) AirTime += deltaTime;
            else AirTime = 0.0f;
        }

        #endregion

        #region - GROUND METHODS -
        /// <summary>
        ///     Keep creature above ground
        /// </summary>
        protected virtual void GroundLift()
        {
            // Not on ground, do nothing
            if (!CheckIsGroundLift()) return;

            //the speed to lift the character on ground
            GroundLiftSpeed = Mathf.Max(0.0f, _groundYPosition - transform.position.y) * GroundLiftMaxSpeed;

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

        protected virtual void SetGroundYPosition(RaycastHit2D leftRaycastHit, RaycastHit2D midRaycastHit, RaycastHit2D rightRraycastHit)
        {
            _groundYPosition = Mathf.Max(leftRaycastHit.point.y, midRaycastHit.point.y, rightRraycastHit.point.y);
        }
        #endregion

        #region - STANDING COLLIDER METHODS -

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

            _ignoreColliders.RemoveWhere((collider) =>
            {
                bool isRemove = !_checkList.Contains(collider);
                if (isRemove)
                {
                    Physics2D.IgnoreCollision(BodyCollider, collider, false);
                }
                return isRemove;
            });
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
                if (_ignoreColliders.Contains(collider)) continue;
                if (collider.gameObject.TryGetComponent<Platform>(out _))
                {
                    _ignoreColliders.Add(collider); 
                    Physics2D.IgnoreCollision(BodyCollider, collider);
                }
            }
        }

        /// <summary>
        ///     Apply force to the standing colliders.
        /// </summary>
        /// <param name="force"></param>
        public virtual void ApplyForceToStandingColliders(Vector2 force)
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
        #endregion

        #region - SLIDING METHODS -

        /// <summary>
        ///     Slide down slope if the angle is too steep
        /// </summary>
        protected virtual void SlideDownOnSlope(float deltaTime)
        {
            if (!IsSliding)
            {
                SlideSpeed = Mathf.Lerp(SlideSpeed, 0f, 2f * deltaTime);
                return;
            }

            float targetSlideSpeed = Mathf.Lerp(0f, 1f, SurfaceAngle / 90.0f) * SlideMaxSpeed;
            SlideSpeed = Mathf.Lerp(SlideSpeed, targetSlideSpeed, 2.0f * deltaTime);
            SlideVelocity = Mathf.Lerp(0.0f, 1.0f, SurfaceAngle / 90.0f) * SlideSpeed * SurfaceDirectionDown;
            Rigidbody.MovePosition(Rigidbody.position + SlideVelocity * deltaTime);
        }
        #endregion

        #region - SURFACE METHODS -
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

        #endregion

        #region - EVENT REGISTER -

        protected virtual void Status_OnDeath(object sender)
        {

        }
        protected virtual void Status_OnRevive(object sender)
        {

        }

        #endregion

        public virtual void TriggerUpdateCollider() => IsUpdateCollider = true;
        
        #endregion
    }
}