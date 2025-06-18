using System;
using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyAnimationEventReceiver : MonoBehaviour, IHasOwner<Enemy>
    {
        [SerializeField] private Enemy _owner;


        public Enemy Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public void OnFootstep()
        {
            Owner.Action.FootStepEventCalling();
        }

        public void OnAttack()
        {
            Owner.Action.AttackEventCalling();
        }

        public void OnDieFx()
        {

        }
    }
}
