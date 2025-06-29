using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
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

        /// <summary>
        ///     Creates a drag-and-drop area and returns the dropped objects of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of object to accept (e.g., Sprite, Texture2D, GameObject) </typeparam>
        /// <param name="label"> Label to show above the box </param>
        /// <param name="boxMessage"> Message inside the drop box </param>
        /// <param name="boxHeight"> Height of the drop box </param>
        /// <param name="filter"> Optional filter to further validate objects </param>
        /// <returns> List of valid dropped objects </returns>
        public static List<T> DragAndDropArea<T>(
            string label = null,
            string boxMessage = "Drop objects here",
            float boxHeight = 50f,
            Func<T, bool> filter = null
        ) where T : UnityEngine.Object
        {
            List<T> droppedObjects = new();
            Event evt = Event.current;

            // Optional label above the box
            if (!string.IsNullOrEmpty(label)) EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            // Create the rectangular drop zone
            Rect dropArea = GUILayoutUtility.GetRect(0, boxHeight, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, boxMessage);

            // Only handle DragUpdated or DragPerform events
            if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform)
                return droppedObjects;

            // Return if mouse is not over drop area
            if (!dropArea.Contains(evt.mousePosition))
                return droppedObjects;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy; // Show visual copy cursor

            // If the user released the mouse to drop the object(s)
            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                {
                    // Only accept valid type + pass optional filter
                    if (obj is T validObj)
                    {
                        if (filter == null || filter.Invoke(validObj)) 
                            droppedObjects.Add(validObj);
                    }
                }

                evt.Use(); // Mark event as handled
            }

            return droppedObjects;
        }

        /// <summary>
        ///     Draws a list of ObjectFields for displaying object references.
        /// </summary>
        /// <typeparam name="T">Type of object (e.g., Sprite, GameObject)</typeparam>
        /// <param name="objects">List of objects to display</param>
        /// <param name="labelPrefix">Prefix label for each field</param>
        public static void ObjectListField<T>(
            List<T> objects, 
            ref Vector2 scrollPosition,
            string labelPrefix = null, 
            Func<T, string> getName =  null,
            float maxHeight = 200f, 
            float objectHeight = 18f
        ) where T : UnityEngine.Object
        {
            if (objects == null) return;

            EditorGUILayout.Space();
            if (labelPrefix != null) EditorGUILayout.LabelField($"{labelPrefix} ({objects.Count}):", EditorStyles.label);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (objects.Count == 0) EditorGUILayout.LabelField("List is empty");
                else
                {
                    float estimatedHeight = objects.Count * objectHeight;
                    float height = Mathf.Min(estimatedHeight, maxHeight);

                    using EditorGUILayout.ScrollViewScope scroll = new(scrollPosition, GUILayout.Height(height));
                    scrollPosition = scroll.scrollPosition;

                    for (int i = 0; i < objects.Count; i++)
                    {
                        string name = (getName == null) ? $"{labelPrefix ?? string.Empty} {i}" : getName.Invoke(objects[i]);
                        objects[i] = (T)EditorGUILayout.ObjectField(name, objects[i], typeof(T), false);
                    }
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear", GUILayout.Width(100))) objects.Clear();
            GUILayout.EndHorizontal();
        }

        public static T ObjectFieldSaveRefs<T>(T obj, string objPrefsKey, string label = null) where T : UnityEngine.Object
        {
            T newObject = EditorGUILayout.ObjectField(label ?? string.Empty, obj, typeof(T), false) as T;
            if (newObject != null && newObject != obj)
            {
                obj = newObject;
                if (obj != null)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    EditorPrefs.SetString(objPrefsKey, path);
                }
            }
            return newObject;
        }

        public static string SortingLayerField(ref int selectedLayerIndex, ref int orderInLayer)
        {
            string[] sortingLayerNames = EditorLayerUtils.GetSortingLayerNames();

            // Show Sorting Layer dropdown
            selectedLayerIndex = EditorGUILayout.Popup("Sorting Layer", selectedLayerIndex, sortingLayerNames);

            // Show Order in Layer
            orderInLayer = EditorGUILayout.IntField("Order in Layer", orderInLayer);

            if (selectedLayerIndex >= 0 && selectedLayerIndex < sortingLayerNames.Length)
                return sortingLayerNames[selectedLayerIndex];
            return sortingLayerNames.Length > 0 ? sortingLayerNames[0] : "Default";
        }
    }
}