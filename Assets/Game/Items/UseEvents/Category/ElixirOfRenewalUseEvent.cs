using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Items
{
    public class ElixirOfRenewalUseEvent : UseEvent
    {
        [SerializeField] private float _healPerTime = 8f;
        [SerializeField] private float _buffRatio = .25f;
        [SerializeField] private float _buffDuration = 20f;

        public override bool OnUse(object sender, UseEventArgs args)
        {
            if (args == null) return false;
            if (args.User == null) return false;

            // Add Thirst
            if (args.User.Stats is IHasSustenance hasSustenance)
            {
                hasSustenance.SustenanceGroup.Thirst.AddToCurrentValue(null, $"Elixir Of Renewal buff", 1f, StatValueType.Ratio);
            }

            StatusEffectsManager.Instance.SendEffect<GreenPotionBuff_StatusEffect>(null, args.User, new EffectDataContainer()
            {
                Level = 2,
                Strength = _healPerTime,
                Duration = _buffDuration,
            });

            StatusEffectsManager.Instance.SendEffect<ElixirOfRenewalBuff_StatusEffect>(null, args.User, new EffectDataContainer()
            {
                Level = 2,
                Strength = _buffRatio,
                Duration = _buffDuration,
            });
            return true;
        }
    }
}