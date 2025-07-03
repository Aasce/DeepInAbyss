using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class Lamp : MonoBehaviour, IEnviromentComponent, IOptimizedComponent
    {
        bool IOptimizedComponent.IsActive => gameObject.activeSelf;
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.gameObject.SetActive(state);
        }
    }
}