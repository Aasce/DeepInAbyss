using System;

namespace Asce.Managers
{
    /// <summary>
    ///     Provides data for events that signal a change from one value to another.
    /// </summary>
    /// <typeparam name="T"> The type of the value being tracked. </typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        protected T _oldValue;
        protected T _newValue;

        /// <summary>
        ///     The previous value before the change occurred.
        /// </summary>
        public T OldValue
        {
            get => _oldValue; 
            protected set => _oldValue = value;
        }

        /// <summary>
        ///     The new value after the change occurred.
        /// </summary>
        public T NewValue
        {
            get => _newValue;
            protected set => _newValue = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="oldValue"> The value before the change. </param>
        /// <param name="newValue"> The value after the change. </param>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    /// <summary>
    ///     Provides data for events that signal a change from one float value to another.
    /// </summary>
    public class ValueChangedEventArgs : ValueChangedEventArgs<float>
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class for float values.
        /// </summary>
        /// <param name="oldValue"> The float value before the change. </param>
        /// <param name="newValue"> The float value after the change. </param>
        public ValueChangedEventArgs(float oldValue, float newValue) 
            : base(oldValue, newValue)
        {

        }
    }
}
