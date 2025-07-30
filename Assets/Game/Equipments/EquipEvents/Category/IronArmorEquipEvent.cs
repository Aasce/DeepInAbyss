using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class IronArmorEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _armorValue = new(15f, StatValueType.Plat);
        [SerializeField] private StatValue _resistanceValue = new(8f, StatValueType.Plat);

        public string Reason => "Iron Armor equipment";

        public StatValue ArmorValue => _armorValue;
        public StatValue ResistanceValue => _resistanceValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
			{
				hasDefense.DefenseGroup.Armor.AddAgent(creature.gameObject, Reason, _armorValue);
				hasDefense.DefenseGroup.Resistance.AddAgent(creature.gameObject, Reason, _resistanceValue);
			}
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
			{
				hasDefense.DefenseGroup.Armor.RemoveAgent(creature.gameObject, Reason);
				hasDefense.DefenseGroup.Resistance.RemoveAgent(creature.gameObject, Reason);
			}
        }
    }
}
