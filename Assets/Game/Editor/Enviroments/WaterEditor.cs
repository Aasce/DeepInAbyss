using Asce.Game.Enviroments;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Asce.Editors.Enviroments
{
    [CustomEditor(typeof(Water))]
    [CanEditMultipleObjects]
    public class WaterEditor : Editor
    {
        private Water _water;

        private void OnEnable()
        {
            _water = (Water)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            root.Add(new VisualElement { style = { height = 10 } });


            Button generateMeshButton = new Button(() =>
            {
                _water.GenerateMesh();
                EditorUtility.SetDirty(_water);
            });
            generateMeshButton.text = "Generate Mesh";
            root.Add(generateMeshButton);

            Button placeEdgeColliderButton = new Button(() =>
            {
                _water.ResetCollider();
                EditorUtility.SetDirty(_water);
            });
            placeEdgeColliderButton.text = "Place Collider";
            root.Add(placeEdgeColliderButton);

            return root;
        }

        private void OnSceneGUI()
        {
            //Draw Wire frame box for the water mesh
            Handles.color = Color.white;
            Vector3 center = _water.transform.position;
            Vector3 size = new Vector3(_water.Width, _water.Height, 0.1f);

            Handles.DrawWireCube(center, size);

            // Handles for width and height
            float handleSize = HandleUtility.GetHandleSize(center) * 0.1f;
            Vector3 snap = Vector3.one * 0.1f;

            // Corner handles
            Vector3[] corners = new Vector3[4];
            corners[0] = center + new Vector3(-_water.Width * 0.5f, -_water.Height * 0.5f, 0); // Bottom Left
            corners[1] = center + new Vector3(_water.Width * 0.5f, -_water.Height * 0.5f, 0); // Bottom Right
            corners[2] = center + new Vector3(-_water.Width * 0.5f, _water.Height * 0.5f, 0); // Top Left
            corners[3] = center + new Vector3(_water.Width * 0.5f, _water.Height * 0.5f, 0); // Top Right

            // Handle for each corner
            EditorGUI.BeginChangeCheck();
            Vector3 newBottomLeft = Handles.FreeMoveHandle(corners[0], handleSize, snap, Handles.CubeHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_water, "Change Water Size");
                float calculatedWidthMax = corners[1].x - newBottomLeft.x;
                float calculatedHeightMax = corners[3].y - newBottomLeft.y;
                ChangeDimensions(calculatedWidthMax, calculatedHeightMax);
                _water.transform.position += new Vector3((newBottomLeft.x - corners[0].x) * 0.5f, (newBottomLeft.y - corners[0].y) * 0.5f, 0);
            }

            EditorGUI.BeginChangeCheck();
            Vector3 newBottomRight = Handles.FreeMoveHandle(corners[1], handleSize, snap, Handles.CubeHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_water, "Change Water Size");
                float calculatedWidthMax = newBottomRight.x - corners[0].x;
                float calculatedHeightMax = corners[3].y - newBottomRight.y;
                ChangeDimensions(calculatedWidthMax, calculatedHeightMax);
                _water.transform.position += new Vector3((newBottomRight.x - corners[1].x) * 0.5f, (newBottomRight.y - corners[1].y) * 0.5f, 0);
            }

            EditorGUI.BeginChangeCheck();
            Vector3 newTopLeft = Handles.FreeMoveHandle(corners[2], handleSize, snap, Handles.CubeHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_water, "Change Water Size");
                float calculatedWidthMax = corners[3].x - newTopLeft.x;
                float calculatedHeightMax = newTopLeft.y - corners[0].y;
                ChangeDimensions(calculatedWidthMax, calculatedHeightMax);
                _water.transform.position += new Vector3((newTopLeft.x - corners[2].x) * 0.5f, (newTopLeft.y - corners[2].y) * 0.5f, 0);
            }

            EditorGUI.BeginChangeCheck();
            Vector3 newTopRight = Handles.FreeMoveHandle(corners[3], handleSize, snap, Handles.CubeHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_water, "Change Water Size");
                float calculatedWidthMax = newTopRight.x - corners[2].x;
                float calculatedHeightMax = newTopRight.y - corners[1].y;
                ChangeDimensions(calculatedWidthMax, calculatedHeightMax);
                _water.transform.position += new Vector3((newTopRight.x - corners[3].x) * 0.5f, (newTopRight.y - corners[3].y) * 0.5f, 0);
            }

            if (GUI.changed)
            {
                _water.GenerateMesh();
                _water.ResetCollider();
                EditorUtility.SetDirty(_water);
            }
        }

        private void ChangeDimensions(float calculatedWidthMax, float calculatedHeightMax)
        {
            _water.Width = Mathf.Max(0.1f, calculatedWidthMax);
            _water.Height = Mathf.Max(0.1f, calculatedHeightMax);
        }
    }
}