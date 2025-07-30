using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    [Serializable]
    public class StatusEffectController
    {
        [SerializeField, SerializeReference] protected List<StatusEffect> _statusEffects = new();

        protected ReadOnlyCollection<StatusEffect> _readonlyEffects;
        protected float _deltaTime;


        public event Action<object, StatusEffect> OnEffectAdded;
        public event Action<object, StatusEffect> OnEffectRemoved;
        public event Action<object> OnEffectsCleared;


        public ReadOnlyCollection<StatusEffect> Effects => _readonlyEffects ??= _statusEffects.AsReadOnly();


        public void Update(float deltaTime)
        {
            _deltaTime = deltaTime;
            _statusEffects.RemoveAll(LoopEffects);
        }

        public void AddEffect(StatusEffect effect)
        {
            if (effect.IsNull()) return;
            if (_statusEffects.Contains(effect)) return;

            switch (effect.Information.ApplyType) 
            {
                case EffectApplyType.StackSameSender:
                    this.StackEffectSameSender(effect);
                    break;

                case EffectApplyType.StackAnySender:
                    this.StackEffectAnySender(effect);
                    break;

                case EffectApplyType.Reset:
                    this.ResetEffect(effect);
                    break;

                case EffectApplyType.Default:
                    this.ApplyEffect(effect);
                    break;
            }

        }

        public void RemoveEffect(StatusEffect effect)
        {
            if (effect.IsNull()) return;
            if (_statusEffects.Remove(effect))
            {
                effect.Unapply();
                OnEffectRemoved?.Invoke(this, effect);
            }
        }

        public void RemoveEffect(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                StatusEffect effect = _statusEffects[i];
                if (effect.IsNull()) continue;
                
                if (!string.Equals(effect.Information.Name, name)) continue;
                
                effect.Unapply();
                _statusEffects.RemoveAt(i);
                OnEffectRemoved?.Invoke(this, effect);
            }
        }

        public void RemoveAll(Func<StatusEffect, bool> func)
        {
            if (func == null) return;
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                StatusEffect effect = _statusEffects[i];
                if (effect.IsNull()) continue;
                
                bool isRemove = func.Invoke(effect);
                if (!isRemove) continue;

                effect.Unapply();
                _statusEffects.RemoveAt(i);
                OnEffectRemoved?.Invoke(this, effect);
            }
        }

        public void ClearEffect()
        {
            foreach (StatusEffect effect in _statusEffects)
            {
                if (effect == null) continue;
                effect.Unapply();
            }
            _statusEffects.Clear();
            OnEffectsCleared?.Invoke(this);
        }

        protected void ApplyEffect(StatusEffect effect)
        {
            if (effect.IsNull()) return;
            _statusEffects.Add(effect);
            effect.Apply();
            OnEffectAdded?.Invoke(this, effect);
        }

        protected void StackEffectSameSender(StatusEffect effect)
        {
            if (effect is StackStatusEffect stackableEffect)
                foreach (StatusEffect existing in _statusEffects)
                {
                    if (existing.IsNull()) continue;
                    if (existing.Target != effect.Target) continue;
                    if (existing.GetType() != effect.GetType()) continue;
                    if (existing.Sender != effect.Sender) continue;
                
                    (existing as StackStatusEffect).Stacking(stackableEffect);
                    return;
                }

            // Apply Effect if no effect stacked
            this.ApplyEffect(effect);
        }

        protected void StackEffectAnySender(StatusEffect effect)
        {
            if (effect is StackStatusEffect stackableEffect) 
                foreach (StatusEffect existing in _statusEffects)
                {
                    if (existing.IsNull()) continue;
                    if (existing.Target != effect.Target) continue;
                    if (existing.GetType() != effect.GetType()) continue;
                
                    (existing as StackStatusEffect).Stacking(stackableEffect);
                    return;
                }

            // Apply Effect if no effect stacked
            this.ApplyEffect(effect);
        }

        protected void ResetEffect(StatusEffect effect)
        {
            bool isReseted = false;
            foreach (StatusEffect existing in _statusEffects)
            {
                if (existing.IsNull()) continue;
                if (existing.Target != effect.Target) continue;
                if (existing.GetType() != effect.GetType()) continue;

                existing.Duration.Reset();
                isReseted = true;
            }
            if (isReseted) return; // Dont apply effect if Any Effect reseted

            this.ApplyEffect(effect);
        }

        protected bool LoopEffects(StatusEffect effect)
        {
            if (effect.IsNull()) return true;
            effect.Tick(_deltaTime);
            if (effect.Duration.IsComplete)
            {
                effect.Unapply();
                OnEffectRemoved?.Invoke(this, effect);
                return true;
            }

            return false;
        }
    }
}