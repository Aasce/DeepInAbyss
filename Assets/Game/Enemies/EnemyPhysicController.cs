using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyPhysicController : CreaturePhysicController, IHasOwner<Enemy>
    {
        [Space]
        [SerializeField] protected float _airMaxSpeed = 2.0f;                         // maxSpeed move speed while in air
        [SerializeField] protected float _airAcceleration = 8.0f;                     // air acceleration

        protected float _currentSpeed;
        protected float _currentAcceleration;
        protected float _currentDragAcceleration;

        public new Enemy Owner 
        { 
            get => base.Owner as Enemy; 
            set => base.Owner = value; 
        }

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


        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
        }

        protected override void HandleSpeedAndAcceleration()
        {
            base.HandleSpeedAndAcceleration();

            // On Ground
            if (IsGrounded)
            {
                CurrentDragAcceleration = _groundDrag;

                if (Owner.Action.IsRunning) this.SetSpeedAndAcceleration(Owner.Action.RunAcceleration, Owner.Action.RunMaxSpeed);
                else this.SetSpeedAndAcceleration(Owner.Action.WalkAcceleration, Owner.Action.WalkMaxSpeed);

                if (Owner.Action.IsInJumpPrepare) CurrentAcceleration = 0;

                //float targetSurfaceSpeedMultiply = Mathf.Sin(Mathf.Min(SurfaceAngleForward, 90.0f) * Mathf.Deg2Rad);
                //SurfaceSpeedMultiply = targetSurfaceSpeedMultiply < 1.0f
                //    ? Mathf.Lerp(SurfaceSpeedMultiply, targetSurfaceSpeedMultiply, Time.fixedDeltaTime)
                //    : 1.0f;

                //CurrentSpeed *= SurfaceSpeedMultiply;
                return;
            }

            // In Air
            CurrentAcceleration = _airAcceleration;
            CurrentSpeed = _airMaxSpeed;
            CurrentDragAcceleration = _airDrag * Mathf.Abs(currentVelocity.x);
        }

        protected virtual void SetSpeedAndAcceleration(float acceleration, float maxSpeed)
        {
            CurrentAcceleration = acceleration;
            CurrentSpeed = maxSpeed;
        }
    }
}