using Asce.Game.Stats;
using Asce.Game.VFXs;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Sunder_StatusEffect : StackStatusEffect
    {
        protected StatAgent _sunderArmorAgent;
        protected VFXObject _vfxObject;

        public override string Name => "Sunder";

        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            _vfxObject.DespawnTime.BaseTime = Duration.BaseTime;
            this.ApplyReducedArmor();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _vfxObject.VFXFollowCreature(Target);

        }

        public override void Unapply()
        {
            if (_vfxObject != null) _vfxObject.Stop();
            this.UnapplyReductedMaxArmor();
        }

        public override void Stacking(StackStatusEffect stackEffect)
        {
            base.Stacking(stackEffect);
            this.StackStrength(stackEffect);
            Duration.Reset();
            _vfxObject.DespawnTime.Reset();
        }

        protected virtual void ApplyReducedArmor()
        {
            if (Sender == null) return;
            if (Target == null || Target.Stats == null) return;
            _sunderArmorAgent = Target.Stats.DefenseGroup.Armor.AddAgent(Sender.gameObject, $"{Sender.Information.Name} sunder", -_strength, StatValueType.Ratio);
        }

        protected virtual void UnapplyReductedMaxArmor()
        {
            if (Target == null || Target.Stats == null) return;
            Target.Stats.DefenseGroup.Armor.RemoveAgent(_sunderArmorAgent);
        }

        protected virtual void StackStrength(StackStatusEffect stackEffect)
        {
            if (stackEffect is not Sunder_StatusEffect sunderEffect) return;
            if (Target == null || Target.Stats == null) return;

            _strength += sunderEffect._strength;

            // Update agent value accordingly
            _sunderArmorAgent.Value = -_strength;
            Target.Stats.DefenseGroup.Armor.UpdateValue();
        }
    }
}
