using Asce.Game.Enviroments;
using Asce.Managers.Utils;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Asce.Game.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class CharacterPhysicController : CreaturePhysicController, IHasOwner<Character>
    {
        #region - FIELDS -

        private readonly Vector2 _centerPositionOffset = new(0.0f, Constants.PIXEL_SIZE * 30f);
        private readonly Vector2 _neckPositionOffset = new(0.0f, Constants.PIXEL_SIZE * 40f);
        private readonly Vector2 _bottomPositionOffset = new(0.0f, Constants.PIXEL_SIZE);

        [Space]
        [SerializeField] protected float _airMaxSpeed = 2.0f;                         // maxSpeed move speed while in air
        [SerializeField] protected float _airAcceleration = 8.0f;                     // air acceleration

        protected float _currentSpeed;
        protected float _currentAcceleration;
        protected float _currentDragAcceleration;


        protected bool _hasReachedLadderTop;
        protected bool _hasReachedLadderBottom;
        protected Ladder _ladder; // the ladder the character is climbing, if there is;    

        #endregion

        #region - EVENTS -
        public event Action<object> OnFootstepEvent;


        #endregion

        #region - PROPERTIES - 

        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }

        public new CapsuleCollider2D BodyCollider
        {
            get => base.BodyCollider as CapsuleCollider2D;
            set => base.BodyCollider = value;
        }

        private Vector2 CenterPosition => (Vector2)transform.position + _centerPositionOffset;   //world position of the character's center
        private Vector2 NeckPosition => (Vector2)transform.position + _neckPositionOffset;       //world position of the character's neck
        private Vector2 BottomPosition => (Vector2)transform.position + _bottomPositionOffset;   //world position of the character collider's bottom

        public float CurrentSpeed
        {
            get => _currentSpeed;
            protected set => _currentSpeed = value;
        }
        public float CurrentAcceleration
        {
            get => _currentAcceleration;
            protected set => _currentAcceleration = value;
        }
        public float CurrentDragAcceleration
        {
            get => _currentDragAcceleration;
            protected set => _currentDragAcceleration = value;
        }


        #region - CROUCH -
        private float CrouchRaycastDistance => _colliderSize.y + Constants.PIXEL_SIZE;
        private Vector2 CrouchRaycastFrontPosition
        {
            get
            {
                var pos = BodyCollider.bounds.center;
                pos.x += Constants.PIXEL_SIZE * 10f * Owner.Status.FacingDirectionValue;
                pos.y = BodyCollider.bounds.min.y;
                return pos;
            }
        }
        private Vector2 CrouchRaycastBackPosition
        {
            get
            {
                var pos = BodyCollider.bounds.center;
                pos.x -= Constants.PIXEL_SIZE * 10f * Owner.Status.FacingDirectionValue;
                pos.y = BodyCollider.bounds.min.y;
                return pos;
            }
        }

        public bool CanExitCrouching
        {
            get
            {
                if (Owner.Action.IsCrouching == false) return true;

                RaycastHit2D frontHit = gameObject.Raycast(CrouchRaycastFrontPosition, Vector2.up, CrouchRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
                RaycastHit2D backHit = gameObject.Raycast(CrouchRaycastBackPosition, Vector2.up, CrouchRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
                return (frontHit.collider == null) && (backHit.collider == null);
            }
        }

        #endregion

        #region - CRAWL -
        private Vector2 CrawlEnterRaycastPosition => new(transform.position.x, transform.position.y + Constants.PIXEL_SIZE * 16f);
        private Vector2 CrawlExitRaycastPosition => new(transform.position.x, transform.position.y + Constants.PIXEL_SIZE * 10f);
        private float CrawlEnterRaycastDistance => Constants.PIXEL_SIZE * 32f;
        private float CrawlExitRaycastDistance => Constants.PIXEL_SIZE * 37f;

        #endregion

        #region - CLIMB LADDER - 
        /// <summary>
        ///     Raycast parameters for climbing to the ledge the ladder connects to
        /// </summary>
        public Vector2 LadderExitRaycastPosition => (Vector2)transform.position + new Vector2(15 * Owner.Status.FacingDirectionValue, 40) * Constants.PIXEL_SIZE;
        public float LadderExitRaycastDistance => 40 * Constants.PIXEL_SIZE;

        public Ladder Ladder
        {
            get => _ladder;
            protected set => _ladder = value;
        }

        public bool HasReachedLadderTop
        {
            get => _hasReachedLadderTop;
            protected set => _hasReachedLadderTop = value;
        }

        public bool HasReachedLadderBottom
        {
            get => _hasReachedLadderBottom;
            protected set => _hasReachedLadderBottom = value;
        }

        #endregion

        #region - LEDGE CLIMB -


        //raycast parameters for climbing up ledge
        public Vector2 LedgeClimbRaycastPosition => (Vector2)transform.position + new Vector2(Constants.PIXEL_SIZE * Owner.Status.FacingDirectionValue * 14f, LedgeClimbRaycastHeight);
        public float LedgeClimbRaycastHeight => Constants.PIXEL_SIZE * 48f;
        public float LedgeClimbRaycastDistance => Constants.PIXEL_SIZE * 26f;

        #endregion

        #region - GROUND -

        public override Vector2 GroundRaycastFrontPosition
        {
            get
            {
                float xAxis = Owner.Action.IsCrawling ? 16f * Constants.PIXEL_SIZE : 8f * Constants.PIXEL_SIZE;
                return GroundRaycastMidPosition + new Vector2(xAxis * Owner.Status.FacingDirectionValue, 0.0f);
            }
        }
        public override Vector2 GroundRaycastBackPosition
        {
            get
            {
                float xAxis = Owner.Action.IsCrawling ? 16f * Constants.PIXEL_SIZE : 8f * Constants.PIXEL_SIZE;
                return GroundRaycastMidPosition + new Vector2(xAxis * -Owner.Status.FacingDirectionValue, 0.0f);
            }
        }

        #endregion

        #region - AIR -
        public override bool IsInAir => !IsGrounded && !Owner.Action.IsClimbingLadder && !Owner.Action.IsExitingLadder && !Owner.Action.IsClimbingLedge;

        #endregion

        #region - COLLIDER - 
        protected readonly Vector2 _colliderSize = new(0.375f, 1.3125f);
        protected readonly Vector2 _colliderOffset = new(0.0f, 1.15625f);

        protected readonly Vector2 _colliderCrouchSize = new(0.375f, 1.0f);
        protected readonly Vector2 _colliderCrouchOffset = new(0.0f, 1.0f);

        protected readonly Vector2 _colliderCrawlSize = new(1.0f, 0.5f);
        protected readonly Vector2 _colliderCrawlOffset = new(0.1875f, 0.5f);

        #endregion

        #endregion

        #region - UNITY CALLBACKS - 
        protected override void Start()
        {
            base.Start();
            UpdateCollider();
        }

        protected override void Update()
        {
            this.HandleLadderClimb();
            this.HandleSlideDown();
            this.GetDownPlatform();

            base.Update();
        }

        protected override void PhysicUpdate(float deltaTime)
        {
            base.PhysicUpdate(deltaTime);


            this.ApplyRootMotion();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Ladder ladder))
            {
                if (this.Ladder == null) this.Ladder = ladder;
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Ladder ladder))
            {
                if (this.Ladder == ladder)
                {
                    this.Ladder = null;
                    Owner.Action.IsClimbingLadder = false;
                }
            }
        }

        #endregion

        #region - METHODS -

        // Ground & Platform

        /// <summary>
        ///     Keep character above ground
        /// </summary>
        protected override bool CheckIsGroundLift()
        {
            if (!IsGrounded) return false;
            if (Owner.Action.IsClimbingLadder) return false;
            if (Owner.Action.IsExitingLadder) return false;
            if (Owner.Action.IsClimbingLedge) return false;

            return true;
        }

        /// <summary>
        ///     When player is standing on a one-way platform and has down direction input
        ///     get down this platform
        /// </summary>
        /// <param name="direction"></param>
        protected virtual void GetDownPlatform()
        {
            if (!IsStandingOnPlatform) return;
            if (!IsGetDownPlatform) return;

            _getDownPlatformCooldown.Update(Time.deltaTime);

            if (_getDownPlatformCooldown.IsComplete)
            {
                this.IgnoreStandingPlatforms();
                Owner.View.TriggerGetDownPlatformAnimation();

                _getDownPlatformCooldown.Reset();
            }
        }

        public bool CanClimbToLadder()
        {
            if (Ladder == null) return false;

            bool isClimbTop = NeckPosition.y < Ladder.TopPosition.y && CenterPosition.y > Ladder.BottomPosition.y;
            bool isClimbBottom = BottomPosition.y > Ladder.BottomPosition.y;
            return isClimbTop || isClimbBottom;
        }

        public float CalculateLadderEnterHeight()
        {
            return LadderExitRaycastPosition.y - Ladder.TopPosition.y;
        }

        public void UnLadder()
        {
            this.Ladder = null;
        }

        public bool CheckSurfaceToClimbOn(out RaycastHit2D hit)
        {
            hit = gameObject.Raycast(LadderExitRaycastPosition, Vector2.down, LadderExitRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
            Debug.DrawRay(LadderExitRaycastPosition, Vector2.down * LadderExitRaycastDistance, Color.red, 1f);
            return (hit.collider != null);
        }

        public virtual bool FindLedgeToClimb(Vector2 ledgePosition, out RaycastHit2D hit)
        {
            Vector2 rayPosition = ledgePosition + new Vector2(0.0f, Constants.PIXEL_SIZE);
            float rayDistance = Constants.PIXEL_SIZE * 4.0f;

            hit = gameObject.Raycast(rayPosition, Vector2.down, rayDistance, _groundCheckLayerMask, isIgnorePlatform: true, skipColliders: _ignoreColliders);
            Debug.DrawRay(rayPosition, Vector2.down * (hit.collider != null ? hit.distance : rayDistance), Color.yellow, 1f);
            return hit.collider != null;
        }

        public virtual bool CanClimbLedge(out RaycastHit2D hit)
        {
            hit = gameObject.Raycast(LedgeClimbRaycastPosition, Vector2.down, LedgeClimbRaycastDistance, _groundCheckLayerMask, true, _ignoreColliders);
            return hit.collider != null;
        }

        public virtual bool CheckEnoughSpaceToLedge(Vector2 upCheckPosition, out RaycastHit2D hit)
        {
            hit = gameObject.Raycast(upCheckPosition, Vector2.up, Constants.PIXEL_SIZE * 32f, _groundCheckLayerMask, true, _ignoreColliders);
            return hit.collider == null;
        }

        protected virtual void SetSpeedAndAcceleration(float acceleration, float maxSpeed)
        {
            CurrentAcceleration = acceleration;
            CurrentSpeed = maxSpeed;
        }

        public virtual void Accelerate(float speed)
        {
            currentVelocity.x = speed;
            Rigidbody.linearVelocity = currentVelocity;
        }

        public bool CanEnterCrawling()
        {
            if (Owner.Action.IsCrawling == true) return true;

            RaycastHit2D leftHit = gameObject.Raycast(CrawlEnterRaycastPosition, Vector2.left, CrawlEnterRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
            RaycastHit2D rightHit = gameObject.Raycast(CrawlEnterRaycastPosition, Vector2.right, CrawlEnterRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);

            float distance = 0.0f;
            if (leftHit.collider != null) distance += leftHit.distance;
            else distance += 1.0f;

            if (rightHit.collider != null) distance += rightHit.distance;
            else distance += 1.0f;

            return distance > 0.9f;
        }

        /// <summary>
        ///     Check if there is enough space to get up from crawling
        /// </summary>
        /// <returns></returns>
        public bool CanExitCrawling()
        {
            if (Owner.Action.IsCrawling == false) return true;

            RaycastHit2D hit = gameObject.Raycast(CrawlExitRaycastPosition, Vector2.up, CrawlExitRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
            return (hit.collider == null);
        }

        #endregion

        #region - UPDATE AND HANDLE -
        protected override void HandleSpeedAndAcceleration()
        {
            base.HandleSpeedAndAcceleration();

            // On Ground
            if (IsGrounded)
            {
                CurrentAcceleration = Owner.Action.WalkAcceleration;
                CurrentSpeed = Owner.Action.WalkMaxSpeed;
                CurrentDragAcceleration = _groundDrag;

                if (Owner.Action.IsRunning) this.SetSpeedAndAcceleration(Owner.Action.RunAcceleration, Owner.Action.RunMaxSpeed);
                else if (Owner.Action.IsCrouching) this.SetSpeedAndAcceleration(Owner.Action.CrouchAcceleration, Owner.Action.CrouchMaxSpeed);
                else if (Owner.Action.IsCrawling || Owner.Action.IsCrawlEntering || Owner.Action.IsCrawlExiting)
                    this.SetSpeedAndAcceleration(Owner.Action.CrawlAcceleration, Owner.Action.CrawlMaxSpeed);

                if (Owner.Action.IsDashing) this.SetSpeedAndAcceleration(Owner.Action.DashAcceleration, Owner.Action.DashMaxSpeed);
                

                float targetSurfaceSpeedMultiply = Mathf.Sin(Mathf.Min(SurfaceAngleForward, 90.0f) * Mathf.Deg2Rad);
                SurfaceSpeedMultiply = targetSurfaceSpeedMultiply < 1.0f
                    ? Mathf.Lerp(SurfaceSpeedMultiply, targetSurfaceSpeedMultiply, Time.fixedDeltaTime)
                    : 1.0f;

                CurrentSpeed *= SurfaceSpeedMultiply;
                return;
            }

            // In Air
            CurrentAcceleration = _airAcceleration;
            CurrentSpeed = _airMaxSpeed;
            CurrentDragAcceleration = _airDrag * Mathf.Abs(currentVelocity.x);

            if (Owner.Action.IsCrouching)
            {
                SetSpeedAndAcceleration(Owner.Action.CrouchAcceleration, Owner.Action.CrouchMaxSpeed);
                CurrentDragAcceleration = _groundDrag;
            }
            else if (Owner.Action.IsCrawling || Owner.Action.IsCrawlEntering || Owner.Action.IsCrawlExiting)
            {
                SetSpeedAndAcceleration(Owner.Action.CrawlAcceleration, Owner.Action.CrawlMaxSpeed);
                CurrentDragAcceleration = _groundDrag;
            }
        }

        protected override void UpdateCollider()
        {
            base.UpdateCollider();
            if (!IsUpdateCollider) return;

            Vector2 size;
            Vector2 offset;
            CapsuleDirection2D direction = CapsuleDirection2D.Vertical;

            if (Owner.Action.IsCrawling || Owner.Action.IsDodging)
            {
                size = _colliderCrawlSize;
                offset = _colliderCrawlOffset;

                direction = CapsuleDirection2D.Horizontal;
            }
            else if (Owner.Action.IsCrouching)
            {
                size = _colliderCrouchSize;
                offset = _colliderCrouchOffset;
            }
            else
            {
                size = _colliderSize;
                offset = _colliderOffset;
            }

            // Adjust offset direction based on facing direction if crawling
            if (Owner.Action.IsCrawling) offset.x *= Owner.Status.FacingDirectionValue;

            // Apply collider settings
            BodyCollider.size = size;
            BodyCollider.offset = offset;
            BodyCollider.direction = direction;

            IsUpdateCollider = false;
        }

        protected override void HandleSlideDown()
        {
            base.HandleSlideDown();
            if (Owner.Action.IsClimbingLedge) IsSliding = false;
        }
        protected virtual void HandleLadderClimb()
        {
            if (Owner.Status.IsDead) return;
            if (!Owner.Action.IsClimbEnabled) return;
            if (!Owner.Action.IsClimbingLadder) return;

            HasReachedLadderTop = (NeckPosition.y >= Ladder.TopPosition.y);
            HasReachedLadderBottom = (Mathf.Abs(BottomPosition.y - Ladder.BottomPosition.y) < Constants.PIXEL_SIZE * 2.0f);
        }

        #endregion

        #region - ROOT MOTION AND ANIMATOR - 

        private void ApplyRootMotion()
        {
            //root motion from dodging
            if (Owner.Action.IsDodging)
            {
                if (Mathf.Abs(Owner.View.RootMotionVelocity.magnitude) > Constants.MOVE_THRESHOLD)
                {
                    currentVelocity.x = Owner.View.RootMotionVelocity.x * Owner.Action.DodgeSpeedRootMotionMultiply;
                    if (currentVelocity.y > 0) currentVelocity.y = 0.0f;
                }
            }

            //root motion form ladder entering & exiting, ledge cimbing
            if (Owner.Action.IsExitingLadder || Owner.Action.IsExitingLadder || Owner.Action.IsClimbingLedge)
            {
                if (Mathf.Abs(Owner.View.RootMotionVelocity.magnitude) > Constants.MOVE_THRESHOLD) 
                    currentVelocity = Owner.View.RootMotionVelocity;
            }
        }

        #endregion

        #region - EVENT HANDLING -

        //footstep event has a string parameter telling which animation this event is from.
        public void OnFootstep(AnimationEvent evt)
        {
            if (AirTime > 0.1f) return;

            if (evt.animatorClipInfo.weight < 0.49f) return;

            if (Owner.Action.IsClimbingLedge)
            {
                if (evt.stringParameter == "Ledge Climb") OnFootstepEvent?.Invoke(this);
                return;
            }

            if (Owner.Action.IsCrawling)
            {
                if (evt.stringParameter == "Crawl") OnFootstepEvent?.Invoke(this);
                return;
            }

            if (Owner.Action.IsCrouching)
            {
                if (evt.stringParameter == "Crouch") OnFootstepEvent?.Invoke(this);
                return;
            }

            if (evt.stringParameter == "Walk" && Owner.View.MoveBlend > 0.1f && Owner.View.MoveBlend < 1.1f)
            {
                OnFootstepEvent?.Invoke(this);
                return;
            }
            if (evt.stringParameter == "Run" && Owner.View.MoveBlend > 1.1f)
            {
                OnFootstepEvent?.Invoke(this);
                return;
            }
            if (evt.stringParameter == "Ladder Climb" || evt.stringParameter == "Ladder Exit")
            {
                OnFootstepEvent?.Invoke(this);
                return;
            }
        }

        #endregion

        #region - DEBUG - 
#if UNITY_EDITOR
        [Header("Debug")]
        public bool debug_drawLedgePos;
        public bool debug_drawLedgeRaycast;
        public bool debug_drawGroundCheckRaycast;
        public bool debug_drawGroundNormal;
        public bool debug_drawGroundDir;
        public bool debug_drawCrouchRaycast;
        public bool debug_drawCrawlEnterRaycast;
        public bool debug_drawCrawlExitRaycast;
        public bool debug_drawVelocity;
        public bool debug_drawSlideVelocity;

        private void OnDrawGizmos()
        {
            //ledge position
            if (debug_drawLedgePos)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(Owner.Action.LedgePosition, 0.1f);
            }

            //ledge raycast
            if (debug_drawLedgeRaycast)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(LedgeClimbRaycastPosition, LedgeClimbRaycastPosition - new Vector2(0.0f, LedgeClimbRaycastDistance));
            }

            //ground check normal
            if (debug_drawGroundCheckRaycast)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(GroundRaycastFrontPosition, GroundRaycastFrontPosition - new Vector2(0.0f, GroundRaycastDistance));
                Gizmos.DrawLine(GroundRaycastMidPosition, GroundRaycastMidPosition - new Vector2(0.0f, GroundRaycastDistance));
                Gizmos.DrawLine(GroundRaycastBackPosition, GroundRaycastBackPosition - new Vector2(0.0f, GroundRaycastDistance));
            }

            //ground normal
            if (debug_drawGroundNormal && IsGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position - Vector3.forward, transform.position + new Vector3(SurfaceNormal.x, SurfaceNormal.y) * 2.0f - Vector3.forward);
            }

            //ground direction
            if (debug_drawGroundDir && IsGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position - Vector3.forward, transform.position + new Vector3(SurfaceDirection.x, SurfaceDirection.y) * 2.0f - Vector3.forward);
            }

            //crouch raycast
            if (debug_drawCrouchRaycast)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(CrouchRaycastFrontPosition, CrouchRaycastFrontPosition + Vector2.up * CrouchRaycastDistance);
                Gizmos.DrawLine(CrouchRaycastBackPosition, CrouchRaycastBackPosition + Vector2.up * CrouchRaycastDistance);
            }
            //crawl enter raycast
            if (debug_drawCrawlEnterRaycast)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(CrawlEnterRaycastPosition, CrawlEnterRaycastPosition + Vector2.left * CrawlEnterRaycastDistance);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(CrawlEnterRaycastPosition, CrawlEnterRaycastPosition + Vector2.right * CrawlEnterRaycastDistance);
            }

            //crawl exit raycast
            if (debug_drawCrawlExitRaycast)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(CrawlExitRaycastPosition, CrawlExitRaycastPosition + Vector2.up * CrawlExitRaycastDistance);
            }

            //velocity
            if (debug_drawVelocity)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position - Vector3.forward, transform.position - Vector3.forward + new Vector3(currentVelocity.x, currentVelocity.y, 0.0f));
            }

            //slide velocity
            if (debug_drawSlideVelocity && IsSliding)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position - Vector3.forward, transform.position - Vector3.forward + new Vector3(SlideVelocity.x, SlideVelocity.y, 0.0f));
            }

        }
#endif
        #endregion
    }
}
