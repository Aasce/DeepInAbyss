using Asce.Game.Entities;
using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public abstract class EquipEvent : GameComponent
    {
        public abstract void OnEquip(ICreature creature);
        public abstract void OnUnequip(ICreature creature);
    }
}
