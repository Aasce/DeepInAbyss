using Asce.Game.Combats;
using Asce.Game.Stats;
using Asce.Game.VFXs;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Ignite_StatusEffect : StatusEffect
    {
        protected Cooldown _damageDealCooldown = new(0.5f);
        protected VFXObject _vfxObject;

        public override string Name => "Ignite";

        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            _damageDealCooldown.Reset();

            this.ApplyReducedHealing();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            this.DamageDealing(deltaTime);
            _vfxObject.VFXFollowCreature(Target);
        }

        public override void Unapply()
        {
            if (_vfxObject != null) _vfxObject.Stop();
            UnapplyReducedHealing();
        }


        protected virtual void ApplyReducedHealing()
        {
            if (Target == null || Target.Stats == null) return;
            if (Target.Stats is not IHasSurvivalStats hasSurvivalStats) return;
            _agents["HealScale"] = hasSurvivalStats.HealthGroup.HealScale.AddAgent(Author, $"ignite effect", 0.5f, StatValueType.Scale);
        }

        protected virtual bool UnapplyReducedHealing()
        {
            if (Target == null || Target.Stats == null) return false;
            if (Target.Stats is not IHasSurvivalStats hasSurvivalStats) return false;
            hasSurvivalStats.HealthGroup.HealScale.RemoveAgent(_agents.GetValueOrDefault("HealScale"));
            return true;
        }

        protected virtual void DamageDealing(float deltaTime)
        {
            if (Target == null || Target.Stats == null) return;

            _damageDealCooldown.Update(deltaTime);
            if (_damageDealCooldown.IsComplete)
            {
                CombatSystem.DamageDealing(new DamageContainer(Sender, Target)
                {
                    Damage = _strength,
                    DamageType = DamageType.TrueDamage,
                    SourceType = DamageSourceType.Effect,
                });

                _damageDealCooldown.Reset();
            }
        }

    }
}