using Asce.Game.Entities;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    [System.Serializable]
    public abstract class StackStatusEffect : StatusEffect, IStackStatusEffect
    {
        [SerializeField] protected int _maxStack;
        [SerializeField] protected int _currentStack = 1;


        public int MaxStack => _maxStack;
        public int CurrentStack
        {
            get => _currentStack;
            set => _currentStack = value;
        }


        public override void Set(Creature sender, Creature target, EffectDataContainer data)
        {
            base.Set(sender, target, data);
            if (data == null) return;
            _maxStack = data.MaxStack;
        }

        public virtual void Stacking(StackStatusEffect stackEffect)
        {
            this.CurrentStack += stackEffect.CurrentStack;
        }
    }
}