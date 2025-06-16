using Asce.Game.Combats;
using Asce.Game.Entities;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Equipments
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class Weapon : MonoBehaviour
    {
        // Ref
        [SerializeField, HideInInspector] protected WeaponView _view;
        [SerializeField, HideInInspector] protected Collider2D _collider;
        [SerializeField, HideInInspector] protected Rigidbody2D _rigidBody;
        protected ICreature _owner;

        [SerializeField] protected Transform _tip;

        [Space]
        [SerializeField] protected WeaponType _weaponType = WeaponType.Sword;
        [SerializeField] protected AttackType _attackType = AttackType.None;
        [SerializeField] protected AttackType _meleeAttackType = AttackType.Swipe;


        public WeaponView View => _view;
        public Collider2D Collider => _collider;
        public Rigidbody2D Rigidbody => _rigidBody;
        public ICreature Owner
        {
            get => _owner;
            set
            {
                if (_owner == value) return;
                _owner = value;
            }
        }

        public WeaponType WeaponType
        {
            get => _weaponType;
            protected set => _weaponType = value;
        }
        public AttackType AttackType
        {
            get => _attackType;
            protected set => _attackType = value;
        }
        public AttackType MeleeAttackType
        {
            get => _meleeAttackType;
            protected set => _meleeAttackType = value;
        }
        public Vector2 TipPosition
        {
            get => (_tip == null) ? transform.position + transform.right : _tip.position;
        }

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
    }
}
