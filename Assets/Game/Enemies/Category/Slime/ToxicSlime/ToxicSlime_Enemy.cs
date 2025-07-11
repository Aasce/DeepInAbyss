using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{
    public class ToxicSlime_Enemy : Slime_Enemy
    {
        protected override void Start()
        {
            base.Start();

            Stats.OnBeforeSendDamage += Stats_OnBeforeSendDamage;
            Stats.OnAfterSendDamage += Stats_OnAfterSendDamage;
        }

        protected virtual void Stats_OnBeforeSendDamage(object sender, Combats.DamageContainer args)
        {
            Creature creature = args.Receiver.Owner;
            if (creature == null) return;
            if (creature.StatusEffect == null) return;

            StatusEffectsManager.Instance.SendEffect<Decay_StatusEffect>(this, creature, new()
            {
                Strength = 1 - (90 / (90 + Stats.Strength.Value * 0.1f)),
                Duration = 20f
            });
        }

        protected virtual void Stats_OnAfterSendDamage(object sender, Combats.DamageContainer args)
        {
            Creature creature = args.Receiver.Owner;
            if (creature == null) return;
            if (creature.StatusEffect == null) return;

            // StatusEffectsManager.Instance.SendEffect("Freeze", this, creature, new EffectDataContainer(1.5f, 0.5f));
        }
    }
}
