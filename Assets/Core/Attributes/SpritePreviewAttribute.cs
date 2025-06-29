using UnityEngine;

namespace Asce.Managers.Attributes
{
    public class SpritePreviewAttribute : PropertyAttribute
    {
        public float PreviewSize { get; set; }

        public SpritePreviewAttribute(float height = 64f)
        {
            PreviewSize = height;
        }
    }
}