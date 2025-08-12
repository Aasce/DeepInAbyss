using Asce.Game.Stats;
using Asce.Game.VFXs;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class ElixirOfTheBloodforgedBuff_StatusEffect : StatusEffect
    {
        protected Cooldown _buffCooldown = new(1f);

        protected VFXObject _vfxObject;

        public override string Name => "Elixir Of The Bloodforged Buff";

        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            _buffCooldown.Reset();
            this.ApplyStats();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            this.Buff(deltaTime);
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

            // Strength
            if (Target.Stats is IHasStrength hasStrength)
            {
                _agents["Strength"] = hasStrength.Strength.AddAgent(Author, $"elixir of the bloodforged buff effect", _strength, StatValueType.Flat);
            }
        }

        protected virtual void UnapplyStats()
        {
            if (Target == null || Target.Stats == null) return;

            // Strength
            if (Target.Stats is IHasStrength hasStrength)
            {
                hasStrength.Strength.RemoveAgent(_agents.GetValueOrDefault("Strength"));
            }
        }

        protected virtual void Buff(float deltaTime)
        {
            if (Target == null || Target.Stats == null) return;

            _buffCooldown.Update(deltaTime);
            if (_buffCooldown.IsComplete)
            {
                // Strength
                if (Target.Stats is IHasStrength hasStrength)
                {
                    if (_agents.TryGetValue("Strength", out StatAgent agent))
                    {
                        agent.Value += _strength;
                        hasStrength.Strength.UpdateValue();
                    }
                }

                _buffCooldown.Reset();
            }
        }
    }
}