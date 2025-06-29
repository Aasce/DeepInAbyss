using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class TextureUtils
    {
        // Cached checker texture instance.
        private static Texture2D _checkerTexture;

        /// <summary>
        ///     Gets a lazily created, reusable checker texture.
        ///     <br/>
        ///     The checker texture is created once using default gray colors and reused for efficiency.
        /// </summary>
        public static Texture2D SimpleCheckerTexture
            => _checkerTexture != null
                ? _checkerTexture
                : _checkerTexture = CreateCheckerTexture(
                    16,
                    ColorUtils.Grayscale(0.258f),
                    ColorUtils.Grayscale(0.184f),
                    repeatTime: 3
                );

        /// <summary>
        ///     Creates a checkerboard texture with the specified size, colors, and repeat times.
        /// </summary>
        /// <param name="size"> Size of each checker cell (in pixels). </param>
        /// <param name="color1"> First color used in the checker pattern. </param>
        /// <param name="color2"> Second color used in the checker pattern. </param>
        /// <param name="repeatTime"> Number of times the checker pattern is repeated in each axis. </param>
        /// <returns> Generated checkerboard texture. </returns>
        public static Texture2D CreateCheckerTexture(
            int size,
            Color color1,
            Color color2,
            int repeatTime = 1
        )
            => CreateCheckerTexture("Checkerboard", size, color1, color2, repeatTime);

        /// <summary>
        ///     Creates a named checkerboard texture with the specified configuration.
        /// </summary>
        /// <param name="name"> Name of the generated texture asset. </param>
        /// <param name="size"> Size of each checker cell (in pixels). </param>
        /// <param name="color1"> First color used in the checker pattern. </param>
        /// <param name="color2"> Second color used in the checker pattern. </param>
        /// <param name="repeatTime"> Number of times the checker pattern is repeated in each axis. </param>
        /// <returns> Generated checkerboard texture. </returns>
        public static Texture2D CreateCheckerTexture(
            string name,
            int size,
            Color color1,
            Color color2,
            int repeatTime = 1
        )
        {
            // Total texture width and height is double the cell size multiplied by repeat count
            int axis = size * 2 * repeatTime;

            Texture2D texture = new(axis, axis)
            {
                name = name,
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Point // Disable texture filtering to keep pixels sharp
            };

            // Fill each pixel by determining the checker pattern
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    // Determine if the current square is even or odd by dividing pixel coordinates by cell size
                    bool even = ((x / size) + (y / size)) % 2 == 0;
                    texture.SetPixel(x, y, even ? color1 : color2);
                }
            }

            // Apply changes to make texture usable in rendering
            texture.Apply();

            return texture;
        }
    }
}