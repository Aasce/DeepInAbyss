using UnityEngine;

namespace Asce.Game.Equipments
{
    public class OtherSlot : EquipmentSlot, IEquipmentSlot
    {
        [SerializeField] protected Projectile _projectile;
        [SerializeField] protected bool _isProjectileReady = false;


        public Projectile Projectile
        {
            get => _projectile;
            set => _projectile = value;
        }

        public bool IsProjectileReady
        {
            get => _isProjectileReady;
            set => _isProjectileReady = value;
        }


        public void HoldProjectile(Projectile projectile)
        {
            this.DetachProjectile(); // Detach any existing projectile before attaching a new one
            if (projectile == null) return;

            Projectile = projectile;

            Projectile.transform.SetParent(transform);
            Projectile.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, 90.0f));
            Projectile.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Projectile.Rigidbody.simulated = false;
            Projectile.Collider.isTrigger = true;
        }

        public virtual void DetachProjectile()
        {
            if (Projectile == null) return;

            Projectile.transform.SetParent(null);
            Projectile.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            Projectile.Rigidbody.simulated = true;
            Projectile.Collider.isTrigger = false;

            Projectile = null;
        }

        public virtual void DestroyProjectile()
        {
            if (Projectile != null)
            {
                Destroy(Projectile.gameObject);
                Projectile = null;
            }
        }
    }
}