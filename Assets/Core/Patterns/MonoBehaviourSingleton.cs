using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Managers
{
    /// <summary>
    ///     A generic singleton base class for <see cref="MonoBehaviour"/> components.
    ///     <br/>
    ///     Ensures only one instance of a <see cref="MonoBehaviour"/> of type <typeparamref name="T"/> exists in the scene.
    /// </summary>
    /// <typeparam name="T"> Type of the <see cref="MonoBehaviour"/> to be singleton. </typeparam>
    public abstract class MonoBehaviourSingleton<T> : GameComponent where T : MonoBehaviour
    {
        // Static reference to the singleton instance
        private static T _instance;

        /// <summary>
        ///     Gets the singleton instance of type <typeparamref name="T"/>.
        ///     If no instance is found in the scene, logs an error.
        /// </summary>
        public static T Instance
        {
            get
            {
                // If instance is not yet set, try to find it in the scene
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        // ColorWrap is assumed to be a custom extension for colored logging
                        Debug.LogError($"[{nameof(MonoBehaviourSingleton<T>).ColorWrap(Color.red)}] No instance of {typeof(T)} found in scene.");
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        ///     Ensures that only one instance of <typeparamref name="T"/> is assigned.
        ///     Destroys duplicate instances if they exist.
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                // Set this instance as the singleton
                _instance = this as T;
            }
            else if (_instance != this)
            {
                // If another instance already exists, destroy this one to enforce singleton
                Destroy(gameObject);
            }
        }

        /// <summary>
        ///     Unity OnDestroy method. Clears the instance reference if it is being destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
