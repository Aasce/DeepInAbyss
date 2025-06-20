using System.Collections.Generic;

namespace Asce.BehaviourTrees
{
    /// <summary>
    ///     A key-value store shared among behaviour tree nodes to exchange data.
    /// </summary>
    public class Blackboard
    {
        private readonly Dictionary<string, object> _data = new();

        /// <summary>
        ///     Sets a value by key.
        /// </summary>
        public void Set<T>(string key, T value)
        {
            if (key == null) return;
            _data[key] = value;
        }

        /// <summary>
        ///     Gets the value associated with the key.
        /// </summary>
        /// <returns> Returns default(T) if the key doesn't exist or cannot be cast. </returns>
        public T Get<T>(string key)
        {
            if (key == null) return default;
            return _data.TryGetValue(key, out object value) ? (T)value : default;
        }

        /// <summary>
        ///     Attempts to get the value associated with the key.
        /// </summary>
        /// <returns> Returns true if the key exists and can be cast to T, false otherwise. </returns>
        public bool TryGet<T>(string key, out T value)  
        {
            value = default;
            if (key == null) return false;
            if (_data.TryGetValue(key, out object objValue) && objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Checks whether the key exists in the blackboard.
        /// </summary>
        /// <returns> Returns true if the key exists, false otherwise. </returns>
        public bool Contains(string key)
        {
            if (key == null) return false;
            return _data.ContainsKey(key);
        }

        /// <summary>
        ///     Removes a key-value pair.
        /// </summary>
        /// <returns> Returns true if the key was found and removed, false otherwise. </returns>
        public bool Remove(string key)
        {
            if (key == null) return false;
            return _data.Remove(key);
        }

        /// <summary>
        ///     Clears all key-value pairs.
        /// </summary>
        public void Clear() => _data.Clear();
        
    }
}
