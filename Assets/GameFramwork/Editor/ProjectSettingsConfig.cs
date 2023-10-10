using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wanderer
{
    public class ProjectSettingsConfig
    {
        public static T LoadJson<T>(string key)
        {
            string path = ConfigPath(key);
            if (File.Exists(path))
            {
                return JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            return default(T);
        }

        public static void SaveJson<T>(string key,T value)
        {
            string path = ConfigPath(key);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, JsonUtility.ToJson(value));
        }

        public static string ConfigPath(string key)
        {
            return Path.Combine(Application.dataPath, "../ProjectSettings", key);
        }

    }
}