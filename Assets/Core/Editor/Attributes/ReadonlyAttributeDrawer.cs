using Asce.Managers.Attributes;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors.Attribures
{
    [CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    public class ReadonlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
