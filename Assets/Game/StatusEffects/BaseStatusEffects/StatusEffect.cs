using Asce.Game.Entities;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    [System.Serializable]
    public abstract class StatusEffect : IStatusEffect
    {
        [SerializeField] SO_StatusEffectInformation _information;

        [Space]
        [SerializeField] protected Creature _sender;
        [SerializeField] protected Creature _target;

        [Space]
        [SerializeField] protected int _level = 1;
        [SerializeField] protected float _strength = 0f;
        [SerializeField] protected Cooldown _duration = new(1f);

        public abstract string Name { get; }
        public SO_StatusEffectInformation Information => _information;

        public Creature Sender => _sender;
        public Creature Target => _target;
        
        public int Level
        {
            get => _level;
            set => _level = value;
        }
        
        public Cooldown Duration => _duration;

        public StatusEffect() { }

        public abstract void Apply();
        public virtual void Tick(float deltaTime)
        {
            Duration.Update(deltaTime);
        }
        public abstract void Unapply();

        public void SetInformation(SO_StatusEffectInformation information)
        {
            if (information == null) return;
            _information = information;
        }
        public virtual void Set(Creature sender, Creature target, EffectDataContainer data)
        {
            this.SetSender(sender);
            this.SetTarget(target);
            if (data == null) return;

            _level = data.Level;
            _strength = data.Strength;
            _duration.SetBaseTime(data.Duration);
        }

        protected virtual void SetSender(Creature sender)
        {
            _sender = sender;
        }
        protected virtual void SetTarget(Creature target)
        {
            _target = target;
        }
    }
}
