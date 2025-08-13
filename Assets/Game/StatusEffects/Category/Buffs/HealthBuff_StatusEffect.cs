using Asce.Game.Stats;
using Asce.Game.VFXs;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class HealthBuff_StatusEffect : StatusEffect
    {
        protected VFXObject _vfxObject;

        public override string Name => "Health Buff";

        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            this.ApplyStats();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
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
			
			if (Target.Stats is IHasHealth hasHealth) 
			{
				_agents["Health"] = hasHealth.HealthGroup.Health.AddAgent(Author, $"health buff effect", _strength, StatValueType.Flat);
			}
        }

        protected virtual void UnapplyStats()
        {
            if (Target == null || Target.Stats == null) return;
			
			if (Target.Stats is IHasHealth hasHealth) 
			{
				hasHealth.HealthGroup.Health.RemoveAgent(_agents.GetValueOrDefault("Health"));
			}
        }
    }
}
