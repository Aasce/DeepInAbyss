using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasMovement<T> : IEntity where T : IMovable
    {
        public T Movement { get; }
    }
}
