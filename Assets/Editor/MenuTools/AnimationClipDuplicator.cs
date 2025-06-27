using System.IO;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    public class AnimationClipDuplicator
    {
        [MenuItem("Tools/Duplicate Animation Clip")]
        public static void DuplicateAnimation()
        {
            var originalClip = Selection.activeObject as AnimationClip;
            if (originalClip == null)
            {
                Debug.LogError("Please select an AnimationClip to duplicate.");
                return;
            }

            string path = AssetDatabase.GetAssetPath(originalClip);
            string newPath = Path.GetDirectoryName(path) + "/" + originalClip.name + "_Copy.anim";

            AnimationClip newClip = new AnimationClip();
            EditorUtility.CopySerialized(originalClip, newClip);
            AssetDatabase.CreateAsset(newClip, newPath);
            AssetDatabase.SaveAssets();

            Debug.Log("Copied to: " + newPath);
        }
    }
}