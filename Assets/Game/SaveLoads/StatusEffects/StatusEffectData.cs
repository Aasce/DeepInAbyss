using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class StatusEffectData : SaveData, ISaveData<StatusEffect>, ICreateData<StatusEffect>
    {
        public string name;
        public int level;
        public int stack;
        public float strength;
        public float baseDuration;
        public float expriedDuration;

        public void Save(in StatusEffect target)
        {
            if (target.IsNull()) return;
            name = target.Information.Name;
            level = target.Level;
            strength = target.Strength;
            baseDuration = target.Duration.BaseTime;
            expriedDuration = target.Duration.CurrentTime;

            if (target is StackStatusEffect stackEffect) stack = stackEffect.CurrentStack;
        }

        public StatusEffect Create()
        {
            StatusEffect target = StatusEffectsManager.Instance.CreateEffectByName(name);
            if (target == null) return null;

            target.Level = level;
            target.Strength = strength;
            target.Duration.SetBaseTime(baseDuration);
            target.Duration.CurrentTime = expriedDuration;

            if (target is StackStatusEffect stackEffect) stackEffect.CurrentStack = stack;

            return target;
        }
    }
}