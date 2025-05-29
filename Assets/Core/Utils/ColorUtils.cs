using System.Text.RegularExpressions;
using UnityEngine;

namespace Asce.Managers.Utils
{
    /// <summary>
    ///     Utility class for working with color values and hexadecimal color strings.
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        ///     Regular expression to validate hex color codes.
        ///     Supports 3-digit, 6-digit, and 8-digit hex values with '#' prefix.
        /// </summary>
        public static readonly Regex hexColorRegex = new(@"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$");

        /// <summary>
        ///     Checks whether a given string is a valid hex color code.
        /// </summary>
        /// <param name="hexCode"> The hex string to validate (e.g., "#FF0000" or "#FFF"). </param>
        /// <returns>
        ///     True if the string is a valid hex color; otherwise, false.
        /// </returns>
        public static bool IsHexColorCode(string hexCode)
        {
            return !string.IsNullOrEmpty(hexCode) && hexColorRegex.IsMatch(hexCode);
        }

        /// <summary>
        ///     Converts a valid hex color code into a UnityEngine.Color.
        ///     Returns <see cref="Color.clear"/> if the input is invalid or parsing fails.
        /// </summary>
        /// <param name="hexCode"> Hexadecimal color string (e.g., "#FF0000FF"). </param>
        /// <returns>
        ///     Converted Color or <see cref="Color.clear"/> on failure.
        /// </returns>
        public static Color HexToColor(string hexCode)
        {
            if (!IsHexColorCode(hexCode)) return Color.clear;

            // Unity accepts hex with or without "#" using ColorUtility
            if (ColorUtility.TryParseHtmlString(hexCode, out Color color))
            {
                return color;
            }

            return Color.clear; // Fallback on invalid parsing
        }

        /// <summary>
        ///     Converts a UnityEngine.Color to an 8-digit hexadecimal color string (including alpha).
        /// </summary>
        /// <param name="color"> The UnityEngine.Color to convert. </param>
        /// <returns> Hex color string in #RRGGBBAA format. </returns>
        public static string ColorToHexCode(Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGBA(color)}";
        }
    }
}
