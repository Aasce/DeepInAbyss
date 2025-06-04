using System;
using System.Reflection;

namespace Asce.Managers
{
    /// <summary>
    ///     A generic singleton base class.
    /// </summary>
    /// <typeparam name="T"> Type of the Class to be singleton. </typeparam>
    public abstract class Singleton<T> where T : class
    {
        private static T _instance;
        private static readonly object _lock = new();

        /// <summary>
        ///     Gets the singleton instance of type <typeparamref name="T"/>.
        ///     Create Instance if instance is null.
        /// </summary>
        public static T Instance
        {
            get 
            { 
                if (_instance == null) _instance = CreateInstance();
                return _instance;
            }
        }


        /// <summary>
        ///     Create Instance of class <typeparamref name="T"/>. Use Reflection to get its NonPublic Constructor.
        /// </summary>
        /// <returns> Returns instance of <typeparamref name="T"/> </returns>
        /// <exception cref="MissingMethodException"> Throws exception if <typeparamref name="T"/> has no Constructor with no parameters. </exception>
        private static T CreateInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    ConstructorInfo contructor = typeof(T).GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null, Type.EmptyTypes, null) 
                    ?? throw new MissingMethodException($"[{typeof(T)}] must have a private or protected parameterless constructor");

                    _instance = (T)contructor.Invoke(null);
                }
                return _instance;
            }
        }
    }
}