using Asce.Game.Combats;
using Asce.Game.Stats;
using Asce.Game.VFXs;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class GreenPotionBuff_StatusEffect : StatusEffect
    {
        protected Cooldown _healCooldown = new(0.6f);
        protected VFXObject _vfxObject;

        public override string Name => "Green Potion Buff";

        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            _healCooldown.Reset();
            this.ApplyStats();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            this.Healing(deltaTime);
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

            // Heal Scale
            if (Target.Stats is IHasHealth hasHealth)
            {
                _agents["HealScale"] = hasHealth.HealthGroup.HealScale.AddAgent(Author, $"green potion buff effect", 0.1f, StatValueType.Ratio);
            }
        }

        protected virtual void UnapplyStats()
        {
            if (Target == null || Target.Stats == null) return;

            // Heal Scale
            if (Target.Stats is IHasHealth hasHealth)
            {
                hasHealth.HealthGroup.HealScale.RemoveAgent(_agents.GetValueOrDefault("HealScale"));
            }
        }

        protected virtual void Healing(float deltaTime)
        {
            if (Target == null || Target.Stats == null) return;

            _healCooldown.Update(deltaTime);
            if (_healCooldown.IsComplete)
            {
                CombatSystem.Healing(Sender, Target.Stats, _strength);
                _healCooldown.Reset();
            }
        }
    }
}