using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyView : CreatureView, IHasOwner<Enemy>, IViewController
    {
        // current move blend, for blending idle, walk, run animation, lerps to target move blend on frame update
        protected float _moveBlend;
        protected float _movingBlendTransitionSpeed = 2.0f;

        [Header("Enemy View FX")]
        [SerializeField] private GameObject _fx;
        [SerializeField] private GameObject _dieFxPrefab;

        [Space]
        [SerializeField] protected Renderer _renderer;

        public new Enemy Owner
        {
            get => base.Owner as Enemy;
            set => base.Owner = value;
        }
        
        // Move Blend
        public float MoveBlend
        {
            get => _moveBlend;
            protected set => _moveBlend = value;
        }


        protected override void Start()
        {
            base.Start();
            if (Animator != null) Animator.SetFloat("LoopCycleOffset", Random.value);
        }

        protected override void Update()
        {
            base.Update();
            this.UpdateMoveBlend();
        }

        public virtual void AttackTrigger() => Animator.SetTrigger("Attack");


        protected override void UpdateAnimator()
        {
            base.UpdateAnimator();
            if (Animator == null) return;

            Animator.SetFloat("MovingBlend", MoveBlend);
            Animator.SetFloat("SpeedVertical", Owner.PhysicController.Rigidbody.linearVelocityY);
            Animator.SetBool("IsGrounded", Owner.PhysicController.IsGrounded);
            // Animator.SetBool("IsJumpPrepare", Owner.Action.IsInJumpPrepare);
            if (Owner.Action.IsAttacking) Animator.SetTrigger("Attack");
        }

        protected virtual void UpdateMoveBlend()
        {
            float targetMoveBlend = 0.0f;
            if (Owner.Action.IsMoving)
            {
                if (Owner.Action.IsRunning) targetMoveBlend = 1.0f;
                else targetMoveBlend = 0.5f;
            }

            MoveBlend = Mathf.MoveTowards(MoveBlend, targetMoveBlend, Time.deltaTime * _movingBlendTransitionSpeed);
        }

        protected override void ResetRendererList()
        {
            base.ResetRendererList();
            if (_renderer != null) Renderers.Add(_renderer);
        }
        protected override void Status_OnDeath(object sender)
        {
            base.Status_OnDeath(sender);
            if (_fx != null) _fx.SetActive(false);
            OnDieFx();
        }

        protected override void Status_OnRevive(object sender)
        {
            base.Status_OnRevive(sender);
            if (_fx != null) _fx.SetActive(true);
        }

        protected override void Status_OnFacingChanged(object sender, FacingType facing)
        {
            base.Status_OnFacingChanged(sender, facing);
        }


        public void OnDieFx()
        {
            if (_dieFxPrefab != null) Instantiate(_dieFxPrefab, transform.position, Quaternion.identity, transform.parent);
        }
    }
}