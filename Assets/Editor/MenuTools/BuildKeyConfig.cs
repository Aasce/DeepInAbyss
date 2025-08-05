using Asce.Managers;
using UnityEditor;
using UnityEngine;

namespace Asce.Editors
{
    public class BuildKeyConfig
    {
        [MenuItem("Tools/Apply Encryption Key from .env")]
        public static void ApplyKeyFromEnv()
        {
            string key = EnvLoader.Get("EncryptionKey");
            string iv = EnvLoader.Get("EncryptionIV");

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(iv))
            {
                Debug.LogError("EncryptionKey or EncryptionIV is empty!");
                return;
            }

            PlayerPrefs.SetString("Build_EncKey", key);
            PlayerPrefs.SetString("Build_EncIV", iv);
            Debug.Log("Key/IV saved to PlayerPrefs for build.");
        }
    }
}