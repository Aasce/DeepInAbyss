using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasView<T> : IEntity where T : IViewController
    {
        T View { get; }
    }
}
