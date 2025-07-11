using Asce.Game.StatusEffects;
using Asce.Managers.Attributes;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureStatusEffect : MonoBehaviour, IHasOwner<Creature>
    {
        [SerializeField, Readonly] private Creature _owner;

        [Space]
        [SerializeField] protected StatusEffectController _effectsController = new();

        //public event Action<object, StatusEffect> OnApply;
        //public event Action<object, StatusEffect> OnUnapply;

        public StatusEffectController Controller => _effectsController;

        public Creature Owner
        {
            get => _owner;
            set => _owner = value;
        }

        protected virtual void Start()
        {
            if (Owner != null)
            {
                Owner.Status.OnDeath += Status_OnDeath;
            }
        }

        protected virtual void Update()
        {
            _effectsController.Update(Time.deltaTime);
        }

        public virtual void AddEffect(StatusEffect effect)
        {
            _effectsController.AddEffect(effect);
        }

        protected virtual void Status_OnDeath(object sender)
        {
            _effectsController.ClearEffect();
        }

    }
}