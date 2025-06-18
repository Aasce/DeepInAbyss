using Asce.Game.Combats;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{
    public class Slime_Enemy : Enemy
    {
        [SerializeField] protected LayerMask _targetLayerMask;
        protected Collider2D[] _targetColliders;
        protected Character _target;

        [SerializeField] protected HitBox _damageHitBox = new();
        protected HashSet<GameObject> _damagedObject = new ();

        public HitBox DamageHitBox => _damageHitBox;

        protected override void Update()
        {
            base.Update();
            if (IsControled) return;
            if (Status.IsDead) return;

            _targetColliders = Physics2D.OverlapCircleAll(transform.position, Stats.ViewRadius.Value, _targetLayerMask);

            _target = null;
            foreach (Collider2D collider in _targetColliders)
            {
                if (!collider.enabled) continue;
                if (collider.transform ==  transform) continue;
                if (!collider.TryGetComponent(out Character character)) continue;

                _target = character;
                break;
            }

            if (_target != null && _target.Status.IsAlive)
            {
                Vector2 deltaPosition = _target.transform.position - transform.position;
                Action.Looking(true, _target.transform.position);
                
                bool isRun = deltaPosition.magnitude > 3f;
                if (deltaPosition.magnitude <= 1.5f)
                {
                    Action.Attacking(true);
                    Action.Moving(Vector2.zero);
                }
                else Action.Moving(new Vector2(deltaPosition.x, 0f));
                Action.Running(isRun);
            }
            else
            {
                Action.Looking(false);
                Action.Attacking(false);
                Action.Moving(Vector2.zero);
                Action.Running(false);
            }
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
