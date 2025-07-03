using Asce.Managers.Pools;
using Asce.Managers.Utils;
using System.Collections;
using UnityEngine;

namespace Asce.Game.Spawners
{
    [RequireComponent(typeof(SpawnPositionController))]
    public abstract class EntitySpawner<T> : MonoBehaviour, ISpawner<T>, IOptimizedComponent where T : MonoBehaviour
    {
        [SerializeField, HideInInspector] protected SpawnPositionController _positionController;
        [SerializeField] protected Pool<T> _pools = new();
        [SerializeField] protected float _despawnDelay = 0f;
        [SerializeField] protected Cooldown _spawnCooldown = new(10f);

        [Tooltip("Maximum number of objects to spawn. -1 means no limit.")]
        [SerializeField, Min(-1)] protected int _maxSpawnCount = -1;


        public SpawnPositionController PositionController => _positionController;
        public bool IsMaxSpawnCount => _maxSpawnCount != -1 && _pools.Activities.Count >= _maxSpawnCount;

        bool IOptimizedComponent.IsActive => this.enabled;
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.ActivateOutsideView;

        protected virtual void Awake()
        {
            this.LoadComponent(out _positionController);
        }

        protected virtual void Update()
        {
            _spawnCooldown.Update(Time.deltaTime);
            if (_spawnCooldown.IsComplete)
            {
                this.Spawn();
                _spawnCooldown.Reset();
            }
        }

        public virtual T Spawn()
        {
            if (IsMaxSpawnCount) return null;

            Vector2 position = (_positionController != null) ? _positionController.GetPosition() : transform.position;
            return this.Spawn(position);
        }

        public virtual T Spawn(Vector2 position)
        {            
            T newObject = _pools.Activate();
            if (newObject == null) return null;

            newObject.transform.position = position;

            return newObject;
        }

        public virtual void Despawn(T obj)
        {
            if (obj == null) return;
            if (_despawnDelay > 0) StartCoroutine(DespawnAfterDelay(obj, _despawnDelay));
            else DespawnImmediately(obj);
        }

        protected virtual IEnumerator DespawnAfterDelay(T obj, float despawnDelay)
        {
            yield return new WaitForSeconds(despawnDelay);
            if (obj == null) yield break;
            DespawnImmediately(obj);
        }

        public virtual void DespawnImmediately(T obj) => _pools.Deactivate(obj);

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.enabled = state;
        }
    }
}
