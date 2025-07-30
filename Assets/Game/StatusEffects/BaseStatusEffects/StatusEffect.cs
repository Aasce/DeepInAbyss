using Asce.Game.Entities;
using Asce.Game.Stats;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    [System.Serializable]
    public abstract class StatusEffect : IStatusEffect
    {
        [SerializeField] SO_StatusEffectInformation _information;

        [Space]
        [SerializeField] protected ICreature _sender;
        [SerializeField] protected ICreature _target;

        [Space]
        [SerializeField] protected int _level = 1;
        [SerializeField] protected float _strength = 0f;
        [SerializeField] protected Cooldown _duration = new(1f);

        protected Dictionary<string, StatAgent> _agents = new();

        public abstract string Name { get; }
        public SO_StatusEffectInformation Information => _information;

        public ICreature Sender => _sender;
        public ICreature Target => _target;

        public GameObject Author => Sender == null ? null : Sender.gameObject;

        public int Level
        {
            get => _level;
            set => _level = value;
        }
        public float Strength
        {
            get => _strength;
            set => _strength = value;
        }        
        public Cooldown Duration => _duration;
        public Dictionary<string, StatAgent> Agents => _agents;

        public StatusEffect() { }

        public abstract void Apply();
        public virtual void Tick(float deltaTime)
        {
            Duration.Update(deltaTime);
        }
        public abstract void Unapply();

        public virtual void SetInformation(SO_StatusEffectInformation information)
        {
            if (information == null) return;
            _information = information;
        }
        public virtual void Set(ICreature sender, ICreature target, EffectDataContainer data)
        {
            this.SetSender(sender);
            this.SetTarget(target);
            if (data == null) return;

            _level = data.Level;
            _strength = data.Strength;
            _duration.SetBaseTime(data.Duration);
        }

        public virtual void SetSender(ICreature sender)
        {
            _sender = sender;
        }
        public virtual void SetTarget(ICreature target)
        {
            _target = target;
        }
    }
}
