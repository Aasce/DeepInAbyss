using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class RubyStaffEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _strengthValue = new(0.1f, StatValueType.Ratio);
        [SerializeField] private StatValue _armorValue = new(15f, StatValueType.Plat);

        public string Reason => "Ruby Staff equipment";

        public StatValue StrengthValue => _strengthValue;
        public StatValue ArmorValue => _armorValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.AddAgent(creature.gameObject, Reason, _strengthValue);

            if (creature.Stats is IHasDefense hasDefense)
                hasDefense.DefenseGroup.Armor.AddAgent(creature.gameObject, Reason, _armorValue);
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.RemoveAgent(creature.gameObject, Reason);

            if (creature.Stats is IHasDefense hasDefense)
                hasDefense.DefenseGroup.Armor.RemoveAgent(creature.gameObject, Reason);
        }
    }
}
