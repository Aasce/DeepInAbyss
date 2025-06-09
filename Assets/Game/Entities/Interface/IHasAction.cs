using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasAction<T> : IEntity
    {
        public T Action { get; }
    }
}
