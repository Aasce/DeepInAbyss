using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class UICanvasUtils
    {
        /// <summary>
        ///     Clamps the anchored position of a RectTransform to ensure it stays fully within its direct parent container.
        ///     Works only when the parent is a valid RectTransform.
        /// </summary>
        /// <param name="element">The RectTransform to clamp.</param>
        /// <returns>Clamped anchored position relative to the parent.</returns>
        public static Vector2 ClampToParent(RectTransform element)
        {
            if (element == null) return Vector2.zero;
            if (element.parent == null) return element.anchoredPosition;

            RectTransform parent = element.parent as RectTransform;
            if (parent == null) return element.anchoredPosition;

            Vector2 parentSize = parent.rect.size;
            Vector2 elementSize = element.rect.size;

            // Calculate pivot offset
            Vector2 pivotOffset = new(
                elementSize.x * element.pivot.x,
                elementSize.y * element.pivot.y
            );

            // Compute clamping boundaries based on size and pivot
            Vector2 min = -parentSize * 0.5f + pivotOffset;
            Vector2 max = parentSize * 0.5f - (elementSize - pivotOffset);

            Vector2 current = element.anchoredPosition;

            float clampedX = Mathf.Clamp(current.x, min.x, max.x);
            float clampedY = Mathf.Clamp(current.y, min.y, max.y);

            return new Vector2(clampedX, clampedY);
        }

        /// <summary>
        ///     Clamps the anchored position of an element within a specific container RectTransform,
        ///     even if they are not directly related in the hierarchy.
        ///     This version assumes container is above element in hierarchy.
        /// </summary>
        /// <param name="element">The RectTransform to clamp.</param>
        /// <param name="container">The container RectTransform to clamp within.</param>
        /// <returns>Clamped anchored position in the element's parent space.</returns>
        public static Vector2 ClampToContainer(RectTransform element, RectTransform container)
        {
            if (element == null) return Vector2.zero;
            if (container == null) return element.anchoredPosition;

            // Prevent invalid clamping if container is a child of the element
            if (container.IsChildOf(element)) return element.anchoredPosition;

            // Convert element position to container's local space
            Vector2 localInContainer = container.InverseTransformPoint(element.position);

            Vector2 containerSize = container.rect.size;
            Vector2 elementSize = element.rect.size;

            Vector2 pivotOffset = new(
                elementSize.x * element.pivot.x,
                elementSize.y * element.pivot.y
            );

            Vector2 min = -containerSize * 0.5f + pivotOffset;
            Vector2 max = containerSize * 0.5f - (elementSize - pivotOffset);

            float clampedX = Mathf.Clamp(localInContainer.x, min.x, max.x);
            float clampedY = Mathf.Clamp(localInContainer.y, min.y, max.y);

            // Convert clamped point back to element's parent space
            Vector3 clampedWorld = container.TransformPoint(new Vector3(clampedX, clampedY, 0f));
            Vector2 finalAnchored = element.parent.InverseTransformPoint(clampedWorld);

            return finalAnchored;
        }

        /// <summary>
        ///     Clamps the anchored position of a RectTransform to remain fully within the bounds of a container RectTransform,
        ///     accounting for arbitrary hierarchy, scale, and layout.
        ///     Use this for floating elements, tooltips, or popups across nested canvases.
        /// </summary>
        /// <param name="element">The UI element to clamp.</param>
        /// <param name="container">The container RectTransform that defines the boundary.</param>
        /// <returns>Clamped anchored position in the element's parent local space.</returns>
        public static Vector2 ClampToContainerWorld(RectTransform element, RectTransform container)
        {
            if (element == null || container == null) return Vector2.zero;
            if (container.IsChildOf(element)) return element.anchoredPosition;

            // Get world corners of both element and container
            Vector3[] elementCorners = new Vector3[4];
            Vector3[] containerCorners = new Vector3[4];

            element.GetWorldCorners(elementCorners);
            container.GetWorldCorners(containerCorners);

            Vector2 elementWorldSize = elementCorners[2] - elementCorners[0];

            Vector2 pivotOffset = new(
                element.rect.width * element.pivot.x,
                element.rect.height * element.pivot.y
            );

            // Convert element pivot world position to container local space
            Vector2 elementPivotWorld = element.position;
            Vector2 localInContainer = container.InverseTransformPoint(elementPivotWorld);

            Vector2 containerSize = container.rect.size;

            Vector2 min = -containerSize * 0.5f + pivotOffset;
            Vector2 max = containerSize * 0.5f - (element.rect.size - pivotOffset);

            float clampedX = Mathf.Clamp(localInContainer.x, min.x, max.x);
            float clampedY = Mathf.Clamp(localInContainer.y, min.y, max.y);

            // Convert clamped point back to world then to element's parent space
            Vector3 clampedWorld = container.TransformPoint(new Vector3(clampedX, clampedY, 0f));
            Vector2 clampedAnchored = element.parent.InverseTransformPoint(clampedWorld);

            return clampedAnchored;
        }

        /// <summary>
        /// Clamps a local position to remain inside a container RectTransform, assuming both are in the same canvas space.
        /// </summary>
        public static Vector2 ClampLocalAnchoredPosition(RectTransform element, RectTransform container, Vector2 targetLocalPos)
        {
            Vector2 containerSize = container.rect.size;
            Vector2 elementSize = element.rect.size;

            Vector2 pivotOffset = new(
                elementSize.x * element.pivot.x,
                elementSize.y * element.pivot.y
            );

            Vector2 min = -containerSize * 0.5f + pivotOffset;
            Vector2 max = containerSize * 0.5f - (elementSize - pivotOffset);

            float clampedX = Mathf.Clamp(targetLocalPos.x, min.x, max.x);
            float clampedY = Mathf.Clamp(targetLocalPos.y, min.y, max.y);

            return new Vector2(clampedX, clampedY);
        }
    }
}
