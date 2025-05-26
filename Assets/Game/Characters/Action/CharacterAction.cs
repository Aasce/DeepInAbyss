using Asce.Managers;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities
{

    public class CharacterAction : CreatureAction, IHasOwner<Character>, IMovable
    {
        #region - FIELDS -
        [Header("Look")]
        [SerializeField] protected bool _isLooking = false;                              // look input
        [SerializeField] protected Vector2 _targetPosition = Vector2.zero;                  // the look at and point at target


        [Header("Movement")]
        [SerializeField] protected bool _isMoveEnable = true;
        protected bool _isMoving;                                                     // is the character moving
        protected bool _isRunning;                                                    // is the character running
        protected float _idleTime;

        protected int _horizontalMoveDirection = 1;                                             // move direction, 1: forward, -1:backward
        protected float _moveBlend;                                                   // current move blend, for blending idle, walk, run animation, lerps to target move blend on frame update


        [Header("Speed")]
        [SerializeField] protected float _baseSpeed = 5.0f;                            // max speed of the character

        [Space]
        [SerializeField] protected float _walkSpeedScale = 1f;                        // maxSpeed walk speed
        [SerializeField] protected float _walkAcceleration = 10.0f;                   // walking acceleration

        [Space]
        [SerializeField] protected float _runSpeedScale = 1.5f;                         // maxSpeed run speed
        [SerializeField] protected float _runAcceleration = 15.0f;                    // running acceleration

        [Space]
        [SerializeField] protected float _crouchSpeedScale = 0.5f;                      // maxSpeed move speed while crouching
        [SerializeField] protected float _crouchAcceleration = 8.0f;                  // crouching acceleration

        [Space]
        [SerializeField] protected float _crawlSpeedScale = 0.4f;                       // maxSpeed move speed while crawling
        [SerializeField] protected float _crawlAcceleration = 8.0f;                   // crawling acceleration

        [Space]
        [SerializeField] protected float _dashStartSpeedScale = 4f;
        [SerializeField] protected float _dashSpeedScale = 2f;
        [SerializeField] protected float _dashAcceleration = 20.0f;

        [Header("Dash")]
        private bool _isDashing;
        [SerializeField] protected bool _isDashEnabled = true;
        [SerializeField] protected Cooldown _dashingTime = new(1.0f);
        [SerializeField] protected Cooldown _dashCooldown = new(1.0f);

        [Header("Crounch and Crawl")]
        protected bool _isCrouching;

        protected bool _isCrawling;
        protected bool _isCrawlEntering;
        protected bool _isCrawlExiting;

        [Header("Jump")]
        [SerializeField] protected bool _jumpEnabled = true;
        [SerializeField] protected float _jumpForce = 5.0f;                           // speed applied to character when jump
        
        [SerializeField] protected float _jumpTolerance = 0.15f;                      // when the character's air time is less than this value, it is still able to jump
        [SerializeField] protected float _jumpGravityMutiplier = 0.6f;                // gravity multiplier when character is jumping, should be within [0.0,1.0], set it to lower value so that the longer you press the jump button, the higher the character can jump    
        [SerializeField] protected float _fallGravityMutiplier = 1.3f;                // gravity multiplier when character is falling, should be equal or greater than 1.0
        [SerializeField] protected Cooldown _jumpCooldown = new (0.2f);

        protected Vector2 _startJumpVelocity;

        [Header("Climb")]
        private readonly float _climbPositionLerpSpeed = 7.5f;                  // lerp speed when moving the character to the climb position
        [SerializeField] protected bool _climbEnabled = true;
        [SerializeField] protected float _climbSpeed = 1.0f; // climb speed
        [SerializeField] protected float _climbSpeedFast = 1.5f; // climb speed when runing
        protected float _climbingSpeedMultiply;

        protected bool _isEnteringLadder;
        protected bool _isClimbingLadder;
        protected bool _isExitingLadder;

        protected float _ladderEnterHeight;
        protected float _ladderExitHeight; // y distance from the button of the character to the surface when exiting from ladder

        protected Cooldown _ladderToAirTime = new(0.5f);
        protected float _ladderExitPositionZ; // origin position z of the character, used for resetting the position z after exiting the ladder

        [Header("Dodge")]
        [SerializeField] protected bool _isDdodgeEnabled = true;
        [SerializeField] protected float _dodgeSpeedMultiply = 1.25f;
        [SerializeField] protected Cooldown _dodgeCooldown = new(0.1f);

        protected bool _isDodging;
        protected int _dodgeDirection; //dodge direction   1: front  -1:back
        protected int _dodgeFacing;

        [Header("Ledge")]
        [SerializeField] private bool _ledgeEnabled = true;
        protected bool _isClimbingLedge; // Is the character climbing ledge
        protected bool _isLedgeClimbLocked;
        protected float _ledgeHeight;
        protected Vector2 _ledgePosition;

        #region - CONTROL FIELDS- 
        [SerializeField] private Vector2 _controlMove = Vector2.zero;
        [SerializeField] private bool _controlRun = false;
        [SerializeField] private bool _controlDash = false;
        [SerializeField] private bool _controlDodge = false;
        [SerializeField] private bool _controlCrouch = false;
        [SerializeField] private bool _controlCrawl = false;
        [SerializeField] private bool _controlJump = false;
        // [SerializeField] private bool inputAttack = false;                 
        // [SerializeField] private bool inputMelee = false;
        #endregion

        #endregion

        #region - EVENTS -
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

        #region - MOVEMENT -
        public bool IsMoveEnable
        {
            get => _isMoveEnable;
            set => _isMoveEnable = value;
        }
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

        public float MoveBlend
        {
            get => _moveBlend;
            protected set => _moveBlend = value;
        }

        public bool IsIdle => !Owner.Status.IsDead && !IsMoving && !Owner.PhysicController.IsInAir && !IsClimbingLadder && !IsExitingLadder && !IsClimbingLedge;

        public float IdleTime
        {
            get => _idleTime;
            protected set => _idleTime = value;
        }
        #endregion

        #region - SPEED -
        public float BaseSpeed
        {
            get => _baseSpeed;
            protected set => _baseSpeed = value;
        }

        public float WalkMaxSpeed => BaseSpeed * _walkSpeedScale;
        public float WalkAcceleration => _walkAcceleration;

        public float RunMaxSpeed => BaseSpeed * _runSpeedScale;
        public float RunAcceleration => _runAcceleration;

        public float CrouchMaxSpeed => BaseSpeed * _crouchSpeedScale;
        public float CrouchAcceleration => _crouchAcceleration;

        public float CrawlMaxSpeed => BaseSpeed * _crawlSpeedScale;
        public float CrawlAcceleration => _crawlAcceleration;

        public float DashStartSpeed => BaseSpeed * _dashStartSpeedScale;
        public float DashMaxSpeed => BaseSpeed * _dashSpeedScale;
        public float DashAcceleration => _dashAcceleration;
        #endregion

        #region - DASH -
        public bool IsDashEnabled
        {
            get => _isDashEnabled;
            set => _isDashEnabled = value;
        }

        public bool IsDashing
        {
            get => _isDashing;
            protected set => _isDashing = value;
        }
        #endregion

        #region - CROUCH AND CRAWL -
        public bool IsCrouching
        {
            get => _isCrouching;
            set => _isCrouching = value;
        }

        public bool IsCrawling
        {
            get => _isCrawling;
            set => _isCrawling = value;
        }
        public bool IsCrawlEntering
        {
            get => _isCrawlEntering;
            set => _isCrawlEntering = value;
        }
        public bool IsCrawlExiting
        {
            get => _isCrawlExiting;
            set => _isCrawlExiting = value;
        }

        #endregion

        #region - JUMP -
        public bool IsJumpEnabled
        {
            get => _jumpEnabled;
            set => _jumpEnabled = value;
        }
        public float JumpForce
        {
            get => _jumpForce;
            set => _jumpForce = value;
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

        #region - CLIMB -

        public bool IsClimbEnabled
        {
            get => _climbEnabled;
            set => _climbEnabled = value;
        }

        //is the character climbing ladder
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
                    pos.z = Owner.PhysicController.Ladder.ClimbPosition.z;
                }
                else pos.z = _ladderExitPositionZ;
                transform.position = pos;
            }
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

        public float ClimbingSpeedMultiply
        {
            get => _climbingSpeedMultiply;
            protected set => _climbingSpeedMultiply = value;
        }

        public bool IsEnteringLadder
        {
            get => _isEnteringLadder;
            set => _isEnteringLadder = value;
        }
        public bool IsExitingLadder
        {
            get => _isExitingLadder;
            set => _isExitingLadder = value;
        }

        public float LadderEnterHeight
        {
            get => _ladderEnterHeight;
            set => _ladderEnterHeight = value;
        }

        public float LadderExitHeight
        {
            get => _ladderExitHeight;
            set => _ladderExitHeight = value;
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
                    _dodgeFacing = Owner.Status.FacingDirectionValue;
                }
                else
                {
                    // If dodge into a place where there is only space for crawling
                    if (Owner.PhysicController.CanExitCrawling()) IsCrawling = true;
                }

                Owner.PhysicController.TriggerUpdateCollider();
                Owner.View.SetIsDodgingAnimation(_isDodging, DodgeDirection);
            }
        }

        public int DodgeDirection
        {
            get => _dodgeDirection;
            set => _dodgeDirection = value;
        }

        public int DodgeFacing
        {
            get => _dodgeFacing;
            set => _dodgeFacing = value;
        }

        public float DodgeSpeedMultiply => _dodgeSpeedMultiply;

        #endregion

        #region - LEDGE -

        public bool IsLedgeEnabled
        {
            get => _ledgeEnabled;
            protected set => _ledgeEnabled = value;
        }

        public bool IsClimbingLedge
        {
            get => _isClimbingLedge;
            set => _isClimbingLedge = value;
        }

        public bool IsLedgeClimbLocked
        {
            get => _isLedgeClimbLocked;
            set => _isLedgeClimbLocked = value;
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

        #endregion

        #region - CONTROL PROPERTIES -
        public Vector2 ControlMove
        {
            get => _controlMove;
            set => _controlMove = value;
        }

        public bool ControlRun
        {
            get => _controlRun;
            set => _controlRun = value;
        }
        public bool ControlDash
        {
            get => _controlDash;
            set => _controlDash = value;
        }
        public bool ControlDodge
        {
            get => _controlDodge;
            set => _controlDodge = value;
        }
        public bool ControlCrouch
        {
            get => _controlCrouch;
            set => _controlCrouch = value;
        }
        public bool ControlCrawl
        {
            get => _controlCrawl;
            set => _controlCrawl = value;
        }
        public bool ControlJump
        {
            get => _controlJump;
            set => _controlJump = value;
        }

        // public bool InputAttack
        // {
        //     get => inputAttack;
        //     set => inputAttack = value;
        // }
        #endregion

        #endregion

        #region - METHODS -

        #region - UNITY METHODS -
        protected override void Start()
        {
            base.Start();
            BaseSpeed = Owner.Stats.Speed.Value; // Set base speed from stats
            JumpForce = Owner.Stats.JumpForce.Value; // Set jump force from stats

            Owner.Stats.Speed.OnValueChanged += Speed_OnValueChanged;
            Owner.Stats.JumpForce.OnValueChanged += JumpForce_OnValueChanged;
        }

        protected override void Update()
        {
            base.Update();
            UpdateTime(Time.deltaTime);

            HandleMove(ControlMove.x, ControlRun);
            HandleDash(Time.deltaTime);
            HandleJump(ControlJump, ControlMove.x, Time.deltaTime);
            HandleDodge();
            HandleLedgeClimb(ControlMove.x);

            UpdateMoveBlend();

            Dash(ControlDash, DashStartSpeed);

            Crouch(ControlCrouch);
            Crawl(ControlCrawl, ControlMove.x);

            LadderEnterCheck(ControlMove);
            LadderExitCheck(ControlMove);

            Dodge(ControlDodge, ControlMove.x);
        }

        #endregion

        #region - PHYSIC METHODS -
        public virtual void PhysicUpdate(float deltaTime)
        {
            Move(ControlMove.x);
            Jump(ControlJump, deltaTime);
            LadderClimb(ControlMove.y);
            DodgeUpdate();
            LedgeClimb();
        }

        #endregion

        #region - CONTROLL METHODS -

        public void ControlMoving(Vector2 moveDiretion, bool isRunning = false)
        {
            ControlMove = moveDiretion;
            ControlRun = isRunning;
            if (isRunning)
            {
                // Run cancels crouch and crawl
                ControlCrouch = false;
                ControlCrawl = false;
            }

            if (Owner.Status.IsDead) ControlMove = Vector2.zero;
        }

        public void ControlDashing(bool state)
        {
            _controlDash = state;

            // Do not allow dash when climbing ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                ControlDash = false;
            }

            // Do not allow dash when climbing ledge
            if (IsClimbingLedge)
            {
                ControlDash = false;
            }

            // Do not allow dash when crouching and crawling
            if (IsCrouching || IsCrawling)
            {
                ControlDash = false;
            }
        }

        public void ControlDodging(bool state)
        {
            ControlDodge = state;
        }

        public void ControlJumping(bool state)
        {
            ControlJump = state;
            if (state)
            {
                // Jump cancels crouch and crawl
                ControlCrouch = false;
                ControlCrawl = false;
            }
        }

        public void ControlCrouching(bool state)
        {
            if (Owner.PhysicController.IsInAir) return;
            if (state)
            {
                if (ControlCrawl && Owner.PhysicController.CanExitCrawling())
                {
                    ControlCrawl = false;
                    ControlCrouch = true;
                }
                else
                {
                    ControlCrouch = !ControlCrouch;
                }

                if (!Owner.PhysicController.CanExitCrouching) ControlCrouch = true;
            }

            // Do not allow entering crouch when climbing ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                ControlCrouch = false;
            }
        }

        public void ControlCrawling(bool state)
        {
            if (Owner.PhysicController.IsInAir) return;
            if (state)
            {
                ControlCrawl = !ControlCrawl;
                if (Owner.PhysicController.CanEnterCrawling() == false) ControlCrawl = false;
                if (Owner.PhysicController.CanExitCrawling() == false) ControlCrawl = true;
                if (ControlCrawl == false) ControlCrouch = false;
            }

            // Do not allow entering crawl when climbing ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                ControlCrawl = false;
            }
        }

        public void ControlLooking(bool isLooking, Vector2 targetPosition)
        {
            IsLooking = isLooking;
            if (isLooking) TargetPosition = targetPosition;
        }

        #endregion

        #region - HANDLING METHODS -
        protected virtual void HandleMove(float direction, bool isRunning)
        {
            if (Owner.Status.IsDead) return;
            if (!IsMoveEnable) return;

            // Set isMoving and isRunning
            IsMoving = (Mathf.Abs(direction) > Constants.MOVE_THRESHOLD);
            if (!IsMoving) return;

            // Disallow running backward
            if (Owner.View.IsLookingAtTarget && Mathf.Sign(direction) != Owner.Status.FacingDirectionValue)
                IsRunning = false;
            else IsRunning = isRunning;

            // Is the characterView moving backward
            HorizontalMoveDirection = (Mathf.Sign(direction) * Owner.Status.FacingDirectionValue) == 1 ? 1 : -1;
        }

        protected virtual void HandleDash(float deltaTime)
        {
            _dashCooldown.Update(deltaTime);
            _dashingTime.Update(deltaTime);

            if (_dashingTime.IsComplete) IsDashing = false;
        }

        protected virtual void HandleJump(bool isJump, float horizontalDirection, float deltaTime)
        {
            if (Owner.Status.IsDead) return;
            if (!IsJumpEnabled) return;

            // Disable jump while crawling or dodging
            if (IsCrawling || IsCrawlEntering || IsCrawlExiting || IsDodging)
            {
                _jumpCooldown.Reset();
                return;
            }

            // Jump cooldown, not in air and cooldown is not complete
            if (!Owner.PhysicController.IsInAir && !_jumpCooldown.IsComplete)
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
            if (Owner.PhysicController.IsGrounded || (Owner.PhysicController.AirTime >= 0f && Owner.PhysicController.AirTime <= JumpTolerance))
            {
                Owner.PhysicController.IsGrounded = false;
                IsClimbingLadder = false;
            }

            // Jump from ladder
            if (IsClimbingLadder)
            {
                Owner.PhysicController.IsGrounded = false;
                IsEnteringLadder = false;
                IsExitingLadder = false;
                IsClimbingLadder = false;

                //mix ladder direction or move direction to jump direction
                if (Mathf.Abs(horizontalDirection) < Constants.MOVE_THRESHOLD)
                {
                    jumpDirection = Vector2.up + new Vector2((int)Owner.PhysicController.Ladder.Direction, 0.0f) * 0.25f;
                }
                else
                {
                    jumpDirection = Vector2.up + new Vector2(Mathf.Sign(horizontalDirection), 0.0f) * 0.5f;
                }
            }

            // Jump while entering or exiting climbing 
            if (IsEnteringLadder || IsExitingLadder)
            {
                Owner.PhysicController.IsGrounded = false;
                IsEnteringLadder = false;
                IsExitingLadder = false;
                IsClimbingLadder = false;

                jumpDirection = new Vector2(-Owner.Status.FacingDirectionValue, 0.0f) * 0.5f;
            }

            // Jump while climbing ledge
            if (IsClimbingLedge || IsLedgeClimbLocked)
            {
                Owner.PhysicController.IsGrounded = false;
                IsClimbingLedge = false;
                IsLedgeClimbLocked = false;

                jumpDirection = Vector2.up + new Vector2(-Owner.Status.FacingDirectionValue, 0.0f) * 0.5f;
            }

            StartJumpVelocity = jumpDirection.normalized * JumpForce;
            _jumpCooldown.Reset();
        }

        #endregion

        #region - UPDATE METHODS -
        public virtual void UpdateTime(float deltaTime)
        {
            if (Owner.Status.IsDead) return;
            //idle timer
            if (IsIdle) IdleTime += deltaTime;
            else IdleTime = 0.0f;
        }

        protected override void UpdateFacing()
        {
            if (Owner.Status.IsDead) return;
            base.UpdateFacing();

            // Dodge
            if (IsDodging)
            {
                Owner.Status.FacingDirectionValue = DodgeFacing;
                return;
            }

            // Ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                if (Owner.PhysicController.Ladder != null) Owner.Status.FacingDirectionValue = -(int)Owner.PhysicController.Ladder.Direction;
                return;
            }

            // Look at target
            if (Owner.View.IsLookingAtTarget)
            {
                Owner.Status.FacingDirection = Owner.View.LookAtTargetFacing(TargetPosition);
                return;
            }

            // Move
            if (Mathf.Abs(ControlMove.x) > Constants.MOVE_THRESHOLD)
            {
                Owner.Status.FacingDirectionValue = Mathf.RoundToInt(Mathf.Sign(ControlMove.x));
                return;
            }
        }
        #endregion

        #region - MOVE METHODS -

        protected virtual void Move(float direction)
        {
            if (IsClimbingLadder) return;
            if (IsClimbingLedge) return;

            float currentSpeed = Owner.PhysicController.CurrentSpeed;
            float currentAcceleration = Owner.PhysicController.CurrentAcceleration;
            float currentDragAcceleration = Owner.PhysicController.CurrentDragAcceleration;
            Vector2 currentVelocity = Owner.PhysicController.currentVelocity;

            bool shouldMove = Mathf.Abs(direction) > Constants.MOVE_THRESHOLD;

            // Speed limit
            // On ground
            if (Owner.PhysicController.IsGrounded)
            {
                float speed = currentVelocity.magnitude;
                if (Mathf.Abs(direction) > Constants.MOVE_THRESHOLD && speed > currentSpeed) speed = Mathf.MoveTowards(speed, currentSpeed, currentDragAcceleration * Time.fixedDeltaTime);
                if (Mathf.Abs(direction) <= Constants.MOVE_THRESHOLD) speed = Mathf.MoveTowards(speed, 0.0f, currentDragAcceleration * Time.fixedDeltaTime);

                currentVelocity = currentVelocity.normalized * speed;
            }

            // In air, set limit to x and y direction separately
            else if (Owner.PhysicController.IsInAir)
            {
                float speedX = Mathf.Abs(currentVelocity.x);
                if (speedX > currentSpeed) speedX = Mathf.MoveTowards(speedX, currentSpeed, currentDragAcceleration * Time.fixedDeltaTime);
                currentVelocity.x = Mathf.Sign(currentVelocity.x) * speedX;

                float speedY = Mathf.Abs(currentVelocity.y);
                speedY = Mathf.Min(speedY, Physic2DUtils.AIR_VELOCITY_Y_LIMIT);

                currentVelocity.y = Mathf.Sign(currentVelocity.y) * speedY;
            }

            // Force moving when dashing
            if (IsDashing) shouldMove = true;

            // Cancel shouldMove if the maxSpeed speed limit is reached at left or right direction
            if (shouldMove)
            {
                if (direction > 0.0f && currentVelocity.x > currentSpeed) shouldMove = false;
                else if (direction < 0.0f && currentVelocity.x < -currentSpeed) shouldMove = false;
            }

            // Apply acceleration
            if (shouldMove)
            {
                if (Owner.PhysicController.IsGrounded)
                    currentVelocity += currentAcceleration * HorizontalMoveDirection * Time.fixedDeltaTime * Owner.PhysicController.SurfaceDirection;
                else
                    currentVelocity += currentAcceleration * Owner.Status.FacingDirectionValue * HorizontalMoveDirection * Time.fixedDeltaTime * Vector2.right;
            }

            Owner.PhysicController.currentVelocity = currentVelocity;
        }

        protected virtual void UpdateMoveBlend()
        {
            float targetMoveBlend = 0.0f;
            if (IsMoving)
            {
                if (IsRunning) targetMoveBlend = 3.0f;
                else targetMoveBlend = 1.0f;
            }

            MoveBlend = Mathf.Lerp(MoveBlend, targetMoveBlend, 7.0f * Time.deltaTime);
        }
        #endregion

        #region - DASH METHODS -
        protected virtual bool CanDash()
        {
            if (!_isDashEnabled || Owner.Status.IsDead || Owner.View.IsPointingAtTarget)
            {
                return false;
            }

            if (!Owner.PhysicController.IsGrounded || IsCrouching)
            {
                return false;
            }

            return _dashCooldown.IsComplete;
        }
        public void Dash(bool isDash, float dashStartSpeed)
        {
            if (isDash == false) return;
            if (!CanDash()) return;

            Owner.PhysicController.Accelerate(dashStartSpeed * Owner.Status.FacingDirectionValue);

            IsDashing = true;
            _dashCooldown.Reset();
            _dashingTime.Reset();
        }
        #endregion

        #region - CROUCH AND CRAWL METHODS -

        // Crouch and Crawl

        public virtual void Crouch(bool state)
        {
            bool shouldCrouch = state;
            if (Owner.PhysicController.AirTime > 1.0f) shouldCrouch = false;

            //if there is no enough space to stand, keep crouching
            if (IsCrouching)
            {
                shouldCrouch = shouldCrouch || (!Owner.PhysicController.CanExitCrouching);
            }

            if (IsCrouching != shouldCrouch)
            {
                IsCrouching = shouldCrouch;
                Owner.PhysicController.TriggerUpdateCollider();
            }
        }

        protected virtual void Crawl(bool state, float direction)
        {
            bool shouldCrawl = state;
            if (Owner.PhysicController.AirTime > 1.0f) shouldCrawl = false;

            //if there is no enough space to get up, keep craling
            if (IsCrawling)
            {
                shouldCrawl = shouldCrawl || (!Owner.PhysicController.CanExitCrawling());
            }

            if (IsCrawling != shouldCrawl)
            {
                IsCrawling = shouldCrawl;
                Owner.PhysicController.TriggerUpdateCollider();

                // if (isCrawling) IsDrawingBow = false;
            }

            Owner.View.SetCrawlSpeedMultiplyAnimation(Mathf.Abs(direction) * HorizontalMoveDirection);
        }

        public void OnCrawlEnter() => IsCrawlEntering = true;
        public void OnCrawlEntered() => IsCrawlEntering = false;
        public void OnCrawlExit() => IsCrawlExiting = true;
        public void OnCrawlExited() => IsCrawlExiting = false;

        #endregion

        #region - JUMP METHODS -
        protected virtual void Jump(bool isJump, float deltaTime)
        {
            if (!IsJumpEnabled) return;

            // Apply start jump velocity
            if (StartJumpVelocity.magnitude > 0.01f)
            {
                Vector2 jumpDirection = StartJumpVelocity.normalized;
                float dot = Vector2.Dot(Owner.PhysicController.currentVelocity, jumpDirection);
                if (dot < 0) Owner.PhysicController.currentVelocity -= dot * jumpDirection;

                Owner.PhysicController.currentVelocity += StartJumpVelocity;
                Owner.PhysicController.currentVelocity.y = Mathf.Min(Owner.PhysicController.currentVelocity.y, StartJumpVelocity.y * 1.25f);

                StartJumpVelocity = Vector2.zero;

                //apply jump force to standing collider
                Vector2 force = Physics2D.gravity * JumpForce * Owner.PhysicController.Weight;
                Owner.PhysicController.ApplyForceToStandingColliders(force);

                //event
                OnJump?.Invoke(this);
            }

            // Jumping up with continuous jump input
            // Set jump gravity so that the longer the jump key is pressed, the higher the character can jump
            if (Owner.PhysicController.IsInAir)
            {
                if (isJump && Owner.PhysicController.currentVelocity.y > 0)
                {
                    Owner.PhysicController.currentVelocity.y += Physics2D.gravity.y * (_jumpGravityMutiplier - 1.0f) * deltaTime;
                }
                // Jumping up without input
                else if (Owner.PhysicController.currentVelocity.y > 0.01f)
                {
                    Owner.PhysicController.currentVelocity.y += Physics2D.gravity.y * (_fallGravityMutiplier - 1.0f) * deltaTime;
                }
            }
        }
        #endregion

        #region - CLIMB METHODS -

        // Ladder Climb

        protected virtual void LadderClimb(float direction)
        {
            if (!IsClimbEnabled) return;
            if (Owner.Status.IsDead) return;

            if (IsExitingLadder)
            {
                Owner.PhysicController.currentGravityScale = 0.0f;
            }

            if (!IsClimbingLadder) return;

            Owner.PhysicController.currentGravityScale = 0.0f;
            Owner.PhysicController.currentVelocity.x = 0.0f;
            float currentClimbSpeed = IsRunning ? ClimbSpeedFast : ClimbSpeed;

            //handle climb movement
            Owner.PhysicController.currentVelocity.x = 0.0f;
            if (Mathf.Abs(direction) > Constants.MOVE_THRESHOLD)
            {
                Owner.PhysicController.currentVelocity.y = currentClimbSpeed * Mathf.Sign(direction);
                _climbingSpeedMultiply = (Owner.PhysicController.currentVelocity.y / ClimbSpeed);
            }
            else
            {
                Owner.PhysicController.currentVelocity.y = 0.0f;
                _climbingSpeedMultiply = 0.0f;
            }


            // Reach top of the ladder
            if (Owner.PhysicController.HasReachedLadderTop)
            {
                if (Owner.PhysicController.currentVelocity.y > 0.0f)
                {
                    Owner.PhysicController.currentVelocity.y = 0.0f;
                    _climbingSpeedMultiply = 0.0f;
                }
            }

            // Reach top bottom of the ladder
            // Using 2 pixel size as tolerance
            else if (Owner.PhysicController.HasReachedLadderBottom)
            {
                if (Owner.PhysicController.currentVelocity.y < 0.0f)
                {
                    Owner.PhysicController.currentVelocity.y = 0.0f;
                    _climbingSpeedMultiply = 0.0f;
                }
            }

            // Move characterView to the cimbing position defined by the ladder
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, Owner.PhysicController.Ladder.ClimbPosition.x, Time.fixedDeltaTime * _climbPositionLerpSpeed);
            transform.position = pos;

            // As entering ladder animation's root motion somehow don't work
            // Manual add down input
            if (IsEnteringLadder)
            {
                Owner.PhysicController.currentVelocity.y = currentClimbSpeed * -1.0f;
                _climbingSpeedMultiply = (Owner.PhysicController.currentVelocity.y / ClimbSpeed);
            }
        }


        protected virtual void LadderExitCheck(Vector2 direction)
        {
            if (Owner.Status.IsDead) return;
            if (IsClimbingLadder == false) return;
            if (IsEnteringLadder) return;

            // Reach ladder top, has up direction or has forward direction
            if ((Owner.PhysicController.HasReachedLadderTop && direction.y > Constants.MOVE_THRESHOLD) ||
                (Mathf.Abs(direction.x) > Constants.MOVE_THRESHOLD && (direction.x * Owner.Status.FacingDirectionValue) > 0))
            {
                // Have climbable surface, to exit ladder
                if (Owner.PhysicController.CheckSurfaceToClimbOn(out RaycastHit2D hit))
                {
                    IsExitingLadder = true;
                    IsClimbingLadder = false;
                    LadderExitHeight = hit.point.y - transform.position.y;
                }
            }

            // Reach ladder bottom, have down direction
            if (Owner.PhysicController.HasReachedLadderBottom && direction.y < -Constants.MOVE_THRESHOLD)
            {
                // is grounded, exit ladder
                if (Owner.PhysicController.IsGrounded)
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
                        Owner.PhysicController.Ladder = null;
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
            if (Owner.PhysicController.Ladder == null) return;
            if (Mathf.Abs(direction.x) > Constants.MOVE_THRESHOLD) return;
            // if (isDrawingBow) return;
            if (IsCrawling || IsCrawlEntering || IsCrawlExiting) return;

            if (Owner.PhysicController.CanClimbToLadder())
            {
                //climb up to ladder
                if (direction.y > Constants.MOVE_THRESHOLD)
                    IsClimbingLadder = true;

                //climb down to ladder, may needs ladder enter animation
                if (direction.y < -Constants.MOVE_THRESHOLD)
                {
                    IsClimbingLadder = true;
                    LadderEnterHeight = Owner.PhysicController.CalculateLadderEnterHeight();
                    if (LadderEnterHeight > -Mathf.Epsilon && LadderEnterHeight < Constants.PIXEL_SIZE * 41f)
                    {
                        IsEnteringLadder = true;
                    }
                }
            }
        }

        public void OnLadderEntered() => IsEnteringLadder = false;
        public void OnLadderExited() => IsExitingLadder = false;

        #endregion

        #region - DODGE METHODS -
        protected virtual void Dodge(bool inputDodge, float direction)
        {
            if (!IsDodgeEnabled) return;
            if (IsCrawling || IsClimbingLedge || IsEnteringLadder || IsExitingLadder) return;


            if (IsDodging == false && !_dodgeCooldown.IsComplete)
            {
                _dodgeCooldown.Update(Time.deltaTime);
                return;
            }

            if (Owner.PhysicController.AirTime > 0.1f) return;
            if (inputDodge == false) return;
            if (Mathf.Abs(direction) < Constants.MOVE_THRESHOLD) return;

            //set dodge direction
            if (direction * Owner.Status.FacingDirectionValue > 0) DodgeDirection = 1;
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

        protected virtual void DodgeUpdate()
        {
            //snap characterView to ground better when dodging
            if (IsDodging && Owner.PhysicController.AirTime > 0.01f)
            {
                Owner.PhysicController.currentVelocity.y -= 25.0f * Time.fixedDeltaTime;
            }
        }
        protected virtual void HandleDodge()
        {
            // If dodging but in the air, set to no dodge
            if (IsDodging)
            {
                if (Owner.PhysicController.AirTime > 0.2f) IsDodging = false;
            }
        }
        #endregion

        #region - LEDGE METHODS -

        protected virtual void LedgeClimb()
        {
            if (!IsClimbingLedge) return;

            if (Owner.PhysicController.FindLedgeToClimb(LedgePosition, out RaycastHit2D hit))
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

        protected virtual void HandleLedgeClimb(float moveDirection)
        {
            if (Owner.Status.IsDead || !IsLedgeEnabled || IsClimbingLadder || IsExitingLadder || IsCrawling || IsDodging)
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
                if (Mathf.Abs(moveDirection) >= Constants.MOVE_THRESHOLD && Mathf.Sign(moveDirection) != Owner.Status.FacingDirectionValue)
                {
                    IsClimbingLedge = false;
                    return;
                }

                return;
            }

            //ledge climb enter check 

            // Not moving or moving in the opposite direction
            if (Mathf.Abs(moveDirection) < Constants.MOVE_THRESHOLD || Mathf.Sign(moveDirection) != Owner.Status.FacingDirectionValue)
                return;

            //check for climbable ledge
            RaycastHit2D hit = Owner.PhysicController.CanClimbLedge();
            LedgePosition = hit.point;

            //check up direction for enough space to climb
            bool upAvailable = true;
            if (hit.collider != null)
            {
                Vector2 upCheckPosition = LedgePosition - new Vector2(0.0f, Constants.PIXEL_SIZE);
                RaycastHit2D hitUp = Owner.PhysicController.CheckSpaceToLedge(upCheckPosition);
                upAvailable = (hitUp.collider == null);
            }

            bool ledgeAvailable = (hit.collider != null) && upAvailable;

            //start climbing ledge
            // Ledge available and not climbing ledge
            if (ledgeAvailable && !IsClimbingLedge)
            {
                IsClimbingLedge = true;
                LedgeHeight = Owner.PhysicController.LedgeClimbRaycastHeight - hit.distance;

                Owner.PhysicController.Rigidbody.linearVelocity = Vector2.zero;
            }

            IsLedgeClimbLocked = false;
        }

        #endregion

        #region - EVENT REGISTER METHODS -
        protected virtual void Speed_OnValueChanged(object sender, ValueChangedEventArgs args)
        {
            BaseSpeed = args.NewValue;
        }

        protected virtual void JumpForce_OnValueChanged(object sender, ValueChangedEventArgs args)
        {
            JumpForce = args.NewValue;
        }

        #endregion

        #endregion
    }
}
