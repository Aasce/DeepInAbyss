using Asce.Game.Combats;
using Asce.Managers;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyAction : CreatureAction, IHasOwner<Enemy>, IActionController, ILookable, IMovable, IRunnable, IJumpable, IAttackable
    {
        [Header("Look")]
        [SerializeField] protected bool _isLooking = false;
        [SerializeField] protected Vector2 _targetPosition = Vector2.zero;

        [Header("Speed")]
        [SerializeField] protected float _baseSpeed = 5.0f;

        [Space]
        [SerializeField] protected float _walkSpeedScale = 1f;
        [SerializeField] protected float _walkAcceleration = 10.0f;

        [Space]
        [SerializeField] protected float _runSpeedScale = 1.5f;
        [SerializeField] protected float _runAcceleration = 15.0f;

        [Header("Movement")]
        [SerializeField] protected bool _isMoveEnable = true;
        protected bool _isMoving;
        protected bool _isRunning;
        protected float _idleTime;

        [Header("UpdateJump")]
        [SerializeField] protected bool _isJumpEnabled = true;
        [SerializeField] protected bool _isJumping;
        [SerializeField] protected float _jumpForce = 5.0f;

        [SerializeField] protected float _jumpGravityMutiplier = 0.6f;
        [SerializeField] protected float _fallGravityMutiplier = 1.3f;
        [SerializeField] protected Cooldown _jumpDelayCooldown = new(0.15f);
        [SerializeField] protected Cooldown _jumpCooldown = new(0.2f);
        protected bool _isInJumpPrepare = false;

        [Header("Attack")]
        [SerializeField] protected bool _isStartAttack = false;
        [SerializeField] protected bool _isAttacking = false;
        [SerializeField] protected bool _canAttackWhenMoving = true;
        [SerializeField] protected bool _canAttackInAir = true;
        [SerializeField] protected bool _isStopWhenAttack = true;
        [SerializeField] protected Cooldown _attackCooldown = new(2f);

        #region - CONTROL FIELDS - 
        [Header("Control")]
        [SerializeField] private Vector2 _moveDirection = Vector2.zero;
        [SerializeField] private bool _runState = false;
        [SerializeField] private bool _jumpTrigger = false;
        [SerializeField] private bool _attackTrigger = false;
        [SerializeField] private bool _meleeAttackTrigger = false;
        #endregion

        #region - EVENTS - 
        public event Action<object> OnFootstep;

        public event Action<object> OnMoveStart;
        public event Action<object> OnMoveEnd;

        public event Action<object> OnRunStart;
        public event Action<object> OnRunEnd;

        public event Action<object> OnJump;

        public event Action<object, AttackEventArgs> OnAttackStart;
        public event Action<object, AttackEventArgs> OnAttackHit;
        public event Action<object, AttackEventArgs> OnAttackEnd;
        #endregion


        public new Enemy Owner
        {
            get => base.Owner as Enemy;
            set => base.Owner = value;
        }

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

        public float BaseSpeed
        {
            get => _baseSpeed;
            protected set => _baseSpeed = value;
        }

        public float WalkMaxSpeed => BaseSpeed * _walkSpeedScale;
        public float WalkAcceleration => _walkAcceleration;

        public float RunMaxSpeed => BaseSpeed * _runSpeedScale;
        public float RunAcceleration => _runAcceleration;

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
        public bool IsIdle => !Owner.Status.IsDead && !IsMoving && !Owner.PhysicController.IsInAir;

        public bool IsRunning
        {
            get => _isRunning;
            protected set
            {
                if (_isRunning == value) return;
                _isRunning = value;

                // Events
                if (_isRunning) OnRunStart?.Invoke(this);
                else OnRunEnd?.Invoke(this);
            }
        }

        public bool IsJumpEnabled
        {
            get => _isJumpEnabled;
            set => _isJumpEnabled = value;
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
        public bool IsInJumpPrepare
        {
            get => _isInJumpPrepare;
            protected set => _isInJumpPrepare = value;
        }

        public bool IsStartAttack
        {
            get => _isStartAttack;
            set
            {
                if (_isStartAttack == value) return;
                _isStartAttack = value;
                if (_isStartAttack) OnAttackStart?.Invoke(Owner, new AttackEventArgs(AttackType.Special));
            }
        }
        public bool IsAttacking
        {
            get => _isAttacking;
            set => _isAttacking = value;
        }
        protected bool CanAttackWhenMoving
        {
            get => _canAttackWhenMoving;
            set => _canAttackWhenMoving = value;
        }
        protected bool CanAttackInAir
        {
            get => _canAttackInAir;
            set => _canAttackInAir = value;
        }
        public bool CanMovingWhenAttack
        {
            get => _isStopWhenAttack;
            set => _isStopWhenAttack = value;
        }


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


        protected override void Awake()
        {
            base.Awake();
        }

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

            this.UpdateMove(MoveDirection.x);
            this.UpdateJump(Time.deltaTime);
            this.UpdateAttack(Time.deltaTime);

            this.HandleMove(Time.deltaTime, MoveDirection.x);
            this.HandleJump(Time.deltaTime);
        }

        public void Moving(Vector2 diretion)
        {
            MoveDirection = diretion;
        }
        public void Running(bool state)
        {
            RunState = state;
        }
        public void Jumping(bool state)
        {
            JumpTrigger = state;
        }
        public void Looking(bool isLooking, Vector2 targetPosition = default)
        {
            IsLooking = isLooking;
            if (IsLooking) 
            { 
                TargetPosition = targetPosition;
                float direction = (TargetPosition - (Vector2)Owner.transform.position).x;
                Owner.Status.FacingDirection = EnumUtils.GetFacingByDirection(direction);
            }
        }
        public void Attacking(bool isAttacking)
        {
            if (_attackCooldown.IsComplete) AttackTrigger = isAttacking;
            else AttackTrigger = false; // Reset attack trigger if cooldown is not complete
        }
        public void MeleeAttacking(bool isAttacking)
        {
            if (_attackCooldown.IsComplete) MeleeAttackTrigger = isAttacking;
            else MeleeAttackTrigger = false; // Reset melee attack trigger if cooldown is not complete
        }


        protected virtual void HandleMove(float deltaTime, float direction)
        {
            float currentSpeed = Owner.PhysicController.CurrentSpeed;
            float currentAcceleration = Owner.PhysicController.CurrentAcceleration;
            float currentDragAcceleration = Owner.PhysicController.CurrentDragAcceleration;
            float currentVelocityX = Owner.PhysicController.Rigidbody.linearVelocityX;

            // Has horizontal movement input
            if (IsMoving)
            {
                //if current horizontal speed is out of allowed range, let it fall to the allowed range
                bool shouldMove = true;
                if (direction > 0 && currentVelocityX > currentSpeed)
                {
                    currentVelocityX = Mathf.MoveTowards(currentVelocityX, currentSpeed, currentDragAcceleration * deltaTime);
                    shouldMove = false;
                }
                if (direction < 0 && currentVelocityX < -currentSpeed)
                {
                    currentVelocityX = Mathf.MoveTowards(currentVelocityX, currentSpeed, currentDragAcceleration * deltaTime);
                    shouldMove = false;
                }

                // Otherwise, add movement acceleration to cureent velocity
                if (shouldMove)
                {
                    currentVelocityX += currentAcceleration * deltaTime * direction;
                    currentVelocityX = Mathf.Clamp(currentVelocityX, -currentSpeed, currentSpeed);
                }
            }
            // No horizontal movement input, brake to speed zero
            else
            {
                currentVelocityX = Mathf.MoveTowards(currentVelocityX, 0.0f, currentDragAcceleration * deltaTime);
            }

            //set modified velocity back to rigidbody
            Owner.PhysicController.Rigidbody.linearVelocityX = currentVelocityX;
        }

        protected virtual void UpdateMove(float direction)
        {
            if (Owner.Status.IsDead || !IsMoveEnabled)
            {
                IsRunning = false;
                IsMoving = false;
                return;
            }

            bool canMove = true;
            if (!CanMovingWhenAttack && (IsAttacking)) canMove = false;
            if (IsInJumpPrepare || JumpTrigger || IsJumping) canMove = false;

            if (!canMove) direction = 0f;

            IsRunning = RunState;
            IsMoving = Mathf.Abs(direction) > Constants.MOVE_THRESHOLD;
        }

        protected virtual void HandleJump(float deltaTime)
        {
            if (!IsJumpEnabled || !JumpTrigger) return;

            float currentVelocityY = Owner.PhysicController.Rigidbody.linearVelocityY;
            bool canJump = Owner.PhysicController.IsGrounded && _jumpCooldown.IsComplete;

            if (canJump)
            {
                _jumpCooldown.Reset();

                currentVelocityY += _jumpForce;
                IsJumping = true;
                Owner.PhysicController.IsGrounded = false;

                // Events
                OnJump?.Invoke(this);
            }
            if (currentVelocityY > 0)
            {
                float multiplier = _jumpGravityMutiplier;
                currentVelocityY += Physics.gravity.y * (multiplier - 1.0f) * deltaTime;
            }

            JumpTrigger = false;

            //set modified velocity back to rigidbody
            Owner.PhysicController.Rigidbody.linearVelocityY = currentVelocityY;
        }
        protected virtual void UpdateJump(float deltaTime)
        {
            if (!Owner.PhysicController.IsGrounded)
            {
                IsJumping = false;
                return;
            }
            _jumpCooldown.Update(deltaTime);
        }

        protected virtual void UpdateAttack(float deltaTime)
        {
            _attackCooldown.Update(deltaTime);

            if (IsStartAttack && !_attackCooldown.IsComplete)
            {
                IsStartAttack = false;
            }

            if (AttackTrigger || MeleeAttackTrigger)
            {
                this.Attack();
                AttackTrigger = false;
                MeleeAttackTrigger = false;
            }
        }
        protected virtual void Attack()
        {
            if (!CanAttackInAir && Owner.PhysicController.IsInAir) return;
            if (!CanAttackWhenMoving && IsMoving) return;

            IsStartAttack = true;
            IsAttacking = true;
            _attackCooldown.Reset();
        }

        protected override void UpdateFacing()
        {
            base.UpdateFacing();
            if (IsAttacking)
            {
                return;
            }

            if (Mathf.Abs(MoveDirection.x) > Constants.MOVE_THRESHOLD)
            {
                Owner.Status.FacingDirectionValue = Mathf.RoundToInt(Mathf.Sign(MoveDirection.x));
                return;
            }
        }


        public virtual void AttackEventCalling()
        {
            OnAttackHit?.Invoke(Owner, new AttackEventArgs(AttackType.Special));
            IsAttacking = false; // Reset attacking state after attack event is called
        }
        public virtual void FootStepEventCalling() => OnFootstep?.Invoke(this);

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
    }
}