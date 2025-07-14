using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Entity : GameComponent, IEntity, IOptimizedComponent
    {
        [SerializeField] protected SO_EntityInformation _information;
        [SerializeField] protected EntityStatus _status = new();

        public virtual SO_EntityInformation Information => _information;
        public virtual EntityStatus Status => _status;

        bool IOptimizedComponent.IsActive => this.gameObject.activeSelf;
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        protected virtual void Awake () 
        {
            Status.Entity = this;
        }
        protected virtual void Start () { }
        protected virtual void OnEnable () { }
        protected virtual void OnDisable () { }

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.gameObject.SetActive(state);
        }
    }
}