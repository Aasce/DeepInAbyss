using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Managers.Pools
{
    /// <summary>
    ///     A generic object pool for Unity components.
    ///     <br/>
    ///     Manages instantiation, reuse, and recycling of component instances.
    /// </summary>
    /// <typeparam name="T"> A Unity Component type. </typeparam>
    [Serializable]
    public class Pool<T> where T : Component
    {
        [Tooltip("The prefab used to instantiate new objects.")]
        [SerializeField] protected T _prefab;

        [Tooltip("List of currently active instances")]
        [SerializeField] protected List<T> _activities = new();

        [Tooltip("Queue of inactive instances (available to reuse)")]
        [SerializeField] protected Queue<T> _pools = new();

        [Header("Configs")]
        [Tooltip("If true, objects will be SetActive(true/false) on activate/deactivate")]
        [SerializeField] protected bool _isSetActive = true;

        [Tooltip("Parent transform to attach activated objects to")]
        [SerializeField] protected Transform _parent;
        [SerializeField] protected bool _worldPositionStays = true;

        /// <summary>
        ///     The prefab used to instantiate new objects.
        /// </summary>
        public virtual T Prefab
        {
            get => _prefab;
            set => _prefab = value;
        }

        /// <summary>
        ///     The list of currently active objects.
        /// </summary>
        public virtual List<T> Activities => _activities;

        /// <summary>
        ///     The queue of currently inactive objects.
        /// </summary>
        public virtual Queue<T> Pools => _pools;

        /// <summary>
        ///     The total count of both active and pooled objects.
        /// </summary>
        public virtual int Count => _activities.Count + _pools.Count;

        /// <summary>
        ///     Whether to set active/inactive on objects during activation/deactivation.
        /// </summary>
        public virtual bool IsSetActive
        {
            get => _isSetActive;
            set => _isSetActive = value;
        }

        /// <summary>
        ///     The parent transform where activated objects will be assigned to.
        /// </summary>
        public virtual Transform Parent
        {
            get => _parent;
            set => _parent = value;
        }

        /// <summary>
        ///     See also <see cref="Transform.SetParent(Transform, bool)"/>
        /// </summary>
        public virtual bool WorldPositionStays
        {
            get => _worldPositionStays;
            set => _worldPositionStays = value;
        }

        public Pool() : this (null, null, true) { }
        public Pool(T prefab, Transform parent) : this(prefab, parent, true) { }
        public Pool(T prefab, bool isSetActive) : this(prefab, null, isSetActive) { }
        public Pool(T prefab, Transform parent, bool isSetActive)
        {
            Prefab = prefab;
            Parent = parent;
            IsSetActive = isSetActive;
        }


        /// <summary>
        ///     Activates an object from the pool, or creates a new one if none available.
        /// </summary>
        /// <param name="action"> Optional action to perform on the object after activation. </param>
        /// <returns> The activated object. </returns>
        public virtual T Activate(Action<T> action = null)
        {
            T obj = this.Get() ?? this.Create(); // Try to get from pool, else create
            action?.Invoke(obj);

            if (IsSetActive) obj.gameObject.SetActive(true);
            obj.transform.SetParent(Parent != null ? Parent : null, _worldPositionStays); // Assign to parent if available

            return obj;
        }

        /// <summary>
        ///     Deactivates an object and returns it to the pool.
        /// </summary>
        public virtual void Deactivate(T obj)
        {
            this.TryDeactivate(obj);
        }

        /// <summary>
        ///     Tries to deactivate an object if it is in the active list.
        /// </summary>
        /// <returns> True if the object was successfully deactivated. </returns>
        public virtual bool TryDeactivate(T obj)
        {
            if (_activities.Remove(obj))
            {
                _pools.Enqueue(obj);
                if (IsSetActive) obj.gameObject.SetActive(false);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Deactivates the object at a specific index in the active list.
        /// </summary>
        public virtual void DeactivateAt(int index)
        {
            if (index < 0 || index >= _activities.Count) return;

            T obj = _activities[index];
            _activities.RemoveAt(index);
            _pools.Enqueue(obj);
            if (IsSetActive) obj.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Deactivates all active objects that match a condition.
        /// </summary>
        public virtual void DeactivateMatch(Func<T, bool> match)
        {
            // Loop backward to safely remove from list
            for (int i = _activities.Count - 1; i >= 0; i--)
            {
                if (match(_activities[i]))
                {
                    DeactivateAt(i);
                }
            }
        }

        /// <summary>
        ///     Gets an object from the pool if available.
        /// </summary>
        protected virtual T Get()
        {
            if (_pools.Count <= 0) return null;
            T obj = _pools.Dequeue();
            _activities.Add(obj);

            return obj;
        }

        /// <summary>
        ///     Creates a new instance of the prefab and adds it to the active list.
        /// </summary>
        protected virtual T Create()
        {
            if (Prefab == null)
            {
                Debug.LogError("Prefab is null", Prefab);
                return null;
            }

            T obj = GameObject.Instantiate(Prefab, Parent != null ? Parent : null, _worldPositionStays);
            _activities.Add(obj);
            return obj;
        }

        /// <summary>
        ///     Clears all active objects, deactivating them and returning them to the pool.
        /// </summary>
        /// <param name="isDeactive"> Force set inactive even if _isSetActive is false. </param>
        /// <param name="onClear"> Optional callback on each cleared object. </param>
        public virtual void Clear(bool isDeactive = false, Action<T> onClear = null)
        {
            // Reverse loop for safer modification
            while (_activities.Count > 0)
            {
                T obj = _activities[^1]; // ^1 is the last element (C# 8+ syntax)
                _activities.RemoveAt(_activities.Count - 1);
                _pools.Enqueue(obj);

                onClear?.Invoke(obj);
                if (IsSetActive || isDeactive) obj.gameObject.SetActive(false);
            }
        }

        /// <summary>
        ///     Checks if an object exists in either the active or pool lists.
        /// </summary>
        public virtual bool Contains(T item)
        {
            return _activities.Contains(item) || _pools.Contains(item);
        }
    }
}
