using Asce.Game.Entities;
using Asce.Managers;
using System;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class StatusEffectsManager : MonoBehaviourSingleton<StatusEffectsManager>
    {
        [SerializeField] protected SO_StatusEffectData _statusEffects;


        public SO_StatusEffectData StatusEffects => _statusEffects;

        public T SendEffect<T>(Creature sender, Creature target, EffectDataContainer data) where T : StatusEffect, new()
        {
            if (data == null) return null;
            T effect = new();

            SO_StatusEffectInformation information = StatusEffects.GetInformationByName(effect.Name);
            if (information == null) return null;

            effect.SetInformation(information);
            effect.Set(sender, target, data);
            if (target != null && target.StatusEffect != null)
                target.StatusEffect.AddEffect(effect);
            return effect;
        }
    }
}
