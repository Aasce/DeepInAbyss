using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyView : CreatureView, IHasOwner<Enemy>
    {
        public new Enemy Owner
        {
            get => base.Owner as Enemy;
            set => base.Owner = value;
        }


    }
}