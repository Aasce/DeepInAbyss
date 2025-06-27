using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Entity : MonoBehaviour, IEntity
    {
        [SerializeField] protected SO_EntityInformation _information;
        [SerializeField] protected EntityStatus _status = new();

        public virtual SO_EntityInformation Information => _information;
        public virtual EntityStatus Status => _status;


        protected virtual void Awake () 
        {
            Status.Entity = this;
        }
        protected virtual void Start () { }
        protected virtual void OnEnable () { }
        protected virtual void OnDisable () { }
    }
}