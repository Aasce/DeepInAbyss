using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace Asce.Managers
{
    public static class EnvLoader
    {
        private static bool _loaded;
        private static readonly Dictionary<string, string> _env = new();

        private static readonly string SecretsDir = Path.Combine(Application.streamingAssetsPath, "Secrets");
        private static readonly string EnvPath = Path.Combine(SecretsDir, ".env");

        /// <summary>
        /// Load .env from StreamingAssets/Secrets. If missing, generate with AES keys.
        /// </summary>
        public static void LoadEnv()
        {
            if (_loaded) return;

            EnsureSecretsDirExists();
            EnsureEnvExists();

            foreach (var line in File.ReadAllLines(EnvPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                _env[key] = value;
            }

            _loaded = true;
        }

        public static string Get(string key)
        {
            LoadEnv();
            if (string.IsNullOrEmpty(key)) return string.Empty;
            return _env.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            string val = Get(key).ToLowerInvariant();
            if (string.IsNullOrEmpty(val)) return defaultValue;
            if (val == "true" || val == "1" || val == "yes") return true;
            if (val == "false" || val == "0" || val == "no") return false;
            return defaultValue;
        }

        /// <summary>
        /// Get AES key as byte[] from Base64 string in .env
        /// </summary>
        public static byte[] GetBytesFromBase64(string key)
        {
            string val = Get(key);
            if (string.IsNullOrEmpty(val)) return Array.Empty<byte>();

            try
            {
                return Convert.FromBase64String(val);
            }
            catch
            {
                Debug.LogWarning($"[EnvLoader] Invalid Base64 for key: {key}");
                return Array.Empty<byte>();
            }
        }

        private static void EnsureSecretsDirExists()
        {
            if (!Directory.Exists(SecretsDir))
            {
                Directory.CreateDirectory(SecretsDir);
                Debug.Log($"[EnvLoader] Created Secrets directory at {SecretsDir}");
            }
        }

        private static void EnsureEnvExists()
        {
            if (!File.Exists(EnvPath))
            {
                string key = Convert.ToBase64String(GenerateRandomBytes(32)); // AES-256
                string iv = Convert.ToBase64String(GenerateRandomBytes(16));  // IV

                string envContent =
                    "USE_ENCRYPTION=true\n" +
                    $"EncryptionKey={key}\n" +
                    $"EncryptionIV={iv}\n";

                File.WriteAllText(EnvPath, envContent);
                Debug.Log($"[EnvLoader] Generated new .env at {EnvPath}");
            }
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }
    }
}
