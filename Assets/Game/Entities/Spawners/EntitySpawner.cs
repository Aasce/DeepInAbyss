using Asce.Game.Entities;
using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.SaveLoads;
using Asce.Managers.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Spawners
{
    [RequireComponent(typeof(SpawnPositionController))]
    public abstract class EntitySpawner<T> : GameComponent, ISpawner<T>, IOptimizedComponent, IUniqueIdentifiable where T : Entity
    {
        [SerializeField, Readonly] protected string _id;
        [SerializeField, Readonly] protected SpawnPositionController _positionController;

        [Space]
        [SerializeField] protected T _prefab;
        [SerializeField] protected List<T> _entities = new();

        [SerializeField] protected float _despawnDelay = 0f;
        [SerializeField] protected Cooldown _spawnCooldown = new(10f);

        [Tooltip("Maximum number of objects to spawn. -1 means no limit.")]
        [SerializeField, Min(-1)] protected int _maxSpawnCount = -1;
        protected List<PendingSpawnData> _pendingSpawns = new();

        public string ID => _id;
        public SpawnPositionController PositionController => _positionController;
        public List<T> Entities => _entities;
        public bool IsMaxSpawnCount => _maxSpawnCount != -1 && _entities.Count >= _maxSpawnCount;

        bool IOptimizedComponent.IsActive => this.enabled;
        Bounds IOptimizedComponent.Bounds => PositionController != null ? PositionController.Bounds : new Bounds(transform.position, Vector2.one);
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.ActivateOutsideView;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _positionController);
        }

        protected virtual void Update()
        {
            if (IsMaxSpawnCount) return;

            _spawnCooldown.Update(Time.deltaTime);
            if (_spawnCooldown.IsComplete)
            {
                this.QueueSpawn();
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
            if (_prefab == null) return null;
            T newEntity = EntitiesSpawnManager.Instance.Spawn<T>(_prefab, position, Quaternion.identity);
            if (newEntity == null) return null;

            _entities.Add(newEntity);
            newEntity.transform.position = position;
            newEntity.Status.SpawnPosition = position;
            newEntity.Status.SetStatus(EntityStatusType.Alive);

            return newEntity;
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

        public virtual void DespawnImmediately(T entity)
        {
            EntitiesSpawnManager.Instance.Despawn(entity);
            _entities.Remove(entity);
        }

        /// <summary>
        ///     Queue a spawn request (store data but do not spawn immediately)
        /// </summary>
        protected virtual void QueueSpawn()
        {
            Vector2 position = (_positionController != null) ? _positionController.GetPosition() : transform.position;
            _pendingSpawns.Add(new PendingSpawnData { position = position });
        }

        /// <summary>
        /// Execute all queued spawns immediately.
        /// </summary>
        protected virtual void ExecutePendingSpawns()
        {
            if (_prefab == null) return;

            for (int i = _pendingSpawns.Count - 1; i >= 0; i--)
            {
                var spawnData = _pendingSpawns[i];
                T newEntity = EntitiesSpawnManager.Instance.Spawn<T>(_prefab, spawnData.position, Quaternion.identity);
                if (newEntity != null)
                {
                    _entities.Add(newEntity);
                    newEntity.transform.position = spawnData.position;
                    newEntity.Status.SpawnPosition = spawnData.position;
                    newEntity.Status.SetStatus(EntityStatusType.Alive);
                }
                _pendingSpawns.RemoveAt(i);
            }
        }

        void IOptimizedComponent.SetActivate(bool state)
        {
            this.enabled = state;
            if (!state) 
            {
                this.ExecutePendingSpawns();
            }
        }
    }

    public struct PendingSpawnData
    {
        public Vector2 position;
    }
}
