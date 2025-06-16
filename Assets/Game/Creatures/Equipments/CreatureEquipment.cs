using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureEquipment : MonoBehaviour, IHasOwner<Creature>
    {
        [SerializeField, HideInInspector] private Creature _owner;

        public virtual Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        protected virtual void Reset()
        {

        }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            Owner.Status.OnDeath += Status_OnDeath;
        }

        protected virtual void Status_OnDeath(object sender)
        {

        }
    }
}