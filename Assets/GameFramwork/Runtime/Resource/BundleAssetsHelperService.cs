using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


namespace Wanderer
{
    public sealed class BundleAssetsHelperService : MonoService, IAssetsHelper
    {
        //资源下载
        [Inject]
        public IWebRequest webRequest { get; set; }

        //版本信息
        private ResourceVersion m_resourceVersion;

        //是否正在更新中
        private bool m_update;

		private Dictionary<string, DynamicBundleVersion> m_allBundleVersions ;

        private string m_remotePath;
        private string m_localPath;

		public BundleAssetsHelperService()
        {
            m_resourceVersion = UnityEngine.Resources.Load<ResourceVersion>("BundleVersion");
			m_allBundleVersions = new Dictionary<string, DynamicBundleVersion>();

			m_remotePath = m_resourceVersion.GetRemoteURL();
            m_localPath = m_resourceVersion.GetLocalPath();
		}

        public void CheckUpdate(string name,Action<bool,string> needUpdateCallback)
        {
            Toast.Info("检查资源中...");
			if (name == null)
			{
                name = "";
			}

			DynamicBundleVersion bundleVersion;
			if (!m_allBundleVersions.TryGetValue(name, out bundleVersion))
			{
				bundleVersion = new DynamicBundleVersion(name, m_remotePath, m_localPath, m_resourceVersion.AssetVersionName);
				m_allBundleVersions.Add(name, bundleVersion);
			}

			if (bundleVersion != null)
			{
				bundleVersion.CheckUpdate(needUpdateCallback, webRequest);
			}
        }

        /// <summary>
        /// 异步加载Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <param name="callback"></param>
        public void LoadAsset<T>(string assetPath, Action<T> callback) where T : UnityEngine.Object
        {
            assetPath = assetPath.ToLower();

            LoadAssetBundle(assetPath, (ab) => {
                if (ab != null)
                {
                    ab.LoadAssetAsync<T>(assetPath).completed += (asyncHandle) =>
                    {
                        AssetBundleRequest abRequest = asyncHandle as AssetBundleRequest;
                        callback?.Invoke((T)abRequest.asset);
                    };
                }
                else
                {
                    Log.Error("Cannot find assetbundle from: {0}", assetPath);
                    callback?.Invoke(null);
                }
            });
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            assetPath = assetPath.ToLower();
            var ab = LoadAssetBundle(assetPath);
            if (ab != null)
            {
                var loadAsset = ab.LoadAsset<T>(assetPath);
                return loadAsset;
            }
            else
            {
                Log.Error("Cannot find assetbundle from: {0}", assetPath);
            }

            return null;
        }

		public UnityEngine.Object[] LoadAllAsset(string assetPath)
		{
			assetPath = assetPath.ToLower();
			var ab = LoadAssetBundle(assetPath);
			if (ab != null)
			{
				var loadAsset = ab.LoadAllAssets();
				return loadAsset;
			}
			else
			{
				Log.Error("Cannot find assetbundle from: {0}", assetPath);
			}

			return null;
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

		public Assembly LoadExtendAssembly(string dllPath, string pdbPath)
        {
            var dllTextAsset = LoadAsset<TextAsset>(dllPath);
            var pdbTextAsset = LoadAsset<TextAsset>(dllPath);

            var dllBytes = dllTextAsset != null ? dllTextAsset.bytes : null;
            var pdbBytes = pdbTextAsset != null ? pdbTextAsset.bytes : null;


            if (dllBytes != null)
            {
               return  Assembly.Load(dllBytes, pdbBytes);
            }

            return null;
        }
      
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetName"></param>
        public void UnloadAsset(string assetPath)
        {
            if (m_allBundleVersions != null)
            {
                foreach (var item in m_allBundleVersions.Values)
                {
                    if (item.HasAssetAddress(assetPath))
                    {
                        var bundleProvider = item.LoadBundleProviderFromAssetPath(assetPath);
                        if (bundleProvider != null)
                        {
                            bundleProvider.SetRefCount(-1);
                        }
                        return;
                    }
                }
            }
		}


        public void UnloadBundle(string name)
        {
            if (m_allBundleVersions != null)
            {
                if (m_allBundleVersions.TryGetValue(name, out DynamicBundleVersion dynamicBundle))
                {
                    dynamicBundle.Dispose();
                    m_allBundleVersions.Remove(name);
                }
            }
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneAsync(string scenePath, LoadSceneMode mode, Action<AsyncOperation> callback)
        {
            AsyncOperation asyncOperation = null;
            try
            {
                LoadAssetBundle(scenePath, (ab) => {
                    if (ab != null)
                    {
                        asyncOperation = SceneManager.LoadSceneAsync(scenePath, mode);
                        callback?.Invoke(asyncOperation);
                    }
                    else
                    {
                        Log.Error("Cannot find assetbundle from: {0}", scenePath);
                        callback?.Invoke(null);
                    }
                });
               
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

		
		/// <summary>
		/// 卸载场景
		/// </summary>
		/// <param name="sceneName"></param>
		public AsyncOperation UnloadSceneAsync(string scenePath)
        {
            var asyncHandle = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scenePath);
            asyncHandle.completed += (handle) =>
            {
				foreach (var item in m_allBundleVersions.Values)
				{
					if (item.HasAssetAddress(scenePath))
					{
						var bundleProvider = item.LoadBundleProviderFromAssetPath(scenePath);
						if (bundleProvider != null)
						{
							item.UnloadBundleProvider(bundleProvider);
						}
						return;
					}
				}
			};
            return asyncHandle;
        }

        public void UpdateAssets(string name, Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback)
        {
            if (m_allBundleVersions.TryGetValue(name, out DynamicBundleVersion bundleVersion))
            {
                bundleVersion.UpdateAsset(callback,downloadComplete, errorCallback);
			}
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Clear()
        {
			foreach (var item in m_allBundleVersions.Values)
			{
				item.Dispose();
			}
			m_allBundleVersions.Clear();
		}




		#region 内部函数

		private AssetBundle LoadAssetBundle(string assetPath)
		{
			foreach (var item in m_allBundleVersions.Values)
			{
				if (item.HasAssetAddress(assetPath))
				{
					var bundleProvider = item.LoadBundleProviderFromAssetPath(assetPath);
					if (bundleProvider != null)
					{
						bundleProvider.SetRefCount(1);
						return bundleProvider.GetBundle();
					}
					return null;
				}
			}
			return null;
		}

		private void LoadAssetBundle(string assetPath, Action<AssetBundle> onGetBundleCallback)
		{
			bool findAsset = false;
			foreach (var item in m_allBundleVersions.Values)
			{
				if (item.HasAssetAddress(assetPath))
				{
					item.LoadBundleProviderFromAssetPath(assetPath, (bundleProvider) => {
						if (bundleProvider != null)
						{
							bundleProvider.SetRefCount(1);
							onGetBundleCallback?.Invoke(bundleProvider.GetBundle());
						}
						else
						{
							onGetBundleCallback?.Invoke(null);
						}
					});
					findAsset = true;
					return;
				}
			}

			if (!findAsset)
			{
				onGetBundleCallback?.Invoke(null);
			}
		}


		

        #endregion

    }

}
