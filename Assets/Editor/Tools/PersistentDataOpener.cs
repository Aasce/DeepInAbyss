using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace Asce.Editors
{
    public class PersistentDataOpener
    {
        [MenuItem("Asce/Open Persistent Data Folder _%e")]
        public static void OpenPersistentDataFolder()
        {
            string path = Application.persistentDataPath;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                UnityEngine.Debug.Log($"Created persistent data folder at: {path}");
            }

            OpenInFileExplorer(path);
        }

        private static void OpenInFileExplorer(string path)
        {
#if UNITY_EDITOR_WIN
            var processInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(processInfo);
#else
        UnityEngine.Debug.LogWarning("This tool is designed for Windows platform only.");
#endif
        }
    }
}
