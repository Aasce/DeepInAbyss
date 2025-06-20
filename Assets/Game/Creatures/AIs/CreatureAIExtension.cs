using UnityEngine;

namespace Asce.Game.Entities.AIs
{
    public static class CreatureAIExtension
    {
        public static T FindTarget<T>(this Creature creature, LayerMask layer) where T : ICreature
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(creature.transform.position, creature.Stats.ViewRadius.Value, layer);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled) continue;
                if (collider.transform == creature.transform) continue;
                if (!collider.TryGetComponent(out T target)) continue;
                if (target.Status.IsDead) continue;

                return target;
            }

            return default;
        }
    }
}