using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class HelmEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _armorValue = new(StatType.Armor, 20f, StatValueType.Flat);
        [SerializeField] private StatValue _resistanceValue = new(StatType.Resistance, 25f, StatValueType.Flat);

        [Space]
        [SerializeField] private float _shieldScale = 0.5f;

        public string Reason => "Helm equipment";

        public StatValue ArmorValue => _armorValue;
        public StatValue ResistanceValue => _resistanceValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
			{
				hasDefense.DefenseGroup.Armor.AddAgent(creature.gameObject, Reason, _armorValue);
				hasDefense.DefenseGroup.Resistance.AddAgent(creature.gameObject, Reason, _resistanceValue);
            }

            creature.OnAfterTakeDamage += Creature_AfterTakeDamage;
            creature.OnAfterSendDamage += Creature_OnAfterSendDamage;
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

            creature.OnAfterTakeDamage -= Creature_AfterTakeDamage;
            creature.OnAfterSendDamage -= Creature_OnAfterSendDamage;
        }

        public override string GetDescription(bool isPretty = false) => this.GenerateDescription(isPretty, _armorValue, _resistanceValue);

        private void Creature_AfterTakeDamage(object sender, DamageContainer container)
        {
            ICreature creature = (ICreature)sender;
            if (container.SourceType == Combats.DamageSourceType.Falling) return;
            if (creature.Equipment is not IHasHeadSlot headSlot) return;

            headSlot.HeadSlot.DeductDurability(container.Damage * DeductDurabilityScale);
        }

        private void Creature_OnAfterSendDamage(object sender, DamageContainer container)
        {
            ICreature creature = (ICreature)sender;
            if (container.SourceType != Combats.DamageSourceType.Default) return;

            if (creature.Stats is not IHasDefense hasDefense) return;
            hasDefense.DefenseGroup.Shield.AddAgent(null, "Helm bonus", container.FinalDamage * _shieldScale);
        }

    }
}
