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

        protected override DamageContainer DealDamageTo(IEntity target, Vector2 position)
        {
            DamageContainer damageContainer = base.DealDamageTo(target, position);
            if (damageContainer == null) return null;

            StatusEffectsManager.Instance.SendEffect<Ignite_StatusEffect>(Owner, target as ICreature, new EffectDataContainer()
            {
                Strength = _igniteStrength,
                Duration = _igniteDuration
            });

            return damageContainer;
        }
    }
}
