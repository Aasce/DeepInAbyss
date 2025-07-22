using Asce.Managers.Attributes;
using UnityEngine;

namespace Asce.Game.Equipments.Weapons
{
    public class WeaponView : ViewController
    {
        [SerializeField, Readonly] protected WeaponObject _weapon;
        [SerializeField] protected Renderer _renderer;

        public WeaponObject Owner
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