using Asce.Game.Stats;
using Asce.Game.VFXs;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Freeze_StatusEffect : StatusEffect
    {
        protected StatAgent _freezeSpeedAgent;
        protected StatAgent _freezeJumpForceAgent;

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
                _freezeSpeedAgent = hasSpeed.Speed.AddAgent(Sender.gameObject, $"{Sender.Information.Name} freeze", -_strength, StatValueType.Ratio);
            }

            // Jump Force
            if (Target.Stats is IHasJumpForce hasJumpForce)
            {
                _freezeJumpForceAgent = hasJumpForce.JumpForce.AddAgent(Sender.gameObject, $"{Sender.Information.Name} freeze", -_strength, StatValueType.Ratio);
            }
        }

        protected virtual void UnapplyReductedSpeedAndJumpSpeed()
        {
            if (Target == null || Target.Stats == null) return;

            // Speed
            if (Target.Stats is IHasSpeed hasSpeed)
            {
                hasSpeed.Speed.RemoveAgent(_freezeSpeedAgent);
            }

            // Jump Force
            if (Target.Stats is IHasJumpForce hasJumpForce)
            {
                hasJumpForce.JumpForce.RemoveAgent(_freezeJumpForceAgent);
            }
        }
    }
}