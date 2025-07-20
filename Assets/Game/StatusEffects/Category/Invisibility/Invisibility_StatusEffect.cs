using Asce.Game.Combats;
using UnityEngine;

namespace Asce.Game.StatusEffects
{
    public class Invisibility_StatusEffect : StatusEffect
    {
        public override string Name => "Invisibility";
        public float Invisibility => Mathf.Clamp01(_strength);

        public override void Apply()
        {
            if (Target == null || Target.View == null) return;
            Target.View.Alpha = Invisibility;
            if (Target.Stats != null) Target.OnAfterSendDamage += Target_SendDamage;
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

        }

        public override void Unapply()
        {
            if (Target == null || Target.View == null) return;
            Target.View.Alpha = 1f;
            if (Target.Stats != null) Target.OnAfterSendDamage -= Target_SendDamage;
        }

        protected void Target_SendDamage(object sender, DamageContainer container)
        {
            this.Duration.ToComplete();
        }
    }
}
