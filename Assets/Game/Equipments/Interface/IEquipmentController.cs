using Asce.Game.Entities;

namespace Asce.Game.Equipments
{
    public interface IEquipmentController
    {
        ICreature Owner { get; }
    }
}