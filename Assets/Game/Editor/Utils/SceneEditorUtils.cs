using Asce.Game.Combats;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    public static class SceneEditorUtils
    {
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
        public static void DrawHitBox(this HitBox hitBox, Vector2 ownerPosition = default, int facingDirection = 0)
        {
            if (hitBox == null) return;

            // Get the hitbox world position, size, and rotation angle
            Vector2 position = hitBox.GetPosition(ownerPosition, facingDirection == 0 ? 1 : facingDirection);
            Vector2 size = hitBox.GetSize();
            float angle = hitBox.GetAngle();

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

    }
}