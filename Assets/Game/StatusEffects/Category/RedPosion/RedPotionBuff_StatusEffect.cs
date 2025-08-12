using Asce.Game.Combats;
using Asce.Game.Stats;
using Asce.Game.VFXs;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class RedPotionBuff_StatusEffect : StatusEffect
    {
        protected VFXObject _vfxObject;

        public override string Name => "Red Potion Buff";

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

            // Defense
            if (Target.Stats is IHasDefense hasDefense)
            {
                _agents["Armor"] = hasDefense.DefenseGroup.Armor.AddAgent(Author, $"red potion buff effect", _strength, StatValueType.Ratio);
                _agents["Resistance"] = hasDefense.DefenseGroup.Resistance.AddAgent(Author, $"red potion buff effect", _strength, StatValueType.Ratio);
            }
			
            // Strength
            if (Target.Stats is IHasStrength hasStrength)
            {
                _agents["Strength"] = hasStrength.Strength.AddAgent(Author, $"red potion buff effect", _strength, StatValueType.Ratio);
            }
        }

        protected virtual void UnapplyStats()
        {
            if (Target == null || Target.Stats == null) return;

            // Defense
            if (Target.Stats is IHasDefense hasDefense)
            {
                hasDefense.DefenseGroup.Armor.RemoveAgent(_agents.GetValueOrDefault("Armor"));
                hasDefense.DefenseGroup.Resistance.RemoveAgent(_agents.GetValueOrDefault("Resistance"));
            }
			
            // Strength
            if (Target.Stats is IHasStrength hasStrength)
            {
                hasStrength.Strength.RemoveAgent(_agents.GetValueOrDefault("Strength"));
            }
        }
    }
}