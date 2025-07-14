using Asce.Game.Combats;
using Asce.Game.Entities.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{
    public class Spider_Enemy : Enemy
    {
        [SerializeField] protected LayerMask _targetLayerMask;
        protected Collider2D[] _targetColliders;
        protected Character _target;

        [SerializeField] protected HitBox _damageHitBox = new();
        protected HashSet<GameObject> _damagedObject = new();

        public HitBox DamageHitBox => _damageHitBox;

        protected override void Update()
        {
            base.Update();
        }

        protected override void Action_OnAttack(object sender)
        {
            base.Action_OnAttack(sender);
            _damagedObject.Clear();

            Collider2D[] colliders = _damageHitBox.Hit(transform.position, Status.FacingDirectionValue);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled) continue;
                if (collider.transform == transform) continue; // Ignore self
                if (_damagedObject.Contains(collider.gameObject)) continue; // Avoid dealing damage to the same creature multiple times
                if (!collider.TryGetComponent(out ICreature creature)) continue;

                CombatSystem.DamageDealing(new DamageContainer(Stats, creature.Stats)
                {
                    Damage = Stats.Strength.Value,
                    DamageType = DamageType.Physical,
                });

                _damagedObject.Add(collider.gameObject);
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            DrawCircle(transform.position, Stats.ViewRadius.Value, 32);
        }

        void DrawCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * radius;

            for (int i = 1; i <= segments; i++)
            {
                float rad = Mathf.Deg2Rad * (i * angleStep);
                Vector3 nextPoint = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}
