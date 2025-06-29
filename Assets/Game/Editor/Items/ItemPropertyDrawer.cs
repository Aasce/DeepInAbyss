using Asce.Game.Items;
using Asce.Managers.Utils;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomPropertyDrawer(typeof(ItemProperty), useForChildren: true)]
    public class ItemPropertyDrawer : PropertyDrawer
    {
        private readonly static float buttonHeight = 18f;
        private readonly static float spacing = 5f;
        private List<Type> _types;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.managedReferenceValue == null
                ? EditorGUIUtility.singleLineHeight
                : EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.managedReferenceValue == null)
            {
                this.DrawTypeSelectionButton(position, property);
            }
            else
            {
                Rect boxRect = new (position.x, position.y, position.width, EditorGUI.GetPropertyHeight(property, label, true));
                EditorGUI.PropertyField(boxRect, property, true);
            }

            EditorGUI.EndProperty();
        }

        private void DrawTypeSelectionButton(Rect position, SerializedProperty property)
        {
            float buttonWidth = Mathf.Min(120f, position.width);
            float buttonX = position.x + position.width - buttonWidth;
            float buttonY = position.y + (position.height - buttonHeight) * 0.5f;

            float labelWidth = position.width - buttonWidth - spacing;

            Rect dropdownRect = new(buttonX, buttonY, buttonWidth, buttonHeight);
            Rect labelRect = new(position.x, buttonY, labelWidth, buttonHeight);

            EditorGUI.LabelField(labelRect, "None Property", EditorStyles.label);

            if (GUI.Button(dropdownRect, "Add Property")) this.ShowTypeSelectionMenu(property);
        }

        private void ShowTypeSelectionMenu(SerializedProperty property)
        {
            var menu = new GenericMenu();
            _types ??= TypeCacheUtils.GetConcreteSubclassesOf<ItemProperty>();

            foreach (var type in _types)
            {
                string menuName = TypeCacheUtils.GetMenuName(type);
                menu.AddItem(new GUIContent(menuName), false, () =>
                {
                    property.serializedObject.Update();
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }
    }
}