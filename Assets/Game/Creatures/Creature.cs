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

        protected virtual void Awake()
        {
            if (Movement != null) Movement.Owner = this;
        }

    }
}
