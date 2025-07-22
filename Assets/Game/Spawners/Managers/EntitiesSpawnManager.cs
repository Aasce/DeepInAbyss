using Asce.Game.Entities;
using Asce.Managers;
using Asce.Managers.Pools;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Spawners
{
    /// <summary>
    ///     Manages pools of entity enemies to optimize performance by reusing instances.
    /// </summary>
    public class EntitiesSpawnManager : MonoBehaviourSingleton<EntitiesSpawnManager>
    {
        /// <summary> Dictionary storing pools of entity objects by their prefab name. </summary>
        protected Dictionary<string, Pool<Entity>> _pools = new();

        public Dictionary<string, Pool<Entity>> Pools => _pools;

        /// <summary>
        ///     Spawns a <see cref="Entity"/> using a prefab reference.
        /// </summary>
        public virtual Entity Spawn(Entity prefab, Vector2 position, Quaternion rotation = default)
            => Spawn<Entity>(prefab, position, rotation);

        /// <summary>
        ///     Spawns a <see cref="Entity"/> using a prefab name.
        /// </summary>
        public virtual Entity Spawn(string name, Vector2 position, Quaternion rotation = default)
            => Spawn<Entity>(name, position, rotation);

        /// <summary>
        ///     Spawns a <see cref="Entity"/> from a prefab reference with generic type casting.
        /// </summary>
        public virtual T Spawn<T>(Entity prefab, Vector2 position, Quaternion rotation = default) where T : Entity
        {
            if (prefab == null) return null;
            string name = prefab.Information.Name;
            if (!_pools.ContainsKey(name)) CreatePool(prefab);

            return Spawn<T>(name, position, rotation);
        }

        /// <summary>
        ///     Spawns a <see cref="entity"/> from a registered pool by name with generic type casting.
        /// </summary>
        public virtual T Spawn<T>(string name, Vector2 position, Quaternion rotation = default) where T : Entity
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (!_pools.TryGetValue(name, out var pool) || pool == null) return null;

            Entity newEntity = pool.Activate();
            if (newEntity == null) return null;

            newEntity.transform.SetPositionAndRotation(position, rotation);
            newEntity.gameObject.SetActive(true);
            return newEntity as T;
        }

        /// <summary>
        ///     Despawns the specified entity and returns it to its pool.
        /// </summary>
        /// <param name="entity">The entity instance to despawn.</param>
        public virtual void Despawn(Entity entity)
        {
            if (entity == null) return;

            string name = entity.Information.Name;
            if (_pools.TryGetValue(name, out var pool) && pool != null)
            {
                pool.Deactivate(entity);
            }
            else
            {
                // Fallback: just disable if pool not found
                entity.gameObject.SetActive(false);
            }
        }


        /// <summary>
        ///     Registers a prefab to be pooled if not already created.
        /// </summary>
        public virtual void Register(Entity prefab)
        {
            if (prefab == null) return;
            string name = prefab.Information.Name;
            if (!_pools.ContainsKey(name))
                CreatePool(prefab);
        }

        /// <summary>
        ///     Creates a new <see cref="Pool{T}"/> for the specified enemy prefab.
        /// </summary>
        protected virtual bool CreatePool(Entity prefab)
        {
            if (prefab == null) return false;
            string name = prefab.Information.Name;

            if (_pools.ContainsKey(name))
            {
                if (_pools[name] != null && _pools[name].Prefab != prefab)
                    return false;
            }

            Transform parent = new GameObject(name + "_Pool").transform;
            parent.SetParent(transform);

            _pools[name] = new Pool<Entity>
            {
                Prefab = prefab,
                IsSetActive = false,
                Parent = parent
            };

            return true;
        }

        /// <summary>
        ///     Deactivates all active instances of a given enemy by name.
        /// </summary>
        public virtual void DeactivateAll(string name)
        {
            if (_pools.TryGetValue(name, out var pool) && pool != null)
                pool.Clear(isDeactive: true);
        }
    }
}
