using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class WoodenBowEquipEvent : EquipEvent
    {
        [SerializeField] private float DeductDurability = 10f;

        [Space]
        [SerializeField] private StatValue _strengthValue = new(StatType.Strength, 10f, StatValueType.Flat);
        [SerializeField] private StatValue _viewRadiusValue = new(StatType.ViewRadius, 6f, StatValueType.Flat);

        public string Reason => "Wooden Bow equipment";

        public StatValue StrengthValue => _strengthValue;
        public StatValue ViewRadiusValue => _viewRadiusValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.AddAgent(creature.gameObject, Reason, _strengthValue);
			
			if (creature.Stats is IHasViewRadius hasViewRadius)
				hasViewRadius.ViewRadius.AddAgent(creature.gameObject, Reason, _viewRadiusValue);

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
			
			if (creature.Stats is IHasViewRadius hasViewRadius)
				hasViewRadius.ViewRadius.RemoveAgent(creature.gameObject, Reason);

            creature.OnAfterSendDamage -= Creature_OnAfterSendDamage;
            if (creature.Action is IAttackable attackable)
            {
                attackable.OnAttackEnd -= Creature_OnAttackEnd;
            }
        }

        public override string GetDescription(bool isPretty = false) => this.GenerateDescription(isPretty, _strengthValue, _viewRadiusValue);

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
