using Asce.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    public class SuspensionBridge : GameComponent, IEnviromentComponent, IOptimizedComponent
    {
        [SerializeField] private Transform _leftAnchor;
        [SerializeField] private Transform _rightAnchor;
        [SerializeField] protected List<SuspensionBridgePart> _parts = new();

        [Space]
        [SerializeField] private SuspensionBridgePart _partPrefab;
        [SerializeField] private float _partSpace = 0.47f;


        public Transform LeftAnchor => _leftAnchor;
        public Transform RightAnchor => _rightAnchor;
        public List<SuspensionBridgePart> Parts => _parts;

        public SuspensionBridgePart PartPrefab => _partPrefab;
        public float PartSpace => _partSpace;

        bool IOptimizedComponent.IsActive => gameObject.activeSelf;
        Bounds IOptimizedComponent.Bounds
        {
            get
            {
                if (RightAnchor == null) return new Bounds(LeftAnchor.position, Vector3.one);
                Vector3 min = Vector3.Min(LeftAnchor.position, RightAnchor.position);
                Vector3 max = Vector3.Max(LeftAnchor.position, RightAnchor.position);

                // Add a small padding in Y-axis to account for the vertical space bridge might take
                float paddingY = 1.0f;
                min.y -= paddingY;
                max.y += paddingY;

                Vector3 size = max - min;
                Vector3 center = (min + max) * 0.5f;

                return new Bounds(center, size);
            }
        }

        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.gameObject.SetActive(state);
        }
    }
}