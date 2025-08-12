using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Items
{
    public class ElixirOfTheSkyboundUseEvent : UseEvent
    {
		[SerializeField] private float _buffRatio = 0.5f;
		[SerializeField] private float _buffDuration = 20f;
		
        public override bool OnUse(object sender, UseEventArgs args)
        {
            if (args == null) return false;
            if (args.User == null) return false;

            // Add Thirst
            if (args.User.Stats is IHasSustenance hasSustenance)
            {
                hasSustenance.SustenanceGroup.Thirst.AddToCurrentValue(null, $"Elixir Of The Skybound buff", 1f, StatValueType.Ratio);
            }
			
            StatusEffectsManager.Instance.SendEffect<BluePotionBuff_StatusEffect>(null, args.User, new EffectDataContainer()
			{
				Level = 2,
				Strength = _buffRatio,
				Duration = _buffDuration,
            });
			
            StatusEffectsManager.Instance.SendEffect<Invisibility_StatusEffect>(null, args.User, new EffectDataContainer()
			{
				Level = 1,
				Strength = 0.2f,
				Duration = _buffDuration,
            });

            return true;
        }
    }
}