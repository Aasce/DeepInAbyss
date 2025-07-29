using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Spawners
{
    public class SpawnPointController : SpawnPositionController
    {
        [SerializeField] protected bool _isRandom = false;
        [SerializeField] protected float _raycastDistance = 5f;

        [Space]
        [SerializeField] protected int _currentPointIndex = 0;
        [SerializeField] protected List<Transform> _points = new();

        protected Bounds _boundsCache;

        public override Bounds Bounds
        {
            get
            {
                if (_boundsCache == null)
                {
                    if (_points == null || _points.Count == 0)
                    {
                        _boundsCache = new Bounds(transform.position, Vector3.zero);
                        return _boundsCache;
                    }

                    _boundsCache = new (_points[0].position, Vector3.zero);
                    for (int i = 1; i < _points.Count; i++) _boundsCache.Encapsulate(_points[i].position);
                }
                return _boundsCache;
            }
        }

        public bool IsRandom
        {
            get => _isRandom;
            set => _isRandom = value;
        }
        public float RaycastDistance
        {
            get => _raycastDistance;
            set => _raycastDistance = value;
        }

        public int CurrentPointIndex
        {
            get => _currentPointIndex;
            set
            {
                if (_points.Count == 0) return;

                _currentPointIndex = value;
                if (_currentPointIndex < 0) _currentPointIndex = _points.Count - 1;
                else if (_currentPointIndex >= _points.Count) _currentPointIndex = 0;
            }
        }

        public List<Transform> Points => _points;


        public override Vector2 GetPosition()
        {
            Vector2 point;
            if (Points.Count == 0) point = transform.position;
            else point = Points[CurrentPointIndex].position;

            this.AdvanceToNextSpawnPoint();

            Vector2? hitPoint = this.RaycastFrom(point, RaycastDistance);
            if (hitPoint.HasValue) return hitPoint.Value;

            return point;
        }

        protected virtual void AdvanceToNextSpawnPoint()
        {
            if (Points.Count == 0) return; // No points to update

            if (IsRandom) CurrentPointIndex = Random.Range(0, Points.Count);
            else CurrentPointIndex++; // Move to the next point for the next call
        }
    }
}
