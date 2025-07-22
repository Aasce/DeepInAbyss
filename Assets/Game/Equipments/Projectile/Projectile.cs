using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Equipments 
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        // Ref
        [SerializeField, Readonly] protected Rigidbody2D _rigidbody;
        [SerializeField, Readonly] protected Collider2D _collider;
        [SerializeField, Readonly] protected ProjectileView _view;
        protected ICreature _owner;

        [SerializeField] protected bool _isLaunched;
        [SerializeField] protected Cooldown _despawnCooldown = new(10f);
        protected bool _hasHit;

        [Space]
        [SerializeField] protected float _damage = 0f;
        [SerializeField] protected float _penetration = 0f;
        [SerializeField] protected DamageType _damageType = DamageType.Physical;

        [Space]
        [SerializeField] protected bool _isSetLayerOnLaunch = false;

        [Space]
        [SerializeField] protected bool _isSetLayerOnHit = false;

        public event Action<object> OnLaunch;
        public event Action<object, Collision2D> OnHit;


        public ICreature Owner
        {
            get => _owner;
            protected set => _owner = value;
        }

        public Rigidbody2D Rigidbody => _rigidbody;
        public Collider2D Collider => _collider;
        public ProjectileView View => _view;

        public virtual bool IsLaunched
        {
            get => _isLaunched;
            set
            {
                if (_isLaunched == value) return;
                _isLaunched = value;

                if (_isLaunched)
                {
                    _rigidbody.simulated = true;

                    OnLaunched();
                }
            }
        }

        public virtual bool HasHit
        {
            get => _hasHit;
            protected set
            {
                if (_hasHit == value) return;
                _hasHit = value;

                _rigidbody.gravityScale = 1.0f;

                // Set layer
                if (_isSetLayerOnHit)
                {

                }
            }
        }

        public Vector2 Velocity
        {
            get => _rigidbody.linearVelocity;
            set => _rigidbody.linearVelocity = value;
        }

        protected virtual void Reset()
        {
            this.LoadComponent(out _rigidbody);
            this.LoadComponent(out _collider);
            if (this.LoadComponent(out _view)) View.Owner = this;
        }

        protected virtual void Start()
        {
            if (!IsLaunched) _rigidbody.simulated = false;
        }


        protected virtual void Update()
        {
            if (IsLaunched == false) return;

            _despawnCooldown.Update(Time.deltaTime);
            if (_despawnCooldown.IsComplete)
            {
                Despawn();
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (HasHit) return;
            if (Owner != null && Owner.gameObject == collision.gameObject) return;

            HasHit = true;
            this.OnHitted(collision);
        }


        public virtual void OnAttach(ICreature creature)
        {
            Owner = creature;
            View.SortingLayer = Owner.View.SortingLayer;
            View.SortingOrder = Owner.View.SortingOrder - 1;
        }

        public virtual void OnDetach()
        {

        }

        protected virtual void OnLaunched()
        {
            // Events
            OnLaunch?.Invoke(this);
        }

        protected virtual void OnHitted(Collision2D collision)
        {
            OnHit?.Invoke(this, collision);
        }

        public virtual void SetDamage(float damage, float penetration = 0f, DamageType damageType = DamageType.Physical)
        {
            _damage = damage;
            _penetration = penetration;
            _damageType = damageType;
        }

        public virtual void Launching(float force)
        {
            transform.SetParent(null, true);
            transform.localScale = Vector3.one;
            IsLaunched = true;
            Collider.isTrigger = false;
            Rigidbody.linearVelocity = force * transform.right;
        }


        protected virtual void Despawn()
        {
            Destroy(gameObject);
        }

        protected virtual void DealDamageTo(IEntity target, Vector2 position)
        {
            if (target == null) return;
            if (target.Status.IsDead) return;

            CombatSystem.DamageDealing(new DamageContainer(Owner, target as ITakeDamageable)
            {
                Damage = _damage,
                DamageType = _damageType,
                Penetration = _penetration,

                Position = position,
            });
        }
    }
}