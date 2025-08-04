using Asce.Game.Entities;
using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class GoldenSwordEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _strengthValue = new(StatType.Strength, 10f, StatValueType.Flat);

        [Space]
        [SerializeField, Min(0f)] private float _sunderStrength = 0.05f;
        [SerializeField, Min(0f)] private float _sunderDuration = 10f;

        public string Reason => "Golden Sword equipment";
        public StatValue StrengthValue => _strengthValue;
        public float SunderStrength => _sunderStrength;
        public float SunderDuration => _sunderDuration;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
                hasStrength.Strength.AddAgent(creature.gameObject, Reason, _strengthValue);

            creature.OnBeforeSendDamage += Creature_OnBeforeSendDamage;
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
            if (creature.Stats is IHasStrength hasStrength)
                hasStrength.Strength.RemoveAgent(creature.gameObject, Reason);

            creature.OnBeforeSendDamage -= Creature_OnBeforeSendDamage;
        }

        private void Creature_OnBeforeSendDamage(object sender, Combats.DamageContainer args)
        {
            if (args == null) return;

            ICreature owner = sender as ICreature;
            ICreature creature = args.Receiver as ICreature;

            if (owner == null || creature == null) return;
            if (args.SourceType != Combats.DamageSourceType.Default) return;

            StatusEffectsManager.Instance.SendEffect<Sunder_StatusEffect>(owner, creature, new EffectDataContainer()
            {
                Strength = _sunderStrength,
                Duration = _sunderDuration
            });
        }

        public override string GetDescription(bool isPretty = false) => this.GenerateDescription(isPretty, _strengthValue);
    }
}
