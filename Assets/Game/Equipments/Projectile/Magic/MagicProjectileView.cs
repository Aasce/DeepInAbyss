using UnityEngine;

namespace Asce.Game.Equipments
{
    public class MagicProjectileView : ProjectileView
    {
        protected override void ResetRendererList()
        {
            base.ResetRendererList();
            Renderers.AddRange(transform.GetComponentsInChildren<Renderer>());
        }
    }
}