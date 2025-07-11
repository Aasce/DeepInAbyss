using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Entity : MonoBehaviour, IEntity, IOptimizedComponent
    {
        [SerializeField] protected SO_EntityInformation _information;
        [SerializeField] protected EntityStatus _status = new();

        public virtual SO_EntityInformation Information => _information;
        public virtual EntityStatus Status => _status;

        bool IOptimizedComponent.IsActive => this.gameObject.activeSelf;
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        protected virtual void Reset() { this.RefReset(); }
        protected virtual void Awake () 
        {
            Status.Entity = this;
        }
        protected virtual void Start () { }
        protected virtual void OnEnable () { }
        protected virtual void OnDisable () { }
        protected virtual void RefReset() { }

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.gameObject.SetActive(state);
        }
    }
}