using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class IronSwordEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _strengthValue = new(5f, StatValueType.Plat);

        public string Reason => "Iron Sword equipment";
        public StatValue StrengthValue => _strengthValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.AddAgent(creature.gameObject, Reason, _strengthValue);
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.RemoveAgent(creature.gameObject, Reason);
        }
    }
}
