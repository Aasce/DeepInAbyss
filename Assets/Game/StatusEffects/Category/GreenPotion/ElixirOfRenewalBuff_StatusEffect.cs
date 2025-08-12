using Asce.Game.Stats;
using Asce.Game.VFXs;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class ElixirOfRenewalBuff_StatusEffect : StatusEffect
    {
        protected VFXObject _vfxObject;

        public override string Name => "Elixir Of Renewal Buff";

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

            // Health
            if (Target.Stats is IHasHealth hasHealth)
            {
                float buffAmount = Mathf.Max(hasHealth.HealthGroup.Health.Value * _strength, 100f);
                _agents["Health"] = hasHealth.HealthGroup.Health.AddAgent(Author, $"elixir of renewal buff effect", buffAmount, StatValueType.Flat);
            }

            // Stamina
            if (Target.Stats is IHasStamina hasStamina)
            {
                float buffAmount = Mathf.Max(hasStamina.Stamina.Value * _strength, 100f);
                _agents["Stamina"] = hasStamina.Stamina.AddAgent(Author, $"elixir of renewal buff effect", buffAmount, StatValueType.Flat);
            }
        }

        protected virtual void UnapplyStats()
        {
            if (Target == null || Target.Stats == null) return;

            // Health
            if (Target.Stats is IHasHealth hasHealth)
            {
                hasHealth.HealthGroup.Health.RemoveAgent(_agents.GetValueOrDefault("Health"));
            }

            // Stamina
            if (Target.Stats is IHasStamina hasStamina)
            {
                hasStamina.Stamina.RemoveAgent(_agents.GetValueOrDefault("Stamina"));
            }
        }
    }
}