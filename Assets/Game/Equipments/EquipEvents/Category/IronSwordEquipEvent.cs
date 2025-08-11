using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class IronSwordEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _strengthValue = new(StatType.Strength, 5f, StatValueType.Flat);

        public string Reason => "Iron Sword equipment";
        public StatValue StrengthValue => _strengthValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.AddAgent(creature.gameObject, Reason, _strengthValue);

            creature.OnAfterSendDamage += Creature_OnAfterSendDamage;
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
				hasStrength.Strength.RemoveAgent(creature.gameObject, Reason);

            creature.OnAfterSendDamage -= Creature_OnAfterSendDamage;
        }

        public override string GetDescription(bool isPretty = false) => this.GenerateDescription(isPretty, _strengthValue);

        private void Creature_OnAfterSendDamage(object sender, Combats.DamageContainer container)
        {
            ICreature creature = (ICreature)sender;
            if (container.SourceType != Combats.DamageSourceType.Default) return;
            if (creature.Equipment is not IHasWeaponSlot weaponSlot) return;

            weaponSlot.WeaponSlot.DeductDurability(container.Damage * DeductDurabilityScale);
        }

    }
}
