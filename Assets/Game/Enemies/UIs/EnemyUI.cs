using Asce.Game.UIs.Creatures;
using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyUI : CreatureUI, IHasOwner<Enemy>, ICreatureUI
    {
        public new Enemy Owner
        {
            get => base.Owner as Enemy;
            set => base.Owner = value;
        }

        protected override void Start()
        {
            base.Start();

        }

        protected override void Register()
        {
            base.Register();
            
        }
    }
}