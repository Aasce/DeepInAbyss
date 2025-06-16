using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasEquipment<T> : IEntity
    {
        T Equipment { get; }
    }
}
