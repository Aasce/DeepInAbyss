using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasView<T> : IEntity where T : IView
    {
        T View { get; }
    }
}
