using Asce.Game.Enviroments;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(Ladder))]
    public class LadderEditor : Editor
    {
        private Ladder _ladder;

        private int _numOfParts = 3;
        private int _selectedSortingLayerIndex;
        private int _orderInLayer;
        private readonly List<Transform> _toDelete = new();

        private bool _showCustomLadderControls = true;

        private void OnEnable()
        {
            _ladder = (Ladder)target;
            _numOfParts = _ladder.transform.childCount;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorLayoutUtils.Foldout(ref _showCustomLadderControls, "Custom Ladder Controls", () =>
            {
                _numOfParts = EditorGUILayout.IntField("Number of parts", _numOfParts);

                EditorGUILayout.Space();

                string layerName = EditorLayoutUtils.SortingLayerField(ref _selectedSortingLayerIndex, ref _orderInLayer);

                if (GUILayout.Button("Set Ladder"))
                {
                    if (_numOfParts >= 2)
                    {
                        Undo.RecordObject(_ladder.gameObject, "Set Ladder parts Count");
                        SetNumOfLadderParts(_numOfParts);
                        _ladder.SetSortingLayerAndOrder(layerName, _orderInLayer);

                        EditorUtility.SetDirty(_ladder.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("Num of parts must be >= 2");
                    }
                }
            });
        }

        public void SetNumOfLadderParts(int num)
        {
            if (num <= 1) return;

            this.DeleteLadderPart();

            _ladder.TopPart.localPosition = Vector3.zero;
            _ladder.BottomPart.localPosition = Vector3.down * (num - 1) * _ladder.PartSize;

            for (int i = 1; i < num - 1; i++)
            {
                Transform middlePart = _ladder.MiddleParts.GetRandomElement();
                Transform middle = (Transform)PrefabUtility.InstantiatePrefab(middlePart, _ladder.transform);

                middle.name = $"{middlePart.name} {i}";
                middle.SetParent(_ladder.transform, false);
                middle.localPosition = Vector3.down * i * _ladder.PartSize;
            }

            this.UpdateCollider();
        }

        protected void UpdateCollider()
        {
            if (_ladder.Collider == null) return;

            float ySize = _numOfParts * _ladder.PartSize;

            Vector2 size = _ladder.Collider.size;
            Vector2 offset = _ladder.Collider.offset;

            size.y = ySize;
            offset.y = -ySize * 0.5f;

            _ladder.Collider.size = size;
            _ladder.Collider.offset = offset;
        }

        protected void DeleteLadderPart()
        {
            foreach (Transform child in _ladder.transform)
            {
                if (child != _ladder.TopPart && child != _ladder.BottomPart)
                {
                    _toDelete.Add(child);
                }
            }
            Managers.Utils.TransformUtils.DestroyTransforms(_toDelete);
            _toDelete.Clear();
        }
    }
}