using Asce.Game.Combats;
using Asce.Game.Stats;
using Asce.Game.VFXs;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Underwater_StatusEffect : StatusEffect
    {
        protected Cooldown _damageDealCooldown = new(2f);
        protected VFXObject _vfxObject;
        protected float _gravityScale;

        public override string Name => "Underwater";

        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            this.ApplyReducedBreath();
            if (Target != null) Target.OnBeforeTakeDamage += Target_OnBeforeTakeDamage; 
            if (Target != null)
            {
                Target.OnBeforeTakeDamage += Target_OnBeforeTakeDamage;
                if (Target.PhysicController != null)
                {
                    _gravityScale = Target.PhysicController.GravityScale;
                    Target.PhysicController.GravityScale = .5f;
                }
            }
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
            this.UnapplyReducedBreath();
            if (Target != null)
            {
                Target.OnBeforeTakeDamage -= Target_OnBeforeTakeDamage;
                if (Target.PhysicController != null) Target.PhysicController.GravityScale = _gravityScale;
            }
        }


        protected virtual void ApplyReducedBreath()
        {
            if (Target == null || Target.Stats == null) return;
            if (Target.Stats is IHasSustenance hasSustenance)
            {
                float changeValue = hasSustenance.SustenanceGroup.Breath.ChangeStat.Value + _strength;
                _agents["Breath"] = hasSustenance.SustenanceGroup.Breath.ChangeStat.AddAgent(Author, $"underwater effect", -changeValue);
            }

            if (Target.Stats is IHasJumpForce hasJumpForce)
            {
                _agents["JumpForce"] = hasJumpForce.JumpForce.AddAgent(Author, "underwater effect", 0.5f, StatValueType.Scale);
            }
        }

        protected virtual bool UnapplyReducedBreath()
        {
            if (Target == null || Target.Stats == null) return false;
            if (Target.Stats is IHasSustenance hasSustenance)
            {
                hasSustenance.SustenanceGroup.Breath.ChangeStat.RemoveAgent(_agents.GetValueOrDefault("Breath"));
            }

            if (Target.Stats is IHasJumpForce hasJumpForce)
            {
                hasJumpForce.JumpForce.RemoveAgent(_agents.GetValueOrDefault("JumpForce"));
            }
            return true;
        }

        protected virtual void DamageDealing(float deltaTime)
        {
            if (Target == null || Target.Stats == null) return; 
            if (Target.Stats is not IHasSustenance hasSustenance) return;
            if (!hasSustenance.SustenanceGroup.Breath.IsEmpty) return;

            _damageDealCooldown.Update(deltaTime);
            if (_damageDealCooldown.IsComplete)
            {
                CombatSystem.DamageDealing(new DamageContainer(Sender, Target)
                {
                    Damage = Target.Stats.HealthGroup.Health.Value * 0.2f,
                    DamageType = DamageType.TrueDamage,
                    SourceType = DamageSourceType.Effect,
                });

                _damageDealCooldown.Reset();
            }
        }
        protected virtual void Target_OnBeforeTakeDamage(object sender, DamageContainer damageContainer)
        {
            if (damageContainer.SourceType != DamageSourceType.Falling) return;
            damageContainer.Damage *= 0.1f;
        }

    }
}