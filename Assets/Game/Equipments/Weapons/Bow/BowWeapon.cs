using UnityEngine;

namespace Asce.Game.Equipments
{
    public class BowWeapon : Weapon
    {
        // Ref
        [SerializeField] protected ArrowProjectile _arrowPrefab;

        [Space]
        [SerializeField] protected float _force = 15f;

        public new BowWeaponView View => base.View as BowWeaponView;
        public ArrowProjectile ArrowPrefab => _arrowPrefab;

        public float Force
        {
            get => _force;
            set => _force = value;
        }


        protected override void Reset()
        {
            base.Reset();
            WeaponType = WeaponType.Bow;
            AttackType = Combats.AttackType.Archery;
        }

        protected override void Start()
        {
            base.Start();
        }

        public void Launch(Projectile projectile)
        {
            if (projectile == null) return;
            projectile.Launching(Force);
        }
    }
}
