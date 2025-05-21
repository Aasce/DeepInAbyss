using Asce.Game.Enviroments;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors.Enviroments
{
    [CustomEditor(typeof(SuspensionBridge))]
    public class SuspensionBridgeEditor : Editor
    {
        private SuspensionBridge _bridge;

        private int _numParts = 3;
        private readonly List<Transform> _toDelete = new();

        private bool _showCustomBridgeControls = true;

        private void OnEnable()
        {
            _bridge = (SuspensionBridge)target;
            _numParts = _bridge.transform.childCount - 2; // Exclude anchors
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorLayoutUtils.Foldout(ref _showCustomBridgeControls, "Custom Bridge Controls", () =>
            {
                _numParts = EditorGUILayout.IntField("Number of Platforms", _numParts);

                EditorGUILayout.Space();

                if (GUILayout.Button("Set Bridge"))
                {
                    if (_numParts > 0)
                    {
                        Undo.RecordObject(_bridge.gameObject, "Set Bridge Count");
                        SetNumOfParts(_numParts);

                        EditorUtility.SetDirty(_bridge.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("Num of parts must be greater than zero");
                    }
                }
            });
        }


        public void SetNumOfParts(int num)
        {
            if (num <= 0) return; // Num of parts must be greater than zero

            this.DeleteParts();

            _bridge.LeftAnchor.localPosition = Vector3.zero;
            if (_bridge.RightAnchor != null) _bridge.RightAnchor.localPosition = Vector3.right * (num * _bridge.PartSpace + 0.1f);

            Rigidbody2D previousRigidbody = _bridge.LeftAnchor.GetComponent<Rigidbody2D>();
            for (int i = 0; i < num; i++)
            {
                Transform partObject = Instantiate(_bridge.PartPrefab, _bridge.transform);

                partObject.name = $"{_bridge.PartPrefab.name} {i}";
                partObject.SetParent(_bridge.transform, false);

                partObject.localPosition = Vector3.right * (i * _bridge.PartSpace + 0.1f);

                if (partObject.TryGetComponent(out HingeJoint2D hingeJoint))
                {
                    hingeJoint.connectedBody = previousRigidbody;

                    previousRigidbody = partObject.GetComponent<Rigidbody2D>();

                    if (i == 0)
                    {
                        hingeJoint.connectedAnchor = Vector2.zero;
                    }
                }

                if (i == num - 1)
                {
                    if (_bridge.RightAnchor == null) continue;

                    Rigidbody2D rightAnchorRb = _bridge.RightAnchor.GetComponent<Rigidbody2D>();
                    if (rightAnchorRb == null) continue;

                    HingeJoint2D hingeToRightAnchor = partObject.AddComponent<HingeJoint2D>();
                    hingeToRightAnchor.connectedBody = rightAnchorRb;
                    hingeToRightAnchor.autoConfigureConnectedAnchor = false;
                    hingeToRightAnchor.anchor = Vector2.right * 0.42f;
                    hingeToRightAnchor.connectedAnchor = Vector2.zero;

                    hingeToRightAnchor.useLimits = true;
                    hingeToRightAnchor.limits = new JointAngleLimits2D
                    {
                        min = -15f,
                        max = 15f
                    };                 
                }
            }
        }

        protected void DeleteParts()
        {
            foreach (Transform child in _bridge.transform)
            {
                if (child != _bridge.LeftAnchor && child != _bridge.RightAnchor)
                {
                    _toDelete.Add(child);
                }
            }
            Managers.Utils.TransformUtils.DestroyTransforms(_toDelete);
            _toDelete.Clear();
        }
    }
}