using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class EmeraldStaffEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _healScaleValue = new(0.1f, StatValueType.Plat);
        [SerializeField] private StatValue _resistanceValue = new(15f, StatValueType.Plat);

        public string AddHealScaleReason => "Emerald Staff add heal scale";
        public string AddResistanceReason => "Emerald Staff add resistance";

        public StatValue HealScaleValue => _healScaleValue;
        public StatValue ResistanceValue => _resistanceValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasHealth hasHealth)
                hasHealth.HealthGroup.HealScale.AddAgent(creature.gameObject, AddHealScaleReason, _healScaleValue);

            if (creature.Stats is IHasDefense hasDefense)
                hasDefense.DefenseGroup.Resistance.AddAgent(creature.gameObject, AddResistanceReason, _resistanceValue);
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasHealth hasHealth)
                hasHealth.HealthGroup.HealScale.RemoveAgent(creature.gameObject, AddHealScaleReason);

            if (creature.Stats is IHasDefense hasDefense)
                hasDefense.DefenseGroup.Resistance.RemoveAgent(creature.gameObject, AddResistanceReason);
        }
    }
}
