using Asce.Game.Enviroments;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(PlatformPlat))]
    public class PlatformPlatEditor : Editor
    {
        private PlatformPlat _platform;

        private int _numPlatforms = 3;
        private int _selectedSortingLayerIndex;
        private int _orderInLayer;
        private readonly List<Transform> _toDelete = new();

        private bool _showCustomPlatformControls = true;

        private void OnEnable()
        {
            _platform = (PlatformPlat)target;
            _numPlatforms = _platform.transform.childCount;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorLayoutUtils.Foldout(ref _showCustomPlatformControls, "Custom Platform Controls", () =>
            {
                _numPlatforms = EditorGUILayout.IntField("Number of Platforms", _numPlatforms);

                EditorGUILayout.Space();

                string layerName = EditorLayoutUtils.SortingLayerField(ref _selectedSortingLayerIndex, ref _orderInLayer);

                if (GUILayout.Button("Set Platforms"))
                {
                    if (_numPlatforms >= 2)
                    {
                        Undo.RecordObject(_platform.gameObject, "Set Platform Count");
                        SetNumOfPlatform(_numPlatforms);
                        _platform.SetSortingLayerAndOrder(layerName, _orderInLayer);

                        EditorUtility.SetDirty(_platform.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("Num of platform must be >= 2");
                    }
                }
            });
        }


        public void SetNumOfPlatform(int num)
        {
            if (num <= 1) return;

            this.DeletePlatform();

            _platform.LeftEnd.localPosition = Vector3.zero;
            _platform.RightEnd.localPosition = Vector3.right * (num - 2) * _platform.PartSize;

            for (int i = 0; i < num - 2; i++)
            {
                Transform middle = Instantiate(_platform.MiddlePrefab, _platform.transform);

                middle.name = $"{_platform.MiddlePrefab.name} {i}";
                middle.SetParent(_platform.transform, false);
                middle.localPosition = Vector3.right * (i * _platform.PartSize);
            }

            this.UpdateCollider();
        }

        protected void UpdateCollider()
        {
            if (_platform.Collider == null) return;

            float xSize = _numPlatforms * _platform.PartSize;

            Vector2 size = _platform.Collider.size;
            Vector2 offset = _platform.Collider.offset;

            size.x = xSize;
            offset.x = xSize / 2f - _platform.PartSize;

            _platform.Collider.size = size;
            _platform.Collider.offset = offset;
        }

        protected void DeletePlatform()
        {
            foreach (Transform child in _platform.transform)
            {
                if (child != _platform.LeftEnd && child != _platform.RightEnd)
                {
                    _toDelete.Add(child);
                }
            }
            Managers.Utils.TransformUtils.DestroyTransforms(_toDelete);
            _toDelete.Clear();
        }

    }
}