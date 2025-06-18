using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasAction<T> : IEntity where T : IActionController
    {
        public T Action { get; }
    }
}
