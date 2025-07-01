using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Utils
{
    /// <summary>
    ///     Utility class for handling common view rendering operations.
    /// </summary>
    public static class ViewUtils
    {
        /// <summary>
        ///     Sets the alpha (transparency) value of all renderers associated with the view.
        ///     Supports both material property blocks with "_Alpha" and SpriteRenderers.
        /// </summary>
        /// <param name="view"> The view implementing <see cref="IViewController"/> interface. </param>
        /// <param name="alpha"> The alpha value to set, clamped between 0 and 1. </param>
        public static void SetRendererAlpha(this IViewController view, float alpha)
        {
            if (view == null) return;
            if (view.Renderers.Count <= 0) return;

            alpha = Mathf.Clamp01(alpha);
            view.MPBAlpha.Clear(); // Clear the material property block

            // Try different alpha application methods in priority order
            if (TryApplyMaterialAlpha(view, alpha)) return;
            if (TryApplySpriteRendererAlpha(view, alpha)) return;
        }

        /// <summary>
        ///     Tries to apply the alpha value using a material property block with "_Alpha".
        /// </summary>
        /// <param name="view"> The view with renderers. </param>
        /// <param name="alpha"> The alpha value. </param>
        /// <returns> Returns true if the alpha was applied using material property block, otherwise false. </returns>
        private static bool TryApplyMaterialAlpha(IViewController view, float alpha)
        {
            Material sharedMaterial = view.Renderers[0].sharedMaterial;
            if (sharedMaterial == null || !sharedMaterial.HasProperty("_Alpha"))
                return false;

            view.MPBAlpha.SetFloat("_Alpha", alpha);
            view.MPBAlpha.SetVector("unity_SpriteColor", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            view.MPBAlpha.SetVector("unity_SpriteProps", new Vector4(1.0f, 1.0f, -1.0f, 0));

            foreach (Renderer renderer in view.Renderers)
            {
                if (renderer == null) continue;
                renderer.SetPropertyBlock(view.MPBAlpha);
            }

            return true;
        }

        /// <summary>
        ///     Tries to apply the alpha value by directly modifying the color of SpriteRenderers.
        /// </summary>
        /// <param name="view"> The view with renderers. </param>
        /// <param name="alpha"> The target alpha value. </param>
        /// <returns> Returns true if at least one <see cref="SpriteRenderer"> was found and modified, otherwise false. </returns>
        private static bool TryApplySpriteRendererAlpha(IViewController view, float alpha)
        {
            bool applied = false;

            foreach (Renderer renderer in view.Renderers)
            {
                if (renderer is SpriteRenderer spriteRenderer)
                {
                    Color newColor = spriteRenderer.color.WithAlpha(alpha);
                    spriteRenderer.color = newColor;
                    applied = true;
                }
            }

            return applied;
        }
    }

}