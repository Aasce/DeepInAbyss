using Asce.Game.Spawners;
using Asce.Managers.Utils;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(SpawnPointController))]
    public class SpawnPointControllerEditor : Editor
    {
        private static readonly string numOfPointPrefsKey = "SpawnPointControllerEditor_NumOfPoint";
        public static readonly string pointHolderName = "Points";

        SpawnPointController _spawnPoint;


        private bool _showCustomControls = true;
        private int _numOfPoint = 1;

        protected virtual void OnEnable()
        {
            _spawnPoint = (SpawnPointController)target;
            _numOfPoint = EditorPrefs.GetInt(numOfPointPrefsKey, 1);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorLayoutUtils.Foldout(ref _showCustomControls, "Custom Controls", () =>
            {
                if (GUILayout.Button("Load Points")) this.LoadPoints();
                
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                _numOfPoint = EditorGUILayout.IntField("Number of point", _numOfPoint);
                if (EditorGUI.EndChangeCheck()) EditorPrefs.SetInt(numOfPointPrefsKey, _numOfPoint);
                
                if (GUILayout.Button("Create Points")) this.CreatePoints();
            });
        }

        protected virtual void OnSceneGUI()
        {
            this.DrawPoints();
        }

        private void LoadPoints()
        {
            _spawnPoint.Points.Clear();
            Transform points = _spawnPoint.transform.Find(pointHolderName);
            if (points == null) return;

            int i = 0;
            foreach (Transform point in points) 
            {
                point.name = $"Point {i++}";
                _spawnPoint.Points.Add(point);
            }

            EditorUtility.SetDirty(_spawnPoint);
        }

        private void CreatePoints()
        {
            Undo.RecordObject(_spawnPoint, "Create Points");

            Transform points = _spawnPoint.transform.Find(pointHolderName);
            if (points == null) points = new GameObject().transform;
            else points.DestroyChildren();

            points.SetParent(_spawnPoint.transform);
            points.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            points.name = pointHolderName;

            _spawnPoint.Points.Clear();
            for (int i = 0; i < _numOfPoint; i++)
            {
                Transform newPoint = new GameObject().transform;
                newPoint.SetParent(points);
                newPoint.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                newPoint.name = $"Point {i}";

                _spawnPoint.Points.Add(newPoint);
            }

            EditorUtility.SetDirty(_spawnPoint);
        }

        private void DrawPoints()
        {
            bool isMultiSelect = Selection.objects.Length > 1;

            foreach (Transform point in _spawnPoint.Points)
            {
                if (point == null) continue;

                // Draw draggable point with name label
                SceneEditorUtils.DrawDraggablePoint2D(
                    point.position,
                    (newPosition) =>
                    {
                        Undo.RecordObject(point, $"Move Point {point.name}");
                        point.position = newPosition;
                        EditorUtility.SetDirty(point);
                    }, label: isMultiSelect ? $"[{_spawnPoint.name.ColorWrap(Color.green)}] {point.name}" : point.name);

                // Draw downward raycast indicator
                float raycastDistance = _spawnPoint.RaycastDistance;
                Handles.color = Color.red;
                Handles.DrawLine(point.position, point.position + Vector3.down * raycastDistance);
            }
        }
    }
}