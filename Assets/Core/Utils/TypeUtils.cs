using Asce.Managers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class TypeUtils
    {
        public static List<Type> GetConcreteSubclassesOf<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => SafeGetTypes(x))
                .Where(t => typeof(T).IsAssignableFrom(t)
                            && !t.IsAbstract
                            && !t.IsInterface)
                .ToList();
        }

        public static string GetMenuName(Type type)
        {
            return type.GetCustomAttribute<MenuNameAttribute>()?.DisplayName ?? type.Name;
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
        {
            try 
            { 
                return assembly.GetTypes(); 
            }
            catch (ReflectionTypeLoadException e) 
            { 
                return e.Types.Where(t => t != null); 
            }
        }

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