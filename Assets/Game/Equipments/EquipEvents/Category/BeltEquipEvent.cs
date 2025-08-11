using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class BeltEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _armorValue = new(StatType.Armor, 10f, StatValueType.Flat);
        public string Reason => "Belt equipment";
        public StatValue ArmorValue => _armorValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
				hasDefense.DefenseGroup.Armor.AddAgent(creature.gameObject, Reason, _armorValue);

            creature.OnAfterTakeDamage += Creature_AfterTakeDamage;
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
				hasDefense.DefenseGroup.Armor.RemoveAgent(creature.gameObject, Reason);

            creature.OnAfterTakeDamage += Creature_AfterTakeDamage;
        }

        public override string GetDescription(bool isPretty = false) => this.GenerateDescription(isPretty, _armorValue);

        private void Creature_AfterTakeDamage(object sender, DamageContainer container)
        {
            ICreature creature = (ICreature)sender;
            if (container.SourceType == Combats.DamageSourceType.Falling) return;
            if (creature.Equipment is not IHasLegsSlot legsSlot) return;

            legsSlot.LegsSlot.DeductDurability(container.Damage * DeductDurabilityScale);
        }

    }
}
