using Asce.Game.Stats;
using Asce.Game.VFXs;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Thirsty_StatusEffect : StatusEffect
    {
        protected Cooldown _deadCooldown = new(10f);
        protected VFXObject _vfxObject;

        public override string Name => "Thirsty";

        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            _deadCooldown.SetBaseTime(_strength);

            this.ApplyReducedSpeed();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _deadCooldown.Update(deltaTime);
            if (_deadCooldown.IsComplete) this.Dead();

            _vfxObject.VFXFollowCreature(Target);
        }

        public override void Unapply()
        {
            if (_vfxObject != null) _vfxObject.Stop();
            this.UnapplyReducedSpeed();
        }


        protected virtual void ApplyReducedSpeed()
        {
            if (Target == null || Target.Stats == null) return;
            if (Target.Stats is not IHasSpeed hasSpeed) return;
            _agents["Speed"] = hasSpeed.Speed.AddAgent(Author, $"thirtsy effect", 0.1f, StatValueType.Scale);
        }

        protected virtual bool UnapplyReducedSpeed()
        {
            if (Target == null || Target.Stats == null) return false;
            if (Target.Stats is not IHasSpeed hasSpeed) return false;
            hasSpeed.Speed.RemoveAgent(_agents.GetValueOrDefault("Speed"));
            return true;
        }

        protected virtual void Dead()
        {
            if (Target == null) return;
            Target.Status.SetStatus(Entities.EntityStatusType.Dead);
            Duration.ToComplete();
        }

    }
}