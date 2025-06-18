using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Equipments 
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        // Ref
        [SerializeField, HideInInspector] protected Rigidbody2D _rigidbody;
        [SerializeField, HideInInspector] protected Collider2D _collider;
        protected ICreature _owner;

        [SerializeField] protected bool _isLaunched;
        [SerializeField] protected Cooldown _despawnCooldown = new(10f);
        protected bool _hasHit;

        [Space]
        [SerializeField] protected float _damage = 0f;
        [SerializeField] protected float _penetration = 0f;
        [SerializeField] protected DamageType _damageType = DamageType.Physical;

        [Space]
        [SerializeField] protected bool _isSetZPositionOnLaunch = false;
        [SerializeField] protected bool _isZPostionLaunchLocal = true;
        [SerializeField] protected float _zPositionLaunch = 0.0f;

        [Space]
        [SerializeField] protected bool setZPosOnHit = false;
        [SerializeField] protected bool isZPosHitLocal = true;
        [SerializeField] protected float zPosHit = 0.0f;

        public event Action<object> OnLaunch;
        public event Action<object, Collision2D> OnHit;


        public ICreature Owner
        {
            get => _owner;
            set
            {
                _owner = value;
            }
        }

        public Rigidbody2D Rigidbody => _rigidbody;
        public Collider2D Collider => _collider;

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

                    // Set launch z pos
                    if (_isSetZPositionOnLaunch)
                    {
                        var pos = _isZPostionLaunchLocal ? transform.localPosition : transform.position;
                        pos.z = _zPositionLaunch;

                        if (_isZPostionLaunchLocal) transform.localPosition = pos;
                        else transform.position = pos;
                    }

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

                //set hit z pos
                if (setZPosOnHit)
                {
                    var pos = isZPosHitLocal ? transform.localPosition : transform.position;
                    pos.z = zPosHit;

                    if (isZPosHitLocal) transform.localPosition = pos;
                    else transform.position = pos;
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

            if (collision.gameObject.TryGetComponent(out ICreature creature))
            {
                Vector2 position = collision.contactCount > 0 ? collision.GetContact(0).point : (Vector2)collision.transform.position;
                this.DealDamage(creature, position);
            }

            HasHit = true;
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

        protected virtual void OnLaunched()
        {
            // Events
            OnLaunch?.Invoke(this);
        }

        protected virtual void Despawn()
        {
            Destroy(gameObject);
        }

        protected virtual void DealDamage(ICreature target, Vector2 position)
        {
            if (target == null) return;
            if (target.Status.IsDead) return;

            CombatSystem.DamageDealing(new DamageContainer(Owner.Stats, target.Stats)
            {
                Damage = _damage,
                DamageType = _damageType,
                Penetration = _penetration,

                Position = position,
            });
        }
    }
}