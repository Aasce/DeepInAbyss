using UnityEngine;
using UnityEngine.UI;

namespace Asce.Managers.UIs
{
    /// <summary>
    ///     Invisible UI element that can receive raycasts without being visible.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class InvisibleGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear(); // No mesh (completely invisible)
        }
    }
}
