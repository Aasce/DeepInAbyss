using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class WoodenBowEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _strengthValue = new(10f, StatValueType.Plat);
        [SerializeField] private StatValue _viewRadiusValue = new(6f, StatValueType.Plat);

        public string Reason => "Wooden Bow equipment";

        public StatValue StrengthValue => _strengthValue;
        public StatValue ViewRadiusValue => _viewRadiusValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.AddAgent(creature.gameObject, Reason, _strengthValue);
			
			if (creature.Stats is IHasViewRadius hasViewRadius)
				hasViewRadius.ViewRadius.AddAgent(creature.gameObject, Reason, _viewRadiusValue);
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.RemoveAgent(creature.gameObject, Reason);
			
			if (creature.Stats is IHasViewRadius hasViewRadius)
				hasViewRadius.ViewRadius.RemoveAgent(creature.gameObject, Reason);
        }
    }
}
