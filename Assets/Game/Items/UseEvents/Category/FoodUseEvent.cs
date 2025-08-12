using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Items
{
	public class FoodUseEvent : UseEvent
    {
		[SerializeField] private float _value = 20;

        public override bool OnUse(object sender, UseEventArgs args)
        {
			if (args == null) return false;
			if (args.User == null) return false;
			
            // Add Hunger
            if (args.User.Stats is IHasSustenance hasSustenance)
            {
                hasSustenance.SustenanceGroup.Hunger.AddToCurrentValue(null, $"eat", _value);
            }
			
			return true;			
		}
	}	
}