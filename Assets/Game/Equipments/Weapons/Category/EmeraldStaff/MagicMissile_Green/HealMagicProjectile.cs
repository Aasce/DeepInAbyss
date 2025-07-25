using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class HealMagicProjectile : MagicProjectile
    {
        [Header("Heal Magic")]
        [SerializeField, Min(0f)] private float _healScale = 0.1f;

        protected override void Explosion(Vector2 position)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, _explosionRadius, _explosionLayerMask);
            float totalDamage = 0f;

            foreach (Collider2D collider in colliders)
            {
                if (collider == null) continue;
                if (!collider.enabled) continue;
                if (collider.gameObject == Owner.gameObject) continue;
                if (!collider.TryGetComponent(out IEntity entity)) continue;

                DamageContainer damageContainer = this.DealDamageTo(target: entity, entity.gameObject.transform.position);
                if (damageContainer == null) continue;
                totalDamage += damageContainer.FinalDamage;
            }

            this.Heal(totalDamage);
        }

        protected virtual void Heal(float amount)
        {
            if (Owner == null) return;
            if (amount <= 0f) return;
            if (Owner.Stats is IHasHealth hasHealth)
            {
                float healAmount = amount * _healScale;
                CombatSystem.Healing(Owner, hasHealth, healAmount, position: Owner.gameObject.transform.position);
            }
        }
    }
}
