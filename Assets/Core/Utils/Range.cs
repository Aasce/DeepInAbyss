using UnityEngine;

namespace Asce.Managers.Utils
{
    /// <summary>
    ///     Represents a range of float values with inclusive minimum and maximum.
    /// </summary>
    [System.Serializable]
    public struct Range
    {
        /// <summary>
        ///     A constant range from 0 to 1.
        /// </summary>
        public static readonly Range zeroToOne = new(0f, 1f);

        [SerializeField] private float _min;
        [SerializeField] private float _max;

        /// <summary>
        ///     Gets or sets the minimum value of the range.
        ///     <br/>
        ///     If set to a value greater than <see cref="Max"/>, then <see cref="Max"/> is also set to match the new value.
        /// </summary>
        public float Min
        {
            readonly get => _min;
            set
            {
                _min = value;
                if (_min > _max) _max = _min;
            }
        }

        /// <summary>
        ///     Gets or sets the maximum value of the range.
        ///     <br/>
        ///     If set to a value less than <see cref="Min"/>, then <see cref="Min"/> is also set to match the new value.
        /// </summary>
        public float Max
        {
            readonly get => _max;
            set
            {
                _max = value;
                if (_max < _min) _min = _max;
            }
        }

        /// <summary>
        ///     Returns a random float value within the range.
        /// </summary>
        public readonly float RandomValue => Random.Range(_min, _max);

        /// <summary>
        ///     Initializes a new instance of the <see cref="Range"/> struct with a specified minimum and maximum value.
        /// </summary>
        public Range(float min, float max)
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     Returns a value interpolated linearly between <see cref="Min"/> and <see cref="Max"/> by parameter <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The interpolation factor, typically in [0, 1].</param>
        public readonly float Lerp(float t) => Mathf.Lerp(_min, _max, t);


        /// <summary>
        ///     Determines whether a given value is within the range (inclusive).
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the value is between <see cref="Min"/> and <see cref="Max"/> (inclusive); otherwise, <c>false</c>.
        /// </returns>
        public readonly bool Contains(float value) => value >= _min && value <= _max;

        /// <summary>
        ///     Adds the corresponding minimum and maximum values of two <see cref="Range"/> instances.
        /// </summary>
        public static Range operator +(Range a, Range b)
        {
            return new Range(a.Min + b.Min, a.Max + b.Max);
        }

        /// <summary>
        ///     Subtracts the corresponding minimum and maximum values of one <see cref="Range"/> from another.
        /// </summary>
        public static Range operator -(Range a, Range b)
        {
            return new Range(a.Min - b.Min, a.Max - b.Max);
        }

        /// <summary>
        ///     Scales both the minimum and maximum of the range by a multiplier.
        /// </summary>
        /// <param name="range">The range to scale.</param>
        /// <param name="multiplier">The value to multiply the range with.</param>
        /// <returns>A new scaled <see cref="Range"/>.</returns>
        public static Range operator *(Range range, float multiplier)
        {
            return new Range(range.Min * multiplier, range.Max * multiplier);
        }

        /// <summary>
        ///     Returns a string representation of the range in the format "[Min, Max]".
        /// </summary>
        public override readonly string ToString()
        {
            return $"[{_min}, {_max}]";
        }
    }

}