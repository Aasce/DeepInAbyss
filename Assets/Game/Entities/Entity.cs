using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Entity : MonoBehaviour, IEntity
    {


        protected virtual void Awake () { }
        protected virtual void Start () { }
        protected virtual void OnEnable () { }
        protected virtual void OnDisable () { }
    }
}