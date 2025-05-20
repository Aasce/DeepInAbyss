using System.Collections.Generic;
using UnityEngine;


namespace Asce.Game.Entities
{
    public abstract class CreatureView : MonoBehaviour, IHasOwner<Creature>
    {
        [SerializeField] private Creature _owner;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected EntityRootMotionReceiver _rootMotionReceiver;

        protected readonly List<Renderer> _renderers = new();
        protected MaterialPropertyBlock _mpbAlpha;

        [Space]
        [SerializeField] protected FacingType _facing = FacingType.None;
        [SerializeField] protected float _alpha = 1.0f;
        [SerializeField] protected bool _isDead;

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
        
        protected virtual List<Renderer> Renderers => _renderers;
        protected virtual MaterialPropertyBlock MPBAlpha => _mpbAlpha ??= new MaterialPropertyBlock();

        public virtual FacingType Facing
        {
            get => _facing;
            set
            {
                if (value == FacingType.None) return;
                if (_facing == value) return;

                _facing = value;

                if (Animator != null)
                {
                    Animator.transform.localScale = new Vector3(1.0f, 1.0f, (int)_facing);
                    Vector3 pos = Animator.transform.localPosition;
                    pos.x = 0.064f * -(int)_facing;
                    Animator.transform.localPosition = pos;
                }
            }
        }

        public virtual float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = Mathf.Clamp01(value);

                MPBAlpha.SetFloat("_Alpha", _alpha);
                foreach (Renderer renderer in Renderers)
                {
                    if (renderer == null) continue;
                    renderer.SetPropertyBlock(MPBAlpha);
                }
            }
        }

        public virtual bool IsDead
        {
            get => _isDead;
            set
            {
                if (_isDead == value) return;
                _isDead = value;
                Animator.SetBool("IsDead", _isDead);
            }
        }

        public virtual bool IsLookingAtTarget 
        { 
            get => _isLookingAtTarget;
            set => _isLookingAtTarget = value; 
        }

        public virtual void LookAtTarget(bool isLooking, Vector2 targetPosition)
        {

        }

        //the look at target override _facing
        public FacingType LookAtTargetFacing(Vector3 inputTarget, FacingType facingDir)
        {
            FacingType targetFacing = EnumUtils.GetFacingByDirection(Mathf.Sign(inputTarget.x - transform.position.x));
            if (targetFacing == 0) targetFacing = facingDir;
            return targetFacing;
        }

        protected virtual void ResetRendererList()
        {
            Renderers.Clear();
        }
    }
}