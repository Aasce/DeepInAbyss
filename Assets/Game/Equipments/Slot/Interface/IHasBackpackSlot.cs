using Asce.Game.Equipments;

namespace Asce.Game.Entities
{
    public interface IHasBackpackSlot : IHasEquipmentSlot
    {
        public BackpackSlot BackpackSlot { get; }
    }
}