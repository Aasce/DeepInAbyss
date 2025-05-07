using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Creature : Entity, IHasMovement<CreatureMovement>, IHasStats<CreatureStats, SO_CreatureBaseStats>
    {
        [SerializeField] private CreaturePhysicController _collider;
        [SerializeField] private CreatureMovement _movement;
        [SerializeField] private CreatureStats _stats;

        public CreaturePhysicController PhysicController => _collider;
        public CreatureMovement Movement => _movement;
        public CreatureStats Stats => _stats;

        protected override void Awake()
        {
            base.Awake();
            if (Movement != null) Movement.Owner = this;
            if (Stats != null) Stats.Owner = this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Stats != null) Stats.ResetStats();
        }
    }
}
