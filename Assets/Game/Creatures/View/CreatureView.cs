using Asce.Game.Utils;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;


namespace Asce.Game.Entities
{
    public abstract class CreatureView : MonoBehaviour, IHasOwner<Creature>, IView
    {
        [SerializeField, HideInInspector] private Creature _owner;
        [SerializeField, HideInInspector] protected Animator _animator;
        [SerializeField, HideInInspector] protected EntityRootMotionReceiver _rootMotionReceiver;

        protected List<Renderer> _renderers = new();
        protected MaterialPropertyBlock _mpbAlpha;

        [Space]
        [SerializeField] protected float _alpha = 1.0f;

        [SerializeField] protected bool _isLookingAtTarget;

        /// <summary>
        ///     Reference to the creature that owns this stats controller.
        /// </summary>
        public virtual Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public virtual Animator Animator => _animator;

        public virtual Vector2 RootMotionVelocity => _rootMotionReceiver.RootMotionVelocity;
        
        public virtual List<Renderer> Renderers => _renderers;
        public virtual MaterialPropertyBlock MPBAlpha => _mpbAlpha != null ? _mpbAlpha : _mpbAlpha = new MaterialPropertyBlock();


        public virtual float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = Mathf.Clamp01(value);
                this.SetRendererAlpha(_alpha);
            }
        }


        public virtual bool IsLookingAtTarget 
        { 
            get => _isLookingAtTarget;
            set => _isLookingAtTarget = value; 
        }

        protected virtual void Reset()
        {
            transform.LoadComponent(out _animator);
            transform.LoadComponent(out _rootMotionReceiver);
            if (transform.LoadComponent(out _owner))
            {
                Owner.View = this;
            }
        }

        protected virtual void Awake()
        {
            this.ResetRendererList();
        }

        protected virtual void Start()
        {
            if (Owner != null)
            {
                Owner.Status.OnDeath += Status_OnDeath;
                Owner.Status.OnRevive += Status_OnRevive;
                Owner.Status.OnFacingChanged += Status_OnFacingChanged;
            }
        }

        protected virtual void Update()
        {
            this.UpdateAnimator();
        }

        protected virtual void LateUpdate()
        {
            this.LookAtTarget();
        }

        public virtual void LookAtTarget()
        {

        }



        //the look at target override _facing
        public FacingType LookAtTargetFacing(Vector3 inputTarget)
        {
            FacingType targetFacing = EnumUtils.GetFacingByDirection(Mathf.Sign(inputTarget.x - transform.position.x));
            if (targetFacing == 0) targetFacing = Owner.Status.FacingDirection;
            return targetFacing;
        }

        protected virtual void UpdateAnimator()
        {

        }

        public virtual void ResetRendererList()
        {
            Renderers.Clear();
        }

        protected virtual void Status_OnDeath(object sender)
        {
            if (Animator == null) return;
            Animator.SetBool("IsDead", true);
        }

        protected virtual void Status_OnRevive(object sender)
        {
            if (Animator == null) return;
            Animator.SetBool("IsDead", false);
        }

        protected virtual void Status_OnFacingChanged(object sender, FacingType facing)
        {
            if (Animator == null) return;
            
            Animator.transform.localScale = new Vector3(1.0f, 1.0f, (int)facing);
            Vector3 pos = Animator.transform.localPosition;
            pos.x = 0.064f * -(int)facing;
            Animator.transform.localPosition = pos;
        }
    }
}