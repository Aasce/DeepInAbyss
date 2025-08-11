using Asce.Game.Combats;
using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{
    public class OrcWarrior_Enemy : Enemy
    {
        protected override void Start()
        {
            base.Start();
            Action.OnJump += Action_OnJump;
            Action.OnFootstep += Action_OnFootstepEvent;
        }

        protected override void Action_OnAttackHit(object sender, AttackEventArgs args)
        {
            Sounds.AudioManager.Instance.PlaySFX("Creature Base Attack", this.transform.position);
            base.Action_OnAttackHit(sender, args);
        }


        private void Action_OnJump(object obj)
        {
            Sounds.AudioManager.Instance.PlaySFX("Creature Jumping", this.transform.position);
        }


        private void Action_OnFootstepEvent(object sender)
        {
            Sounds.AudioManager.Instance.PlaySFX("Creature Footstep", this.transform.position);
        }
    }
}
