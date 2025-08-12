using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Items
{
	public class MinorRedPotionUseEvent : UseEvent
    {
		[SerializeField] private float _buffRatio = 0.4f;
		[SerializeField] private float _buffDuration = 5f;

        public override bool OnUse(object sender, UseEventArgs args)
        {
			if (args == null) return false;
			if (args.User == null) return false;
			
            // Add Thirst
            if (args.User.Stats is IHasSustenance hasSustenance)
            {
                hasSustenance.SustenanceGroup.Thirst.AddToCurrentValue(null, $"Minor Red Potion buff", 25f);
            }
			
            StatusEffectsManager.Instance.SendEffect<RedPotionBuff_StatusEffect>(null, args.User, new EffectDataContainer()
			{
				Level = 1,
				Strength = _buffRatio,
				Duration = _buffDuration,
            });
			
			return true;			
		}
	}	
}