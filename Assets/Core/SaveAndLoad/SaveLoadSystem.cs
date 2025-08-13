using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace Asce.Managers
{
    public static class SaveLoadSystem
    {
        private static bool UseEncryption
        {
            get
            {
                if (!EnvLoader.GetBool("USE_ENCRYPTION", true))
                {
                    Debug.Log("[SaveLoadSystem] Encryption disabled by config.");
                    return false;
                }

                var key = EncryptionKey;
                var iv = EncryptionIV;

                bool validKey = key.Length == 16 || key.Length == 24 || key.Length == 32;
                bool validIV = iv.Length == 16;

                if (!validKey || !validIV)
                {
                    Debug.LogWarning($"[SaveLoadSystem] Invalid AES key/IV length. " +
                                     $"Key={key.Length} bytes, IV={iv.Length} bytes. Encryption disabled.");
                    return false;
                }

                return true;
            }
        }

        private static byte[] EncryptionKey
        {
            get
            {
                var keyBytes = EnvLoader.GetBytesFromBase64("EncryptionKey");
                if (keyBytes.Length == 0)
                {
                    string fromPrefs = PlayerPrefs.GetString("Build_EncKey", "");
                    if (!string.IsNullOrEmpty(fromPrefs))
                    {
                        try { keyBytes = Convert.FromBase64String(fromPrefs); }
                        catch { Debug.LogWarning("[SaveLoadSystem] Failed to decode Build_EncKey from PlayerPrefs."); }
                    }
                }
                return keyBytes;
            }
        }

        private static byte[] EncryptionIV
        {
            get
            {
                var ivBytes = EnvLoader.GetBytesFromBase64("EncryptionIV");
                if (ivBytes.Length == 0)
                {
                    string fromPrefs = PlayerPrefs.GetString("Build_EncIV", "");
                    if (!string.IsNullOrEmpty(fromPrefs))
                    {
                        try { ivBytes = Convert.FromBase64String(fromPrefs); }
                        catch { Debug.LogWarning("[SaveLoadSystem] Failed to decode Build_EncIV from PlayerPrefs."); }
                    }
                }
                return ivBytes;
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
                    if (encrypted != null)
                        File.WriteAllBytes(path, encrypted);
                    else
                        File.WriteAllText(path, json, Encoding.UTF8);
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
                    if (json != null)
                        return JsonUtility.FromJson<T>(json);
                    else
                        return default;
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

        public static void DeleteAllPersistentData()
        {
            string path = Application.persistentDataPath;

            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                foreach (FileInfo file in directory.GetFiles())
                    file.Delete();
                foreach (DirectoryInfo subDir in directory.GetDirectories())
                    subDir.Delete(true);
            }
        }

        private static string GetFullPath(string fileName)
        {
            return Path.Combine(RootPath, fileName);
        }

        private static byte[] EncryptString(string plainText)
        {
            try
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
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveLoadSystem] Encryption failed, saving as plain text. {e.Message}");
                return null;
            }
        }

        private static string DecryptString(byte[] cipherText)
        {
            try
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
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveLoadSystem] Decryption failed, returning null. {e.Message}");
                return null;
            }
        }
    }
}
