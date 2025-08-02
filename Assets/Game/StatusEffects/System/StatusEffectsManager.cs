using Asce.Game.Entities;
using Asce.Managers;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class StatusEffectsManager : MonoBehaviourSingleton<StatusEffectsManager>
    {
        [SerializeField] protected SO_StatusEffectData _statusEffects;
        protected List<Type> _statusEffectTypes;

        public SO_StatusEffectData StatusEffects => _statusEffects;

        public T SendEffect<T>(ICreature sender, ICreature target, EffectDataContainer data) where T : StatusEffect, new()
        {
            if (target == null || target.StatusEffect == null) return null;
            if (data == null) return null;
            T effect = new();

            SO_StatusEffectInformation information = StatusEffects.GetInformationByName(effect.Name);
            if (information == null) return null;

            effect.SetInformation(information);
            effect.Set(sender, target, data);
            target.StatusEffect.AddEffect(effect);
            return effect;
        }

        public void AsignEffect(StatusEffect effect, ICreature target)
        {
            if (effect.IsNull()) return;
            if (target == null) return;

            effect.SetTarget(target);
            if (target != null && target.StatusEffect != null)
                target.StatusEffect.AddEffect(effect);
        }

        public void RemoveEffect<T>(ICreature target) where T : StatusEffect
        {
            if (target == null || target.StatusEffect == null) return;
            target.StatusEffect.Controller.RemoveEffectByType<T>();
        }

        public StatusEffect CreateEffectByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            SO_StatusEffectInformation information = StatusEffects.GetInformationByName(name);
            if (information == null) return null;

            _statusEffectTypes ??= TypeUtils.GetConcreteSubclassesOf<StatusEffect>();
            foreach (Type type in _statusEffectTypes) 
            {
                // Create an instance of the type
                if (Activator.CreateInstance(type) is StatusEffect statusEffect)
                {
                    if (statusEffect.Name == name)
                    {
                        statusEffect.SetInformation(information);
                        return statusEffect;
                    }
                }
            }
            return null;
        }
    }
}
