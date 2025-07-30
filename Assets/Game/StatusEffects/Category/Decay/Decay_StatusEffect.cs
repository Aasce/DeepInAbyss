using Asce.Game.Stats;
using Asce.Game.VFXs;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Decay_StatusEffect : StackStatusEffect
    {
        protected VFXObject _vfxObject;

        public override string Name => "Decay";
        public float DecayAmount => Mathf.Min(_strength, 0.99f);


        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            _vfxObject.DespawnTime.BaseTime = Duration.BaseTime;
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
            _vfxObject.DespawnTime.Reset();
        }

        protected virtual void ApplyReducedMaxHealth()
        {
            if (Target == null || Target.Stats == null) return;
            _agents["Health"] = Target.Stats.HealthGroup.Health.AddAgent(Author, $"decay effect", -DecayAmount, StatValueType.Ratio);
        }

        protected virtual void UnapplyReductedMaxHealth()
        {
            if (Target == null || Target.Stats == null) return;
            Target.Stats.HealthGroup.Health.RemoveAgent(_agents.GetValueOrDefault("Health"));
            Target.Stats.HealthGroup.Health.AddToCurrentValue(Author, $"decay effect", DecayAmount, StatValueType.Ratio);
        }

        protected virtual void StackDecayStrength(StackStatusEffect stackEffect)
        {
            if (stackEffect is not Decay_StatusEffect decayEffect) return;
            if (Target == null || Target.Stats == null) return;

            _strength += decayEffect._strength;
            _strength = Mathf.Min(_strength, 0.99f);

            // Update agent value accordingly
            _agents.GetValueOrDefault("Health").Value = -DecayAmount;
            Target.Stats.HealthGroup.Health.UpdateValue();
        }
    }
}