using Asce.Game.Entities;
using Asce.Game.VFXs;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class MagicProjectile : Projectile
    {
        [Header("Magic")]
        [SerializeField] protected VFXObject _launchFxPrefab;
        [SerializeField] protected VFXObject _hitFxPrefab;

        [Space]
        [SerializeField] protected float _explosionRadius = 1f;
        [SerializeField] protected LayerMask _explosionLayerMask;

        protected override void Start()
        {
            base.Start();
            VFXsManager.Instance.Register(_launchFxPrefab);
            VFXsManager.Instance.Register(_hitFxPrefab);
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
            if (_hitFxPrefab != null) VFXsManager.Instance.Spawn(_hitFxPrefab, transform.position, transform.rotation);
            _despawnCooldown.ToComplete();

            base.OnCollisionEnter2D(collision);
        }


        protected override void OnLaunched()
        {
            if (_launchFxPrefab != null) VFXsManager.Instance.Spawn(_launchFxPrefab, transform.position, transform.rotation);
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
                if (!collider.TryGetComponent(out IEntity entity)) continue;

                this.DealDamageTo(target: entity, entity.gameObject.transform.position);
            }
        }
    }
}