using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Items
{
	public class MushroomUseEvent : UseEvent
    {
		[SerializeField] private float _value = 10;
		[SerializeField] private float _hungryPerTime = 4f;
		[SerializeField] private float _hungryDuration = 10f;

        public override bool OnUse(object sender, UseEventArgs args)
        {
			if (args == null) return false;
			if (args.User == null) return false;
			
            // Add Hunger
            if (args.User.Stats is IHasSustenance hasSustenance)
            {
                hasSustenance.SustenanceGroup.Hunger.AddToCurrentValue(null, $"Musroom", _value);
            }
			
            StatusEffectsManager.Instance.SendEffect<HungryDebuff_StatusEffect>(null, args.User, new EffectDataContainer()
			{
				Level = 1,
				Strength = _hungryPerTime,
				Duration = _hungryDuration,
            });
			
			return true;			
		}
	}	
}