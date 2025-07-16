using Asce.Game.Enviroments;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors.Enviroments
{
    [CustomEditor(typeof(SuspensionBridge))]
    public class SuspensionBridgeEditor : Editor
    {
        private SuspensionBridge _bridge;
        private int _numParts = 3;
        private float _space = 3;
        private float _connectArchon = 0.4f;
        private bool _showCustomBridgeControls = true;

        private void OnEnable()
        {
            _bridge = (SuspensionBridge)target;
            _numParts = _bridge.Parts.Count;
            _space = 0.1f;
            _connectArchon = 0.4f;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorLayoutUtils.Foldout(ref _showCustomBridgeControls, "Custom Bridge Controls", () =>
            {
                _numParts = EditorGUILayout.IntField("Number of Platforms", _numParts);
                _space = EditorGUILayout.FloatField("Space", _space);
                _connectArchon = EditorGUILayout.FloatField("Connect Archon", _connectArchon);

                if (GUILayout.Button("Set Bridge"))
                {
                    if (_numParts > 0)
                    {
                        Undo.RegisterCompleteObjectUndo(_bridge.gameObject, "Set Suspension Bridge");
                        SetNumOfParts(_numParts);
                        EditorUtility.SetDirty(_bridge);
                    }
                    else
                    {
                        Debug.LogWarning("Number of parts must be greater than zero");
                    }
                }
            });
        }

        private void SetNumOfParts(int num)
        {
            ClearBridgeParts();
            _bridge.Parts.Clear();

            _bridge.LeftAnchor.localPosition = Vector3.zero;
            if (_bridge.RightAnchor != null)
                _bridge.RightAnchor.localPosition = Vector3.right * (num * _bridge.PartSpace + _space);

            // Start with left anchor's Rigidbody
            Rigidbody2D previousRb = _bridge.LeftAnchor.GetComponent<Rigidbody2D>();

            for (int i = 0; i < num; i++)
            {
                // Instantiate SuspensionBridgePart directly
                SuspensionBridgePart part = (SuspensionBridgePart)PrefabUtility.InstantiatePrefab(_bridge.PartPrefab, _bridge.transform);
                if (part == null)
                {
                    Debug.LogError("Failed to instantiate SuspensionBridgePart prefab.");
                    continue;
                }

                Undo.RegisterCreatedObjectUndo(part.gameObject, "Create Bridge Part");

                part.name = $"{_bridge.PartPrefab.name} {i}";
                part.transform.localPosition = Vector3.right * (i * _bridge.PartSpace + _space);
                _bridge.Parts.Add(part);

                
                // Setup hinge without GetComponent
                if (part.HingeJoint != null)
                {
                    part.HingeJoint.connectedBody = previousRb;
                    part.HingeJoint.autoConfigureConnectedAnchor = false;
                    part.HingeJoint.anchor = Vector2.zero;

                    if (_bridge.Parts[0] == part)
                    {
                        part.HingeJoint.connectedAnchor = Vector2.one * 0.1f;
                    }
                    else part.HingeJoint.connectedAnchor = Vector2.right * _connectArchon;
                    
                    previousRb = part.Rigidbody;
                }
            }

            // Connect last part to right anchor
            if (_bridge.Parts.Count > 0 && _bridge.RightAnchor != null && _bridge.RightAnchor.TryGetComponent(out Rigidbody2D rightRb))
            {
                SuspensionBridgePart lastPart = _bridge.Parts[^1];

                /// Add a new HingeJoint2D to lastPart to connect to right anchor
                HingeJoint2D jointToRight = lastPart.gameObject.AddComponent<HingeJoint2D>();
                jointToRight.connectedBody = rightRb;
                jointToRight.autoConfigureConnectedAnchor = false;
                jointToRight.anchor = Vector2.right * _connectArchon;
                jointToRight.connectedAnchor = Vector2.zero;
            }
        }

        private void ClearBridgeParts()
        {
            foreach (var part in _bridge.Parts)
            {
                if (part != null)
                    Undo.DestroyObjectImmediate(part.gameObject);
            }

            _bridge.Parts.Clear();

            // Clean up extras
            List<Transform> extras = new();
            foreach (Transform child in _bridge.transform)
            {
                if (child != _bridge.LeftAnchor && child != _bridge.RightAnchor && child.GetComponent<SuspensionBridgePart>() == null)
                    extras.Add(child);
            }

            foreach (var t in extras)
                Undo.DestroyObjectImmediate(t.gameObject);
        }
    }
}
