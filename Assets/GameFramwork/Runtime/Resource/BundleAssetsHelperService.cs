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

        public BundleAssetsHelperService()
        {
            m_resourceVersion = UnityEngine.Resources.Load<ResourceVersion>("BundleVersion");
        }

        public void CheckUpdate(string name,Action<bool,string,HashSet<AssetHashInfo>> needUpdateCallback)
        {
            Toast.Info("检查资源中...");
            m_resourceVersion.CheckUpdate(needUpdateCallback,name);
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

            m_resourceVersion.LoadAssetBundle(assetPath, (ab) => {
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
            var ab = m_resourceVersion.LoadAssetBundle(assetPath);
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
			var ab = m_resourceVersion.LoadAssetBundle(assetPath);
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
            m_resourceVersion.UnloadAsset(assetPath);
        }


        public void UnloadBundle(string name)
        {
            m_resourceVersion.UnloadDynamicBundle(name);
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
                m_resourceVersion.LoadAssetBundle(scenePath, (ab) => {
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
                m_resourceVersion.UnloadScene(scenePath);
            };
            return asyncHandle;
        }
     
        public void UpdateAssets(string name,HashSet<AssetHashInfo> assets,Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback)
        {
            DownloadAssets(name,assets, callback, downloadComplete, errorCallback);
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Clear()
        {
            m_resourceVersion.Clear();
            //m_mainfest = null;
            //foreach (var item in m_assetBundlePathForBundleProviderMap.Values)
            //{
            //    item.Dispose();
            //}
            //m_assetBundlePathForBundleProviderMap.Clear();
            //m_assetBundlePathForBundleProviderMap = null;

        }

       


        #region 内部函数

        private void DownloadAssets(string name,HashSet<AssetHashInfo> needDownloadAssets, Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback)
        {
            if (m_update)
            {
                Log.Warn("资源正在更新中， 不应该再调用下载");
                return;
            }

            m_update = true;

            if (needDownloadAssets != null && needDownloadAssets.Count > 0)
            {
                string remoteURLRoot = Path.Combine(m_resourceVersion.GetRemoteURL(),name);
                string localPathRoot = Path.Combine(m_resourceVersion.GetLocalPath(),name);
                if (Directory.Exists(localPathRoot))
                {
                    Directory.CreateDirectory(localPathRoot);
                }

                var fileDownloader = webRequest.GetFileDownloader();

                int downloadFileCount = 0;
                double totleFileSize = 0;
                Dictionary<string, AssetHashInfo> downloadFiles = new Dictionary<string, AssetHashInfo>();
                foreach (var item in needDownloadAssets)
                {
                    string remoteUrl = Path.Combine(remoteURLRoot, item.Name);
                    string localPath = Path.Combine(localPathRoot, $"{item.Name}.download");

                    fileDownloader.AddDownloadFile(remoteUrl, localPath);
                    //整理文件大小
                    totleFileSize += item.Size;
                    downloadFiles.Add(localPath, item);
                }
                //下载文件
                fileDownloader.StartDownload((localPath, size, time, speed) =>
                {
                    float progress = Mathf.Clamp01((float)(size / totleFileSize));
                    float remainingTime = (float)((totleFileSize - size) / speed);
                    callback?.Invoke(progress, totleFileSize, speed, remainingTime);
                }, async (localPath) =>
                {
                    //验证文件的完整性
                    string md5 = FileUtility.GetFileMD5(localPath);
                    var assetHashInfo = downloadFiles[localPath];
                    if (assetHashInfo.MD5.Equals(md5))
                    {
                        int index = localPath.LastIndexOf('.');
                        string targetPath = localPath.Substring(0, index);
                        if (File.Exists(targetPath))
                        {
                            File.Delete(targetPath);
                        }
                        File.Move(localPath, targetPath);
                        downloadFiles.Remove(localPath);
                        downloadFileCount++;
                        //更新本地版本信息
                        m_resourceVersion.UpdateLocalVersion(name,assetHashInfo);
                        //下载完成
                        if (downloadFiles.Count == 0)
                        {
                            //更新本地资源
                            if (downloadFileCount == needDownloadAssets.Count)
                            {
                                //if (CheckResource())
                                //{
                                //    UpdateLocalVersion();
                                //}
                            }
                            else
                            {
                                Log.Error("资源下载失败...");
                            }
                            needDownloadAssets = null;
                            m_update = false;
                            fileDownloader.StopDownload();
                            downloadComplete?.Invoke();
                        }
                    }
                    else
                    {
                        File.Delete(localPath);
                        fileDownloader.StopDownload();
                        m_update = false;
                        //throw new GameException($"File integrity verification failed. {localPath}");
                        errorCallback?.Invoke(localPath, "File integrity verification failed.");

                    }
                }, (localPath, error) =>
                {
                    fileDownloader.StopDownload();
                    m_update = false;
                    errorCallback?.Invoke(localPath, error);
                });
            }
            else
            {
                //下载完成
                downloadComplete?.Invoke();
                m_update = false;
            }
        }

        #endregion

    }

}
