using Asce.Game.Spawners;
using Asce.Managers.Utils;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(SpawnAreaController))]
    public class SpawnAreaControllerEditor : Editor
    {
        SpawnAreaController _spawnArea;

        // DrawSpawnArea fields
        const float sampleCountPerUint = 4f;
        Vector3[] polygonPoints;
        private Vector2 _catchCenter;
        private Vector2 _catchSize;

        protected virtual void OnEnable()
        {
            _spawnArea = (SpawnAreaController)target;
        }

        protected virtual void OnSceneGUI()
        {
            Box box = _spawnArea.Box;

            var result = SceneEditorUtils.DrawResizableBox2D(box.Offset, box.Size, _spawnArea.transform.position, 
                label: _spawnArea.name, 
                color: Color.cyan, 
                outlineColor: Color.white.WithAlpha(0f));

            if (result.HasValue)
            {
                Undo.RecordObject(_spawnArea, "Modify Spawn Area Box");
                _spawnArea.Box.Offset = result.Value.newOffset;
                _spawnArea.Box.Size = result.Value.newSize;
                EditorUtility.SetDirty(_spawnArea);
            }
            this.DrawSpawnArea((Vector2)_spawnArea.transform.position + box.Offset, box.Size);
            this.DrawSpawnRaycast();
        }

        private void DrawSpawnRaycast()
        {
            // Only draw when game is running
            if (!EditorApplication.isPlaying)
                return;

            Vector2 origin = _spawnArea.Origin;
            float raycastDistance = origin.y - _spawnArea.BottomY;

            Handles.color = Color.red;
            Handles.DrawWireArc(origin, Vector3.forward, Vector3.right, 360f, 0.25f);
            Handles.DrawLine(origin, origin + Vector2.down * raycastDistance);
        }

        private void DrawSpawnArea(Vector2 center, Vector2 size, Color color = default)
        {
            if (_catchCenter != center || _catchSize != size) {
                Vector2 halfSize = size * 0.5f;
                LayerMask layerMask = _spawnArea.RaycastLayerMask;

                int sampleCount = Mathf.RoundToInt(Mathf.Ceil(size.x * sampleCountPerUint));
                sampleCount = Mathf.Max(sampleCount, 2); // Avoid degenerate polygon

                Vector2 topLeft = center + new Vector2(-halfSize.x, halfSize.y);
                Vector2 topRight = center + new Vector2(halfSize.x, halfSize.y);

                // 1 (topLeft) + sampleCount (raycasts) + 1 (topRight) = totalPoints
                int totalPoints = sampleCount + 3;
                polygonPoints = new Vector3[totalPoints];

                // Add top-left corner
                polygonPoints[0] = topLeft;

                // Raycast down across the top edge
                for (int i = 0; i < sampleCount; i++)
                {
                    float t = i / (sampleCount - 1f);
                    Vector2 lerpPosition = Vector2.Lerp(topLeft, topRight, t);
                    RaycastHit2D hit = Physics2D.Raycast(lerpPosition, Vector2.down, size.y, layerMask);

                    Vector2 point = hit.collider != null
                        ? hit.point
                        : lerpPosition + Vector2.down * size.y;

                    polygonPoints[i + 1] = point;
                }

                // Add top-right corner at the end
                polygonPoints[totalPoints - 2] = topRight;
                polygonPoints[totalPoints - 1] = topLeft;

                _catchCenter = center;
                _catchSize = size;
            }

            // Draw outline
            if (color == default) color = Color.green;
            Handles.color = color;
            Handles.DrawAAPolyLine(6f, polygonPoints);
        }
    }
}
