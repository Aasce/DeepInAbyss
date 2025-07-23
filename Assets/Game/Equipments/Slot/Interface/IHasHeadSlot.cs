using Asce.Game.Equipments;

namespace Asce.Game.Entities
{
    public interface IHasHeadSlot : IHasEquipmentSlot
    {
        public HeadSlot HeadSlot { get; }
    }
}