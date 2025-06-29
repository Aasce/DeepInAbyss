using System;
using System.Collections.Generic;
using System.Linq;

namespace Asce.Managers.Utils
{
    public static class EnumUtils
    {
        /// <summary>
        ///     Returns all flags that are set in a [Flags] enum value.
        /// </summary>
        /// <example>
        ///     Stackable | Usable -> [Stackable, Usable]
        /// </example>
        /// <typeparam name="T"> The enum type with [Flags] attribute. </typeparam>
        /// <param name="value"> The enum value to extract flags from. </param>
        public static IEnumerable<T> GetFlags<T>(this T value) where T : Enum
        {
            ulong val = Convert.ToUInt64(value);

            foreach (T flag in Enum.GetValues(typeof(T)))
            {
                ulong flagVal = Convert.ToUInt64(flag);

                // Skip zero and check if the flag is included
                if (flagVal != 0 && (val & flagVal) == flagVal)
                    yield return flag;
            }
        }

        /// <summary>
        ///     Checks if a specific flag is present in the enum value.
        /// </summary>
        /// <typeparam name="T"> Enum type. </typeparam>
        /// <param name="value"> The source enum value. </param>
        /// <param name="flag"> The flag to check for. </param>
        /// <returns> Returns true if the flag is present. </returns>
        public static bool HasFlag<T>(this T value, T flag) where T : Enum
        {
            ulong val = Convert.ToUInt64(value);
            ulong flagVal = Convert.ToUInt64(flag);
            return (val & flagVal) == flagVal;
        }

        /// <summary>
        ///     Adds a flag to an existing enum value.
        /// </summary>
        /// <returns> Returns a new enum value with the flag included. </returns>
        public static T AddFlag<T>(this T value, T flag) where T : Enum
        {
            ulong val = Convert.ToUInt64(value);
            ulong flagVal = Convert.ToUInt64(flag);
            return (T)Enum.ToObject(typeof(T), val | flagVal);
        }

        /// <summary>
        ///     Removes a flag from an existing enum value.
        /// </summary>
        /// <returns> Returns a new enum value with the flag included. </returns>
        public static T RemoveFlag<T>(this T value, T flag) where T : Enum
        {
            ulong val = Convert.ToUInt64(value);
            ulong flagVal = Convert.ToUInt64(flag);
            return (T)Enum.ToObject(typeof(T), val & ~flagVal);
        }

        /// <summary>
        ///     Returns all enum values.
        /// </summary>
        /// <param name="excludeZero"> If true, it will skip the 'None' or 0 value (useful for [Flags]). </param>
        public static IEnumerable<T> GetEnumValues<T>(bool excludeZero = false) where T : Enum
        {
            foreach (T val in Enum.GetValues(typeof(T)))
            {
                if (excludeZero && Convert.ToUInt64(val) == 0) continue;
                yield return val;
            }
        }

        /// <summary>
        ///     Returns the names of all flags present in a [Flags] enum value.
        /// </summary>
        public static List<string> GetFlagNames<T>(this T value) where T : Enum
        {
            return value.GetFlags().Select(v => v.ToString()).ToList();
        }
    }
}
