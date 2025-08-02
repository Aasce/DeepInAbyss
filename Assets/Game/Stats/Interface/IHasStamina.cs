using UnityEngine;

namespace Asce.Game.Stats
{
    public interface IHasStamina
    {
        public StaminaStat Stamina { get; }
        public bool TryConsumeStamina(float requiredStamina);
    }
}
