using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class Fire : MonoBehaviour, IEnviromentComponent, IOptimizedComponent
    {
        bool IOptimizedComponent.IsActive => gameObject.activeSelf;
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.gameObject.SetActive(state);
        }
    }
}