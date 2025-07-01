using UnityEngine;

namespace Asce.Game.Equipments
{
    public class ArrowProjectileView : ProjectileView
    {
        [SerializeField] protected Renderer _renderer;


        protected override void ResetRendererList()
        {
            base.ResetRendererList();
            if (_renderer != null) Renderers.Add(_renderer);
        }
    }
}