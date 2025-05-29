using UnityEngine;

namespace Asce.Managers.Utils
{
    /// <summary>
    ///     Utility class for applying Unity rich text formatting to strings.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        ///     Wraps the string in a Unity rich text color tag using a hex color code.
        ///     <br/>
        ///     Returns the original string if the color code is invalid.
        /// </summary>
        /// <param name="text"> The input string to format. </param>
        /// <param name="hexColorCode"> Hexadecimal color code (e.g., "#FF0000"). </param>
        /// <returns>
        ///     Color-wrapped string or original string if code is invalid.
        /// </returns>
        public static string ColorWrap(this string text, string hexColorCode)
        {
            if (!ColorUtils.IsHexColorCode(hexColorCode)) return text;

            // Applies <color=...> rich text tag if the hex code is valid
            return $"<color={hexColorCode}>{text}</color>";
        }

        /// <summary>
        ///     Wraps the string in a Unity rich text color tag using a UnityEngine.Color.
        /// </summary>
        /// <param name="text"> The input string to format. </param>
        /// <param name="color"> UnityEngine.Color value. </param>
        /// <returns> Color-wrapped string. </returns>
        public static string ColorWrap(this string text, Color color)
        {
            // Converts Color to hex code, then applies color wrapping
            return text.ColorWrap(ColorUtils.ColorToHexCode(color));
        }

        /// <summary>
        ///     Wraps the string in Unity rich text bold tags.
        /// </summary>
        /// <param name="text"> The input string to format. </param>
        /// <returns> Bold-wrapped string. </returns>
        public static string BoldWrap(this string text)
        {
            return $"<b>{text}</b>";
        }

        /// <summary>
        ///     Wraps the string in Unity rich text italic tags.
        /// </summary>
        /// <param name="text"> The input string to format. </param>
        /// <returns> Italic-wrapped string. </returns>
        public static string ItalicWrap(this string text)
        {
            return $"<i>{text}</i>";
        }
    }
}
