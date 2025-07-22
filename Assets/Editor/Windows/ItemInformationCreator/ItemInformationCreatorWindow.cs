using Asce.Game.Equipments;
using Asce.Game.Items;
using Asce.Managers.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors.Windows
{
    public class ItemInformationCreatorWindow : EditorWindow
    {
        private static readonly string _targetFolderPrefsKey = "ItemInformationCreatorWindow_TargetFolder";
        private static readonly string _itemsDataPrefsKey = "ItemInformationCreatorWindow_ItemsData";

        private DefaultAsset _targetFolder;
        private SO_ItemsData _itemsData;

        private ItemType _selectedItemType = ItemType.Unknown;
        private ItemPropertyType _selectedPropertyType = ItemPropertyType.None;
        private readonly List<Sprite> _sprites = new();

        // Window config
        private Vector2 _mainScrollPosition;
        private Vector2 _spriteScrollPosition;

        [MenuItem("Asce/Windows/Item Information Creator")]
        public static void ShowWindow()
        {
            GetWindow<ItemInformationCreatorWindow>(title: "Item Information Creator");
        }

        private void OnEnable()
        {
            string folderPath = EditorPrefs.GetString(_targetFolderPrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(folderPath))
                _targetFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath);

            string dataPath = EditorPrefs.GetString(_itemsDataPrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(dataPath))
                _itemsData = AssetDatabase.LoadAssetAtPath<SO_ItemsData>(dataPath);
        }

        private void OnGUI()
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(_mainScrollPosition))
            {
                _mainScrollPosition = scroll.scrollPosition;
                GUILayout.Label("Create Item Information Assets", EditorStyles.boldLabel);

                // Target Folder
                _targetFolder = EditorLayoutUtils.ObjectFieldSaveRefs(_targetFolder, _targetFolderPrefsKey, "Target Folder");

                this.DrawItemDataField();
                this.DrawSpriteLoader();
                this.DrawItemInformationField();

            }

            // Create Button
            EditorGUILayout.Space();
            if (GUILayout.Button("Create Item Information Assets")) this.CreateItemInformationAssets();
            EditorGUILayout.Space();
        }

        #region - DRAWER -
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

        private void DrawItemInformationField()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Items information", EditorStyles.boldLabel);

            _selectedItemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", _selectedItemType);
            _selectedPropertyType = (ItemPropertyType)EditorGUILayout.EnumFlagsField("Item Properties", _selectedPropertyType);
        }

        private void DrawSpriteLoader()
        {
            EditorGUILayout.Space();
            List<Object> dropped = EditorLayoutUtils.DragAndDropArea<Object>(
                label: "Drag and Drop Sprites or Textures",
                boxMessage: "Drop Sprite or Texture2D assets here",
                boxHeight: 30f
            );

            foreach (Object obj in dropped)
            {
                if (obj is Sprite sprite)
                {
                    if (!_sprites.Contains(sprite)) _sprites.Add(sprite);
                }
                else if (obj is Texture2D texture)
                {
                    string path = AssetDatabase.GetAssetPath(texture);
                    Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

                    foreach (Object subAsset in subAssets)
                    {
                        if (subAsset is Sprite subSprite && !_sprites.Contains(subSprite))
                            _sprites.Add(subSprite);
                    }
                }
            }

            EditorLayoutUtils.ObjectListField(
                objects: _sprites,
                scrollPosition: ref _spriteScrollPosition,
                labelPrefix: "Sprite",
                getName: (sprite) => sprite != null ? sprite.name : "Sprite",
                objectHeight: 68f
            );
        }

        #endregion

        private void CreateItemInformationAssets()
        {
            if (_targetFolder == null || _itemsData == null)
            {
                Debug.LogError("Please assign a target folder and SO_ItemsData.");
                return;
            }

            string folderPath = AssetDatabase.GetAssetPath(_targetFolder);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Selected target is not a valid folder.");
                return;
            }

            _itemsData.UpdateData();
            int createdCount = 0;
            foreach (Sprite sprite in _sprites)
            {
                if(this.CreateAsset(sprite, folderPath)) createdCount++;
            }

            EditorUtility.SetDirty(_itemsData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Created {createdCount} item(s).", _targetFolder);
            _sprites.Clear();
        }

        private bool CreateAsset(Sprite sprite, string folderPath)
        {
            if (sprite == null) return false;

            string itemName = sprite.name;
            string assetPath = Path.Combine(folderPath, $"{itemName}.asset");

            // Check if already exists
            SO_ItemInformation existing = AssetDatabase.LoadAssetAtPath<SO_ItemInformation>(assetPath);
            if (existing != null)
            {
                Debug.LogWarning($"Item {itemName} already exists at {assetPath}, skipping...", existing);
                return false;
            }

            // Create correct subclass instance
            SO_ItemInformation newItem = CreateItemInformationByType(_selectedItemType);
            if (newItem == null)
            {
                Debug.LogError($"Unsupported item type: {_selectedItemType}, cannot create asset.");
                return false;
            }

            Undo.RegisterCreatedObjectUndo(newItem, "Create Item Information");

            // Initialize properties
            SerializedObject serializedObject = new SerializedObject(newItem);
            serializedObject.FindProperty("_name").stringValue = itemName;
            serializedObject.FindProperty("_icon").objectReferenceValue = sprite;
            serializedObject.FindProperty("_type").enumValueIndex = (int)_selectedItemType;
            serializedObject.FindProperty("_propertyType").longValue = System.Convert.ToInt64(_selectedPropertyType);

            var propertiesProp = serializedObject.FindProperty("_properties");
            propertiesProp.ClearArray();

            foreach (ItemPropertyType flag in _selectedPropertyType.GetFlags())
            {
                ItemProperty newItemProperty = flag.CreatePropertyFromType();
                if (newItemProperty != null)
                {
                    propertiesProp.arraySize++;
                    propertiesProp.GetArrayElementAtIndex(propertiesProp.arraySize - 1).managedReferenceValue = newItemProperty;
                }
            }

            serializedObject.ApplyModifiedProperties();

            AssetDatabase.CreateAsset(newItem, assetPath);
            AssetDatabase.SaveAssets();

            Undo.RecordObject(_itemsData, "Add Item To ItemsData");
            _itemsData.AddItem(newItem);
            return true;
        }

        private SO_ItemInformation CreateItemInformationByType(ItemType type)
        {
            switch (type)
            {
                case ItemType.Weapon:
                    return ScriptableObject.CreateInstance<SO_WeaponInformation>();
                default:
                    return ScriptableObject.CreateInstance<SO_ItemInformation>();
            }
        }
    }
}
