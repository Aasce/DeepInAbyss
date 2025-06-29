using Asce.Managers.Attributes;
using Asce.Managers.Utils;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors.Attribures
{

    /// <summary>
    ///     Custom property drawer that displays a sprite field with a preview image on the right,
    ///     over a checkerboard background.
    /// </summary>
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        ///     Cached reference to the custom attribute for performance.
        /// </summary>
        private SpritePreviewAttribute _attribute;

        /// <summary>
        ///     Calculates the total height of the property drawer, including the sprite preview.
        /// </summary>
        /// <param name="property"> The serialized property being drawn. </param>
        /// <param name="label"> The label of the property. </param>
        /// <returns> The total height in pixels required to draw the property and preview. </returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _attribute ??= (SpritePreviewAttribute)attribute;

            // Add space for the sprite preview and a small padding
            return base.GetPropertyHeight(property, label) + _attribute.PreviewSize + 4f;
        }

        /// <summary>
        ///     Draws the sprite field with a checkerboard background and texture preview to the right.
        /// </summary>
        /// <param name="position"> The on-screen rectangle to draw within. </param>
        /// <param name="property"> The serialized property to draw. </param>
        /// <param name="label"> The GUI label to display next to the field. </param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute ??= (SpritePreviewAttribute)attribute;

            // Draw the object field normally (the property value and label)
            Rect fieldRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(fieldRect, property, label);

            // Exit if there is no sprite assigned
            if (property.objectReferenceValue is not Sprite sprite)
                return;

            // Calculate the rect for the sprite preview on the right
            Rect previewRect = new(
                position.x + position.width - _attribute.PreviewSize,
                position.y + EditorGUIUtility.singleLineHeight + 2f,
                _attribute.PreviewSize,
                _attribute.PreviewSize
            );

            // Draw checkerboard background texture
            GUI.DrawTexture(previewRect, TextureUtils.SimpleCheckerTexture, ScaleMode.ScaleAndCrop);

            // Calculate UVs (texture coordinates) for the portion of the sprite to display
            Rect texCoords = new(
                sprite.textureRect.x / sprite.texture.width,
                sprite.textureRect.y / sprite.texture.height,
                sprite.textureRect.width / sprite.texture.width,
                sprite.textureRect.height / sprite.texture.height
            );

            // Draw the sprite's texture with correct UVs and alpha blending
            GUI.DrawTextureWithTexCoords(previewRect, sprite.texture, texCoords, alphaBlend: true);
        }
    }
}