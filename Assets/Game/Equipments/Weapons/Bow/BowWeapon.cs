using Asce.Game.Combats;
using Asce.Manager.Sounds;
using UnityEngine;

namespace Asce.Game.Equipments.Weapons
{
    public class BowWeapon : WeaponObject
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

        public override void StartAttacking(AttackType attackType = AttackType.None)
        {
            if (Owner == null) return;
            base.StartAttacking(attackType);
            if (attackType == AttackType.Swipe)
                AudioManager.Instance.PlaySFX("Weapon Slash", Owner.gameObject.transform.position, 0.1f);
        }
    }
}
