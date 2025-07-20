using Asce.Game.Stats;
using UnityEngine;


namespace Asce.Game.Combats
{ 
    [System.Serializable]
    public class DamageContainer
    {
        [SerializeField] private ISendDamageable _sender;
        [SerializeField] private ITakeDamageable _receiver;

        [Space]
        [SerializeField] private float _damage;
        [SerializeField] private DamageType _damageType;
        [SerializeField] private float _finalDamage;

        [Space]
        [SerializeField] private float _penetration;
        [SerializeField] private StatValueType _penetrationType;

        [Space]
        [SerializeField] private Vector2 _position;
        [SerializeField] private DamageSourceType _sourceType = DamageSourceType.Default;

        public ISendDamageable Sender
        {
            get => _sender;
            set => _sender = value;
        }
        public ITakeDamageable Receiver
        {
            get => _receiver;
            set => _receiver = value;
        }

        public float Damage
        {
            get => _damage;
            set => _damage = value;
        }

        public DamageType DamageType
        {
            get => _damageType;
            set => _damageType = value;
        }

        public float FinalDamage
        {
            get => _finalDamage;
            set => _finalDamage = value;
        }

        public float Penetration
        {
            get => _penetration;
            set => _penetration = value;
        }

        public StatValueType PenetrationType
        {
            get => _penetrationType;
            set => _penetrationType = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public DamageSourceType SourceType
        {
            get => _sourceType;
            set => _sourceType = value;
        }

        public DamageContainer() 
            : this(null, null, 0f, DamageType.TrueDamage, 0f, StatValueType.Plat, default) { }
        public DamageContainer(ISendDamageable sender, ITakeDamageable receiver) 
            : this(sender, receiver, 0f, DamageType.TrueDamage, default) { }
        public DamageContainer(ISendDamageable sender, ITakeDamageable receiver, float damageValue, float penetration = 0f, Vector2 position = default) 
            : this(sender, receiver, damageValue, DamageType.TrueDamage, penetration, StatValueType.Plat, position) { }

        public DamageContainer(ISendDamageable sender, ITakeDamageable receiver, float damageValue, DamageType type, float penetration = 0f, StatValueType penetrationType = StatValueType.Plat, Vector2 position = default)
        {
            _sender = sender;
            _receiver = receiver;
            _damage = damageValue;
            _damageType = type;

            _penetration = penetration;
            _penetrationType = penetrationType;

            if (position != default) _position = position;
            else if (_receiver != null) _position = _receiver.gameObject.transform.position;
            else _position = Vector2.zero;
        }
    }
}