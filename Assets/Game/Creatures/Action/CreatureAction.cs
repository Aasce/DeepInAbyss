using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public abstract class CreatureAction : MonoBehaviour, IHasOwner<Creature>
    {
        [SerializeField, HideInInspector] private Creature _owner;

        /// <summary>
        ///     Reference to the creature that owns this movement controller.
        /// </summary>
        public virtual Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        protected virtual void Reset()
        {
            if (transform.LoadComponent(out _owner))
            {
                Owner.Action = this;
            }
        }

        protected virtual void Awake()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

            this.UpdateFacing();
        }

        public virtual void PhysicUpdate(float deltaTime)
        {

        }

        protected virtual void UpdateFacing()
        {

        }
    }
}
