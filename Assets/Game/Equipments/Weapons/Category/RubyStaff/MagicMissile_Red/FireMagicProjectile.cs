using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class FireMagicProjectile : MagicProjectile
    {
        [Header("Fire Magic")]
        [SerializeField, Min(0f)] private float _igniteStrength = 5f;
        [SerializeField, Min(0f)] private float _igniteDuration = 3f;

        protected override void DealDamage(ICreature target, Vector2 position)
        {
            if (target == null) return;
            if (target.Status.IsDead) return;
            base.DealDamage(target, position);
            StatusEffectsManager.Instance.SendEffect<Ignite_StatusEffect>(Owner, target, new EffectDataContainer()
            {
                Strength = _igniteStrength,
                Duration = _igniteDuration
            });
        }
    }
}
