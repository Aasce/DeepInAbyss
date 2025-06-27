using Asce.Game.FloatingTexts;
using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{
    public class ToxicSlime_Enemy : Slime_Enemy
    {
        protected override void Start()
        {
            base.Start();

            Stats.OnAfterSendDamage += Stats_OnAfterSendDamage;
        }

        protected virtual void Stats_OnAfterSendDamage(object sender, Combats.DamageContainer args)
        {
            if (args.Receiver is Stats.IHasSpeed hasSpeed)
            {
                if (hasSpeed.Speed.FindAgents(gameObject, "Toxic slowing") != null) return; // Already applied
                hasSpeed.Speed.AddAgent(gameObject, "Toxic slowing", -0.5f, Game.Stats.StatValueType.Ratio, 5f);
                StatValuePopupManager.Instance.CreateValuePopup($"Slow", Color.magenta, size: 40f, args.Position);
            }
        }
    }
}
