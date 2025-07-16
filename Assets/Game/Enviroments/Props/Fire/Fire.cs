using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class Fire : GameComponent, IEnviromentComponent, IOptimizedComponent
    {
        bool IOptimizedComponent.IsActive => gameObject.activeSelf;
        Bounds IOptimizedComponent.Bounds => new Bounds(transform.position, Vector2.one);
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.gameObject.SetActive(state);
        }
    }
}