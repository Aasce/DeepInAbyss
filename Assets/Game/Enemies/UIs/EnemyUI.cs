using Asce.Game.UIs.Creatures;
using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyUI : CreatureUI, IHasOwner<Enemy>, IEntityUI
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

        public override void Register()
        {
            base.Register();
            
        }
    }
}