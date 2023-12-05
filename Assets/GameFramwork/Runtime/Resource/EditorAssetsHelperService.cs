#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using System.Reflection;
using System.IO;

namespace Wanderer
{
    public class EditorAssetsHelperService : IAssetsHelper
    {
        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, Action<AsyncOperation> callback)
        {
            AsyncOperation asyncLoadScene = EditorSceneManager.LoadSceneAsyncInPlayMode(sceneName, new LoadSceneParameters(mode));
            callback?.Invoke(asyncLoadScene);
        }


        public AsyncOperation UnloadSceneAsync(string sceneName)
        {
            return UnitySceneManager.UnloadSceneAsync(sceneName);
        }


        public void LoadAsset<T>(string assetName, Action<T> callback) where T : Object
        {
            callback(AssetDatabase.LoadAssetAtPath<T>(assetName));
        }

		public Object[] LoadAllAsset(string assetPath)
		{
			return AssetDatabase.LoadAllAssetsAtPath(assetPath);
		}

		public T[] LoadAllAsset<T>(string assetPath) where T : UnityEngine.Object
		{
			var objects = LoadAllAsset(assetPath);
			if (objects != null)
			{
				List<T> list = new List<T>();
				foreach (var item in objects)
				{
					if (item is T)
					{
						list.Add((T)item);
					}
				}
				if (list.Count > 0)
				{
					return list.ToArray();
				}
			}

			return null;
		}

        public void LoadAllAsset<T>(string assetPath, Action<T[]> callback) where T : UnityEngine.Object
        { 
            var assets = LoadAllAsset<T>(assetPath);
            callback?.Invoke(assets);
		}

		public void UnloadAsset(string assetName)
        {

        }

        public void Clear()
        {
        }

		public T LoadAsset<T>(string assetName) where T : Object
		{
            return AssetDatabase.LoadAssetAtPath<T>(assetName);
        }

		public void Preload(Action<float> progressCallback)
		{
            progressCallback?.Invoke(1.0f);
        }

        public void CheckUpdate(string name, Action<bool, string> needUpdateCallback)
        {
            needUpdateCallback?.Invoke(false,name);
        }

        public void UpdateAssets(string name, Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback)
        {
            downloadComplete?.Invoke();
        }

        public void LoadExtendAssembly(string path, Action<Assembly> onExtendAssemblyCallback)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            //Editor直接加载Library/ScriptAssemblies的文件
            string dllPath = Path.Combine("Library/ScriptAssemblies", name);
            if (File.Exists(dllPath))
            {
                var assembly = Assembly.Load(File.ReadAllBytes(dllPath));
                onExtendAssemblyCallback?.Invoke(assembly);
            }
            else
            {
                LoadAsset<TextAsset>(path, (textAsset) => {
                    onExtendAssemblyCallback?.Invoke(Assembly.Load(textAsset.bytes));
                });
            }

        }

        public Assembly LoadExtendAssembly(string dllPath, string pdbPath)
        {
            byte[] dllBytes = null;
            byte[] pdbBytes = null; 
            string dllName = Path.GetFileNameWithoutExtension(dllPath);
            string libDllPath = $"Library/ScriptAssemblies/{dllName}";
            if (File.Exists(libDllPath))
            {
                dllBytes = File.ReadAllBytes(libDllPath);
            }
            else
            {
                var dllTextAsset = LoadAsset<TextAsset>(dllPath);
                if (dllTextAsset != null)
                {
                    dllBytes = dllTextAsset.bytes;
                }
            }

            string pdbName = Path.GetFileNameWithoutExtension(pdbPath);
            string libPdbPath = $"Library/ScriptAssemblies/{pdbName}";
            if (File.Exists(libPdbPath))
            {
                pdbBytes = File.ReadAllBytes(libPdbPath);
            }
            else
            {
                var pdbTextAsset = LoadAsset<TextAsset>(pdbPath);
                if (pdbTextAsset != null)
                {
                    pdbBytes = pdbTextAsset.bytes;
                }
            }

            if (dllBytes != null)
            {
                return Assembly.Load(dllBytes, pdbBytes);
            }

            return null;
        }

        public void UnloadBundle(string name)
        {
            
        }
    }
}

#endif
