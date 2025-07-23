using Asce.Game.Equipments;

namespace Asce.Game.Entities
{
    public interface IHasChestSlot : IHasEquipmentSlot
    {
        public ChestSlot ChestSlot { get; }
    }
}