using Asce.Managers;
using Asce.Managers.Attributes;
using Asce.Managers.Pools;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.VFXs
{
    /// <summary>
    ///     Manages pools of VFXObjects to optimize performance by reusing instances.
    /// </summary>
    public class VFXsManager : MonoBehaviourSingleton<VFXsManager>
    {
        // Ref
        [SerializeField, Readonly] protected FullScreenVFXController _fullScreenVFXController;

        [Space]
        [SerializeField] protected SO_StatusEffectVFXs _statusEffectVFXs;

        [Space]
        [Tooltip("Cooldown to control how often deactivation checks are performed.")]
        [SerializeField] protected Cooldown _delayCheckCooldown = new(0.2f);

        /// <summary>  Dictionary storing pools of VFX objects by their prefab name. </summary>
        protected Dictionary<string, Pool<VFXObject>> _vfxPools = new();

        public FullScreenVFXController FullScreenVFXController => _fullScreenVFXController;
        public SO_StatusEffectVFXs StatusEffectVFXs => _statusEffectVFXs;


        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out  _fullScreenVFXController);
        }

        protected virtual void Update()
        {
            // update cooldown and perform periodic deactivation checks.
            _delayCheckCooldown.Update(Time.deltaTime);
            if (_delayCheckCooldown.IsComplete)
            {
                foreach (var kpv in _vfxPools)
                {
                    if (kpv.Value == null) continue;
                    kpv.Value.DeactivateMatch(CheckShouldDeactivate);
                }
                _delayCheckCooldown.Reset();
            }
        }

        /// <summary>
        ///     Spawns a <see cref="VFXObject"/> at the given position and rotation using a prefab reference.
        ///     <br/>
        ///     See <see cref="Spawn{T}(VFXObject, Vector2, Quaternion)"/> for details.
        /// </summary>
        /// <typeparam name="T"> The expected type of the spawned object. </typeparam>
        /// <param name="name"> The name of the prefab. </param>
        /// <param name="position"> Spawn position. </param>
        /// <param name="rotation"> Spawn rotation. </param>
        /// <returns> The spawned instance casted to type T. </returns>
        public virtual VFXObject Spawn(VFXObject prefab, Vector2 position, Quaternion rotation = default)
            => Spawn<VFXObject>(prefab, position, rotation);

        /// <summary>
        ///     Spawns a <see cref="VFXObject"/> at the given position and rotation using a prefab name.
        ///     <br/>
        ///     See <see cref="Spawn{T}(string, Vector2, Quaternion)"/> for details.
        /// </summary>
        /// <typeparam name="T"> The expected type of the spawned object. </typeparam>
        /// <param name="name"> The name of the prefab. </param>
        /// <param name="position"> Spawn position. </param>
        /// <param name="rotation"> Spawn rotation. </param>
        /// <returns> The spawned instance casted to type T. </returns>
        public virtual VFXObject Spawn(string name, Vector2 position, Quaternion rotation = default)
            => Spawn<VFXObject>(name, position, rotation);

        /// <summary>
        ///     Spawns a <see cref="VFXObject"/> from a prefab reference with generic type casting.
        ///     <br/>
        ///     See <see cref="Spawn{T}(string, Vector2, Quaternion)"/> for details.
        /// </summary>
        /// <typeparam name="T"> The expected type of the spawned object. </typeparam>
        /// <param name="name"> The name of the prefab. </param>
        /// <param name="position"> Spawn position. </param>
        /// <param name="rotation"> Spawn rotation. </param>
        /// <returns> The spawned instance casted to type T. </returns>
        public virtual T Spawn<T>(VFXObject prefab, Vector2 position, Quaternion rotation = default) where T : VFXObject
        {
            if (prefab == null) return null;
            string name = prefab.name;
            if (!_vfxPools.ContainsKey(name)) this.CreatePool(prefab);

            return this.Spawn<T>(name, position, rotation);
        }

        /// <summary>
        ///     Spawns a <see cref="VFXObject"/> from a registered pool by name with generic type casting.
        /// </summary>
        /// <typeparam name="T"> The expected type of the spawned object. </typeparam>
        /// <param name="name"> The name of the prefab. </param>
        /// <param name="position"> Spawn position. </param>
        /// <param name="rotation"> Spawn rotation. </param>
        /// <returns> The spawned instance casted to type T. </returns>
        public virtual T Spawn<T>(string name, Vector2 position, Quaternion rotation = default) where T : VFXObject
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (!_vfxPools.ContainsKey(name)) return null;

            Pool<VFXObject> pool = _vfxPools[name];
            if (pool == null) return null;

            VFXObject vfx = pool.Activate();
            if (vfx == null) return null;

            // Reset despawn timer and set position/rotation
            vfx.DespawnTime.Reset();
            vfx.transform.SetPositionAndRotation(position, rotation);
            vfx.gameObject.SetActive(true);
            return vfx as T;
        }

        /// <summary>
        ///     Registers a prefab to be pooled if not already created.
        /// </summary>
        /// <param name="prefab"> The prefab to register. </param>
        public virtual void Register(VFXObject prefab)
        {
            if (prefab == null) return;
            string name = prefab.name;
            if (!_vfxPools.ContainsKey(name)) this.CreatePool(prefab);
        }

        /// <summary>
        ///     Creates a new <see cref="Pool{T}"/> for the specified prefab.
        /// </summary>
        /// <param name="prefab"> The prefab to create the pool for. </param>
        /// <returns> True if pool was created successfully, false otherwise. </returns>
        protected virtual bool CreatePool(VFXObject prefab)
        {
            if (prefab == null) return false;
            string name = prefab.name;

            if (_vfxPools.ContainsKey(name))
            {
                // Avoid overwriting an existing pool with a different prefab
                if (_vfxPools[name] != null && _vfxPools[name].Prefab != prefab) return false;
            }

            // Create new parent transform to organize pooled objects
            Transform newVFXParent = new GameObject(name).transform;
            newVFXParent.SetParent(transform);

            // Create and register new pool
            _vfxPools[name] = new()
            {
                Prefab = prefab,
                IsSetActive = false,
                Parent = newVFXParent
            };

            return true;
        }

        /// <summary>
        ///     Checks whether a <see cref="VFXObject"/> should be deactivated based on its despawn timer.
        ///     <br/>
        ///     Also updates the despawn timer using the cooldown base time.
        /// </summary>
        /// <param name="vfx"> The VFXObject to check. </param>
        /// <returns> True if the object should be deactivated, false otherwise. </returns>
        protected virtual bool CheckShouldDeactivate(VFXObject vfx)
        {
            if (vfx == null) return true;
            if (vfx.DespawnTime.IsComplete) return true;

            // Use fixed time step for consistent checking
            vfx.DespawnTime.Update(_delayCheckCooldown.BaseTime);
            bool isDeactive = vfx.DespawnTime.IsComplete;
            if (isDeactive) 
            {
                vfx.gameObject.SetActive(false);
                return true;
            }
            return false;
        }
    }
}
