using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class Creature : Entity, IMovable
    {
        [SerializeField] private CreatureColliderController _collider;

        public bool IsMovable => true;

        public CreatureColliderController Collider => _collider;

        public abstract void Move(Vector2 direction);
    }
}
