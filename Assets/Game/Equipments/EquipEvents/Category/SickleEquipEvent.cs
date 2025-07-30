using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class SickleEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _strengthValue = new(5f, StatValueType.Plat);
        [SerializeField] private StatValue _speedValue = new(0.6f, StatValueType.Plat);

        public string Reason => "Sickle equipment";

        public StatValue StrengthValue => _strengthValue;
        public StatValue SpeedValue => _speedValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.AddAgent(creature.gameObject, Reason, _strengthValue);
			
			if (creature.Stats is IHasSpeed hasSpeed)
				hasSpeed.Speed.AddAgent(creature.gameObject, Reason, _speedValue);
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.RemoveAgent(creature.gameObject, Reason);
			
			if (creature.Stats is IHasSpeed hasSpeed)
				hasSpeed.Speed.RemoveAgent(creature.gameObject, Reason);
        }
    }
}
