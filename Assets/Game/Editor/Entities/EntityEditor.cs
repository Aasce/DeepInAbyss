using Asce.Game.Entities;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    [CustomEditor(typeof(Entity), true)]
    public class EntityEditor : Editor
    {
        protected static MethodInfo _refResetMethod = typeof(Entity).GetMethod("RefReset", BindingFlags.Instance | BindingFlags.NonPublic);

        [MenuItem("CONTEXT/Entity/Ref Reset")]
        protected static void RefReset(MenuCommand command)
        {
            Entity entity = (Entity)command.context;
            Undo.RecordObject(entity, "Ref Reset");

            CallRefReset(entity);

            EditorUtility.SetDirty(entity);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(entity.gameObject.scene);
        }

        protected static void CallRefReset(Entity entity)
        {
            if (_refResetMethod != null) _refResetMethod.Invoke(entity, null);
            else Debug.LogWarning($"Method 'RefReset' not found on {entity.GetType().Name}");
        }
    }
}