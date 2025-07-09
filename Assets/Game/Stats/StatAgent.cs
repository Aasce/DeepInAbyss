using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Stats
{
    /// <summary>
    ///     Represents a modifier or influence on a stat, including its origin, reason, value, type, and duration.
    /// </summary>
    [Serializable]
    public class StatAgent
    {
        [SerializeField] protected GameObject _author;
        [SerializeField] protected string _reason;
        [SerializeField] protected float _value;
        [SerializeField] protected StatValueType _type;

        [SerializeField] protected Vector2 _position;
        [SerializeField] protected bool _isClearable = true;

        /// <summary>
        ///     The GameObject that applied stat modification.
        /// </summary>
        public GameObject Author
        {
            get => _author;
            set => _author = value;
        }

        /// <summary>
        ///  The reason or source description for stat modification.
        /// </summary>
        public string Reason
        {
            get => _reason;
            set => _reason = value;
        }

        /// <summary>
        ///     The value of the stat modification.
        /// </summary>
        public float Value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        ///     The type of value operation applied to the stat (e.g., Flat, Ratio).
        /// </summary>
        public StatValueType ValueType
        {
            get => _type; 
            set => _type = value;
        }

        /// <summary>
        ///     The position of agent affect.
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        ///     Whether the value can be removed from the stat.
        /// </summary>
        public bool IsClearable
        {
            get => _isClearable;
            set => _isClearable = value;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="StatAgent"/> class 
        ///     with default values.
        /// </summary>
        public StatAgent() 
            : this (null, string.Empty, 0f, StatValueType.Plat, default) 
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatAgent"/> class 
        ///     with an author and reason.
        /// </summary>
        /// <param name="author"> The <see cref="GameObject"/> applying the stat modification. </param>
        /// <param name="reason"> The reason or description of the stat modification. </param>
        public StatAgent(GameObject author, string reason)
            : this (author, reason, 0f, StatValueType.Plat, default)
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatAgent"/> class 
        ///     with an author, reason, value, duration and position.
        /// </summary>
        /// <param name="author"> The <see cref="GameObject"/> applying the stat modification. </param>
        /// <param name="reason"> The reason or description of the stat modification. </param>
        /// <param name="value"> The value of the modification. </param>
        /// <param name="position"> The position affect. </param>
        public StatAgent(GameObject author, string reason, float value, Vector2 position = default)
            : this (author, reason, value, StatValueType.Plat, position)
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatAgent"/> class 
        ///     with full configuration.
        /// </summary>
        /// <param name="author"> The <see cref="GameObject"/> applying the stat modification. </param>
        /// <param name="reason"> The reason or description of the stat modification. </param>
        /// <param name="value"> The value of the modification. </param>
        /// <param name="type"> The type of the stat value (e.g., Flat or Ratio). </param>
        /// <param name="position"> The position affect. </param>
        public StatAgent(GameObject author, string reason, float value, StatValueType type, Vector2 position = default)
        {
            Author = author;
            Reason = reason;
            Value = value;
            ValueType = type;
            Position = position;
        }


        public virtual void ToNotClearable() => IsClearable = false;
    }
}
