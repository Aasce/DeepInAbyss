using Asce.Game.Equipments;
using UnityEngine;

namespace Asce.Game.UIs.Equipments
{
    public interface IUIEquipment
    {
        public IEquipmentController Controller { get; }
    }
}
