using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Asce.Managers
{
    public static class EnvLoader
    {
        private static bool _loaded = false;
        private static Dictionary<string, string> _env = new();

        public static void LoadEnv()
        {
            if (_loaded) return;

            string envPath = Path.Combine(Application.dataPath, "../.env");
            if (!File.Exists(envPath))
            {
                Debug.LogWarning($".env file not found at {envPath}");
                return;
            }

            foreach (var line in File.ReadAllLines(envPath))
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
            if (!_env.TryGetValue(key, out var value)) return string.Empty;
            return value;
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            string val = Get(key).ToLower();
            if (string.IsNullOrEmpty(val)) return defaultValue;
            if (val == "true" || val == "1" || val == "yes") return true;
            if (val == "false" || val == "0" || val == "no") return false;
            return defaultValue;
        }
    }
}
