using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Entity : MonoBehaviour, IEntity
    {
        [SerializeField] protected EntityStatus _status = new EntityStatus();


        public virtual EntityStatus Status => _status;


        protected virtual void Awake () { }
        protected virtual void Start () { }
        protected virtual void OnEnable () { }
        protected virtual void OnDisable () { }
    }
}