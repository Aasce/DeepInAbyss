using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    public static class EditorLayoutUtils
    {
        public static void Foldout(ref bool state, string label, Action onShow, GUIStyle style = null)
        {
            style ??= EditorStyles.foldout;
            state = EditorGUILayout.Foldout(state, label, true, style);
            if (state)
            {
                EditorGUI.indentLevel++;
                onShow?.Invoke();
                EditorGUI.indentLevel--;
            }
        }

        public static string SortingLayerField(ref int selectedLayerIndex, ref int orderInLayer)
        {
            string[] sortingLayerNames = GetSortingLayerNames();

            // Show Sorting Layer dropdown
            selectedLayerIndex = EditorGUILayout.Popup("Sorting Layer", selectedLayerIndex, sortingLayerNames);

            // Show Order in Layer
            orderInLayer = EditorGUILayout.IntField("Order in Layer", orderInLayer);

            if (selectedLayerIndex >= 0 && selectedLayerIndex < sortingLayerNames.Length)
                return sortingLayerNames[selectedLayerIndex];
            return sortingLayerNames.Length > 0 ? sortingLayerNames[0] : "Default";
        }

        private static string[] GetSortingLayerNames()
        {
            Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, null);
        }
    }
}