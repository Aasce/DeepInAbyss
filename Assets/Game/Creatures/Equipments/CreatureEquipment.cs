using Asce.Game.Equipments;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureEquipment : MonoBehaviour, IHasOwner<Creature>, IEquipmentController
    {
        [SerializeField, HideInInspector] private Creature _owner;

        public virtual Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }
        ICreature IEquipmentController.Owner => Owner;

        protected virtual void Reset()
        {
            if (transform.LoadComponent(out _owner))
            {
                Owner.Equipment = this;
            }
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