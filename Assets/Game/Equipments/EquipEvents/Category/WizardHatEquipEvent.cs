using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class WizardHatEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _armorValue = new(StatType.Armor, 5f, StatValueType.Flat);
        [SerializeField] private StatValue _resistanceValue = new(StatType.Resistance, 10f, StatValueType.Flat);
        [SerializeField] private StatValue _viewRadiusValue = new(StatType.ViewRadius, 5f, StatValueType.Flat);

        public string Reason => "Wizard Hat equipment";

        public StatValue ArmorValue => _armorValue;
        public StatValue ResistanceValue => _resistanceValue;
        public StatValue ViewRadiusValue => _viewRadiusValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
			{
				hasDefense.DefenseGroup.Armor.AddAgent(creature.gameObject, Reason, _armorValue);
				hasDefense.DefenseGroup.Resistance.AddAgent(creature.gameObject, Reason, _resistanceValue);
			}
			
			if (creature.Stats is IHasViewRadius hasViewRadius)
				hasViewRadius.ViewRadius.AddAgent(creature.gameObject, Reason, _viewRadiusValue);

            creature.OnAfterTakeDamage += Creature_AfterTakeDamage;
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
			
			if (creature.Stats is IHasViewRadius hasViewRadius)
				hasViewRadius.ViewRadius.RemoveAgent(creature.gameObject, Reason);

            creature.OnAfterTakeDamage -= Creature_AfterTakeDamage;
        }

        public override string GetDescription(bool isPretty = false) => this.GenerateDescription(isPretty, _armorValue, _resistanceValue, _viewRadiusValue);

        private void Creature_AfterTakeDamage(object sender, DamageContainer container)
        {
            ICreature creature = (ICreature)sender;
            if (container.SourceType == Combats.DamageSourceType.Falling) return;
            if (creature.Equipment is not IHasHeadSlot headSlot) return;

            headSlot.HeadSlot.DeductDurability(container.Damage * DeductDurabilityScale);
        }

    }
}
