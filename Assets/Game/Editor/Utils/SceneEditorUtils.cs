using Asce.Managers.Utils;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    public static class SceneEditorUtils
    {
        public static readonly GUIStyle labelStyle = new (GUI.skin.label) { fontSize = 10, alignment = TextAnchor.LowerCenter, richText = true };

        /// <summary>
        ///     Draws a grid in Scene View that dynamically fills the visible area.
        /// </summary>
        /// <param name="origin"> The origin point of the grid. </param>
        /// <param name="cellSize"> The size of each grid cell. </param>
        /// <param name="color"> The color of the grid lines. </param>
        public static void DrawDynamicGrid(Vector3 origin, float cellSize, Color color)
        {
            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (sceneView == null || sceneView.camera == null) return;

            Camera cam = sceneView.camera;
            Vector3 camPos = cam.transform.position;

            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;

            float minX = camPos.x - width / 2f;
            float maxX = camPos.x + width / 2f;
            float minY = camPos.y - height / 2f;
            float maxY = camPos.y + height / 2f;

            int minXGrid = Mathf.FloorToInt((minX - origin.x) / cellSize);
            int maxXGrid = Mathf.CeilToInt((maxX - origin.x) / cellSize);
            int minYGrid = Mathf.FloorToInt((minY - origin.y) / cellSize);
            int maxYGrid = Mathf.CeilToInt((maxY - origin.y) / cellSize);

            Handles.color = color;

            // Vertical lines
            for (int x = minXGrid; x <= maxXGrid; x++)
            {
                Vector3 start = origin + new Vector3(x * cellSize, minYGrid * cellSize, 0);
                Vector3 end = origin + new Vector3(x * cellSize, maxYGrid * cellSize, 0);
                Handles.DrawLine(start, end);
            }

            // Horizontal lines
            for (int y = minYGrid; y <= maxYGrid; y++)
            {
                Vector3 start = origin + new Vector3(minXGrid * cellSize, y * cellSize, 0);
                Vector3 end = origin + new Vector3(maxXGrid * cellSize, y * cellSize, 0);
                Handles.DrawLine(start, end);
            }
        }

        /// <summary>
        ///     Draws a bounds box in the Scene view with an outline and optional fill color.
        /// </summary>
        /// <param name="bounds"> The bounds to draw. </param>
        /// <param name="outline"> The color of the wireframe outline. </param>
        /// <param name="fill"> Optional fill color (with alpha). </param>
        public static void DrawBounds(Bounds bounds, Color outline, Color? fill = null)
        {
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;

            // Draw fill if provided
            if (fill.HasValue)
            {
                Color fillColor = fill.Value;
                Vector3[] verts = new Vector3[4];

                // 2D only - use front face (XY plane)
                verts[0] = new Vector3(center.x - size.x / 2f, center.y - size.y / 2f, center.z);
                verts[1] = new Vector3(center.x - size.x / 2f, center.y + size.y / 2f, center.z);
                verts[2] = new Vector3(center.x + size.x / 2f, center.y + size.y / 2f, center.z);
                verts[3] = new Vector3(center.x + size.x / 2f, center.y - size.y / 2f, center.z);

                Handles.DrawSolidRectangleWithOutline(verts, fillColor, outline);
            }
            else
            {
                // Only draw outline
                Handles.color = outline;
                Handles.DrawWireCube(center, size);
            }
        }


        /// <summary>
        ///     Draws the hitbox as a rotated rectangle using Unity's Handles utility. 
        ///     <br/>
        ///     This is typically used in the Scene view for debugging or visualization.
        /// </summary>
        /// <param name="hitBox"> The HitBox instance to draw. </param>
        /// <param name="ownerPosition"> The world position of the hitbox's owner. </param>
        /// <param name="facingDirection"> 
        ///     The facing direction of the owner (1 for right, -1 for left). 
        ///     <br/>
        ///     If 0, defaults to 1. 
        /// </param>
        public static void DrawBox(Vector2 position, Vector2 size, float angle)
        {
            // Center of the hitbox in world space
            Vector2 center = position;

            // Half-size for calculating corners from center
            Vector2 halfSize = size * 0.5f;

            // Local-space corner points (before rotation)
            Vector2[] corners = new Vector2[]
            {
                new (-halfSize.x, -halfSize.y),
                new (-halfSize.x, halfSize.y),
                new (halfSize.x, halfSize.y),
                new (halfSize.x, -halfSize.y)
            };

            // Create a quaternion for rotating corners by the hitbox angle
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // Apply rotation and translate to world space
            Vector3[] rotatedCorners = new Vector3[4];
            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 rotated = rotation * corners[i];
                rotatedCorners[i] = center + rotated;
            }

            // Draw the solid rectangle with a red outline
            Handles.DrawSolidRectangleWithOutline(
                rotatedCorners,
                new Color(1, 0, 0, 0.05f), // semi-transparent red fill
                Color.red                   // red outline
            );
        }

        public static void DrawBox(Vector2 position, Vector2 size)
        {
            Vector3[] vertices = new Vector3[4];

            float halfWidth = size.x / 2f;
            float halfHeight = size.y / 2f;

            vertices[0] = new Vector3(position.x - halfWidth, position.y - halfHeight);
            vertices[1] = new Vector3(position.x - halfWidth, position.y + halfHeight);
            vertices[2] = new Vector3(position.x + halfWidth, position.y + halfHeight);
            vertices[3] = new Vector3(position.x + halfWidth, position.y - halfHeight);

            Handles.DrawSolidRectangleWithOutline(
                vertices,
                new Color(0, 0, 1, 0.05f), // semi-transparent blue fill
                Color.blue                 // blue outline
            );
        }

        /// <summary>
        /// Draws a 2D draggable point in the Scene view with a label.
        /// Only allows movement in the XY plane. Recommended for 2D projects.
        /// </summary>
        /// <param name="label">The label to display near the point.</param>
        /// <param name="position">Current world position of the point.</param>
        /// <param name="onMoved">Callback that provides the new position if moved.</param>
        public static void DrawDraggablePoint2D(Vector3 position, System.Action<Vector3> onMoved, string label = null)
        {
            Handles.color = Color.red;

            EditorGUI.BeginChangeCheck();

            // Draw a 2D movement handle restricted to XY plane
            Vector3 newPosition = Handles.Slider2D(
                position,
                Vector3.forward,            // The plane normal (Z axis) — movement restricted to XY
                Vector3.right,              // Local X axis
                Vector3.up,                 // Local Y axis
                0.5f,                       // Handle size
                Handles.SphereHandleCap,    // Cap type
                Vector2.zero                // Snap increments (zero = free movement)
            );

            // Draw the label above the point
            if (!string.IsNullOrEmpty(label)) Handles.Label(position + Vector3.up * 0.5f, label, labelStyle);

            // Check for changes and invoke the callback
            if (EditorGUI.EndChangeCheck())
            {
                onMoved?.Invoke(newPosition);
            }
        }

        /// <summary>
        /// Draws a resizable 2D rectangle using corner handles, allowing user interaction.
        /// Returns the updated offset and size.
        /// </summary>
        /// <param name="offset">Offset from the owner position (local center of the box).</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="ownerPosition">The world position of the owner (e.g., transform.position).</param>
        /// <param name="label">Optional label to display above the center of the box.</param>
        /// <returns>Tuple of newOffset and newSize if changed, otherwise null.</returns>
        public static (Vector2 newOffset, Vector2 newSize)? DrawResizableBox2D(
            Vector2 offset,
            Vector2 size,
            Vector2 ownerPosition,
            string label = null, 
            Color color = default,
            Color outlineColor = default)
        {
            // Calculate the center and half size of the box in world space
            Vector2 center = ownerPosition + offset;
            Vector2 halfSize = size * 0.5f;

            // Calculate corners of the box in world space
            Vector2 bottomLeft = center - halfSize;
            Vector2 topRight = center + halfSize;

            Vector2 topLeft = new(bottomLeft.x, topRight.y);
            Vector2 bottomRight = new(topRight.x, bottomLeft.y);

            // Draw the solid rectangle and outline
            if (color == default) color = Color.cyan;
            if (outlineColor == default) outlineColor = color.WithAlpha(0.05f);
            Rect rect = new Rect(center - halfSize, size);
            Handles.color = color;
            Handles.DrawSolidRectangleWithOutline(rect, outlineColor, color);

            // Begin checking for handle movement
            EditorGUI.BeginChangeCheck();

            // Draw draggable handles at the top-left and bottom-right corners
            Vector2 newTopLeft = Handles.Slider2D(
                topLeft,
                Vector3.forward,            // Plane normal for 2D
                Vector3.right,              // X axis direction
                Vector3.up,                 // Y axis direction
                0.1f,                       // Handle size
                Handles.RectangleHandleCap, // Handle shape
                Vector2.zero                // No snapping
            );

            Vector2 newBottomRight = Handles.Slider2D(
                bottomRight,
                Vector3.forward,
                Vector3.right,
                Vector3.up,
                0.1f,
                Handles.RectangleHandleCap,
                Vector2.zero
            );

            // If either handle has moved, compute the new size and offset
            if (EditorGUI.EndChangeCheck())
            {
                Vector2 newCenter = (newTopLeft + newBottomRight) * 0.5f;
                Vector2 newSize = new Vector2(
                    Mathf.Abs(newBottomRight.x - newTopLeft.x),
                    Mathf.Abs(newTopLeft.y - newBottomRight.y)
                );
                Vector2 newOffset = newCenter - ownerPosition;

                // Draw label at updated top-left corner
                if (!string.IsNullOrEmpty(label))
                {
                    Vector2 labelPos = newTopLeft + new Vector2(0.1f, 0.1f);
                    Handles.Label(labelPos, label);
                }

                return (newOffset, newSize);
            }

            // Draw label at original top-left if no change happened
            if (!string.IsNullOrEmpty(label))
            {
                Vector2 labelPos = topLeft + new Vector2(0.1f, 0.5f);
                Handles.Label(labelPos, label);
            }

            return null;
        }

    }
}