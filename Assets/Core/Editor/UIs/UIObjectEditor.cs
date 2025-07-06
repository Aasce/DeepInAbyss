using Asce.Managers.UIs;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors.UIs
{
    [CustomEditor(typeof(UIObject), editorForChildClasses: true)]
    public class UIObjectEditor : Editor
    {
        protected static MethodInfo _refResetMethod = typeof(UIObject).GetMethod("RefReset", BindingFlags.Instance | BindingFlags.NonPublic);

        [MenuItem("CONTEXT/UIObject/Ref Reset")]
        protected static void RefReset(MenuCommand command)
        {
            UIObject uiObject = (UIObject)command.context;
            Undo.RecordObject(uiObject, "Ref Reset");

            CallRefReset(uiObject);

            EditorUtility.SetDirty(uiObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(uiObject.gameObject.scene);
        }

        protected static void CallRefReset(UIObject uiObject)
        {
            if (_refResetMethod != null) _refResetMethod.Invoke(uiObject, null);
            else Debug.LogWarning($"Method 'RefReset' not found on {uiObject.GetType().Name}");
        }
    }
}