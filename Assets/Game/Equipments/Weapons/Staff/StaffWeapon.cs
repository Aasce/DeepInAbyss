using Asce.Game.Combats;
using Asce.Manager.Sounds;
using UnityEngine;

namespace Asce.Game.Equipments.Weapons
{
    public class StaffWeapon : WeaponObject
    {
        [Header("Staff")]
        [SerializeField] protected MagicProjectile _magicPrefab;

        [Space]
        [SerializeField] protected float _speed = 15f;
        [SerializeField] protected float _damage = 15f;
        [SerializeField] protected float _penetration = 0f;

        public MagicProjectile MagicPrefab => _magicPrefab;

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        protected override void Reset()
        {
            base.Reset();
        }

        public void Cast(Vector2 position, Vector2 direction)
        {
            if (MagicPrefab == null) return;

            MagicProjectile projectile = Instantiate(MagicPrefab, null);
            projectile.OnAttach(Owner);
            projectile.transform.position = position;
            projectile.transform.right = direction;

            projectile.SetDamage(_damage, _penetration, Combats.DamageType.Magical);

            projectile.Launching(Speed);
            AudioManager.Instance.PlaySFX("Magic Projectile Launch", position);
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