using Asce.Managers;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Enviroments 
{
    public class Elevator : GameComponent, IEnviromentComponent, IOptimizedComponent
    {
        [Header("Reference")]
        [SerializeField] protected ElavatorPlatform _platform;
        [SerializeField] protected SpriteRenderer _leftChain;
        [SerializeField] protected SpriteRenderer _rightChain;

        [Header("Config")]
        [SerializeField] protected Vector2 _lengthRange = new (2, 5);
        [SerializeField] protected Cooldown _waitCooldown = new(1.0f);
        [SerializeField] protected float _moveSpeed = 3.0f;

        [Space]
        [SerializeField] protected float _length;
        [SerializeField] protected bool _isWaiting = false;
        [SerializeField] protected bool _isMoveUp = false;

        protected float _currentSpeed;
        protected float _targetLength;
        protected SecondOrderDynamics _secondOrderDynamics = new (frequency: 4.0f, damping: 0.3f, response: - 0.3f);

        bool IOptimizedComponent.IsActive => gameObject.activeSelf;
        Bounds IOptimizedComponent.Bounds
        {
            get
            {
                Vector3 center = transform.position;
                Vector2 size = Vector2.zero;

                if (_platform != null)
                {
                    // Attempt to get width from Renderer or Collider2D
                    float width = _platform.Collider.bounds.size.x; 
                    float height = _lengthRange.y;
                    size = new Vector2(width, height);

                    // center is between top and bottom movement
                    center += Vector3.down * (height * 0.5f);
                }

                return new Bounds(center, size);
            }
        }

        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        public Vector2 LengthRange => _lengthRange;
        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = value;
        }

        public float Length
        {
            get => _length;
            set
            {
                _length = Mathf.Max(0f, value);
            }
        }
        public bool IsWaiting
        {
            get => _isWaiting;
            set
            {
                if (_isWaiting == value) return;
                _isWaiting = value;
                _waitCooldown.Reset();
            }
        }
        public bool IsMoveUp
        {
            get => _isMoveUp;
            set => _isMoveUp = value;
        }


        protected virtual void Start()
        {
            Length = IsMoveUp ? LengthRange.y : LengthRange.x;
            _targetLength = Length;

            _secondOrderDynamics.Reset(_targetLength);
        }

        protected virtual void Update()
        {
            if (IsWaiting) this.Waiting(Time.deltaTime);
            else this.Moving();

            _targetLength += _currentSpeed * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            Length = _secondOrderDynamics.Update(_targetLength, Time.fixedDeltaTime);
            this.PartsUpdate();
        }


        protected virtual void Waiting(float deltaTime)
        {
            _waitCooldown.Update(deltaTime);
            if (_waitCooldown.IsComplete) IsWaiting = false;
            _currentSpeed = 0f;
        }

        protected virtual void Moving()
        {
            _currentSpeed = IsMoveUp ? -MoveSpeed : MoveSpeed;

            if (IsMoveUp)
            {
                if (_targetLength < LengthRange.x)
                {
                    IsMoveUp = false;
                    IsWaiting = true;
                }
            }
            else
            {
                if (_targetLength > LengthRange.y)
                {
                    IsMoveUp = true;
                    IsWaiting = true;
                }
            }
        }

        protected virtual void PartsUpdate()
        {
            Vector2 chainSize = new (Constants.PIXEL_SIZE * 3f, Length - Constants.PIXEL_SIZE * 8f);

            if (_platform != null) _platform.Rigidbody.MovePosition(transform.position + Vector3.down * Length);
            if (_leftChain != null) _leftChain.size = chainSize;
            if (_rightChain != null) _rightChain.size = chainSize;
        }

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.gameObject.SetActive(state);
        }
    }
}