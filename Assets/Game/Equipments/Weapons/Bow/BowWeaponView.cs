using UnityEngine;

namespace Asce.Game.Equipments.Weapons
{
    public class BowWeaponView : WeaponView, IViewController
    {
        [SerializeField] protected Renderer _stringRenderer;

        [Space]
        [SerializeField] protected Transform _rigString;
        protected Vector2 _stringPullPosition;
        protected Vector2 _stringOriginPosition;
        protected Vector2 _targetStringPosition;

        [SerializeField] protected bool _isStringPulled;
        private SecondOrderDynamics _secondOrderDynamics = new(4f, 0.3f, 5f);

        public new BowWeapon Weapon
        {
            get => base.Weapon as BowWeapon;
            set => base.Weapon = value;
        }

        public Renderer StringRenderer => _stringRenderer;

        public virtual bool IsStringPulled
        {
            get => _isStringPulled;
            set
            {
                if (_isStringPulled == value) return;
                _isStringPulled = value;
            }
        }

        public virtual Vector2 StringPullPosition
        {
            set => _stringPullPosition = Weapon.transform.InverseTransformPoint(value);
        }


        protected override void Start()
        {
            base.Start();
            _stringOriginPosition = _rigString.localPosition;
            _secondOrderDynamics.Reset(_stringOriginPosition);
        }

        protected virtual void Update()
        {
            _targetStringPosition = IsStringPulled ? _stringPullPosition : _stringOriginPosition;
            _rigString.transform.localPosition = _secondOrderDynamics.Update(_targetStringPosition, Time.deltaTime);
        }

        public virtual void UpdateStringPullPosition(Vector2 position)
        {
            if (!IsStringPulled) return;
            StringPullPosition = position;
        }

        protected override void ResetRendererList()
        {
            base.ResetRendererList();
            if (StringRenderer != null) Renderers.Add(StringRenderer);
        }
    }
}
