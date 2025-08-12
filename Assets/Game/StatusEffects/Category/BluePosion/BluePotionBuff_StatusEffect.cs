using Asce.Game.Combats;
using Asce.Game.Stats;
using Asce.Game.VFXs;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class BluePotionBuff_StatusEffect : StatusEffect
    {
        protected VFXObject _vfxObject;

        public override string Name => "Blue Potion Buff";

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

            // Speed
            if (Target.Stats is IHasSpeed hasSpeed)
            {
                _agents["Speed"] = hasSpeed.Speed.AddAgent(Author, $"blue potion buff effect", _strength, StatValueType.Ratio);
            }
			
            // Jump Force
            if (Target.Stats is IHasJumpForce hasJumpForce)
            {
                _agents["JumpForce"] = hasJumpForce.JumpForce.AddAgent(Author, $"blue potion buff effect", _strength, StatValueType.Ratio);
            }
        }

        protected virtual void UnapplyStats()
        {
            if (Target == null || Target.Stats == null) return;

            // Heal Scale
            if (Target.Stats is IHasSpeed hasSpeed)
            {
                hasSpeed.Speed.RemoveAgent(_agents.GetValueOrDefault("Speed"));
            }
			
            // Jump Force
            if (Target.Stats is IHasJumpForce hasJumpForce)
            {
                hasJumpForce.JumpForce.RemoveAgent(_agents.GetValueOrDefault("JumpForce"));
            }
        }
    }
}