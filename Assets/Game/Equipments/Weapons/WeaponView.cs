using Asce.Managers.Attributes;
using UnityEngine;

namespace Asce.Game.Equipments.Weapons
{
    public class WeaponView : ViewController
    {
        [SerializeField, Readonly] protected Weapon _weapon;
        [SerializeField] protected Renderer _renderer;

        public Weapon Owner
        {
            get => _weapon;
            set => _weapon = value;
        }

        protected override void ResetRendererList()
        {
            base.ResetRendererList();
            if (_renderer != null) Renderers.Add(_renderer);
        }
    }
}