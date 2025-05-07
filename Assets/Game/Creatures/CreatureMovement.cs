using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class CreatureMovement : MonoBehaviour, IHasOwner<Creature>, IMovable
    {
        [SerializeField] private Creature _owner;
        [SerializeField] private bool _isMovable = true;

        /// <summary>
        ///     Reference to the creature that owns this movement controller.
        /// </summary>
        public virtual Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        /// <summary>
        ///     Can be moving if this true.
        /// </summary>
        public bool IsMovable
        {
            get => _isMovable;
            set => _isMovable = value;
        }

        public abstract void Move(Vector2 direction);

        protected virtual void Update()
        {

        }
    }
}
