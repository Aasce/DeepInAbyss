using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Equipments.Weapons
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class WeaponObject : GameComponent
    {
        // Ref
        [SerializeField, Readonly] protected WeaponView _view;
        [SerializeField, Readonly] protected Collider2D _collider;
        [SerializeField, Readonly] protected Rigidbody2D _rigidBody;

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
            protected set
            {
                if (_owner == value) return;
                _owner = value;
            }
        }

        public WeaponType WeaponType => (Information != null) ? Information.WeaponType : WeaponType.None;
        public AttackType AttackType => (Information != null) ? Information.AttackType : AttackType.Swipe;
        public AttackType MeleeAttackType => (Information != null) ? Information.MeleeAttackType : AttackType.Swipe;

        public Vector2 TipPosition => (_tip == null) ? transform.position + transform.right : _tip.position;
        

        protected override void RefReset()
        {
            base.RefReset();
            if (this.LoadComponent(out _view)) View.Owner = this;
            this.LoadComponent(out _collider);
            this.LoadComponent(out _rigidBody);
        }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        public virtual void OnAttach(ICreature creature)
        {
            Owner = creature;
            View.Alpha = Owner.View.Alpha;

            Collider.isTrigger = true;
            Rigidbody.bodyType = RigidbodyType2D.Kinematic;

            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;
        }

        public virtual void OnDetach()
        {
            Collider.isTrigger = false;
            Rigidbody.bodyType = RigidbodyType2D.Dynamic;

            View.Alpha = 1.0f;
            Owner = null;
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
                if (!collider.TryGetComponent(out IEntity entity)) continue;
                if (Owner == entity) continue; // Ignore self

                this.DealingDamage(entity, collider.ClosestPoint(transform.position));
            }
        }

        public virtual void EndAttacking(AttackType attackType = AttackType.None)
        {
            _currentAttackType = AttackType.None;
        }

        public virtual void DealingDamage(IEntity entity, Vector2 position = default)
        {
            float damageScale = (Information != null) ? Information.MeleeDamageScale : 1f;
            CombatSystem.DamageDealing(new DamageContainer(Owner, entity as ITakeDamageable)
            {
                Damage = Owner.Stats.Strength.Value * damageScale,
                DamageType = DamageType.Physical,
                Penetration = 0f,

                Position = position,
            });
        }
    }
}
