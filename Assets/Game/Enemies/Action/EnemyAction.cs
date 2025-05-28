using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyAction : CreatureAction, IHasOwner<Enemy>
    {
        public new Enemy Owner
        {
            get => base.Owner as Enemy;
            set => base.Owner = value;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}