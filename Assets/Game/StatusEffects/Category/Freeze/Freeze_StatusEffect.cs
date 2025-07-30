using Asce.Game.Stats;
using Asce.Game.VFXs;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Freeze_StatusEffect : StatusEffect
    {
        protected VFXObject _vfxObject;

        public override string Name => "Freeze";

        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            this.ApplyReductedSpeedAndJumpSpeed();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _vfxObject.VFXFollowCreature(Target);
        }

        public override void Unapply()
        {
            if (_vfxObject != null) _vfxObject.Stop();
            this.UnapplyReductedSpeedAndJumpSpeed();
        }

        protected virtual void ApplyReductedSpeedAndJumpSpeed()
        {
            if (Sender == null) return;
            if (Target == null || Target.Stats == null) return;

            // Speed
            if (Target.Stats is IHasSpeed hasSpeed)
            {
                _agents["Speed"] = hasSpeed.Speed.AddAgent(Author, $"freeze effect", -_strength, StatValueType.Ratio);
            }

            // Jump Force
            if (Target.Stats is IHasJumpForce hasJumpForce)
            {
                _agents["JumpForce"] = hasJumpForce.JumpForce.AddAgent(Author, $"freeze effect", -_strength, StatValueType.Ratio);
            }
        }

        protected virtual void UnapplyReductedSpeedAndJumpSpeed()
        {
            if (Target == null || Target.Stats == null) return;

            // Speed
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