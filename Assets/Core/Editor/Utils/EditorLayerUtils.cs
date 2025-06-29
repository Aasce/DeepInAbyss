using System;
using System.Reflection;

public static class EditorLayerUtils
{
    public static string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, null);
    }
}
