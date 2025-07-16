using Asce.Game.Enviroments.Zones;
using Asce.Game.Players;
using Asce.Managers;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game
{
    /// <summary>
    ///     Optimizes component activity based on camera visibility.
    ///     Each frame, it processes one SubZone to avoid performance spikes.
    /// </summary>
    public class ZoneOptimizer : GameComponent
    {
        // References
        [SerializeField] protected Camera _mainCamera;

        [Tooltip("Extra bounds to expand the camera view for optimization purposes.")]
        [SerializeField] protected Vector2 _cameraBoundsAdding = Vector2.one * 10f;

        [Tooltip("All zones managed by this optimizer.")]
        [SerializeField] protected List<Zone> _allZones = new();

        [Tooltip("Cooldown between zone visibility checks (in seconds).")]
        [SerializeField] protected Cooldown _checkCooldown = new(0.5f);


        // State tracking for incremental subzone processing
        protected int _zoneIndex = 0;
        protected int _subZoneIndex = 0;
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
            this.UpdateAllZone();
        }

        protected virtual void Update()
        {
            if (_mainCamera == null || _allZones.Count == 0) return;

            _checkCooldown.Update(Time.deltaTime);
            if (_finishedCycle)
            {
                if (_checkCooldown.IsComplete)
                {
                    _zoneIndex = 0;
                    _subZoneIndex = 0;
                    _finishedCycle = false;
                    _checkCooldown.Reset();
                }
                return;
            }

            Bounds cameraBounds = _mainCamera.GetBounds();
            cameraBounds.Expand(CameraBoundsAdding);

            bool done = this.ProcessNextVisibleSubZone(cameraBounds);
            if (done) _finishedCycle = true;
        }

        /// <summary>
        ///     Update all Zone and Sub Zone
        /// </summary>
        protected virtual void UpdateAllZone()
        {
            if (_mainCamera == null || _allZones.Count == 0) return;

            Bounds cameraBounds = _mainCamera.GetBounds();
            cameraBounds.Expand(CameraBoundsAdding);

            foreach (Zone zone in _allZones)
            {
                if (zone == null) continue;
                foreach (SubZone subZone in zone.SubZones)
                {
                    if (subZone == null) continue;

                    this.UpdateOptimizedComponents(subZone, cameraBounds);
                }
            }
        }

        /// <summary>
        ///     Processes a single SubZone's visibility each frame, to reduce performance cost.
        /// </summary>
        /// <param name="cameraBounds"> Expanded camera bounds used for intersection checks. </param>
        protected virtual bool ProcessNextVisibleSubZone(Bounds cameraBounds)
        {
            while (_zoneIndex < _allZones.Count)
            {
                Zone zone = _allZones[_zoneIndex];

                // Skip null zones or zones outside of the camera bounds
                if (zone == null || !cameraBounds.Intersects(zone.ZoneCollider.bounds))
                {
                    _zoneIndex++;
                    _subZoneIndex = 0;
                    continue;
                }

                if (_subZoneIndex >= zone.SubZones.Count)
                {
                    _zoneIndex++;
                    _subZoneIndex = 0;
                    continue;
                }

                SubZone subZone = zone.SubZones[_subZoneIndex++];
                if (subZone == null) return false;

                this.UpdateOptimizedComponents(subZone, cameraBounds);

                return false; // Only process one subzone per frame
            }

            // All subzones processed
            return true;
        }

        /// <summary>
        ///     Enables or disables components in the SubZone based on their individual visibility.
        /// </summary>
        /// <param name="subZone">The SubZone being processed.</param>
        /// <param name="cameraBounds">The expanded camera bounds used for intersection checks.</param>
        protected virtual void UpdateOptimizedComponents(SubZone subZone, Bounds cameraBounds)
        {
            IEnumerable<IOptimizedComponent> optimizedComponents = subZone.OptimizedComponents;
            foreach (IOptimizedComponent component in optimizedComponents)
            {
                if (component == null) continue;

                bool isInView = cameraBounds.Intersects(component.Bounds);

                bool shouldActivate = component.OptimizeBehavior switch
                {
                    OptimizeBehavior.DeactivateOutsideView => isInView,
                    OptimizeBehavior.ActivateOutsideView => !isInView,
                    _ => component.IsActive // No change if not applicable
                };

                if (component.IsActive != shouldActivate)
                {
                    component.SetActivate(shouldActivate);
                }
            }
        }
    }
}