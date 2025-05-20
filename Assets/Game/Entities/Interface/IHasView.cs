using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasView<T> : IEntity
    {
        T View { get; }
    }
}
