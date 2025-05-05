using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Creature : Entity, IHasMovement<CreatureMovement>
    {
        [SerializeField] private CreaturePhysicController _collider;
        [SerializeField] private CreatureMovement _movement;

        public CreaturePhysicController PhysicController => _collider;
        public CreatureMovement Movement => _movement;

        protected virtual void Awake()
        {
            if (Movement != null) Movement.Owner = this;
        }

    }
}
