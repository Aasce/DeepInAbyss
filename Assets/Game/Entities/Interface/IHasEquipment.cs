using Asce.Game.Equipments;
using UnityEngine;

namespace Asce.Game.Entities
{
    public interface IHasEquipment<T> : IEntity where T : IEquipmentController
    {
        T Equipment { get; }
    }
}
