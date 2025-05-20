using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Creature : Entity, IHasView<CreatureView>, IHasMovement<CreatureAction>, IHasStats<CreatureStats, SO_CreatureBaseStats>
    {
        [SerializeField] private CreaturePhysicController _collider;
        [SerializeField] private CreatureView _view;
        [SerializeField] private CreatureAction _movement;
        [SerializeField] private CreatureStats _stats;

        [Space]
        [SerializeField] private bool _isDead;

        public event Action<object> OnDead;


        public CreaturePhysicController PhysicController => _collider;
        public CreatureView View => _view;
        public CreatureAction Movement => _movement;
        public CreatureStats Stats => _stats;

        public bool IsDead
        {
            get => _isDead;
            set
            {
                if (!_isDead && value == true)
                {
                    _isDead = true;
                    OnDead?.Invoke(this);
                    return;
                }
                _isDead = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (PhysicController != null) PhysicController.Owner = this;
            if (Movement != null) Movement.Owner = this;
            if (Stats != null) Stats.Owner = this;
            if (View != null) View.Owner = this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Stats != null) Stats.ResetStats();
        }

    }
}
