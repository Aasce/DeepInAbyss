using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace Asce.Managers
{
    public static class SaveLoadSystem
    {
        private static bool UseEncryption => EnvLoader.GetBool("USE_ENCRYPTION", true);

        private static byte[] EncryptionKey
        {
            get
            {
                string key = EnvLoader.Get("EncryptionKey");
                if (string.IsNullOrEmpty(key)) key = PlayerPrefs.GetString("Build_EncKey", "");
                return Encoding.UTF8.GetBytes(key);
            }
        }

        private static byte[] EncryptionIV
        {
            get
            {
                string iv = EnvLoader.Get("EncryptionIV");
                if (string.IsNullOrEmpty(iv)) iv = PlayerPrefs.GetString("Build_EncIV", "");
                return Encoding.UTF8.GetBytes(iv);
            }
        }


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
                if (UseEncryption)
                {
                    byte[] encrypted = EncryptString(json);
                    File.WriteAllBytes(path, encrypted);
                }
                else
                {
                    File.WriteAllText(path, json, Encoding.UTF8);
                }
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

                if (UseEncryption)
                {
                    byte[] encrypted = File.ReadAllBytes(path);
                    string json = DecryptString(encrypted);
                    return JsonUtility.FromJson<T>(json);
                }
                else
                {
                    string json = File.ReadAllText(path, Encoding.UTF8);
                    return JsonUtility.FromJson<T>(json);
                }
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

        private static byte[] EncryptString(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = EncryptionKey;
                aes.IV = EncryptionIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                    sw.Flush();
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        private static string DecryptString(byte[] cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = EncryptionKey;
                aes.IV = EncryptionIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream(cipherText))
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        ///     Delete all files and folders inside persistentDataPath to reset the game.
        /// </summary>
        public static void DeleteAllPersistentData()
        {
            string path = Application.persistentDataPath;

            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);

                // Delete all files
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                // Delete all subdirectories
                foreach (DirectoryInfo subDir in directory.GetDirectories())
                {
                    subDir.Delete(true);
                }
            }
        }
    }
}
