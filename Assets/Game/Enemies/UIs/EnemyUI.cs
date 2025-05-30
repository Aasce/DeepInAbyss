using Asce.Game.UIs;
using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    public class EnemyUI : CreatureUI, IHasOwner<Enemy>, IWorldUI
    {
        [SerializeField] protected UIHealthBar _healthBar;

        public new Enemy Owner
        {
            get => base.Owner as Enemy;
            set => base.Owner = value;
        }

        public UIHealthBar HealthBar => _healthBar;

    }
}