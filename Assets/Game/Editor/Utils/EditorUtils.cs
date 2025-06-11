using UnityEngine;
using UnityEditor;

namespace Asce.Editors
{
    public static class EditorUtils
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
    }
}