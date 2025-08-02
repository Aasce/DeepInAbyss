using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasPhysicController<T> : IEntity
    {
        public T PhysicController { get; }
    }
}
