using Asce.Game.Combats;
using Asce.Game.Entities;
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
            Collider2D[] colliders = _stabHitBox.Hit(Owner.gameObject.transform.position, Owner.Status.FacingDirectionValue);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled) continue;
                if (!collider.TryGetComponent(out ICreature creature)) continue;
                if (Owner == creature) continue; // Ignore self

                this.DealingDamage(creature, collider.ClosestPoint(transform.position));
            }
        }
    }
}
