using UnityEngine;

namespace Asce.Game.Equipments
{
    public class MagicProjectile : Projectile
    {
        [Header("Magic")]
        [SerializeField] protected GameObject _launchFxPrefab;
        [SerializeField] protected GameObject _hitFxPrefab;

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
            if (Owner != null && Owner == collision.gameObject) return;

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
    }
}