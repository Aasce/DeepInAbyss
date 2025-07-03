using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities.Enemies 
{ 
    public class Enemy : Creature, IHasView<EnemyView>, IHasAction<EnemyAction>, IHasStats<EnemyStats, SO_EnemyBaseStats>, IHasUI<EnemyUI>, IHasSpoils<EnemySpoils>
    {
        public new EnemyPhysicController PhysicController => base.PhysicController as EnemyPhysicController;
        public new EnemyView View => base.View as EnemyView;
        public new EnemyAction Action => base.Action as EnemyAction;
        public new EnemyStats Stats => base.Stats as EnemyStats;
        public new EnemyUI UI => base.UI as EnemyUI;
        public new EnemySpoils Spoils => base.Spoils as EnemySpoils;


        protected override void Reset()
        {
            base.Reset();
        }

        protected override void Start()
        {
            base.Start();
            Action.OnAttack += Action_OnAttack;
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void Action_OnAttack(object sender)
        {

        }
    }
}