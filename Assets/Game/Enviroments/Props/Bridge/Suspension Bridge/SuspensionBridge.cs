using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class SuspensionBridge : MonoBehaviour, IEnviromentComponent, IOptimizedComponent
    {
        [SerializeField] private Transform _leftAnchor;
        [SerializeField] private Transform _rightAnchor;

        [Space]
        [SerializeField] private Transform _partPrefab;
        [SerializeField] private float _partSpace = 0.47f;

        public Transform LeftAnchor => _leftAnchor;
        public Transform RightAnchor => _rightAnchor;

        public Transform PartPrefab => _partPrefab;
        public float PartSpace => _partSpace;

        bool IOptimizedComponent.IsActive => gameObject.activeSelf;
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.gameObject.SetActive(state);
        }
    }
}