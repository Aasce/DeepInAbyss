using Asce.Game.Stats;
using Asce.Game.VFXs;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class HungryDebuff_StatusEffect : StatusEffect
    {
		protected Cooldown _hungryCooldown = new (1f);
        protected VFXObject _vfxObject;

        public override string Name => "Hungry Debuff";

        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
			_hungryCooldown.Reset();
            this.ApplyStats();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
			this.Hungry(deltaTime);
            _vfxObject.VFXFollowCreature(Target);
        }

        public override void Unapply()
        {
            if (_vfxObject != null) _vfxObject.Stop();
            this.UnapplyStats();
        }

        protected virtual void ApplyStats()
        {
            if (Target == null || Target.Stats == null) return;
			
			if (Target.Stats is IHasDefense hasDefense) 
			{
				_agents["Resistance"] = hasDefense.DefenseGroup.Resistance.AddAgent(Author, $"hungry debuff effect", -0.5f, StatValueType.Ratio);
			}
        }

        protected virtual void UnapplyStats()
        {
            if (Target == null || Target.Stats == null) return;
			
			if (Target.Stats is IHasDefense hasDefense) 
			{
				hasDefense.DefenseGroup.Resistance.RemoveAgent(_agents.GetValueOrDefault("Resistance"));
			}
        }
		
		protected virtual void Hungry(float deltaTime)
		{
			_hungryCooldown.Update(deltaTime);
			if (_hungryCooldown.IsComplete) 
			{
				if (Target.Stats is IHasSustenance hasSustenance) 
				{
					hasSustenance.SustenanceGroup.Hunger.AddToCurrentValue(null, $"hungry debuff effect", -_strength);
				}
			
				_hungryCooldown.Reset();
			}			
		}
    }
}
