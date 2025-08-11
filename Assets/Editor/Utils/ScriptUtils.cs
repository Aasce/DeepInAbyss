using UnityEditor;
using UnityEngine;

namespace Asce.Editors.Utils
{
    public static class ScriptUtils
    {
        public static void AssignScriptToPrefab(GameObject prefabInstance, string scriptPath, bool isDebug = true)
        {
            MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
            if (monoScript == null)
            {
                if (isDebug) Debug.LogError($"Could not find script at path: {scriptPath}");
                return;
            }

            System.Type scriptType = monoScript.GetClass();
            if (scriptType == null)
            {
                if (isDebug) Debug.LogError($"Failed to get class from script: {scriptPath}");
                return;
            }

            if (prefabInstance.GetComponent(scriptType) == null)
            {
                prefabInstance.AddComponent(scriptType);
                if (isDebug) Debug.Log($"Added component {scriptType.Name} to prefab.");
            }
            else if (isDebug) Debug.Log($"Component {scriptType.Name} already exists on prefab.");

        }
    }
}