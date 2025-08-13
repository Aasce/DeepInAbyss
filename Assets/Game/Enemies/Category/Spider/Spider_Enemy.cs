using Asce.Game.Combats;
using Asce.Manager.Sounds;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Entities.Enemies.Category
{
    public class Spider_Enemy : Enemy
    {

        protected override void Start()
        {
            base.Start();
            Action.OnJump += Action_OnJump;
            Action.OnFootstep += Action_OnFootstepEvent;
        }

        protected override void Action_OnAttackHit(object sender, AttackEventArgs args)
        {
            AudioManager.Instance.PlaySFX("Creature Base Attack", this.transform.position);
            base.Action_OnAttackHit(sender, args);
        }

        private void Action_OnJump(object obj)
        {
            AudioManager.Instance.PlaySFX("Creature Jumping", this.transform.position);
        }


        private void Action_OnFootstepEvent(object sender)
        {
            AudioManager.Instance.PlaySFX("Creature Footstep", this.transform.position);
        }
    }
}
