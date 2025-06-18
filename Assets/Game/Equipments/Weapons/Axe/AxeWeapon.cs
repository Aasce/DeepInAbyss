using Asce.Game.Combats;
using Asce.Game.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Equipments.Weapons
{
    public class AxeWeapon : Weapon
    {
        [SerializeField] protected HitBox _bladeHitBox = new();
        [SerializeField] protected float _bladeDamageScale = 2f; // Scale for critical damage
        [SerializeField] protected float _bladePenetration = 10f; // Scale for critical damage

        protected HashSet<ICreature> _hitCreatures = new();

        public HitBox BladeHitBox => _bladeHitBox;


        public override void Attacking()
        {
            Collider2D[] colliders = _bladeHitBox.Hit(Owner.gameObject.transform.position, Owner.Status.FacingDirectionValue);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled) continue;
                if (!collider.TryGetComponent(out ICreature creature)) continue;
                if (Owner == creature) continue; // Ignore self

                this.DealingCritialDamage(creature, collider.ClosestPoint(transform.position));
            }

            base.Attacking(); // Calculate colliders for the main hitbox
        }

        public override void DealingDamage(ICreature creature, Vector2 position = default)
        {
            if (_hitCreatures.Contains(creature)) return; // Prevent hitting the same creature multiple times
            base.DealingDamage(creature, position);
        }

        public virtual void DealingCritialDamage(ICreature creature, Vector2 position = default)
        {
            float damageScale = (Information != null) ? Information.MeleeDamageScale : 1f;
            CombatSystem.DamageDealing(new DamageContainer(Owner.Stats, creature.Stats)
            {
                Damage = Owner.Stats.Strength.Value * damageScale * _bladeDamageScale,
                DamageType = DamageType.Physical,
                Penetration = _bladePenetration,

                Position = position,
            });
            _hitCreatures.Add(creature);
        }

        public override void EndAttacking(AttackType attackType = AttackType.None)
        {
            base.EndAttacking(attackType);
            _hitCreatures.Clear();
        }
    }
}
