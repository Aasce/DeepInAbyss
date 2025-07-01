using Asce.Game.Entities;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class MagicProjectile : Projectile
    {
        [Header("Magic")]
        [SerializeField] protected GameObject _launchFxPrefab;
        [SerializeField] protected GameObject _hitFxPrefab;

        [Space]
        [SerializeField] protected float _explosionRadius = 1f;
        [SerializeField] protected LayerMask _explosionLayerMask;

        protected override void Start()
        {
            base.Start();
            _rigidbody.gravityScale = 0f;
        }

        protected override void Update()
        {
            if (!IsLaunched) return;

            base.Update();
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if (HasHit) return;
            if (Owner != null && Owner.gameObject == collision.gameObject) return;

            this.Explosion((collision.contacts.Length > 0) ? collision.contacts[0].point : transform.position);
            if (_hitFxPrefab != null)
            {
                GameObject hitFx = Instantiate(_hitFxPrefab);
                hitFx.transform.SetPositionAndRotation(transform.position, transform.rotation);
            } 
            _despawnCooldown.ToComplete();

            base.OnCollisionEnter2D(collision);
        }


        protected override void OnLaunched()
        {
            if (_launchFxPrefab != null)
            {
                GameObject launchFx = Instantiate(_launchFxPrefab);
                launchFx.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
            base.OnLaunched();
        }

        protected virtual void Explosion(Vector2 position)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, _explosionRadius, _explosionLayerMask);

            foreach (Collider2D collider in colliders)
            {
                if (collider ==  null) continue;
                if (!collider.enabled) continue;
                if (collider.gameObject == Owner.gameObject) continue;
                if (!collider.TryGetComponent(out ICreature creature)) continue;

                this.DealDamage(creature, creature.gameObject.transform.position);
            }
        }
    }
}