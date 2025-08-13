using Asce.Game.Combats;
using Asce.Manager.Sounds;
using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{
    public class Slime_Enemy : Enemy
    {
        protected override void Action_OnAttackHit(object sender, AttackEventArgs args)
        {
            AudioManager.Instance.PlaySFX("Slime Attack", transform.position);
            base.Action_OnAttackHit(sender, args);
        }
    }
}
