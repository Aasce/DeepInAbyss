using Asce.Game.Items;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors.Windows
{
    public class ItemInformationLoaderWindow : EditorWindow
    {
        private static readonly string _itemsDataPrefsKey = "ItemInformationLoaderWindow_ItemsData";

        private SO_ItemsData _itemsData;
        private readonly List<SO_ItemInformation> _itemInformations = new();

        // Window config
        private Vector2 _mainScrollPosition;
        private Vector2 _scrollPosition;

        [MenuItem("Asce/Windows/Item Information Loader")]
        public static void ShowWindow()
        {
            GetWindow<ItemInformationLoaderWindow>(title: "Item Information Loader");
        }

        private void OnEnable()
        {
            string dataPath = EditorPrefs.GetString(_itemsDataPrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(dataPath))
                _itemsData = AssetDatabase.LoadAssetAtPath<SO_ItemsData>(dataPath);
        }

        private void OnGUI()
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(_mainScrollPosition))
            {
                _mainScrollPosition = scroll.scrollPosition;

                GUILayout.Label("Load Item Information into Items Data", EditorStyles.boldLabel);

                this.DrawItemDataField();
                this.DrawItemInfoDragAndDropField();
            }

            // Load Button
            EditorGUILayout.Space();
            if (GUILayout.Button("Load into Items Data")) this.LoadItemsIntoData();
            EditorGUILayout.Space();
        }

        private void LoadItemsIntoData()
        {
            if (_itemsData == null)
            {
                Debug.LogError("Please assign a SO_ItemsData.");
                return;
            }

            int loadedCount = 0;
            foreach (var item in _itemInformations)
            {
                if (item == null) continue;

                _itemsData.AddItem(item);
                loadedCount++;
            }

            EditorUtility.SetDirty(_itemsData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Loaded {loadedCount} item(s) into {_itemsData.name}.");
            _itemInformations.Clear();
        }


        private void DrawItemDataField()
        {
            _itemsData = EditorLayoutUtils.ObjectFieldSaveRefs(_itemsData, _itemsDataPrefsKey, "Item Data");
            if (_itemsData != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Update", GUILayout.Width(100))) _itemsData.UpdateData();
                GUILayout.EndHorizontal();
            }
        }

        private void DrawItemInfoDragAndDropField()
        {
            EditorGUILayout.Space();

            List<SO_ItemInformation> dropped = EditorLayoutUtils.DragAndDropArea<SO_ItemInformation>(
                label: "Drag and Drop Item Informations",
                boxMessage: "Drop Item Information assets here",
                boxHeight: 30f
            );

            foreach (var item in dropped)
                if (item != null && !_itemInformations.Contains(item))
                    _itemInformations.Add(item);

            EditorLayoutUtils.ObjectListField(
                objects: _itemInformations,
                scrollPosition: ref _scrollPosition,
                labelPrefix: "Item",
                maxHeight: 180f,
                getName: (item) => item != null ? item.name : "Item"
            );
        }
    }
}
