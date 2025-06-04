using UnityEngine;

namespace Asce.Managers
{
    /// <summary>
    ///     A generic singleton class for <see cref="MonoBehaviour"/> components that persist across scenes.
    ///     <br/>
    ///     Automatically creates an instance if none exists and prevents recreation when the application is quitting.
    /// </summary>
    /// <typeparam name="T"> Type of the <see cref="MonoBehaviour"/> to be singleton. </typeparam>
    public abstract class DontDestroyOnLoadSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Static reference to the singleton instance
        private static T _instance;

        // Tracks if the application is quitting to avoid creating new instances during shutdown
        private static bool _applicationIsQuitting = false;

        // Lock object for thread safety during instance creation
        private static readonly object _lock = new();

        /// <summary>
        ///     Gets the singleton instance of type <typeparamref name="T"/>. Creates one if it doesn't exist yet.
        ///     Will return null if accessed during application shutdown.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting) return null;

                if (_instance == null)
                    CreateInstance();

                return _instance;
            }
        }

        /// <summary>
        ///     Assigns the singleton instance and prevents it from being destroyed on scene load.
        ///     Destroys duplicates if found.
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject); // Keep this object alive between scene loads
            }
            else if (_instance != this)
            {
                Destroy(gameObject); // Enforce singleton rule by destroying extra instances
            }
        }

        /// <summary>
        ///     Clears the singleton reference if this is the current instance.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        /// <summary>
        ///     Marks that the application is shutting down
        ///     to avoid creating new instances during or after shutdown.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        /// <summary>
        ///     Creates the singleton instance if it doesn't already exist.
        ///     Will not create a new instance if the application is quitting.
        /// </summary>
        /// <returns> The created or existing singleton instance. </returns>
        public static T CreateInstance()
        {
            if (_applicationIsQuitting) return null;

            lock (_lock) // Ensure thread safety
            {
                if (_instance != null)
                    return _instance;

                // Try to find an existing object of type T in the scene
                _instance = Object.FindAnyObjectByType<T>();
                if (_instance != null)
                    return _instance;

                // Create a new GameObject with the required component
                GameObject obj = new(typeof(T).Name);
                _instance = obj.AddComponent<T>();

                // Make the instance persistent across scene loads
                Object.DontDestroyOnLoad(obj);

                return _instance;
            }
        }
    }
}
