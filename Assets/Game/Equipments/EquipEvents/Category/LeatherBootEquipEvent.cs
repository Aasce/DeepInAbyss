using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class LeatherBootEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _armorValue = new(StatType.Armor, 5f, StatValueType.Flat);
        [SerializeField] private StatValue _speedValue = new(StatType.Speed, 0.5f, StatValueType.Flat);

        public string Reason => "Leather Boot equipment";

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

            creature.OnBeforeTakeDamage += Creature_OnBeforeTakeDamage;
            creature.OnAfterTakeDamage += Creature_AfterTakeDamage;
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
				hasDefense.DefenseGroup.Armor.RemoveAgent(creature.gameObject, Reason);
			
			if (creature.Stats is IHasSpeed hasSpeed)
				hasSpeed.Speed.RemoveAgent(creature.gameObject, Reason);

            creature.OnBeforeTakeDamage -= Creature_OnBeforeTakeDamage;
            creature.OnAfterTakeDamage -= Creature_AfterTakeDamage;
        }

        public override string GetDescription(bool isPretty = false) => this.GenerateDescription(isPretty, _armorValue, _speedValue);

        private void Creature_OnBeforeTakeDamage(object sender, DamageContainer container)
        {
            if (container.SourceType != Combats.DamageSourceType.Falling) return;
            container.Damage *= 0.85f;
        }

        private void Creature_AfterTakeDamage(object sender, DamageContainer container)
        {
            ICreature creature = (ICreature)sender;
            if (creature.Equipment is not IHasFeetsSlot feetsSlot) return;

            feetsSlot.FeetsSlot.DeductDurability(container.Damage * DeductDurabilityScale);
        }

    }
}
