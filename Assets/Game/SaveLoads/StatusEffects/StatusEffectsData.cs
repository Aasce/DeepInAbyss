using Asce.Game.Entities;
using Asce.Game.StatusEffects;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class StatusEffectsData : SaveData, ISaveData<CreatureStatusEffect>, ILoadData<CreatureStatusEffect>
    {
        public List<StatusEffectData> statusEffects = new();

        public void Save(in CreatureStatusEffect target)
        {
            if (target == null) return;
            foreach (StatusEffect effect in target.Controller.Effects)
            {
                if (effect == null) continue;
                StatusEffectData data = new();
                data.Save(effect);
                statusEffects.Add(data);
            }
        }

        public bool Load(CreatureStatusEffect target)
        {
            if (target == null) return false;
            foreach (StatusEffectData data in statusEffects)
            {
                if (data == null) continue;
                StatusEffect effect = data.Create();
                if (effect.IsNull()) continue;

                StatusEffectsManager.Instance.AsignEffect(effect, target.Owner);
            }

            return true;
        }

    }
}