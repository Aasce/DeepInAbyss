using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasAction<T> : IEntity where T : IMovable
    {
        public T Action { get; }
    }
}
