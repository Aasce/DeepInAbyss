using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Manager.Sounds;
using UnityEngine;

namespace Asce.Game.Equipments.Weapons
{
    public class SwordWeapon : WeaponObject
    {
        [SerializeField] protected HitBox _stabHitBox = new ();

        public HitBox StabHitBox => _stabHitBox;

        public override void Attacking()
        {
            if (_currentAttackType == AttackType.Stab)
                this.Stabing();
            else base.Attacking();
        }

        protected virtual void Stabing()
        {
            if (Owner == null) return;
            Collider2D[] colliders = _stabHitBox.Hit(Owner.gameObject.transform.position, Owner.Status.FacingDirectionValue);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled) continue;
                if (!collider.TryGetComponent(out ICreature creature)) continue;
                if (Owner == creature) continue; // Ignore self

                this.DealingDamage(creature, collider.ClosestPoint(transform.position));
            }
        }

        public override void StartAttacking(AttackType attackType = AttackType.None)
        {
            if (Owner == null) return;
            base.StartAttacking(attackType);
            AudioManager.Instance.PlaySFX("Weapon Slash", Owner.gameObject.transform.position, 0.1f);
        }
    }
}
