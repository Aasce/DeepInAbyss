using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Entities.Enemies 
{ 
    public class Enemy : Creature, IHasView<EnemyView>, IHasAction<EnemyAction>, IHasStats<EnemyStats, SO_EnemyBaseStats>, IHasUI<EnemyUI>
    {
        [SerializeField, HideInInspector] private EnemyUI _ui;

        public new EnemyPhysicController PhysicController => base.PhysicController as EnemyPhysicController;
        public new EnemyView View => base.View as EnemyView;
        public new EnemyAction Action => base.Action as EnemyAction;
        public new EnemyStats Stats => base.Stats as EnemyStats;
        public EnemyUI UI => _ui;


        protected override void Reset()
        {
            base.Reset();
            if (transform.LoadComponent(out _ui))
            {
                UI.Owner = this;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (UI.HealthBar != null) UI.HealthBar.SetStat(Stats.HealthGroup.Health, Stats.DefenseGroup.Shield);
        }


        protected virtual void Update()
        {
            
        }


    }
}