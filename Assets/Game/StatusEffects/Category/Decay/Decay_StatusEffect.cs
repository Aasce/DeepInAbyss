using Asce.Game.Stats;
using Asce.Game.VFXs;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Decay_StatusEffect : StackStatusEffect
    {
        protected StatAgent _decayHealthAgent;
        protected VFXObject _vfxObject;


        public override string Name => "Decay";
        public float DecayAmount => Mathf.Min(_strength, 0.99f);


        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.transform.position);
            this.ApplyReducedMaxHealth();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _vfxObject.VFXFollowCreature(Target);
        }

        public override void Unapply()
        {
            if (_vfxObject != null) _vfxObject.Stop();
            this.UnapplyReductedMaxHealth();
        }

        public override void Stacking(StackStatusEffect stackEffect)
        {
            base.Stacking(stackEffect);
            this.StackDecayStrength(stackEffect);
            Duration.Reset();
        }

        protected virtual void ApplyReducedMaxHealth()
        {
            if (Sender == null) return;
            if (Target == null || Target.Stats == null) return;
            _decayHealthAgent = Target.Stats.HealthGroup.Health.AddAgent(Sender.gameObject, $"{Sender.name} decay", -DecayAmount, StatValueType.Ratio);
        }

        protected virtual void UnapplyReductedMaxHealth()
        {
            if (Target == null || Target.Stats == null) return;
            Target.Stats.HealthGroup.Health.RemoveAgent(_decayHealthAgent);
            Target.Stats.HealthGroup.Health.AddToCurrentValue(Sender.gameObject, $"{Sender.name} decay", DecayAmount, StatValueType.Ratio);
        }

        protected virtual void StackDecayStrength(StackStatusEffect stackEffect)
        {
            if (stackEffect is not Decay_StatusEffect decayEffect) return;
            if (Target == null || Target.Stats == null) return;

            _strength += decayEffect._strength;
            _strength = Mathf.Min(_strength, 0.99f);

            // Update agent value accordingly
            _decayHealthAgent.Value = -DecayAmount;
            Target.Stats.HealthGroup.Health.UpdateValue();
        }
    }
}