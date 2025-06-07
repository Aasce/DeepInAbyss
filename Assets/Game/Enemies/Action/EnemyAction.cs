using Asce.Managers;
using Asce.Managers.Utils;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyAction : CreatureAction, IHasOwner<Enemy>
    {
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

        [Header("Jump")]
        [SerializeField] protected bool _isJumpEnable = true;
        [SerializeField] protected float _jumpForce = 5.0f;

        [SerializeField] protected float _jumpGravityMutiplier = 0.6f;
        [SerializeField] protected float _fallGravityMutiplier = 1.3f;
        [SerializeField] protected Cooldown _jumpDelayCooldown = new(0.15f);
        [SerializeField] protected Cooldown _jumpCooldown = new(0.2f);
        protected bool _isInJumpPrepare = false;

        [Header("Attack")]
        [SerializeField] protected bool _isAttacking = false;
        [SerializeField] protected bool _canAttackWhenMoving = true;
        [SerializeField] protected bool _canAttackInAir = true;
        [SerializeField] protected Cooldown _attackCooldown = new(2f); 
        private bool _attackTriggered = false;

        #region - CONTROL FIELDS- 
        [SerializeField] private Vector2 _controlMove = Vector2.zero;
        [SerializeField] private bool _controlRun = false;
        [SerializeField] private bool _controlJump = false;
        [SerializeField] private bool _controlAttack = false;
        #endregion

        public new Enemy Owner
        {
            get => base.Owner as Enemy;
            set => base.Owner = value;
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

        public bool IsJumpEnable
        {
            get => _isJumpEnable;
            set => _isJumpEnable = value;
        }
        public float JumpForce
        {
            get => _jumpForce;
            set => _jumpForce = value;
        }
        public bool IsInJumpPrepare
        {
            get => _isInJumpPrepare;
            protected set => _isInJumpPrepare = value;
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
        public bool ControlJump
        {
            get => _controlJump;
            set => _controlJump = value;
        }

        public bool ControlAttack
        {
            get => _controlAttack;
            set => _controlAttack = value;
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
            this.HandleMove(ControlMove.x);
            this.HandleJump(Time.deltaTime);
            this.HandleAttack(Time.deltaTime);

            this.Move(ControlMove.x);
            this.Jump(ControlJump);
        }


        public void ControlMoving(Vector2 moveDiretion, bool isRunning = false)
        {
            ControlMove = moveDiretion;
            ControlRun = isRunning;
        }
        public void ControlJumping(bool state)
        {
            ControlJump = state;
        }
        public void ControlAttacking()
        {
            if (!_attackCooldown.IsComplete) return;
            _attackTriggered = true;
        }


        protected virtual void Move(float direction)
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
                    currentVelocityX = Mathf.MoveTowards(currentVelocityX, currentSpeed, currentDragAcceleration * Time.deltaTime);
                    shouldMove = false;
                }
                if (direction < 0 && currentVelocityX < -currentSpeed)
                {
                    currentVelocityX = Mathf.MoveTowards(currentVelocityX, currentSpeed, currentDragAcceleration * Time.deltaTime);
                    shouldMove = false;
                }

                // Otherwise, add movement acceleration to cureent velocity
                if (shouldMove)
                {
                    currentVelocityX += currentAcceleration * Time.deltaTime * direction;
                    currentVelocityX = Mathf.Clamp(currentVelocityX, -currentSpeed, currentSpeed);
                }
            }
            // No horizontal movement input, brake to speed zero
            else
            {
                currentVelocityX = Mathf.MoveTowards(currentVelocityX, 0.0f, currentDragAcceleration * Time.deltaTime);
            }

            //set modified velocity back to rigidbody
            Owner.PhysicController.Rigidbody.linearVelocityX = currentVelocityX;
        }

        protected virtual void Jump(bool isJumping)
        {
            if (!IsJumpEnable) return; 

            float currentVelocityY = Owner.PhysicController.Rigidbody.linearVelocityY;

            IsInJumpPrepare = isJumping && Owner.PhysicController.IsGrounded && isJumping && _jumpCooldown.IsComplete;
            if (IsInJumpPrepare)
            {
                _jumpDelayCooldown.Update(Time.deltaTime);
                if (_jumpDelayCooldown.IsComplete)
                {
                    _jumpDelayCooldown.Reset();
                    _jumpCooldown.Reset();

                    Owner.PhysicController.IsGrounded = false;
                    currentVelocityY += _jumpForce;
                }
            }
            else _jumpDelayCooldown.Reset();

            if (currentVelocityY > 0)
            {
                float multiplier = isJumping ? _jumpGravityMutiplier : _fallGravityMutiplier;
                currentVelocityY += Physics.gravity.y * (multiplier - 1.0f) * Time.deltaTime;
            }

            //set modified velocity back to rigidbody
            Owner.PhysicController.Rigidbody.linearVelocityY = currentVelocityY;
        }

        protected virtual void Attack()
        {
            if (!Owner.PhysicController.IsGrounded && !CanAttackInAir) return;
            if (!CanAttackWhenMoving && IsMoving) return;
            if (IsInJumpPrepare) return;

            IsAttacking = true;
            _attackCooldown.Reset();
        }

        protected override void UpdateFacing()
        {
            base.UpdateFacing();
            if (Mathf.Abs(ControlMove.x) > Constants.MOVE_THRESHOLD)
            {
                Owner.Status.FacingDirectionValue = Mathf.RoundToInt(Mathf.Sign(ControlMove.x));
                return;
            }

        }

        protected virtual void HandleMove(float direction)
        {
            if (!IsMoveEnable)
            {
                IsRunning = false;
                IsMoving = false;
                return;
            }

            IsRunning = _controlRun;

            if (Owner.Status.IsDead) direction = 0.0f;
            if (!CanAttackWhenMoving && (IsAttacking || _attackTriggered)) direction = 0.0f;
            if (IsInJumpPrepare) direction = 0.0f;

            IsMoving = Mathf.Abs(direction) > Constants.MOVE_THRESHOLD;
        }

        protected virtual void HandleJump(float deltaTime)
        {
            if (!Owner.PhysicController.IsGrounded) return;
            _jumpCooldown.Update(deltaTime);
        }

        protected virtual void HandleAttack(float deltaTime)
        {
            _attackCooldown.Update(deltaTime);

            if (IsAttacking && !_attackCooldown.IsComplete)
            {
                IsAttacking = false;
            }


            if (_attackTriggered)
            {
                this.Attack();
                _attackTriggered = false;
            }
        }

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