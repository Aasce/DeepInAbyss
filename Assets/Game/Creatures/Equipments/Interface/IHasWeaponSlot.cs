using Asce.Game.Equipments;

namespace Asce.Game.Entities
{
    public interface IHasWeaponSlot : ICreatureEquipment
    {
        public WeaponSlot WeaponSlot { get; }
    }
}