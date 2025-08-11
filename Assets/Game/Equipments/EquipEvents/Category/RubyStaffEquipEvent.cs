using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class RubyStaffEquipEvent : EquipEvent
    {
        [SerializeField] private float DeductDurability = 10f;

        [Space]
        [SerializeField] private StatValue _strengthValue = new(StatType.Strength, 0.1f, StatValueType.Ratio);
        [SerializeField] private StatValue _armorValue = new(StatType.Armor, 15f, StatValueType.Flat);

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

            creature.OnAfterSendDamage += Creature_OnAfterSendDamage;
            if (creature.Action is IAttackable attackable)
            {
                attackable.OnAttackEnd += Creature_OnAttackEnd;
            }
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.RemoveAgent(creature.gameObject, Reason);

            if (creature.Stats is IHasDefense hasDefense)
                hasDefense.DefenseGroup.Armor.RemoveAgent(creature.gameObject, Reason);

            creature.OnAfterSendDamage -= Creature_OnAfterSendDamage;
            if (creature.Action is IAttackable attackable)
            {
                attackable.OnAttackEnd -= Creature_OnAttackEnd;
            }
        }

        public override string GetDescription(bool isPretty = false) => this.GenerateDescription(isPretty, _strengthValue, _armorValue);

        private void Creature_OnAfterSendDamage(object sender, Combats.DamageContainer container)
        {
            ICreature creature = (ICreature)sender;
            if (container.SourceType != Combats.DamageSourceType.Default) return;
            if (creature.Equipment is not IHasWeaponSlot weaponSlot) return;

            weaponSlot.WeaponSlot.DeductDurability(container.Damage * DeductDurabilityScale);
        }

        private void Creature_OnAttackEnd(object sender, AttackEventArgs args)
        {
            ICreature creature = (ICreature)sender;
            if (creature.Equipment is not IHasWeaponSlot weaponSlot) return;

            weaponSlot.WeaponSlot.DeductDurability(DeductDurability * DeductDurabilityScale);
        }
    }
}
