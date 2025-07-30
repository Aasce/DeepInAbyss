using Asce.Game.Stats;
using Asce.Game.VFXs;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Weakened_StatusEffect : StatusEffect
    {
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
            if (Target == null || Target.Stats == null) return;

            // Strength
            if (Target.Stats is IHasStrength hasStrength)
            {
                _agents["Strength"] = hasStrength.Strength.AddAgent(Author, $"weakened effect", -_strength, StatValueType.Ratio);
            }

            // Defense
            if (Target.Stats is IHasDefense hasDefense)
            {
                _agents["Armor"] = hasDefense.DefenseGroup.Armor.AddAgent(Author, $"weakened effect", -_strength, StatValueType.Ratio);
                _agents["Resistance"] = hasDefense.DefenseGroup.Resistance.AddAgent(Author, $"weakened effect", -_strength, StatValueType.Ratio);
            }
        }

        protected virtual void UnapplyReducted()
        {
            if (Target == null || Target.Stats == null) return;

            // Strength
            if (Target.Stats is IHasStrength hasStrength)
            {
                hasStrength.Strength.RemoveAgent(_agents.GetValueOrDefault("Strength"));
            }

            // Defense
            if (Target.Stats is IHasDefense hasDefense)
            {
                 hasDefense.DefenseGroup.Armor.RemoveAgent(_agents.GetValueOrDefault("Armor"));
                 hasDefense.DefenseGroup.Resistance.RemoveAgent(_agents.GetValueOrDefault("Resistance"));
            }
        }
    }
}
