using Asce.Game.Entities;
using System;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    [System.Serializable]
    public abstract class StackStatusEffect : StatusEffect, IStackStatusEffect
    {
        [SerializeField] protected int _maxStack = -1;
        [SerializeField] protected int _currentStack = 1;

        public event Action<object, int> OnCurrentStackChanged;
        public int MaxStack => _maxStack >= 1 ? _maxStack : int.MaxValue;
        public int CurrentStack
        {
            get => _currentStack;
            set 
            {
                _currentStack = Mathf.Min(value, MaxStack);
                OnCurrentStackChanged?.Invoke(value, _currentStack);
            }
        }

        public override void SetInformation(SO_StatusEffectInformation information)
        {
            base.SetInformation(information);
            if (information == null) return;
            _maxStack = information.MaxStack;
        }

        public virtual void Stacking(StackStatusEffect stackEffect)
        {
            this.CurrentStack += stackEffect.CurrentStack;
        }
    }
}