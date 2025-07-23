using Asce.Game.Equipments;

namespace Asce.Game.Entities
{
    public interface IHasLegsSlot : IHasEquipmentSlot
    {
        public LegsSlot LegsSlot { get; }
    }
}