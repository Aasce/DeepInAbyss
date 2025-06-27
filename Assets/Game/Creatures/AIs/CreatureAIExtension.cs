using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    public static class CreatureAIExtension
    {
        public static T FindTarget<T>(this Creature self, LayerMask targetLayer, LayerMask groundLayer) where T : ICreature
        {
            float viewDistance = self.Stats.ViewRadius.Value;
            LayerMask seeLayer = targetLayer | groundLayer;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(self.transform.position, viewDistance, targetLayer);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled) continue;
                if (collider.transform == self.transform) continue;
                if (!collider.TryGetComponent(out T target)) continue;
                if (target.Status.IsDead) continue;

                if (self.CanSeeTarget(target, seeLayer)) return target;
            }

            return default;
        }

        public static bool CanSeeTarget<T>(this Creature self, T target, LayerMask seeLayer) where T : ICreature
        {
            Vector2 selfPos = (Vector2)self.transform.position + Vector2.up * self.Status.Height * 0.5f;
            Vector2 targetPos = (Vector2)target.gameObject.transform.position + Vector2.up * target.Status.Height * 0.5f;
            Vector2 direction = targetPos - selfPos;

            RaycastHit2D hit = self.gameObject.Raycast(selfPos, direction.normalized, direction.magnitude, seeLayer);

            if (hit.collider == null) return false;
            return hit.collider.transform == target.gameObject.transform;
        }
    }
}