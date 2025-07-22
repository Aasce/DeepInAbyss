using Asce.Game.Entities;
using Asce.Game.Players;
using Asce.Game.Spawners;
using Asce.Managers;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game
{
    public class EntityOptimizer : GameComponent
    {
        // References
        [SerializeField] protected Camera _mainCamera;

        [Tooltip("Extra bounds to expand the camera view for optimization purposes.")]
        [SerializeField] protected Vector2 _cameraBoundsAdding = Vector2.one * 10f;

        [Tooltip("Cooldown between zone visibility checks (in seconds).")]
        [SerializeField] protected Cooldown _checkCooldown = new(0.5f);


        // State tracking for incremental subzone processing
        protected bool _finishedCycle = false;

        /// <summary>
        ///     The cooldown used for throttling visibility checks.
        /// </summary>
        public Cooldown CheckCooldown => _checkCooldown;

        /// <summary>
        ///     The amount to expand the camera bounds when checking visibility.
        /// </summary>
        public Vector2 CameraBoundsAdding
        {
            get => _cameraBoundsAdding;
            set => _cameraBoundsAdding = value;
        }


        protected virtual void Awake()
        {
            _mainCamera = Player.Instance.CameraController.Camera;
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            if (_mainCamera == null) return;

            CheckCooldown.Update(Time.deltaTime);
            if (!CheckCooldown.IsComplete) return;
            CheckCooldown.Reset();

            Bounds cameraBounds = _mainCamera.GetBounds();
            cameraBounds.Expand(CameraBoundsAdding);

            foreach (var kpv in EntitiesSpawnManager.Instance.Pools)
            {
                if (kpv.Value == null) continue;
                foreach (Entity entity in kpv.Value.Activities)
                {
                    if (entity == null) continue;
                    if (entity is not IOptimizedComponent optimizedComponent) continue;

                    bool isInView = cameraBounds.Intersects(optimizedComponent.Bounds);

                    bool shouldActivate = optimizedComponent.OptimizeBehavior switch
                    {
                        OptimizeBehavior.DeactivateOutsideView => isInView,
                        OptimizeBehavior.ActivateOutsideView => !isInView,
                        _ => optimizedComponent.IsActive // No change if not applicable
                    };

                    if (optimizedComponent.IsActive != shouldActivate)
                    {
                        optimizedComponent.SetActivate(shouldActivate);
                    }
                }
            }
        }
    }
}
