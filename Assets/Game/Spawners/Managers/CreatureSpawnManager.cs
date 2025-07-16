using Asce.Game.Entities;
using Asce.Managers;
using Asce.Managers.Pools;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Spawners
{
    /// <summary>
    ///     Manages pools of Creature enemies to optimize performance by reusing instances.
    /// </summary>
    public class CreatureSpawnManager : MonoBehaviourSingleton<CreatureSpawnManager>
    {
        /// <summary> Dictionary storing pools of Creature objects by their prefab name. </summary>
        protected Dictionary<string, Pool<Creature>> _pools = new();

        public Dictionary<string, Pool<Creature>> Pools => _pools;

        /// <summary>
        ///     Spawns a <see cref="Creature"/> using a prefab reference.
        /// </summary>
        public virtual Creature Spawn(Creature prefab, Vector2 position, Quaternion rotation = default)
            => Spawn<Creature>(prefab, position, rotation);

        /// <summary>
        ///     Spawns a <see cref="Creature"/> using a prefab name.
        /// </summary>
        public virtual Creature Spawn(string name, Vector2 position, Quaternion rotation = default)
            => Spawn<Creature>(name, position, rotation);

        /// <summary>
        ///     Spawns a <see cref="Creature"/> from a prefab reference with generic type casting.
        /// </summary>
        public virtual T Spawn<T>(Creature prefab, Vector2 position, Quaternion rotation = default) where T : Creature
        {
            if (prefab == null) return null;
            string name = prefab.Information.Name;
            if (!_pools.ContainsKey(name)) CreatePool(prefab);

            return Spawn<T>(name, position, rotation);
        }

        /// <summary>
        ///     Spawns a <see cref="Creature"/> from a registered pool by name with generic type casting.
        /// </summary>
        public virtual T Spawn<T>(string name, Vector2 position, Quaternion rotation = default) where T : Creature
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (!_pools.TryGetValue(name, out var pool) || pool == null) return null;

            Creature enemy = pool.Activate();
            if (enemy == null) return null;

            enemy.transform.SetPositionAndRotation(position, rotation);
            enemy.gameObject.SetActive(true);
            return enemy as T;
        }

        /// <summary>
        ///     Despawns the specified creature and returns it to its pool.
        /// </summary>
        /// <param name="creature">The creature instance to despawn.</param>
        public virtual void Despawn(Creature creature)
        {
            if (creature == null) return;

            string name = creature.Information.Name;
            if (_pools.TryGetValue(name, out var pool) && pool != null)
            {
                pool.Deactivate(creature);
            }
            else
            {
                // Fallback: just disable if pool not found
                creature.gameObject.SetActive(false);
            }
        }


        /// <summary>
        ///     Registers a prefab to be pooled if not already created.
        /// </summary>
        public virtual void Register(Creature prefab)
        {
            if (prefab == null) return;
            string name = prefab.Information.Name;
            if (!_pools.ContainsKey(name))
                CreatePool(prefab);
        }

        /// <summary>
        ///     Creates a new <see cref="Pool{T}"/> for the specified enemy prefab.
        /// </summary>
        protected virtual bool CreatePool(Creature prefab)
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

            _pools[name] = new Pool<Creature>
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
