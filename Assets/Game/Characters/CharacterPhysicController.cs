using Asce.Game.Enviroments;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class CharacterPhysicController : CreaturePhysicController, IHasOwner<Character>
    {
        #region - FIELDS -
        private readonly float _climbPositionLerpSpeed = 7.5f;                  // lerp speed when moving the character to the climb position

        private readonly Vector2 _centerPositionOffset = new(0.0f, Constants.PIXEL_SIZE * 30f);
        private readonly Vector2 _neckPositionOffset = new(0.0f, Constants.PIXEL_SIZE * 40f);
        private readonly Vector2 _bottomPositionOffset = new(0.0f, Constants.PIXEL_SIZE);

        [Header("Movement")]
        protected bool _isMoving;                                                     // is the character moving
        protected bool _isRunning;                                                    // is the character running
        protected int _horizontalMoveDirection = 1;                                             // move direction, 1: forward, -1:backward
        protected float _moveBlend;                                                   // current move blend, for blending idle, walk, run animation, lerps to target move blend on frame update
        protected float _targetMoveBlend;                                             // target move blend

        protected float _idleTime;

        [SerializeField] protected float _walkMaxSpeed = 2.5f;                        // maxSpeed walk speed
        [SerializeField] protected float _walkAcceleration = 10.0f;                   // walking acceleration

        [Space]
        [SerializeField] protected float _runMaxSpeed = 5.0f;                         // maxSpeed run speed
        [SerializeField] protected float _runAcceleration = 15.0f;                    // running acceleration

        [Space]
        [SerializeField] protected float _dashSpeedStart = 2.5f;
        [SerializeField] protected float _dashMaxSpeed = 7.0f;
        [SerializeField] protected float _dashAcceleration = 20.0f;

        [Space]
        [SerializeField] protected float _crouchMaxSpeed = 1.0f;                      // maxSpeed move speed while crouching
        [SerializeField] protected float _crouchAcceleration = 8.0f;                  // crouching acceleration

        [Space]
        [SerializeField] protected float _crawlMaxSpeed = 1.0f;                       // maxSpeed move speed while crawling
        [SerializeField] protected float _crawlAcceleration = 8.0f;                   // crawling acceleration

        [Space]
        [SerializeField] protected float _airMaxSpeed = 2.0f;                         // maxSpeed move speed while in air
        [SerializeField] protected float _airAcceleration = 8.0f;                     // air acceleration

        protected float _currentSpeed;
        protected float _currentAcceleration;
        protected float _currentDragAcceleration;

        [Header("Look")]
        [SerializeField] protected bool _isLooking = false;                              // look input
        [SerializeField] protected Vector2 _targetPosition = Vector2.zero;                  // the look at and point at target


        [Header("Crounch and Crawl")]
        protected bool _isCrouching;
        protected bool _shouldCrouch;

        protected bool _shouldCrawl;
        protected bool _isCrawling;
        protected bool _isCrawlEntering;
        protected bool _isCrawlExiting;

        [Header("Jump")]
        [SerializeField] protected bool _jumpEnabled = true;
        [SerializeField] protected float _jumpSpeed = 5.0f;                           // speed applied to character when jump
        [SerializeField] protected float _jumpTolerance = 0.15f;                      // when the character's air time is less than this value, it is still able to jump
        [SerializeField] protected float _jumpGravityMutiplier = 0.6f;                // gravity multiplier when character is jumping, should be within [0.0,1.0], set it to lower value so that the longer you press the jump button, the higher the character can jump    
        [SerializeField] protected float _fallGravityMutiplier = 1.3f;                // gravity multiplier when character is falling, should be equal or greater than 1.0
        [SerializeField] protected Cooldown _jumpCooldown = new (0.2f);

        protected Vector2 _startJumpVelocity;

        [Header("Dash")]
        [SerializeField] protected bool _isDashEnabled = true;
        [SerializeField] protected Cooldown _dashingTime = new (1.0f);
        [SerializeField] protected Cooldown _dashCooldown = new (1.0f);

        private bool _isDashing;

        [Header("Dodge")]
        [SerializeField] protected bool _isDdodgeEnabled = true;
        [SerializeField] protected float _dodgeSpeedMultiply = 1.25f;
        [SerializeField] protected Cooldown _dodgeCooldown = new (0.1f);

        protected bool _isDodging;
        protected int _dodgeDirection; //dodge direction   1: front  -1:back
        protected int _dodgeFacing;

        [Header("Climb")]
        [SerializeField] protected bool _climbEnabled = true;
        [SerializeField] protected float _climbSpeed = 1.0f; // climb speed
        [SerializeField] protected float _climbSpeedFast = 1.5f; // climb speed when runing
        protected float _climbingSpeedMultiply;

        protected Ladder _ladder; // the ladder the character is climbing, if there is;    

        protected bool _isEnteringLadder;
        protected bool _isClimbingLadder;
        protected bool _isExitingLadder;

        protected bool _hasReachedLadderTop;
        protected bool _hasReachedLadderBottom;

        protected float _ladderEnterHeight;
        protected float _ladderExitHeight; // y distance from the button of the character to the surface when exiting from ladder
        protected Cooldown _ladderToAirTime = new (0.5f);
        protected float _ladderExitPositionZ; // origin position z of the character, used for resetting the position z after exiting the ladder

        [Header("Ledge")]
        [SerializeField] private bool _ledgeEnabled = true;
        protected bool _isClimbingLedge; // Is the character climbing ledge
        protected bool _isLedgeClimbLocked;
        protected float _ledgeHeight;
        protected Vector2 _ledgePosition;

        #region - CONTROL PARAMETERS- 
        [SerializeField] private Vector2 _controlMoving = Vector2.zero;                       
        [SerializeField] private bool _controlRunning = false;                
        [SerializeField] private bool _controlDash = false;                   
        [SerializeField] private bool _controlDodge = false;                     
        [SerializeField] private bool _controlCrouching = false;              
        [SerializeField] private bool _controlCrawling = false;               
        [SerializeField] private bool _controlJumping = false;                
        // [SerializeField] private bool inputAttack = false;                 
        // [SerializeField] private bool inputMelee = false;                  

        #endregion

        #endregion

        #region - EVENTS -
        public event Action<object> OnFootstepEvent;
        public event Action<object> OnJump;

        public event Action<object> OnDodgeStart;
        public event Action<object> OnDodgeEnd;

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

        #region - MOVE -
        public bool IsIdle => !IsDead && !IsMoving && !IsInAir && !IsClimbingLadder && !IsExitingLadder && !IsClimbingLedge;

        public bool IsMoving
        {
            get => _isMoving;
            protected set => _isMoving = value;
        }
        public bool IsRunning
        {
            get => _isRunning;
            protected set => _isRunning = value;
        }
        public int HorizontalMoveDirection
        {
            get => _horizontalMoveDirection;
            protected set => _horizontalMoveDirection = (int)Mathf.Sign(value);
        }

        public float IdleTime 
        {
            get => _idleTime;
            protected set => _idleTime = value;
        }
        #endregion

        #region - Dash -
        public bool IsDashing
        {
            get => _isDashing;
            protected set => _isDashing = value;
        }
        #endregion

        #region - LOOK -

        public bool IsLooking
        {
            get => _isLooking;
            protected set => _isLooking = value;
        }

        public Vector2 TargetPosition
        {
            get => _targetPosition;
            protected set => _targetPosition = value;
        }

        #endregion

        #region - JUMP -

        public bool IsJumpEnabled
        {
            get => _jumpEnabled;
            set => _jumpEnabled = value;
        }

        public float JumpSpeed
        {
            get => _jumpSpeed;
            set => _jumpSpeed = value;
        }

        public float JumpTolerance
        {
            get => _jumpTolerance;
            protected set => _jumpTolerance = value;
        }

        public Vector2 StartJumpVelocity
        {
            get => _startJumpVelocity;
            protected set => _startJumpVelocity = value;
        }

        #endregion

        #region - CROUCH -
        private float CrouchRaycastDistance => _colliderSize.y + Constants.PIXEL_SIZE;
        private Vector2 CrouchRaycastFrontPosition
        {
            get
            {
                var pos = BodyCollider.bounds.center;
                pos.x += Constants.PIXEL_SIZE * 10f * (int)FacingDirection;
                pos.y = BodyCollider.bounds.min.y;
                return pos;
            }
        }
        private Vector2 CrouchRaycastBackPosition
        {
            get
            {
                var pos = BodyCollider.bounds.center;
                pos.x -= Constants.PIXEL_SIZE * 10f * (int)FacingDirection;
                pos.y = BodyCollider.bounds.min.y;
                return pos;
            }
        }

        public bool IsCrouching
        {
            get => _isCrouching;
            protected set => _isCrouching = value;
        }
        public bool ShouldCrounch
        {
            get => _shouldCrouch;
            protected set => _shouldCrouch = value;
        }


        public bool CanExitCrouching
        {
            get
            {
                if (IsCrouching == false) return true;

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

        public bool IsCrawling
        {
            get => _isCrawling;
            protected set => _isCrawling = value;
        }

        public bool CanEnterCrawling
        {
            get
            {
                if (IsCrawling == true) return true;

                RaycastHit2D leftHit = gameObject.Raycast(CrawlEnterRaycastPosition, Vector2.left, CrawlEnterRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
                RaycastHit2D rightHit = gameObject.Raycast(CrawlEnterRaycastPosition, Vector2.right, CrawlEnterRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);

                float distance = 0.0f;
                if (leftHit.collider != null) distance += leftHit.distance;
                else distance += 1.0f;

                if (rightHit.collider != null) distance += rightHit.distance;
                else distance += 1.0f;

                return distance > 0.9f;
            }
        }

        //check if there is enough space to get up from crawling
        public bool CanExitCrawling
        {
            get
            {
                if (IsCrawling == false) return true;

                RaycastHit2D hit = gameObject.Raycast(CrawlExitRaycastPosition, Vector2.up, CrawlExitRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
                return (hit.collider == null);
            }
        }

        public bool IsCrawlEntering
        {
            get => _isCrawlEntering;
            protected set => _isCrawlEntering = value;
        }
        public bool IsCrawlExiting
        {
            get => _isCrawlExiting;
            protected set => _isCrawlExiting = value;
        }
        #endregion

        #region - DODGE -

        public bool IsDodgeEnabled
        {
            get => _isDdodgeEnabled;
            set => _isDdodgeEnabled = value;
        }

        public bool IsDodging
        {
            get => _isDodging;
            set
            {
                if (_isDodging == value) return;
                _isDodging = value;

                // Ender dodging
                if (_isDodging)
                {
                    _dodgeFacing = (int)FacingDirection;
                }
                else
                {
                    // If dodge into a place where there is only space for crawling
                    if (CanExitCrawling) IsCrawling = true;
                }

                IsUpdateCollider = true;
                Owner.View.SetIsDodgingAnimation(_isDodging, DodgeDirection);
            }
        }

        public int DodgeDirection
        {
            get => _dodgeDirection;
            set => _dodgeDirection = value;
        }

        #endregion

        #region - CLIMB LADDER - 
        //raycast parameters for climbing to the ledge the ladder connects to
        private Vector2 LadderExitRaycastPosition => (Vector2)transform.position + new Vector2(15 * (int)FacingDirection, 40) * Constants.PIXEL_SIZE; 
        private float LadderExitRaycastDistance => 40 * Constants.PIXEL_SIZE;

        public bool IsClimbEnabled
        {
            get => _climbEnabled;
            set => _climbEnabled = value;
        }

        public float ClimbSpeed
        {
            get => _climbSpeed;
            set => _climbSpeed = value;
        }
        public float ClimbSpeedFast
        {
            get => _climbSpeedFast;
            set => _climbSpeedFast = value;
        }

        public Ladder Ladder
        {
            get => _ladder;
            protected set => _ladder = value;
        }

        public bool IsEnteringLadder
        {
            get => _isEnteringLadder;
            protected set => _isEnteringLadder = value;
        }
        public bool IsExitingLadder
        {
            get => _isExitingLadder;
            protected set => _isExitingLadder = value;
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

        //is the characterView climbing ladder
        public bool IsClimbingLadder
        {
            get => _isClimbingLadder;
            set
            {
                if (_isClimbingLadder == value) return;
                _isClimbingLadder = value;

                //set ladder climb z position
                Vector3 pos = transform.position;
                if (_isClimbingLadder)
                {
                    _ladderExitPositionZ = pos.z;
                    pos.z = Ladder.ClimbPosition.z;
                }
                else pos.z = _ladderExitPositionZ;
                transform.position = pos;
            }
        }

        #endregion

        #region - LEDGE CLIMB -

        public bool IsLedgeEnabled
        {
            get => _ledgeEnabled;
            protected set => _ledgeEnabled = value;
        }

        public bool IsClimbingLedge
        {
            get => _isClimbingLedge;
            protected set => _isClimbingLedge = value;
        }

        public bool IsLedgeClimbLocked
        {
            get => _isLedgeClimbLocked;
            protected set => _isLedgeClimbLocked = value;
        }

        public Vector2 LedgePosition
        {
            get => _ledgePosition;
            protected set => _ledgePosition = value;
        }

        public float LedgeHeight
        {
            get => _ledgeHeight;
            protected set => _ledgeHeight = value;
        }

        //raycast parameters for climbing up ledge
        public Vector2 LedgeClimbRaycastPosition => (Vector2)transform.position + new Vector2(Constants.PIXEL_SIZE * (int)FacingDirection * 10f, LedgeClimbRaycastHeight);
        public float LedgeClimbRaycastHeight => Constants.PIXEL_SIZE * 42f;
        public float LedgeClimbRaycastDistance => Constants.PIXEL_SIZE * 19f;

        #endregion

        #region - GROUND CHECK & GROUND LIFT

        public override Vector2 GroundRaycastFrontPosition
        {
            get
            {
                float xAxis = IsCrawling ? 15 * Constants.PIXEL_SIZE : 6 * Constants.PIXEL_SIZE;
                return GroundRaycastMidPosition + new Vector2(xAxis * (int)FacingDirection, 0.0f);
            }
        }
        public override Vector2 GroundRaycastBackPosition
        {
            get
            {
                float xAxis = IsCrawling ? 15 * Constants.PIXEL_SIZE : 6 * Constants.PIXEL_SIZE;
                return GroundRaycastMidPosition + new Vector2(xAxis * -(int)FacingDirection, 0.0f);
            }
        }

        #endregion

        #region - AIR -
        public override bool IsInAir => !IsGrounded && !IsClimbingLadder && !IsExitingLadder && !IsClimbingLedge;

        #endregion

        #region - COLLIDER - 

        private readonly Vector2 _colliderCrouchSize = new(0.375f, 1.0f);
        private readonly Vector2 _colliderCrouchOffset = new(0.0f, 1.0f);

        private readonly Vector2 _colliderCrawlSize = new(1.0f, 0.5f);
        private readonly Vector2 _colliderCrawlOffset = new(0.1875f, 0.5f);

        #endregion

        #endregion

        #region - UNITY CALLBACKS - 
        protected override void Start()
        {
            base.Start();
            BodyCollider.offset = _colliderOffset;
            BodyCollider.size = _colliderSize;
        }

        protected override void Update()
        {
            this.MoveCheck(_controlMoving.x, _controlRunning);

            this.HandleDash(Time.deltaTime);
            this.Dash(_controlDash);

            this.HandleDodge();
            this.Dodge(_controlDodge, _controlMoving.x);

            this.UpdateMoveBlend();
            this.HandleJump(_controlJumping, _controlMoving.x, Time.deltaTime);

            this.LadderEnterCheck(_controlMoving);
            this.LadderExitCheck(_controlMoving);

            this.HandleLedgeClimb(_controlMoving.x);

            this.Crouch(_controlCrouching);
            this.Crawl(_controlCrawling, _controlMoving.x);

            this.GetDownPlatform(_controlMoving.y);

            this.UpdateFacing(_controlMoving.x, TargetPosition);
            this.UpdateAnimator();

            base.Update();
        }

        protected override void FixedUpdate()
        {
            _currentVelocity = Rigidbody.linearVelocity - Vector2.up * GroundLiftSpeed;
            _currentGravityScale = _gravityScale;

            base.FixedUpdate();

            this.Move(_controlMoving.x);
            this.SlideDownOnSlope(Time.fixedDeltaTime);

            this.Jump(_controlJumping);
            this.LadderClimb(_controlMoving.y);
            this.LedgeClimb();
            this.DodgeUpdate();

            this.ApplyRootMotion();

            Rigidbody.gravityScale = _currentGravityScale;
            Rigidbody.linearVelocity = _currentVelocity + Vector2.up * GroundLiftSpeed;
        }

        protected virtual void LateUpdate()
        {
            Owner.View.LookAtTarget(IsLooking, TargetPosition);
            Owner.View.PointAtTarget(TargetPosition, IsCrawling, IsDodging);
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
                    IsClimbingLadder = false;
                    // _isClimbingLadder = false;
                }
            }
        }

        #endregion

        #region - CONTROLL METHODS -

        public void ControlMoving(Vector2 moveDiretion, bool isRunning = false)
        {
            _controlMoving = moveDiretion;
            _controlRunning = isRunning;
            if (isRunning)
            {
                // Run cancels crouch and crawl
                _controlCrouching = false;
                _controlCrawling = false;
            }

            if (IsDead) _controlMoving = Vector2.zero;
        }

        public void ControlDashing(bool state)
        {
            _controlDash = state;

            // Do not allow dash when climbing ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                _controlDash = false;
            }

            // Do not allow dash when climbing ledge
            if (IsClimbingLedge)
            {
                _controlDash = false;
            }

            // Do not allow dash when crouching and crawling
            if (IsCrouching || IsCrawling)
            {
                _controlDash = false;
            }
        }

        public void ControlDodging(bool state)
        {
            _controlDodge = state;
        }

        public void ControlJumping(bool state)
        {
            _controlJumping = state;
            if (state)
            {
                // Jump cancels crouch and crawl
                _controlCrouching = false;
                _controlCrawling = false;
            }
        }

        public void ControlCrouching(bool state)
        {
            if (IsInAir) return;
            if (state)
            {
                if (_controlCrawling && CanExitCrawling)
                {
                    _controlCrawling = false;
                    _controlCrouching = true;
                }
                else
                {
                    _controlCrouching = !_controlCrouching;
                }

                if (!CanExitCrouching) _controlCrouching = true;
            }

            // Do not allow entering crouch when climbing ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                _controlCrouching = false;
            }
        }

        public void ControlCrawling(bool state)
        {
            if (IsInAir) return;
            if (state)
            {
                _controlCrawling = !_controlCrawling;
                if (CanEnterCrawling == false) _controlCrawling = false;
                if (CanExitCrawling == false) _controlCrawling = true;
                if (_controlCrawling == false) _controlCrouching = false;
            }

            // Do not allow entering crawl when climbing ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                _controlCrawling = false;
            }
        }

        public void ControlLooking(bool isLooking, Vector2 targetPosition)
        {
            IsLooking = isLooking;
            if (isLooking) TargetPosition = targetPosition;
        }

        #endregion

        #region - METHODS -

        // Move

        protected virtual void MoveCheck(float direction, bool isRunning)
        {
            //set isMoving and isRunning
            IsMoving = (Mathf.Abs(direction) > Constants.MOVE_THRESHOLD);
            if (IsMoving == false) return;

            //disallow running backward
            if (Owner.View.IsLookingAtTarget && Mathf.Sign(direction) != (int)FacingDirection) 
                IsRunning = false;
            else IsRunning = isRunning;

            //is the characterView moving backward
            HorizontalMoveDirection = (Mathf.Sign(direction) * (int)FacingDirection) == 1 ? 1 : -1;
        }
        protected virtual void Move(float direction)
        {
            if (IsClimbingLadder) return;
            if (IsClimbingLedge) return;

            bool shouldMove = Mathf.Abs(direction) > Constants.MOVE_THRESHOLD;

            // Speed limit
            // On ground
            if (IsGrounded)
            {
                float speed = _currentVelocity.magnitude;
                if (Mathf.Abs(direction) > Constants.MOVE_THRESHOLD && speed > _currentSpeed) speed = Mathf.MoveTowards(speed, _currentSpeed, _currentDragAcceleration * Time.fixedDeltaTime);
                if (Mathf.Abs(direction) <= Constants.MOVE_THRESHOLD) speed = Mathf.MoveTowards(speed, 0.0f, _currentDragAcceleration * Time.fixedDeltaTime);

                _currentVelocity = _currentVelocity.normalized * speed;
            }

            // In air, set limit to x and y direction separately
            else if (IsInAir)
            {
                float speedX = Mathf.Abs(_currentVelocity.x);
                if (speedX > _currentSpeed) speedX = Mathf.MoveTowards(speedX, _currentSpeed, _currentDragAcceleration * Time.fixedDeltaTime);
                _currentVelocity.x = Mathf.Sign(_currentVelocity.x) * speedX;

                float speedY = Mathf.Abs(_currentVelocity.y);
                speedY = Mathf.Min(speedY, Physic2DUtils.AIR_VELOCITY_Y_LIMIT);

                _currentVelocity.y = Mathf.Sign(_currentVelocity.y) * speedY;
            }

            // Force moving when dashing
            if (IsDashing) shouldMove = true;

            // Cancel shouldMove if the maxSpeed speed limit is reached at left or right direction
            if (shouldMove)
            {
                if (direction > 0.0f && _currentVelocity.x > _currentSpeed) shouldMove = false;
                else if (direction < 0.0f && _currentVelocity.x < -_currentSpeed) shouldMove = false;
            }

            // Apply acceleration
            if (shouldMove)
            {
                if (IsGrounded)
                    _currentVelocity += _currentAcceleration * HorizontalMoveDirection * Time.fixedDeltaTime * SurfaceDirection;
                else
                    _currentVelocity += _currentAcceleration * (int)FacingDirection * HorizontalMoveDirection * Time.fixedDeltaTime * Vector2.right;
            }
        }
        protected virtual void UpdateMoveBlend()
        {
            if (IsMoving)
            {
                _targetMoveBlend = 1.0f;
                if (IsRunning) _targetMoveBlend = 3.0f;
            }
            else _targetMoveBlend = 0.0f;

            _moveBlend = Mathf.Lerp(_moveBlend, _targetMoveBlend, 7.0f * Time.deltaTime);
        }

        // Dash

        protected virtual bool CanDash()
        {
            if (!_isDashEnabled || IsDead || Owner.View.IsPointingAtTarget)
            {
                return false;
            }

            if (IsGrounded == false || IsCrouching == true)
            {
                return false;
            }

            return _dashCooldown.IsComplete;
        }
        public void Dash(bool isDash)
        {
            if (isDash == false) return;
            if (!CanDash()) return;

            _currentVelocity.x = _dashSpeedStart * (int)FacingDirection;
            Rigidbody.linearVelocity = _currentVelocity;

            IsDashing = true;
            _dashCooldown.Reset();
            _dashingTime.Reset();
        }

        // Crouch and Crawl

        public virtual void Crouch(bool state)
        {
            ShouldCrounch = state;
            if (AirTime > 1.0f) ShouldCrounch = false;

            //if there is no enough space to stand, keep crouching
            if (IsCrouching)
            {
                ShouldCrounch = ShouldCrounch || (!CanExitCrouching);
            }

            if (IsCrouching != ShouldCrounch)
            {
                IsCrouching = ShouldCrounch;
                IsUpdateCollider = true;
            }
        }

        protected virtual void Crawl(bool state, float direction)
        {
            _shouldCrawl = state;
            if (AirTime > 1.0f) _shouldCrawl = false;

            //if there is no enough space to get up, keep craling
            if (IsCrawling)
            {
                _shouldCrawl = _shouldCrawl || (!CanExitCrawling);
            }

            if (IsCrawling != _shouldCrawl)
            {
                IsCrawling = _shouldCrawl;
                IsUpdateCollider = true;

                // if (isCrawling) IsDrawingBow = false;
            }

            Owner.View.SetCrawlSpeedMultiplyAnimation(Mathf.Abs(direction) * HorizontalMoveDirection);
        }

        public void OnCrawlEnter() => IsCrawlEntering = true;
        public void OnCrawlEntered() => IsCrawlEntering = false;
        public void OnCrawlExit() => IsCrawlExiting = true;
        public void OnCrawlExited() => IsCrawlExiting = false;

        // Dodge

        protected virtual void Dodge(bool inputDodge, float direction)
        {
            if (!IsDodgeEnabled) return;
            if (IsCrawling || IsClimbingLedge || IsEnteringLadder || IsExitingLadder) return;


            if (IsDodging == false && !_dodgeCooldown.IsComplete)
            {
                _dodgeCooldown.Update(Time.deltaTime);
                return;
            }

            if (AirTime > 0.1f) return;
            if (inputDodge == false) return;
            if (Mathf.Abs(direction) < Constants.MOVE_THRESHOLD) return;

            //set dodge direction
            if (direction * (int)FacingDirection > 0) DodgeDirection = 1;
            else DodgeDirection = -1;

            IsDodging = true;
            _dodgeCooldown.Reset();
        }
        public void DodgeStart() => OnDodgeStart?.Invoke(this);
        public void DodgeEnd()
        {
            IsDodging = false;
            OnDodgeEnd?.Invoke(this);
        }

        // Jump

        protected virtual void Jump(bool isJump)
        {
            if (!IsJumpEnabled) return;

            // Apply start jump velocity
            if (StartJumpVelocity.magnitude > 0.01f)
            {
                Vector2 jumpDirection = StartJumpVelocity.normalized;
                float dot = Vector2.Dot(_currentVelocity, jumpDirection);
                if (dot < 0) _currentVelocity -= dot * jumpDirection;

                _currentVelocity += StartJumpVelocity;
                _currentVelocity.y = Mathf.Min(_currentVelocity.y, StartJumpVelocity.y * 1.25f);

                StartJumpVelocity = Vector2.zero;

                //apply jump force to standing collider
                Vector2 force = Physics2D.gravity * JumpSpeed * _weight;
                this.ApplyForceToStandingColliders(force);

                //event
                OnJump?.Invoke(this);
            }

            //jumping up with continuous jump input
            //set jump gravity so that the longer the jump key is pressed, the higher the characterView can jump
            if (IsInAir)
            {
                if (isJump && _currentVelocity.y > 0)
                {
                    _currentVelocity.y += Physics2D.gravity.y * (_jumpGravityMutiplier - 1.0f) * Time.fixedDeltaTime;
                }
                //jumping up without input
                else if (_currentVelocity.y > 0.01f)
                {
                    _currentVelocity.y += Physics2D.gravity.y * (_fallGravityMutiplier - 1.0f) * Time.fixedDeltaTime;
                }
            }
        }

        // Ladder Climb

        protected virtual void LadderClimb(float direction)
        {
            if (!IsClimbEnabled) return;
            if (IsDead) return;

            if (IsExitingLadder)
            {
                _currentGravityScale = 0.0f;
            }

            if (!IsClimbingLadder) return;

            _currentGravityScale = 0.0f;
            _currentVelocity.x = 0.0f;
            float currentClimbSpeed = IsRunning ? ClimbSpeedFast : ClimbSpeed;

            //handle climb movement
            _currentVelocity.x = 0.0f;
            if (Mathf.Abs(direction) > Constants.MOVE_THRESHOLD)
            {
                _currentVelocity.y = currentClimbSpeed * Mathf.Sign(direction);
                _climbingSpeedMultiply = (_currentVelocity.y / ClimbSpeed);
            }
            else
            {
                _currentVelocity.y = 0.0f;
                _climbingSpeedMultiply = 0.0f;
            }

            HasReachedLadderTop = (NeckPosition.y >= Ladder.TopPosition.y);
            HasReachedLadderBottom = (Mathf.Abs(BottomPosition.y - Ladder.BottomPosition.y) < Constants.PIXEL_SIZE * 2.0f);

            // Reach top of the ladder
            if (HasReachedLadderTop)
            {
                if (_currentVelocity.y > 0.0f)
                {
                    _currentVelocity.y = 0.0f;
                    _climbingSpeedMultiply = 0.0f;
                }
            }

            // Reach top bottom of the ladder
            // Using 2 pixel size as tolerance
            else if (HasReachedLadderBottom)
            {
                if (_currentVelocity.y < 0.0f)
                {
                    _currentVelocity.y = 0.0f;
                    _climbingSpeedMultiply = 0.0f;
                }
            }

            // Move characterView to the cimbing position defined by the ladder
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, Ladder.ClimbPosition.x, Time.fixedDeltaTime * _climbPositionLerpSpeed);
            transform.position = pos;

            // As entering ladder animation's root motion somehow don't work
            // Manual add down input
            if (IsEnteringLadder)
            {
                _currentVelocity.y = currentClimbSpeed * -1.0f;
                _climbingSpeedMultiply = (_currentVelocity.y / ClimbSpeed);
            }
        }
        protected virtual void LadderExitCheck(Vector2 direction)
        {
            if (IsDead) return;
            if (IsClimbingLadder == false) return;
            if (IsEnteringLadder) return;

            // Reach ladder top, has up direction or has forward direction
            if ((HasReachedLadderTop && direction.y > Constants.MOVE_THRESHOLD) ||
                (Mathf.Abs(direction.x) > Constants.MOVE_THRESHOLD && (direction.x * (int)FacingDirection) > 0))
            {
                // Check for surface the characterView can climb onto
                RaycastHit2D hit = gameObject.Raycast(LadderExitRaycastPosition, Vector2.down, LadderExitRaycastDistance, _groundCheckLayerMask, skipColliders: _ignoreColliders);
                bool hasHit = (hit.collider != null);

                // Have climbable surface, to exit ladder
                if (hasHit)
                {
                    IsExitingLadder = true;
                    IsClimbingLadder = false;
                    _ladderExitHeight = hit.point.y - transform.position.y;
                }
            }

            // Reach ladder bottom, have down direction
            if (HasReachedLadderBottom && direction.y < -Constants.MOVE_THRESHOLD)
            {
                // is grounded, exit ladder
                if (IsGrounded)
                {
                    IsClimbingLadder = false;
                }

                // Special case, from ladder to air
                // Reach ladder bottom, but not grounded, down direction persist more than 0.5s
                else
                {
                    _ladderToAirTime.Update(Time.deltaTime);
                    if (_ladderToAirTime.IsComplete)
                    {
                        Ladder = null;
                        IsClimbingLadder = false;

                        Owner.View.TriggerGetDownPlatformAnimation();
                        _ladderToAirTime.Reset();
                    }
                }
            }

        }
        protected virtual void LadderEnterCheck(Vector2 direction)
        {
            if (IsClimbingLadder) return;
            if (Ladder == null) return;
            if (Mathf.Abs(direction.x) > Constants.MOVE_THRESHOLD) return;
            // if (isDrawingBow) return;
            if (IsCrawling || IsCrawlEntering || IsCrawlExiting) return;

            //climb up to ladder
            if (direction.y > Constants.MOVE_THRESHOLD && NeckPosition.y < Ladder.TopPosition.y && CenterPosition.y > Ladder.BottomPosition.y) 
                IsClimbingLadder = true;

            //climb down to ladder, may needs ladder enter animation
            if (direction.y < -Constants.MOVE_THRESHOLD && BottomPosition.y > Ladder.BottomPosition.y)
            {
                IsClimbingLadder = true;
                _ladderEnterHeight = LadderExitRaycastPosition.y - Ladder.TopPosition.y;
                if (_ladderEnterHeight > -Mathf.Epsilon && _ladderEnterHeight < Constants.PIXEL_SIZE * 41f)
                {
                    IsEnteringLadder = true;
                }
            }
        }

        public void OnLadderEntered() => IsEnteringLadder = false;
        public void OnLadderExited() => IsExitingLadder = false;

        // Ledge Climb

        protected virtual void LedgeClimb()
        {
            if (!IsClimbingLedge) return;

            if (FindLedgeToClimb(out RaycastHit2D hit))
            {
                // Update ledge position and character position in case the ledge collider is moving
                transform.position += (Vector3)(hit.point - LedgePosition);
                LedgePosition = hit.point;

                return;
            }

            // If the ledge collider is no more, cancel ledge climbing
            IsClimbingLedge = false;
            return;
        }
        public void OnLedgeClimbLocked() => IsLedgeClimbLocked = true;
        public void OnLedgeClimbFinised()
        {
            IsClimbingLedge = false;
            IsLedgeClimbLocked = false;
        }

        protected virtual bool FindLedgeToClimb(out RaycastHit2D hit)
        {
            Vector2 rayPosition = LedgePosition + new Vector2(0.0f, Constants.PIXEL_SIZE);
            float rayDistance = Constants.PIXEL_SIZE * 4.0f;

            hit = gameObject.Raycast(rayPosition, Vector2.down, rayDistance, _groundCheckLayerMask, isIgnorePlatform: true, skipColliders: _ignoreColliders);
            return hit.collider != null;
        }

        // Ground & Platform

        /// <summary>
        ///     Keep character above ground
        /// </summary>
        protected override bool CheckIsGroundLift()
        {
            if (!IsGrounded) return false;
            if (IsClimbingLadder) return false;
            if (IsExitingLadder) return false;
            if (IsClimbingLedge) return false;

            return true;
        }

        /// <summary>
        ///     When player is standing on a one-way platform and has down direction input
        ///     get down this platform
        /// </summary>
        /// <param name="direction"></param>
        protected virtual void GetDownPlatform(float direction)
        {
            if (!IsStandingOnPlatform) return;
            if (direction >= -Constants.MOVE_THRESHOLD) return;

            _getDownPlatformCooldown.Update(Time.deltaTime);

            if (_getDownPlatformCooldown.IsComplete)
            {
                this.IgnoreStandingPlatforms();
                Owner.View.TriggerGetDownPlatformAnimation();

                _getDownPlatformCooldown.Reset();
            }
        }

        #endregion

        #region - UPDATE AND HANDLE -
        protected override void UpdateTime(float deltaTime)
        {
            base.UpdateTime(deltaTime);

            //idle timer
            if (IsIdle) IdleTime += deltaTime;
            else IdleTime = 0.0f;
        }
        protected override void HandleSpeedAndAcceleration()
        {
            base.HandleSpeedAndAcceleration();
            // Base acceleration and max speed
            if (IsGrounded)
            {
                _currentAcceleration = _walkAcceleration;
                _currentSpeed = _walkMaxSpeed;
                _currentDragAcceleration = _groundDrag;

                // Apply running speed if running
                if (IsRunning)
                {
                    _currentAcceleration = _runAcceleration;
                    _currentSpeed = _runMaxSpeed;
                }

                // Apply crouching or crawling modifiers
                else if (IsCrouching)
                {
                    _currentAcceleration = _crouchAcceleration;
                    _currentSpeed = _crouchMaxSpeed;
                }

                else if (IsCrawling || IsCrawlEntering || IsCrawlExiting)
                {
                    _currentAcceleration = _crawlAcceleration;
                    _currentSpeed = _crawlMaxSpeed;
                }

                if (IsDashing)
                {
                    _currentAcceleration = _dashAcceleration;
                    _currentSpeed = _dashMaxSpeed;
                }

                // Apply surface angle speed modifier
                float targetSurfaceSpeedMultiply = Mathf.Sin(Mathf.Min(SurfaceAngleForward, 90.0f) * Mathf.Deg2Rad);
                _surfaceSpeedMultiply = targetSurfaceSpeedMultiply < 1.0f ? Mathf.Lerp(_surfaceSpeedMultiply, targetSurfaceSpeedMultiply, 1.0f * Time.fixedDeltaTime) : 1.0f;
                _currentSpeed *= _surfaceSpeedMultiply;
            }
            else
            {
                // Airborne acceleration and speed
                _currentAcceleration = _airAcceleration;
                _currentSpeed = _airMaxSpeed;
                _currentDragAcceleration = _airDrag * Mathf.Abs(_currentVelocity.x);

                // Apply crouching or crawling modifiers in air
                if (IsCrouching)
                {
                    _currentAcceleration = _crouchAcceleration;
                    _currentSpeed = _crouchMaxSpeed;
                    _currentDragAcceleration = _groundDrag;
                }
                else if (IsCrawling || IsCrawlEntering || IsCrawlExiting)
                {
                    _currentAcceleration = _crawlAcceleration;
                    _currentSpeed = _crawlMaxSpeed;
                    _currentDragAcceleration = _groundDrag;
                }
            }
        }
        protected override void UpdateCollider()
        {
            base.UpdateCollider();
            if (!IsUpdateCollider) return;

            Vector2 size;
            Vector2 offset;
            CapsuleDirection2D direction = CapsuleDirection2D.Vertical;

            if (IsCrawling || IsDodging)
            {
                size = _colliderCrawlSize;
                offset = _colliderCrawlOffset;

                direction = CapsuleDirection2D.Horizontal;
            }
            else if (IsCrouching)
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
            if (IsCrawling) offset.x *= (int)FacingDirection;

            // Apply collider settings
            BodyCollider.size = size;
            BodyCollider.offset = offset;
            BodyCollider.direction = direction;

            IsUpdateCollider = false;
        }
        protected override void UpdateFacing(float moveDirection, Vector2 targetPosition)
        {
            // Dodge
            if (IsDodging)
            {
                FacingDirection = (FacingType)_dodgeFacing;
                return;
            }

            // Ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                if (Ladder) FacingDirection = (FacingType)(-(int)Ladder.Direction);
                return;
            }

            base.UpdateFacing(moveDirection, targetPosition);
        }
        protected override void HandleSlideDown()
        {
            base.HandleSlideDown();
            if (IsClimbingLedge) IsSliding = false;
        }

        protected virtual void HandleJump(bool isJump, float horizontalDirection, float deltaTime)
        {
            if (IsDead) return;
            if (!IsJumpEnabled) return;

            // Disable jump while crawling or dodging
            if (IsCrawling || IsCrawlEntering || IsCrawlExiting || IsDodging)
            {
                _jumpCooldown.Reset();
                return;
            }

            // Jump cooldown, not in air and cooldown is not complete
            if (!IsInAir && !_jumpCooldown.IsComplete)
                _jumpCooldown.Update(deltaTime);

            if (!isJump || !_jumpCooldown.IsComplete) return;
            
            this.HandleStartJumpVelocity(horizontalDirection);
        }
        protected virtual void HandleStartJumpVelocity(float horizontalDirection)
        {
            // Start jump
            Vector2 jumpDirection = Vector2.up;

            // Jump from ground
            // Also able to jump within air time tolerance
            if (IsGrounded || (AirTime >= 0f && AirTime <= JumpTolerance))
            {
                IsGrounded = false;
                IsClimbingLadder = false;
            }

            // Jump from ladder
            if (IsClimbingLadder)
            {
                IsGrounded = false;
                IsEnteringLadder = false;
                IsExitingLadder = false;
                IsClimbingLadder = false;

                //mix ladder direction or move direction to jump direction
                if (Mathf.Abs(horizontalDirection) < Constants.MOVE_THRESHOLD)
                {
                    jumpDirection = Vector2.up + new Vector2((int)Ladder.Direction, 0.0f) * 0.25f;
                }
                else
                {
                    jumpDirection = Vector2.up + new Vector2(Mathf.Sign(horizontalDirection), 0.0f) * 0.5f;
                }
            }

            // Jump while entering or exiting climbing 
            if (IsEnteringLadder || IsExitingLadder)
            {
                IsGrounded = false;
                IsEnteringLadder = false;
                IsExitingLadder = false;
                IsClimbingLadder = false;

                jumpDirection = new Vector2(-(int)FacingDirection, 0.0f) * 0.5f;
            }

            // Jump while climbing ledge
            if (IsClimbingLedge || IsLedgeClimbLocked)
            {
                IsGrounded = false;
                IsClimbingLedge = false;
                IsLedgeClimbLocked = false;

                jumpDirection = Vector2.up + new Vector2(-(int)FacingDirection, 0.0f) * 0.5f;
            }

            StartJumpVelocity = jumpDirection.normalized * JumpSpeed;
            _jumpCooldown.Reset();
        }

        protected virtual void HandleDash(float deltaTime)
        {
            _dashCooldown.Update(deltaTime);
            _dashingTime.Update(deltaTime);

            if (_dashingTime.IsComplete)
            {
                IsDashing = false;
            }
        }

        protected virtual void DodgeUpdate()
        {
            //snap characterView to ground better when dodging
            if (IsDodging && AirTime > 0.01f)
            {
                _currentVelocity.y -= 25.0f * Time.fixedDeltaTime;
            }
        }
        protected virtual void HandleDodge()
        {
            // If dodging but in the air, set to no dodge
            if (IsDodging)
            {
                if (AirTime > 0.2f) IsDodging = false;
            }
        }

        protected virtual void HandleLedgeClimb(float moveDirection)
        {
            if (IsDead || !IsLedgeEnabled || IsClimbingLadder || IsExitingLadder || IsCrawling || IsDodging)
            {
                IsClimbingLedge = false;
                return;
            }

            //ledge climb exit check 
            if (IsClimbingLedge)
            {
                if (transform.position.y > LedgePosition.y)
                {
                    IsClimbingLedge = false;
                    return;
                }

                // Not move and not locked
                if (Mathf.Abs(moveDirection) < Constants.MOVE_THRESHOLD && !IsLedgeClimbLocked)
                {
                    IsClimbingLedge = false;
                    return;
                }

                // Move in the opposite direction
                if (Mathf.Abs(moveDirection) >= Constants.MOVE_THRESHOLD && Mathf.Sign(moveDirection) != (int)FacingDirection)
                {
                    IsClimbingLedge = false;
                    return;
                }

                return;
            }

            //ledge climb enter check 

            // Not moving or moving in the opposite direction
            if (Mathf.Abs(moveDirection) < Constants.MOVE_THRESHOLD || Mathf.Sign(moveDirection) != (int)FacingDirection) 
                return;

            //check for climbable ledge
            RaycastHit2D hit = gameObject.Raycast(LedgeClimbRaycastPosition, Vector2.down, LedgeClimbRaycastDistance, _groundCheckLayerMask, true, _ignoreColliders);
            LedgePosition = hit.point;

            //check up direction for enough space to climb
            bool upAvailable = true;
            if (hit.collider != null)
            {
                Vector2 upCheckPosition = LedgePosition - new Vector2(0.0f, Constants.PIXEL_SIZE);
                RaycastHit2D hitUp = gameObject.Raycast(upCheckPosition, Vector2.up, Constants.PIXEL_SIZE * 32f, _groundCheckLayerMask, true, _ignoreColliders);
                upAvailable = (hitUp.collider == null);
            }

            bool ledgeAvailable = (hit.collider != null) && upAvailable;

            //start climbing ledge
            // Ledge available and not climbing ledge
            if (ledgeAvailable && !IsClimbingLedge)
            {
                IsClimbingLedge = true;
                LedgeHeight = LedgeClimbRaycastHeight - hit.distance;

                Rigidbody.linearVelocity = Vector2.zero;
            }

            IsLedgeClimbLocked = false;
        }

        #endregion

        #region - ROOT MOTION AND ANIMATOR - 

        private void ApplyRootMotion()
        {
            //root motion from dodging
            if (IsDodging)
            {
                if (Mathf.Abs(Owner.View.RootMotionVelocity.magnitude) > Constants.MOVE_THRESHOLD)
                {
                    _currentVelocity.x = Owner.View.RootMotionVelocity.x * _dodgeSpeedMultiply;
                    if (_currentVelocity.y > 0) _currentVelocity.y = 0.0f;
                }
            }

            //root motion form ladder entering & exiting, ledge cimbing
            if (IsExitingLadder || IsExitingLadder || IsClimbingLedge)
            {
                if (Mathf.Abs(Owner.View.RootMotionVelocity.magnitude) > Constants.MOVE_THRESHOLD) 
                    _currentVelocity = Owner.View.RootMotionVelocity;
            }
        }

        private void UpdateAnimator()
        {
            if (IsDead) return;

            Owner.View.SetVelocityAnimation(_currentVelocity);
            Owner.View.SetIsMovingAnimation(IsMoving);
            Owner.View.SetIsRunningAnimation(IsRunning);
            Owner.View.SetIsDashingAnimation(IsDashing);
            Owner.View.SetMoveBlendAnimation(_moveBlend);
            Owner.View.SetMoveDirectionAnimation(HorizontalMoveDirection);

            Owner.View.SetIsGroundedAnimation(IsGrounded);
            Owner.View.SetIsCrouchingAnimation(IsCrouching);
            Owner.View.SetIsCrawlingAnimation(IsCrawling);
            Owner.View.SetIsClimbingLadderAnimation(IsClimbingLadder);
            Owner.View.SetClimbingSpeedMultiplyAnimation(_climbingSpeedMultiply);

            Owner.View.SetIsClimbingLedgeAnimation(IsClimbingLedge);
            if (IsClimbingLedge) Owner.View.SetLedgeHeightAnimation(LedgeHeight);

            Owner.View.SetIsEnteringLadderAnimation(IsEnteringLadder);
            if (IsEnteringLadder) Owner.View.SetLadderEnterHeightAnimation(_ladderEnterHeight);

            Owner.View.SetIsExitingLadderAnimation(IsExitingLadder);
            if (IsExitingLadder) Owner.View.SetLadderExitHeightAnimation(_ladderExitHeight);

            Owner.View.Facing = FacingDirection;
        }

        #endregion

        #region - EVENT HANDLING -

        //footstep event has a string parameter telling which animation this event is from.
        public void OnFootstep(AnimationEvent evt)
        {
            if (AirTime > 0.1f) return;

            if (evt.animatorClipInfo.weight < 0.49f) return;

            if (IsClimbingLedge)
            {
                if (evt.stringParameter == "Ledge Climb") OnFootstepEvent?.Invoke(this);
                return;
            }

            if (IsCrawling)
            {
                if (evt.stringParameter == "Crawl") OnFootstepEvent?.Invoke(this);
                return;
            }

            if (IsCrouching)
            {
                if (evt.stringParameter == "Crouch") OnFootstepEvent?.Invoke(this);
                return;
            }

            if (evt.stringParameter == "Walk" && _moveBlend > 0.1f && _moveBlend < 1.1f)
            {
                OnFootstepEvent?.Invoke(this);
                return;
            }
            if (evt.stringParameter == "Run" && _moveBlend > 1.1f)
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
                Gizmos.DrawWireSphere(LedgePosition, 0.1f);
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
                Gizmos.DrawLine(transform.position - Vector3.forward, transform.position - Vector3.forward + new Vector3(_currentVelocity.x, _currentVelocity.y, 0.0f));
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
