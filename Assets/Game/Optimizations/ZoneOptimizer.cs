using Asce.Game.Enviroments.Zones;
using Asce.Game.Players;
using Asce.Managers.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asce.Game
{
    /// <summary>
    ///     Optimizes component activity based on camera visibility.
    ///     Each frame, it processes one SubZone to avoid performance spikes.
    /// </summary>
    public class ZoneOptimizer : MonoBehaviour
    {
        // References
        [SerializeField] protected Camera _mainCamera;

        [Tooltip("All zones managed by this optimizer.")]
        [SerializeField] protected List<Zone> _allZones = new();

        [Tooltip("Cooldown between zone visibility checks (in seconds).")]
        [SerializeField] protected Cooldown _checkCooldown = new(0.5f);

        [Tooltip("Extra bounds to expand the camera view for optimization purposes.")]
        [SerializeField] protected Vector2 _boundsAdding = Vector2.one * 10f;

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
        public Vector2 BoundsAdding
        {
            get => _boundsAdding;
            set => _boundsAdding = value;
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
            cameraBounds.Expand(BoundsAdding);

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
            cameraBounds.Expand(BoundsAdding);

            foreach (Zone zone in _allZones)
            {
                if (zone == null) continue;
                foreach (SubZone subZone in zone.SubZones)
                {
                    if (subZone == null) continue;

                    bool visible = cameraBounds.Intersects(subZone.ZoneCollider.bounds);
                    this.UpdateOptimizedComponents(subZone, visible);
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

                bool visible = cameraBounds.Intersects(subZone.ZoneCollider.bounds);
                this.UpdateOptimizedComponents(subZone, visible);

                return false; // Only process one subzone per frame
            }

            // All subzones processed
            return true;
        }

        /// <summary>
        ///     Enables or disables components in the SubZone based on visibility.
        /// </summary>
        /// <param name="subZone"> The SubZone being processed. </param>
        /// <param name="visible"> Whether the subZone is within the expanded camera bounds. </param>
        protected virtual void UpdateOptimizedComponents(SubZone subZone, bool visible)
        {
            IEnumerable<IOptimizedComponent> optimizedComponents = subZone.OptimizedComponents.OfType<IOptimizedComponent>();
            foreach (IOptimizedComponent component in optimizedComponents)
            {
                if (component == null) continue;

                bool shouldActivate = component.OptimizeBehavior switch
                {
                    OptimizeBehavior.DeactivateOutsideView => visible,
                    OptimizeBehavior.ActivateOutsideView => !visible,
                    _ => component.IsActive // No changes
                };

                if (component.IsActive != shouldActivate)
                {
                    component.SetActivate(shouldActivate);
                }
            }
        }

    }
}