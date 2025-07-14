using Asce.Game.Combats;
using Asce.Game.Equipments;
using Asce.Game.Equipments.Weapons;
using Asce.Managers;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities.Characters
{
    public class CharacterAction : CreatureAction, IHasOwner<Character>, IActionController, 
        ILookable, IMovable, IRunnable, IJumpable, ICrouchable, ICrawlable, ILadderClimbable, 
        IDashable, IDodgeable, IAttackable, IThrowableWeapon
    {
        #region - FIELDS -
        [Header("Look")]
        [SerializeField] protected bool _isLooking = false;
        [SerializeField] protected Vector2 _targetPosition = Vector2.zero;


        [Header("Movement")]
        [SerializeField] protected bool _isMoveEnable = true;

        [Tooltip("Is the character moving")]
        [SerializeField] protected bool _isMoving;

        [Tooltip("Is the character running")]
        [SerializeField] protected bool _isRunning;

        protected float _idleTime;
        protected int _horizontalMoveDirection = 1; // move direction, 1: forward, -1:backward


        [Header("Speed")]
        [Tooltip("Speed of the character")]
        [SerializeField] protected float _baseSpeed = 5f;

        [Space]
        [Tooltip("Speed scale when character walking")]
        [SerializeField] protected float _walkSpeedScale = 1f;

        [Tooltip("Acceleration when character walking")]
        [SerializeField] protected float _walkAcceleration = 10f;

        [Space]
        [Tooltip("Speed scale when character running")]
        [SerializeField] protected float _runSpeedScale = 1.5f;

        [Tooltip("Acceleration when character running")]
        [SerializeField] protected float _runAcceleration = 15f;

        [Space]
        [Tooltip("Speed scale when character crouching")]
        [SerializeField] protected float _crouchSpeedScale = 0.5f;

        [Tooltip("Acceleration when character crouching")]
        [SerializeField] protected float _crouchAcceleration = 8f;

        [Space]
        [Tooltip("Speed scale when character crawling")]
        [SerializeField] protected float _crawlSpeedScale = 0.4f;

        [Tooltip("Acceleration when character crawling")]
        [SerializeField] protected float _crawlAcceleration = 8f;

        [Space]
        [Tooltip("Start Speed when character dashing")]
        [SerializeField] protected float _dashStartSpeedScale = 4f;

        [Tooltip("Speed scale when character dashing")]
        [SerializeField] protected float _dashSpeedScale = 2f;

        [Tooltip("Acceleration when character dashing")]
        [SerializeField] protected float _dashAcceleration = 20f;

        [Space]
        [Tooltip("Speed scale when character climbing Ladder")]
        [SerializeField] protected float _climbSpeedScale = 1f;

        [Tooltip("Speed scale when character climbing Ladder and runing")]
        [SerializeField] protected float _climbFastSpeedScale = 1.5f;


        [Header("UpdateDash")]
        [SerializeField] protected bool _isDashEnabled = true;
        [SerializeField] protected bool _isDashing;

        [Space]
        [SerializeField] protected Cooldown _dashingTime = new(1f);
        [SerializeField] protected Cooldown _dashCooldown = new(1f);

        [Header("Crounch and UpdateCrawl")]
        [SerializeField] protected bool _isCrouching;
        [SerializeField] protected bool _isCrawling;
        protected bool _isCrawlEntering;
        protected bool _isCrawlExiting;


        [Header("UpdateJump")]
        [SerializeField] protected bool _jumpEnabled = true;
        [SerializeField] protected bool _isJumping;

        [Tooltip("Vertical force applied to character when jump")]
        [SerializeField] protected float _jumpForce = 5f;

        [Tooltip("When the character's air time is less than this value, it is still able to jump")]
        [SerializeField] protected float _jumpTolerance = 0.15f;

        [Tooltip("Gravity multiplier when character is jumping, should be within [0, 1]," +
            "set it to lower value so that the longer you press the jump button, the higher the character can jump")]
        [SerializeField] protected float _jumpGravityMutiplier = 0.6f;

        [Tooltip("Gravity multiplier when character is falling, should be equal or greater than 1.0")]
        [SerializeField] protected float _fallGravityMutiplier = 1.3f;
        [SerializeField] protected Cooldown _jumpCooldown = new(0.2f);

        protected Vector2 _startJumpVelocity;


        [Header("Climb")]
        [SerializeField] protected bool _climbEnabled = true;
        protected float _climbingSpeedMultiply;

        // Lerp speed when moving the character to the climb position
        protected readonly float _climbPositionLerpSpeed = 7.5f;

        protected bool _isEnteringLadder;
        protected bool _isClimbingLadder;
        protected bool _isExitingLadder;

        protected float _ladderEnterHeight;
        protected float _ladderExitHeight; // y distance from the button of the character to the surface when exiting from ladder

        protected Cooldown _ladderToAirTime = new(0.5f);
        protected float _ladderExitPositionZ; // origin position z of the character, used for resetting the position z after exiting the ladder


        [Header("Dodge")]
        [SerializeField] protected bool _isDdodgeEnabled = true;

        [Tooltip("Is the character dodging")]
        [SerializeField] protected bool _isDodging;

        [Tooltip("Speed multiply when character dodging")]
        [SerializeField] protected float _dodgeSpeedRootMotionMultiply = 1.25f;

        [SerializeField] protected Cooldown _dodgeCooldown = new(0.1f);
        protected int _dodgeDirection; //dodge direction   1: front  -1:back
        protected int _dodgeFacing;


        [Header("Ledge")]
        [SerializeField] protected bool _ledgeEnabled = true;

        [Tooltip("Is the character climbing ledge")]
        [SerializeField] protected bool _isClimbingLedge;
        [SerializeField] protected bool _isLedgeClimbLocked;
        protected float _ledgeHeight;
        protected Vector2 _ledgePosition;


        [Header("Attack")]
        [SerializeField] protected bool _isAttacking;
        [SerializeField] protected bool _isMeleeAttacking;
        [SerializeField] protected AttackType _attackType = AttackType.Swipe;
        [SerializeField] protected AttackType _meleeAttackType = AttackType.Swipe;
        [SerializeField] protected Cooldown _attackCooldown = new(1f);

        [Space]
        [SerializeField] protected float _attackSpeedMultiply = 1f;

        protected bool _isDrawingBow;
        private bool _isArrowDrawn;

        [Header("Throw Owner")]
        [SerializeField] protected float _throwForce = 5f;
        [SerializeField] protected float _throwAngularSpeed = 200f;

        #region - CONTROL FIELDS- 
        [Header("Control")]
        [SerializeField] protected Vector2 _moveDirection = Vector2.zero;
        [SerializeField] protected bool _runState = false;
        [SerializeField] protected bool _dashTrigger = false;
        [SerializeField] protected bool _crouchState = false;
        [SerializeField] protected bool _crawlState = false;
        [SerializeField] protected bool _jumpTrigger = false;
        [SerializeField] protected bool _dodgeTrigger = false;
        [SerializeField] private bool _attackTrigger = false;
        [SerializeField] private bool _meleeAttackTrigger = false;
        #endregion

        #endregion

        #region - EVENTS -

        public event Action<object> OnMoveStart;
        public event Action<object> OnMoveEnd;

        public event Action<object> OnRunStart;
        public event Action<object> OnRunEnd;

        public event Action<object> OnDash;

        public event Action<object> OnCrouchStart;
        public event Action<object> OnCrouchEnd;

        public event Action<object> OnCrawlStart;
        public event Action<object> OnCrawlEnd;

        public event Action<object> OnJump;

        public event Action<object> OnDodgeStart;
        public event Action<object> OnDodgeEnd;

        public event Action<object> OnClimbStart;
        public event Action<object> OnClimbEnd;

        public event Action<object> OnBowPull;
        public event Action<object, AttackEventArgs> OnAttackStart;
        public event Action<object, AttackEventArgs> OnAttackHit;
        public event Action<object, AttackEventArgs> OnAttackEnd;
        public event Action<object, Vector2> OnAttackCast;
        public event Action<object> OnThrow;


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
        public bool IsMoveEnabled
        {
            get => _isMoveEnable;
            set => _isMoveEnable = value;
        }
        public bool IsMoving
        {
            get => _isMoving;
            protected set
            {
                if (_isMoving == value) return;
                _isMoving = value;

                if (_isMoving) OnMoveStart?.Invoke(this);
                else OnMoveEnd?.Invoke(this);
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            protected set
            {
                if (_isRunning == value) return;
                _isRunning = value;

                if (_isRunning) OnRunStart?.Invoke(this);
                else OnRunEnd?.Invoke(this);
            }
        }

        public int HorizontalMoveDirection
        {
            get => _horizontalMoveDirection;
            protected set => _horizontalMoveDirection = (int)Mathf.Sign(value);
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

        public float ClimbMaxSpeed => BaseSpeed * _climbSpeedScale;
        public float ClimbFastMaxSpeed => BaseSpeed * _climbFastSpeedScale;
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
            protected set 
            {
                if (_isDashing ==  value) return;
                _isDashing = value;

                if (_isDashing) OnDash?.Invoke(this);
            }
        }
        #endregion

        #region - CROUCH AND CRAWL -
        public bool IsCrouching
        {
            get => _isCrouching;
            protected set
            {
                if (_isCrouching == value) return;
                _isCrouching = value;

                // Event
                if (_isCrouching) OnCrouchStart?.Invoke(this);
                else OnCrouchEnd?.Invoke(this);
            }
        }

        public bool IsCrawling
        {
            get => _isCrawling;
            protected set
            {
                if (_isCrawling == value) return;
                _isCrawling = value;

                // Event
                if (_isCrawling) OnCrawlStart?.Invoke(this);
                else OnCrawlEnd?.Invoke(this);
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

        #region - JUMP -
        public bool IsJumpEnabled
        {
            get => _jumpEnabled;
            set => _jumpEnabled = value;
        }
        public bool IsJumping
        {
            get => _isJumping;
            protected set => _isJumping = value;
        }
        public float JumpForce
        {
            get => _jumpForce;
            protected set => _jumpForce = value;
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

                if (_isClimbingLadder) OnClimbStart?.Invoke(this);
                else OnClimbEnd?.Invoke(this);

                //set ladder climb z position
                Vector3 pos = Owner.PhysicController.transform.position;
                if (_isClimbingLadder)
                {
                    _ladderExitPositionZ = pos.z;
                    pos.z = Owner.PhysicController.Ladder.ClimbPosition.z;
                }
                else pos.z = _ladderExitPositionZ;
                Owner.PhysicController.transform.position = pos;
            }
        }

        public float ClimbingSpeedMultiply
        {
            get => _climbingSpeedMultiply;
            protected set => _climbingSpeedMultiply = value;
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

        public float LadderEnterHeight
        {
            get => _ladderEnterHeight;
            protected set => _ladderEnterHeight = value;
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
            protected set
            {
                if (_isDodging == value) return;
                _isDodging = value;
                this.OnSetDodging();
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

        public float DodgeSpeedRootMotionMultiply => _dodgeSpeedRootMotionMultiply;

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

        #region - ATTACK - 
        public AttackType AttackType
        {
            get => _attackType;
            set => _attackType = value;
        }
        public AttackType MeleeAttackType
        {
            get => _meleeAttackType;
            set => _meleeAttackType = value;
        }

        public bool IsAttacking
        {
            get => _isAttacking;
            set => _isAttacking = value;
        }

        public bool IsMeleeAttacking
        {
            get => _isMeleeAttacking;
            set => _isMeleeAttacking = value;
        }
        public float AttackSpeedMultiply
        {
            get => _attackSpeedMultiply;
            set => _attackSpeedMultiply = value;
        }

        public bool IsDrawingBow 
        {
            get => _isDrawingBow;
            set
            {
                if (_isDrawingBow == value) return;
                if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder) return;

                if (_attackCooldown.IsComplete) _isDrawingBow = value;
                else _isDrawingBow = false;

                BowWeapon bow = Owner.Equipment.WeaponSlot.CurrentWeapon as BowWeapon;
                Projectile projectile = Owner.Equipment.LeftHandSlot.Projectile;
                if (!_isDrawingBow)
                {
                    if (Owner.Equipment.LeftHandSlot.IsProjectileReady && projectile != null)
                    {
                        // Unable to shoot, destroy arrow projectile
                        if (Owner.Status.IsDead || IsCrawling)
                        {
                            Owner.Equipment.LeftHandSlot.DestroyProjectile();
                        }
                        // Shoot arrow out
                        else
                        {
                            if (bow != null)
                            {
                                projectile.SetDamage(Owner.Stats.Strength.Value, 10f);
                                Owner.Equipment.LeftHandSlot.DetachProjectile(); // Detach the projectile from the character
                                bow.Launch(projectile);

                                _attackCooldown.Reset();
                            }
                        }
                    }

                    Owner.Equipment.LeftHandSlot.IsProjectileReady = false;
                    if (bow != null) bow.View.IsStringPulled = false;
                }

                Owner.View.SetIsDrawingBowAnimation(_isDrawingBow);
            }
        }
        public bool IsArrowDrawn
        {
            get => _isArrowDrawn;
            set
            {
                if (_isArrowDrawn == value) return;
                _isArrowDrawn = value;

                Owner.View.SetIsArrowDrawnAnimation(_isArrowDrawn);
                Owner.View.SetAttackSpeedMultiplyAnimation(AttackSpeedMultiply);
            }
        }
        #endregion

        #region - THROW WEAPON PROPERTIES -
        public float ThrowForce
        {
            get => _throwForce;
            set => _throwForce = value;
        }
        public float ThrowAngularSpeed
        {
            get => _throwAngularSpeed;
            set => _throwAngularSpeed = value;
        }
        #endregion

        #region - CONTROL PROPERTIES -
        public Vector2 MoveDirection
        {
            get => _moveDirection;
            set => _moveDirection = value;
        }

        public bool RunState
        {
            get => _runState;
            set => _runState = value;
        }
        public bool DashTrigger
        {
            get => _dashTrigger;
            set => _dashTrigger = value;
        }
        public bool DodgeTrigger
        {
            get => _dodgeTrigger;
            set => _dodgeTrigger = value;
        }
        public bool CrouchState
        {
            get => _crouchState;
            set => _crouchState = value;
        }
        public bool CrawlState
        {
            get => _crawlState;
            set => _crawlState = value;
        }
        public bool JumpTrigger
        {
            get => _jumpTrigger;
            set => _jumpTrigger = value;
        }

        public bool AttackTrigger
        {
            get => _attackTrigger;
            set => _attackTrigger = value;
        }

        public bool MeleeAttackTrigger
        {
            get => _meleeAttackTrigger;
            set => _meleeAttackTrigger = value;
        }
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
            this.UpdateTime(Time.deltaTime);

            this.HandleMove(MoveDirection.x, RunState);
            this.UpdateDash(Time.deltaTime);
            this.UpdateJump(Time.deltaTime, MoveDirection.x);
            this.UpdateDodge(Time.deltaTime, MoveDirection.x);
            this.UpdateLedgeClimb(MoveDirection.x);
            this.UpdateAttack(Time.deltaTime);
            this.UpdateGetDownPlatform(MoveDirection.y);

            this.UpdateDash();

            this.UpdateCrouch();
            this.UpdateCrawl(MoveDirection.x);

            this.LadderEnterCheck(MoveDirection);
            this.LadderExitCheck(MoveDirection);
        }

        #endregion

        #region - PHYSIC METHODS -
        public override void HandlePhysic(float deltaTime)
        {
            base.HandlePhysic(deltaTime);

            this.HandleUpdateMove(MoveDirection.x);
            this.HandleUpdateJump(deltaTime);
            this.HandleUpdateLadderClimb(deltaTime, MoveDirection.y);
            this.HandleUpdateDodge(deltaTime);
            this.HandleUpdateLedgeClimb();
        }

        #endregion

        #region - CONTROLL METHODS -

        public void Moving(Vector2 moveDiretion)
        {
            MoveDirection = (Owner.Status.IsDead) ? Vector2.zero : moveDiretion;
        }

        public void Running(bool state)
        {
            RunState = state;
            if (state)
            {
                // Run cancels crouch and crawl
                CrouchState = false;
                CrawlState = false;
            }
        }

        public void Dashing()
        {
            // Do not allow dash when climbing ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                DashTrigger = false;
                return;
            }

            // Do not allow dash when climbing ledge
            if (IsClimbingLedge)
            {
                DashTrigger = false;
                return;
            }

            // Do not allow dash when crouching and crawling
            if (IsCrouching || IsCrawling)
            {
                DashTrigger = false;
                return;
            }

            DashTrigger = true;
        }

        public void Dodging()
        {
            DodgeTrigger = true;
        }

        public void Jumping(bool isJump)
        {
            JumpTrigger = isJump;

            if (JumpTrigger)
            {
                // Jump cancels crouch and crawl
                CrouchState = false;
                CrawlState = false;
            }
        }

        public void Crouching()
        {
            if (Owner.PhysicController.IsInAir) return;

            if (CrawlState && Owner.PhysicController.CanExitCrawling())
            {
                CrawlState = false;
                CrouchState = true;
            }
            else
            {
                // Toggle Crouch state
                CrouchState = !CrouchState;
            }

            // Can't get out of a crouching without enough space
            if (!Owner.PhysicController.CanExitCrouching) CrouchState = true;

            // Do not allow entering crouch when climbing ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                CrouchState = false;
            }
        }

        public void Crawling()
        {
            if (Owner.PhysicController.IsInAir) return;

            // Toggle Crouch state
            CrawlState = !CrawlState;

            if (!Owner.PhysicController.CanEnterCrawling()) CrawlState = false; // Dont enough space to enter crawling
            if (!Owner.PhysicController.CanExitCrawling()) CrawlState = true; // Dont enough space to exit crawling

            if (!CrawlState) CrouchState = false; // Exit crawl and crouch

            // Do not allow entering crawl when climbing ladder
            if (IsClimbingLadder || IsEnteringLadder || IsExitingLadder)
            {
                CrawlState = false;
            }
        }

        public void Looking(bool isLooking, Vector2 targetPosition)
        {
            IsLooking = isLooking;
            TargetPosition = targetPosition;
        }

        public void Attacking(bool isAttacking)
        {
            AttackTrigger = isAttacking;
        }
        public void MeleeAttacking(bool isAttacking)
        {
            MeleeAttackTrigger = isAttacking;
        }

        #endregion

        #region - UPDATE METHODS -
        public virtual void UpdateTime(float deltaTime)
        {
            if (Owner.Status.IsDead) return;
            //idle timer
            if (IsIdle) IdleTime += deltaTime;
            else IdleTime = 0f;
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
            if (Mathf.Abs(MoveDirection.x) > Constants.MOVE_THRESHOLD)
            {
                Owner.Status.FacingDirectionValue = Mathf.RoundToInt(Mathf.Sign(MoveDirection.x));
                return;
            }
        }
        #endregion

        #region - MOVE METHODS -
        protected virtual void HandleMove(float direction, bool isRunning)
        {
            if (Owner.Status.IsDead) return;
            if (!IsMoveEnabled) return;

            // Set isMoving and isRunning
            IsMoving = (Mathf.Abs(direction) > Constants.MOVE_THRESHOLD);
            if (!IsMoving)
            {
                IsRunning = false;
                return;
            }

            // Disallow running backward
            if (Owner.View.IsLookingAtTarget && Mathf.Sign(direction) != Owner.Status.FacingDirectionValue)
            {
                IsRunning = false;
            }
            else IsRunning = isRunning;

            // Is the character moving backward
            HorizontalMoveDirection = (Mathf.Sign(direction) * Owner.Status.FacingDirectionValue) == 1 ? 1 : -1;
        }

        protected virtual void HandleUpdateMove(float direction)
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
                if (Mathf.Abs(direction) <= Constants.MOVE_THRESHOLD) speed = Mathf.MoveTowards(speed, 0f, currentDragAcceleration * Time.fixedDeltaTime);

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
                if (direction > 0f && currentVelocity.x > currentSpeed) shouldMove = false;
                else if (direction < 0f && currentVelocity.x < -currentSpeed) shouldMove = false;
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

        #endregion

        #region - DASH METHODS -
        protected virtual bool IsCanDash()
        {
            if (Owner.Status.IsDead) return false;
            if (!_isDashEnabled) return false;
            if (!Owner.PhysicController.IsGrounded || IsCrouching) return false;
            if (Owner.View.IsPointingAtTarget) return false;
            if (!_dashCooldown.IsComplete) return false;

            return true;
        }

        protected virtual void UpdateDash()
        {
            if (!DashTrigger) return;
            DashTrigger = false;

            if (!IsCanDash()) return;

            Owner.PhysicController.Accelerate(DashStartSpeed * Owner.Status.FacingDirectionValue);

            IsDashing = true;
            _dashCooldown.Reset();
            _dashingTime.Reset();
        }

        protected virtual void UpdateDash(float deltaTime)
        {
            _dashCooldown.Update(deltaTime);
            _dashingTime.Update(deltaTime);

            if (_dashingTime.IsComplete) IsDashing = false;
        }

        #endregion

        #region - CROUCH AND CRAWL METHODS -
        protected virtual void UpdateCrouch()
        {
            bool shouldCrouch = CrouchState;
            if (Owner.PhysicController.AirTime > 1f) shouldCrouch = false;

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

        protected virtual void UpdateCrawl(float direction)
        {
            bool shouldCrawl = CrawlState;
            if (Owner.PhysicController.AirTime > 1f) shouldCrawl = false;

            // If there is no enough space to get up, keep craling
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
        protected virtual void HandleUpdateJump(float deltaTime)
        {
            if (!IsJumpEnabled) return;

            Vector2 currentVelocity = Owner.PhysicController.currentVelocity;

            // Apply start jump velocity
            if (StartJumpVelocity.magnitude > 0.01f)
            {
                IsJumping = true;
                Vector2 jumpDirection = StartJumpVelocity.normalized;
                float dot = Vector2.Dot(currentVelocity, jumpDirection);
                if (dot < 0) currentVelocity -= dot * jumpDirection;

                currentVelocity += StartJumpVelocity;
                currentVelocity.y = Mathf.Min(currentVelocity.y, StartJumpVelocity.y * 1.25f);

                StartJumpVelocity = Vector2.zero;

                // Apply jump force to standing collider
                Vector2 force = JumpForce * Owner.PhysicController.Weight * Physics2D.gravity;
                Owner.PhysicController.ApplyForceToStandingColliders(force);

                // Event
                OnJump?.Invoke(this);
            }

            // Jumping up with continuous jump input
            // Set jump gravity so that the longer the jump key is pressed, the higher the character can jump
            if (Owner.PhysicController.IsInAir)
            {
                if (currentVelocity.y > 0.01f)
                {
                    float multiplier = (JumpTrigger && currentVelocity.y > 0f) ? _jumpGravityMutiplier : _fallGravityMutiplier;
                    currentVelocity.y += Physics2D.gravity.y * (multiplier - 1f) * deltaTime;
                }
            }

            // Stop jumping when rising ends
            if (IsJumping && currentVelocity.y <= 0.01f)
            {
                IsJumping = false;
            }

            // Reassign to PhysicController
            Owner.PhysicController.currentVelocity = currentVelocity;
        }

        protected virtual void UpdateJump(float deltaTime, float horizontalDirection)
        {
            if (Owner.Status.IsDead) return;
            if (!IsJumpEnabled) return;

            // Disable jump while crawling or dodging
            if (IsCrawling || IsCrawlEntering || IsCrawlExiting || IsDodging)
            {
                _jumpCooldown.Reset();
                return;
            }

            // Jump cooling down, not in air and cooldown is not complete
            if (!Owner.PhysicController.IsInAir && !_jumpCooldown.IsComplete)
                _jumpCooldown.Update(deltaTime);

            if (!JumpTrigger || !_jumpCooldown.IsComplete) return;

            this.HandleStartJumpVelocity(horizontalDirection);
            _jumpCooldown.Reset();
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

                // Mix ladder direction or move direction to jump direction
                if (Mathf.Abs(horizontalDirection) < Constants.MOVE_THRESHOLD)
                {
                    jumpDirection = Vector2.up + new Vector2((int)Owner.PhysicController.Ladder.Direction, 0f) * 0.25f;
                }
                else
                {
                    jumpDirection = Vector2.up + new Vector2(Mathf.Sign(horizontalDirection), 0f) * 0.5f;
                }
            }

            // Jump while entering or exiting climbing 
            if (IsEnteringLadder || IsExitingLadder)
            {
                Owner.PhysicController.IsGrounded = false;
                IsEnteringLadder = false;
                IsExitingLadder = false;
                IsClimbingLadder = false;

                jumpDirection = new Vector2(-Owner.Status.FacingDirectionValue, 0f) * 0.5f;
            }

            // Jump while climbing ledge
            if (IsClimbingLedge || IsLedgeClimbLocked)
            {
                Owner.PhysicController.IsGrounded = false;
                IsClimbingLedge = false;
                IsLedgeClimbLocked = false;

                jumpDirection = Vector2.up + new Vector2(-Owner.Status.FacingDirectionValue, 0f) * 0.5f;
            }

            StartJumpVelocity = jumpDirection.normalized * JumpForce;
        }

        #endregion

        #region - CLIMB METHODS -
        protected virtual void HandleUpdateLadderClimb(float deltaTime, float yDirection)
        {
            if (!IsClimbEnabled) return;
            if (Owner.Status.IsDead) return;

            if (IsExitingLadder)
            {
                Owner.PhysicController.currentGravityScale = 0f;
            }

            if (!IsClimbingLadder) return;

            Owner.PhysicController.currentGravityScale = 0f;
            Owner.PhysicController.currentVelocity.x = 0f;
            float currentClimbSpeed = IsRunning ? ClimbFastMaxSpeed : ClimbMaxSpeed;
            float currentVeloityY = Owner.PhysicController.currentVelocity.y;

            // Handle climb movement
            if (Mathf.Abs(yDirection) > Constants.MOVE_THRESHOLD)
            {
                currentVeloityY = currentClimbSpeed * Mathf.Sign(yDirection);
                _climbingSpeedMultiply = (currentVeloityY / ClimbMaxSpeed);
            }
            else
            {
                currentVeloityY = 0f;
                _climbingSpeedMultiply = 0f;
            }

            // Reach top of the ladder
            if (Owner.PhysicController.HasReachedLadderTop)
            {
                if (currentVeloityY > 0f)
                {
                    currentVeloityY = 0f;
                    _climbingSpeedMultiply = 0f;
                }
            }

            // Reach top bottom of the ladder
            else if (Owner.PhysicController.HasReachedLadderBottom)
            {
                if (currentVeloityY < 0f)
                {
                    currentVeloityY = 0f;
                    _climbingSpeedMultiply = 0f;
                }
            }

            // Move character to the cimbing position defined by the ladder
            Vector3 pos = Owner.PhysicController.transform.position;
            pos.x = Mathf.Lerp(pos.x, Owner.PhysicController.Ladder.ClimbPosition.x, deltaTime * _climbPositionLerpSpeed);
            Owner.PhysicController.transform.position = pos;

            // As entering ladder animation's root motion somehow don't work
            // Manual add down input
            if (IsEnteringLadder)
            {
                currentVeloityY = currentClimbSpeed * -1f;
                _climbingSpeedMultiply = (currentVeloityY / ClimbMaxSpeed);
            }

            // Reassign to PhysicController
            Owner.PhysicController.currentVelocity.y = currentVeloityY;
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
                    LadderExitHeight = hit.point.y - Owner.PhysicController.transform.position.y;
                }
            }

            // Reach ladder bottom, have down direction
            if (Owner.PhysicController.HasReachedLadderBottom && direction.y < -Constants.MOVE_THRESHOLD)
            {
                // is grounded, exit ladder
                if (Owner.PhysicController.IsGrounded) IsClimbingLadder = false;

                // Special case, from ladder to air
                // Reach ladder bottom, but not grounded, down direction persist more than _ladderToAirTime.BaseTime
                else
                {
                    _ladderToAirTime.Update(Time.deltaTime);
                    if (_ladderToAirTime.IsComplete)
                    {
                        IsClimbingLadder = false;

                        Owner.PhysicController.UnLadder();
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

            if (!Owner.PhysicController.CanClimbToLadder()) return;
            
            // Climb up to ladder
            if (direction.y > Constants.MOVE_THRESHOLD) 
                IsClimbingLadder = true;

            // Climb down to ladder, may needs ladder enter animation
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

        public void OnLadderEntered() => IsEnteringLadder = false;
        public void OnLadderExited() => IsExitingLadder = false;

        #endregion

        #region - DODGE METHODS -
        protected virtual void HandleUpdateDodge(float deltaTime)
        {
            // Snap character to ground better when dodging
            if (IsDodging && Owner.PhysicController.AirTime > 0.01f)
            {
                Owner.PhysicController.currentVelocity.y -= 25f * deltaTime;
            }
        }

        protected virtual void UpdateDodge(float deltaTime, float direction)
        {
            // If dodging but in the air, set to no dodge
            if (IsDodging)
            {
                if (Owner.PhysicController.AirTime > 0.2f) 
                    IsDodging = false;
            }

            // Update dodge cooldown
            if (!IsDodging && !_dodgeCooldown.IsComplete)
            {
                _dodgeCooldown.Update(deltaTime);
            }

            if (!IsDodgeEnabled) return;
            if (!DodgeTrigger) return;
            DodgeTrigger = false;

            if (IsCrawling || IsClimbingLedge || IsEnteringLadder || IsExitingLadder) return;
            if (Owner.PhysicController.AirTime > 0.1f) return;
            if (!_dodgeCooldown.IsComplete) return;
            if (Mathf.Abs(direction) < Constants.MOVE_THRESHOLD) return;

            // Set dodge direction
            if (direction * Owner.Status.FacingDirectionValue > 0) DodgeDirection = 1;
            else DodgeDirection = -1;

            IsDodging = true;
            DodgeTrigger = false;
            _dodgeCooldown.Reset();
        }

        public void DodgeStart() => OnDodgeStart?.Invoke(this);
        public void DodgeEnd()
        {
            IsDodging = false;
            OnDodgeEnd?.Invoke(this);
        }

        protected virtual void OnSetDodging()
        {
            // Ender dodging
            if (IsDodging)
            {
                DodgeFacing = Owner.Status.FacingDirectionValue;
            }
            else
            {
                // If dodge into a place where there is only space for crawling
                if (Owner.PhysicController.CanExitCrawling()) IsCrawling = true;
            }

            Owner.PhysicController.TriggerUpdateCollider();
            Owner.View.SetIsDodgingAnimation(IsDodging, DodgeDirection);
        }
        #endregion

        #region - LEDGE METHODS -

        protected virtual void HandleUpdateLedgeClimb()
        {
            if (!IsClimbingLedge) return;

            if (Owner.PhysicController.FindLedgeToClimb(LedgePosition, out RaycastHit2D hit))
            {
                // Update ledge position and character position in case the ledge collider is moving
                Owner.PhysicController.transform.position += (Vector3)(hit.point - LedgePosition);
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

        protected virtual void UpdateLedgeClimb(float moveDirection)
        {
            if (Owner.Status.IsDead || !IsLedgeEnabled || IsClimbingLadder || IsExitingLadder || IsCrawling || IsDodging)
            {
                IsClimbingLedge = false;
                return;
            }

            // Ledge climb exit check 
            if (IsClimbingLedge)
            {
                if (Owner.PhysicController.transform.position.y > LedgePosition.y)
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

            // Ledge climb enter check 

            // Not moving or moving in the opposite direction
            if (Mathf.Abs(moveDirection) < Constants.MOVE_THRESHOLD || Mathf.Sign(moveDirection) != Owner.Status.FacingDirectionValue)
                return;

            // Check for climbable ledge
            bool canClimbLedge = Owner.PhysicController.CanClimbLedge(out RaycastHit2D hit);
            LedgePosition = hit.point;

            // Check up direction for enough space to climb
            bool upAvailable = true;
            if (canClimbLedge)
            {
                Vector2 upCheckPosition = LedgePosition + new Vector2(0f, Constants.PIXEL_SIZE);
                upAvailable = Owner.PhysicController.CheckEnoughSpaceToLedge(upCheckPosition, out RaycastHit2D hitUp);
            }

            bool ledgeAvailable = (hit.collider != null) && upAvailable;

            // Start climbing ledge
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

        #region - ATTACK -
        protected virtual void UpdateAttack(float deltaTime)
        {
            Weapon weapon = Owner.Equipment.WeaponSlot.CurrentWeapon;
            AttackType currentAttackType = AttackType.Swipe;

            if (weapon != null)
            {
                currentAttackType = MeleeAttackTrigger ? MeleeAttackType : AttackType;
                if (IsCrawling)
                {
                    currentAttackType = MeleeAttackType;
                }
                else if (AttackType == AttackType.Archery && weapon is BowWeapon)
                {
                    IsDrawingBow = AttackTrigger;
                }
            }

            _attackCooldown.Update(deltaTime);
            if (_attackCooldown.IsComplete)
            {
                IsAttacking = AttackTrigger || MeleeAttackTrigger;
                IsMeleeAttacking = MeleeAttackTrigger;
            }
            else
            {
                IsAttacking = false;
                IsMeleeAttacking = false;
            }
            
            Owner.View.SetAttackActionIndexAnimation(currentAttackType.ToIntValue());
            Owner.View.SetIsAttackingAnimation(IsAttacking);
            Owner.View.SetAttackSpeedMultiplyAnimation(AttackSpeedMultiply);
        }

        public void ArrowDraw()
        {
            if (!IsDrawingBow) return;
            Owner.Equipment.CreateProjectile();

            IsArrowDrawn = true;
        }
        public void ArrowNock()
        {
            if (Owner.Equipment.WeaponSlot.CurrentWeapon is BowWeapon bow)
            {
                if (IsDrawingBow) bow.View.IsStringPulled = true;
                else bow.View.IsStringPulled = false;
            }
            OnBowPull?.Invoke(this);
        }

        public void ArrowReady()
        {
            if (IsDrawingBow) Owner.Equipment.LeftHandSlot.IsProjectileReady = true;
        }

        public void ArrowPutBack()
        {
            IsArrowDrawn = false;
            Owner.Equipment.LeftHandSlot.DestroyProjectile();
        }

        public void AttackStart() => OnAttackStart?.Invoke(this, new AttackEventArgs(Owner, IsMeleeAttacking ? MeleeAttackType : AttackType));
        public void AttackHit() => OnAttackHit?.Invoke(this, new AttackEventArgs(Owner, IsMeleeAttacking ? MeleeAttackType : AttackType));
        public void AttackEnd()
        {
            OnAttackEnd?.Invoke(this, new AttackEventArgs(Owner, IsMeleeAttacking ? MeleeAttackType : AttackType));
            _attackCooldown.Reset();
        }

        public void AttackCast()
        {
            Vector2 dir = Owner.View.IsPointingAtTarget ? Owner.View.LookDirection(TargetPosition) : Owner.Equipment.WeaponSlot.CurrentWeapon.transform.right;
            OnAttackCast?.Invoke(this, dir);
        }

        public void Throwing()
        {
            if (IsAttacking || IsMeleeAttacking) return;
            if (IsArrowDrawn) return;

            Weapon weapon = Owner.Equipment.WeaponSlot.CurrentWeapon;
            if (weapon == null) return;

            Owner.Equipment.WeaponSlot.DetachWeapon();

            weapon.Rigidbody.linearVelocity = Owner.PhysicController.currentVelocity;
            weapon.Rigidbody.angularVelocity = -Owner.Status.FacingDirectionValue * ThrowAngularSpeed;

            Vector2 targetDirection = TargetPosition - (Vector2)Owner.transform.position;
            weapon.Rigidbody.AddForce(targetDirection.normalized * ThrowForce);

            OnThrow?.Invoke(this);
        }
        #endregion

        #region - GET DOWN PLATFORM METHODS -
        protected virtual void UpdateGetDownPlatform(float yDirection)
        {
            Owner.PhysicController.IsGetDownPlatform = yDirection < 0f && Mathf.Abs(yDirection) > Constants.MOVE_THRESHOLD;
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
