using Asce.Game.VFXs;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public static class StatusEffectExtension
    {
        public static bool IsNull(this StatusEffect statusEffect)
        {
            if (statusEffect == null) return true;
            if (statusEffect.Information == null) return true;
            return false;
        }
    }
}