using Asce.Game.Stats;
using Asce.Game.StatusEffects;
using UnityEngine;

namespace Asce.Game.Items
{
	public class DrinkUseEvent : UseEvent
    {
		[SerializeField] private float _value = 30;

        public override bool OnUse(object sender, UseEventArgs args)
        {
			if (args == null) return false;
			if (args.User == null) return false;
			
            // Add Thirst
            if (args.User.Stats is IHasSustenance hasSustenance)
            {
                hasSustenance.SustenanceGroup.Thirst.AddToCurrentValue(null, $"Drink", _value);
            }
			
			return true;			
		}
	}	
}