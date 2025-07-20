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

        /// <summary>
        ///     Sanitizes the input string by removing special characters and converting it to CamelCase.
        /// </summary>
        /// <param name="input">The input string to sanitize and convert.</param>
        /// <returns>
        ///     A CamelCase version of the sanitized input string.
        ///     <br/>
        ///     If the input is null, empty, or contains no valid characters, 
        ///     returns <see cref="string.Empty"/>.
        /// </returns>
        public static string SanitizeAndCamelCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Remove all characters that are not letters, digits, or spaces
            var clean = System.Text.RegularExpressions.Regex.Replace(input, "[^a-zA-Z0-9 ]", "");

            // Split the string into words by space
            string[] words = clean.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            
            if (words.Length == 0) return string.Empty; // return if no words remain after cleaning 

            // Capitalize the first letter of each word
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }

            // Combine the words into a single CamelCase string
            return string.Join("", words);
        }

    }
}
