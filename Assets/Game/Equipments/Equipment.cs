using Asce.Game.Entities;
using Asce.Game.Items;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class Equipment : Item
    {
        public new SO_EquipmentInformation Information => base.Information as SO_EquipmentInformation;

        public Equipment (SO_EquipmentInformation information) : base (information)
        {

        }

        public virtual void Equip(ICreature carrier)
        {
            // Implement equip logic here
        }
        public virtual void Unequip(ICreature carrier)
        {
            // Implement unequip logic here
        }
    }
}