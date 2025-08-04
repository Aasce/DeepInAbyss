using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class EmeraldStaffEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _healScaleValue = new(StatType.HealthScale, 0.1f, StatValueType.Flat);
        [SerializeField] private StatValue _resistanceValue = new(StatType.Resistance, 15f, StatValueType.Flat);

        public string Reason => "Emerald Staff equipment";

        public StatValue HealScaleValue => _healScaleValue;
        public StatValue ResistanceValue => _resistanceValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasHealth hasHealth)
                hasHealth.HealthGroup.HealScale.AddAgent(creature.gameObject, Reason, _healScaleValue);

            if (creature.Stats is IHasDefense hasDefense)
                hasDefense.DefenseGroup.Resistance.AddAgent(creature.gameObject, Reason, _resistanceValue);
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasHealth hasHealth)
                hasHealth.HealthGroup.HealScale.RemoveAgent(creature.gameObject, Reason);

            if (creature.Stats is IHasDefense hasDefense)
                hasDefense.DefenseGroup.Resistance.RemoveAgent(creature.gameObject, Reason);
        }

        public override string GetDescription(bool isPretty = false) => this.GenerateDescription(isPretty, _healScaleValue, _resistanceValue);
    }
}
