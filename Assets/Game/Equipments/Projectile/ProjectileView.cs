using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Equipments
{
    public class ProjectileView : ViewController
    {
        [SerializeField, Readonly] protected Projectile _owner;

        public Projectile Owner
        {
            get => _owner;
            set => _owner = value;
        }

        protected override void Reset()
        {
            base.Reset();
            this.LoadComponent(out _owner);
        }

    }
}