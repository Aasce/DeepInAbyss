using Asce.Managers;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class Lamp : GameComponent, IEnviromentComponent, IOptimizedComponent
    {

        [Space]
        [SerializeField] private Vector2 _boundSize = Vector2.one;

        bool IOptimizedComponent.IsActive => gameObject.activeSelf;
        Bounds IOptimizedComponent.Bounds => new(transform.position, _boundSize);
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.gameObject.SetActive(state);
        }
    }
}