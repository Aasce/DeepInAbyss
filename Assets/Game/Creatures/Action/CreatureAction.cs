using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class CreatureAction : MonoBehaviour, IHasOwner<Creature>, IMovable
    {
        [SerializeField] private Creature _owner;

        /// <summary>
        ///     Reference to the creature that owns this movement controller.
        /// </summary>
        public virtual Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }







    }
}
