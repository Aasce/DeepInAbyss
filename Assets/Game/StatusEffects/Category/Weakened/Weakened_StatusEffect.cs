using Asce.Game.Stats;
using Asce.Game.VFXs;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Weakened_StatusEffect : StatusEffect
    {
        protected StatAgent _strengthAgent;
        protected StatAgent _armorAgent;
        protected StatAgent _resistanceAgent;

        protected VFXObject _vfxObject;

        public override string Name => "Weakened";


        public override void Apply()
        {
            _vfxObject = VFXsManager.Instance.RegisterAndSpawnEffect(Name, Target.gameObject.transform.position);
            this.ApplyReducted();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _vfxObject.VFXFollowCreature(Target);
        }

        public override void Unapply()
        {
            if (_vfxObject != null) _vfxObject.Stop();
            this.UnapplyReducted();
        }

        protected virtual void ApplyReducted()
        {
            if (Sender == null) return;
            if (Target == null || Target.Stats == null) return;

            // Strength
            if (Target.Stats is IHasStrength hasStrength)
            {
                _strengthAgent = hasStrength.Strength.AddAgent(Sender.gameObject, $"{Sender.Information.Name} weakened", -_strength, StatValueType.Ratio);
            }

            // Defense
            if (Target.Stats is IHasDefense hasDefense)
            {
                _armorAgent = hasDefense.DefenseGroup.Armor.AddAgent(Sender.gameObject, $"{Sender.Information.Name} weakened", -_strength, StatValueType.Ratio);
                _resistanceAgent = hasDefense.DefenseGroup.Resistance.AddAgent(Sender.gameObject, $"{Sender.Information.Name} weakened", -_strength, StatValueType.Ratio);
            }
        }

        protected virtual void UnapplyReducted()
        {
            if (Target == null || Target.Stats == null) return;

            // Strength
            if (Target.Stats is IHasStrength hasStrength)
            {
                hasStrength.Strength.RemoveAgent(_strengthAgent);
            }

            // Defense
            if (Target.Stats is IHasDefense hasDefense)
            {
                 hasDefense.DefenseGroup.Armor.RemoveAgent(_armorAgent);
                 hasDefense.DefenseGroup.Resistance.RemoveAgent(_resistanceAgent);
            }
        }
    }
}
