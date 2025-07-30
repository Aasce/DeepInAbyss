using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class IronBootEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _armorValue = new(10f, StatValueType.Plat);
        [SerializeField] private StatValue _speedValue = new(0.5f, StatValueType.Plat);

        public string Reason => "Iron Boot equipment";

        public StatValue ArmorValue => _armorValue;
        public StatValue SpeedValue => _speedValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
				hasDefense.DefenseGroup.Armor.AddAgent(creature.gameObject, Reason, _armorValue);
			
			if (creature.Stats is IHasSpeed hasSpeed)
				hasSpeed.Speed.AddAgent(creature.gameObject, Reason, _speedValue);
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
				hasDefense.DefenseGroup.Armor.RemoveAgent(creature.gameObject, Reason);
			
			if (creature.Stats is IHasSpeed hasSpeed)
				hasSpeed.Speed.RemoveAgent(creature.gameObject, Reason);
        }
    }
}
