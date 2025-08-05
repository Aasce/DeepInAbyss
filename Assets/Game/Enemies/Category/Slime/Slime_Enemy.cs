using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{
    public class Slime_Enemy : Enemy
    {
        protected override void Action_OnAttack(object sender)
        {
            Sounds.AudioManager.Instance.PlaySFX("Slime Attack", transform.position);
            base.Action_OnAttack(sender);
        }
    }
}
