using Asce.Game.Stats;
using Asce.Game.VFXs;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class ArmorBuff_StatusEffect : StatusEffect
    {
        protected VFXObject _vfxObject;

        public override string Name => "Armor Buff";

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
			
			if (Target.Stats is IHasDefense hasDefense) 
			{
				_agents["Armor"] = hasDefense.DefenseGroup.Armor.AddAgent(Author, $"armor buff effect", _strength, StatValueType.Flat);
			}
        }

        protected virtual void UnapplyStats()
        {
            if (Target == null || Target.Stats == null) return;
			
			if (Target.Stats is IHasDefense hasDefense) 
			{
				hasDefense.DefenseGroup.Armor.RemoveAgent(_agents.GetValueOrDefault("Armor"));
			}
        }
    }
}
