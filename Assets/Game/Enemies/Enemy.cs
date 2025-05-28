using UnityEngine;

namespace Asce.Game.Entities.Enemies 
{ 
    public class Enemy : Creature, IHasAction<EnemyAction>, IHasStats<EnemyStats, SO_EnemyBaseStats>
    {
        public new EnemyPhysicController PhysicController => base.PhysicController as EnemyPhysicController;
        public new EnemyView View => base.View as EnemyView;
        public new EnemyAction Action => base.Action as EnemyAction;
        public new EnemyStats Stats => base.Stats as EnemyStats;

        protected virtual void Update()
        {
            
        }


    }
}