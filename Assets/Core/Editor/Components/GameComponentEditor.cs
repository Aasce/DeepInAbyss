using Asce.Managers;
using Asce.Managers.SaveLoads;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(GameComponent), editorForChildClasses: true)]
    public class GameComponentEditor : Editor
    {
        protected static readonly BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        #region - REF RESET -
        protected static MethodInfo _refResetMethod = typeof(GameComponent).GetMethod("RefReset", _bindingFlags);

        [MenuItem("CONTEXT/GameComponent/Ref Reset")]
        protected static void RefReset(MenuCommand command)
        {
            GameComponent gameComponent = (GameComponent)command.context;
            Undo.RecordObject(gameComponent, "Ref Reset");

            CallRefReset(gameComponent);

            EditorUtility.SetDirty(gameComponent);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameComponent.gameObject.scene);
        }

        protected static void CallRefReset(GameComponent gameComponent)
        {
            if (_refResetMethod != null)
                _refResetMethod.Invoke(gameComponent, null);
            else
                Debug.LogWarning($"Method 'RefReset' not found on {gameComponent.GetType().Name}");
        }
        #endregion

        #region - GENERATE ID -
        private static readonly string _idFieldName = "_id";

        [MenuItem("CONTEXT/GameComponent/Generate ID", validate = true)]
        private static bool ValidateGenerateID(MenuCommand command)
        {
            return command.context is IUniqueIdentifiable;
        }

        [MenuItem("CONTEXT/GameComponent/Generate ID")]
        private static void GenerateID(MenuCommand command)
        {
            if (command.context is not IUniqueIdentifiable identifiable)
            {
                Debug.LogWarning("Selected object does not implement IUniqueIdentifiable.");
                return;
            }

            var obj = (Object)command.context;
            var type = obj.GetType();
            var field = type.GetField(_idFieldName, _bindingFlags);

            if (field == null || field.FieldType != typeof(string))
            {
                Debug.LogError($"Field '{_idFieldName}' not found or not string on type {type.Name}.");
                return;
            }

            string currentID = (string)field.GetValue(obj);

            if (string.IsNullOrEmpty(currentID))
            {
                Undo.RecordObject(obj, "Generate ID");
                string newID = System.Guid.NewGuid().ToString();
                field.SetValue(obj, newID);

                Debug.Log($"Generated new ID for {obj.name}: {newID}");

                EditorUtility.SetDirty(obj);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(((Component)obj).gameObject.scene);
            }
            else Debug.LogWarning($"{obj.name} already has ID: {currentID}");
        }
        #endregion

    }
}
