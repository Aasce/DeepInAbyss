using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Items
{
	public class MinorGreenPotionUseEvent : UseEvent 
	{
		[SerializeField] private float _healPerTime = 2.6f;
		[SerializeField] private float _healDuration = 5f;

		public override bool OnUse(object sender, UseEventArgs args)
		{
			if (args == null) return false;
			if (args.User == null) return false;

            // Add Thirst
            if (args.User.Stats is IHasSustenance hasSustenance)
            {
                hasSustenance.SustenanceGroup.Thirst.AddToCurrentValue(null, $"Minor Green Potion buff", 25f);
            }

            StatusEffectsManager.Instance.SendEffect<GreenPotionBuff_StatusEffect>(null, args.User, new EffectDataContainer()
			{
				Level = 1,
				Strength = _healPerTime,
				Duration = _healDuration,
            });
			return true;
		}
	}	
}