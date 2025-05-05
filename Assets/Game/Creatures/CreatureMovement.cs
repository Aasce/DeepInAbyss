using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class CreatureMovement : MonoBehaviour, IMovable
    {
        [SerializeField] private Creature _owner;
        [SerializeField] private bool _isMovable = true;

        public bool IsMovable
        {
            get => _isMovable;
            set => _isMovable = value;
        }

        public virtual Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public abstract void Move(Vector2 direction);

        protected virtual void Update()
        {

        }
    }
}
