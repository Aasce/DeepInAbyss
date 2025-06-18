using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Equipments.Weapons
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class Weapon : MonoBehaviour
    {
        // Ref
        [SerializeField, HideInInspector] protected WeaponView _view;
        [SerializeField, HideInInspector] protected Collider2D _collider;
        [SerializeField, HideInInspector] protected Rigidbody2D _rigidBody;

        [SerializeField] protected SO_WeaponInformation _information;
        protected ICreature _owner;

        [Space]
        [SerializeField] protected Transform _tip;
        [SerializeField] protected HitBox _hitBox = new();

        protected AttackType _currentAttackType = AttackType.None;

        public WeaponView View => _view;
        public Collider2D Collider => _collider;
        public Rigidbody2D Rigidbody => _rigidBody;
        public HitBox HitBox => _hitBox;

        public SO_WeaponInformation Information => _information;
        public ICreature Owner
        {
            get => _owner;
            set
            {
                if (_owner == value) return;
                _owner = value;
            }
        }

        public WeaponType WeaponType => (Information != null) ? Information.WeaponType : WeaponType.None;
        public AttackType AttackType => (Information != null) ? Information.AttackType : AttackType.Swipe;
        public AttackType MeleeAttackType => (Information != null) ? Information.MeleeAttackType : AttackType.Swipe;

        public Vector2 TipPosition => (_tip == null) ? transform.position + transform.right : _tip.position;
        

        protected virtual void Reset()
        {
            if (this.LoadComponent(out _view))
            {
                View.Weapon = this;
            }
            this.LoadComponent(out _collider);
            this.LoadComponent(out _rigidBody);
        }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }


        public virtual void StartAttacking(AttackType attackType = AttackType.None)
        {
            _currentAttackType = attackType;
        }

        public virtual void Attacking()
        {
            Collider2D[] colliders = _hitBox.Hit(Owner.gameObject.transform.position, Owner.Status.FacingDirectionValue);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled) continue;
                if (!collider.TryGetComponent(out ICreature creature)) continue;
                if (Owner == creature) continue; // Ignore self

                this.DealingDamage(creature, collider.ClosestPoint(transform.position));
            }
        }

        public virtual void EndAttacking(AttackType attackType = AttackType.None)
        {
            _currentAttackType = AttackType.None;
        }

        public virtual void DealingDamage(ICreature creature, Vector2 position = default)
        {
            float damageScale = (Information != null) ? Information.MeleeDamageScale : 1f;
            CombatSystem.DamageDealing(new DamageContainer(Owner.Stats, creature.Stats)
            {
                Damage = Owner.Stats.Strength.Value * damageScale,
                DamageType = DamageType.Physical,
                Penetration = 0f,

                Position = position,
            });
        }
    }
}
