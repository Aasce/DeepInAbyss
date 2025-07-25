using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class RubyStaffEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _strengthValue = new(0.1f, StatValueType.Ratio);
        [SerializeField] private StatValue _armorValue = new(15f, StatValueType.Plat);

        public string AddStrengthReason => "Ruby Staff add strength";
        public string AddArmorReason => "Ruby Staff add armor";

        public StatValue StrengthValue => _strengthValue;
        public StatValue ArmorValue => _armorValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.AddAgent(creature.gameObject, AddStrengthReason, _strengthValue);

            if (creature.Stats is IHasDefense hasDefense)
                hasDefense.DefenseGroup.Armor.AddAgent(creature.gameObject, AddArmorReason, _armorValue);
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.RemoveAgent(creature.gameObject, AddStrengthReason);

            if (creature.Stats is IHasDefense hasDefense)
                hasDefense.DefenseGroup.Armor.RemoveAgent(creature.gameObject, AddArmorReason);
        }
    }
}
