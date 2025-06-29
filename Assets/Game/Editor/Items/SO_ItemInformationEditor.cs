using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Asce.Game.Items;

namespace Asce.Editors
{
    [CustomEditor(typeof(SO_ItemInformation))]
    [CanEditMultipleObjects]
    public class SO_ItemInformationEditor : Editor
    {
        private ReorderableList _propertyList;

        private void OnEnable()
        {
            SerializedProperty listProp = serializedObject.FindProperty("_properties");

            _propertyList = new ReorderableList(serializedObject, listProp, draggable: true, displayHeader: true, displayAddButton: true, displayRemoveButton: true);

            _propertyList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Item Properties");
            };

            _propertyList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUI.indentLevel++;
                var element = listProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
                EditorGUI.indentLevel--;
            };

            _propertyList.elementHeightCallback = index =>
            {
                var element = listProp.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 4f;
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, "_properties");

            _propertyList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}