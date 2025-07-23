using Asce.Game.Equipments;

namespace Asce.Game.Entities
{
    public interface IHasWeaponSlot : IHasEquipmentSlot
    {
        public WeaponSlot WeaponSlot { get; }
    }
}