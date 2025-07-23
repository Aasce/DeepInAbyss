using Asce.Game.Equipments;

namespace Asce.Game.Entities
{
    public interface IHasFeetsSlot : IHasEquipmentSlot
    {
        public FeetsSlot FeetsSlot { get; }
    }
}