using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Asce.Managers
{
    public static class SaveLoadSystem
    {
        public static string RootPath => Application.persistentDataPath;

        public static void Save<T>(T data, string fileName)
        {
            try
            {
                string path = GetFullPath(fileName);

                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(path, json, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static T Load<T>(string fileName)
        {
            try
            {
                string path = GetFullPath(fileName);
                if (!File.Exists(path)) return default;

                string json = File.ReadAllText(path, Encoding.UTF8);
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return default;
            }
        }
        public static bool Exists(string fileName)
        {
            return File.Exists(GetFullPath(fileName));
        }

        public static void Delete(string fileName)
        {
            string path = GetFullPath(fileName);
            if (File.Exists(path)) File.Delete(path);
        }

        private static string GetFullPath(string fileName)
        {
            return Path.Combine(RootPath, fileName);
        }
    }
}
