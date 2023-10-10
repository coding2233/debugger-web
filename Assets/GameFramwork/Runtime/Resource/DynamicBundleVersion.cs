// 资源版本管理
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Wanderer
{
    internal class DynamicBundleVersion : System.IDisposable
    {
        private string m_name;
        private string m_remoteURL;
        private string m_localPath;
        private string m_assetVersionName;

        /// <summary>
        /// 远程的版本信息
        /// </summary>
        /// <value></value>
        private AssetVersion RemoteVersion;

        /// <summary>
        /// 本地的版本信息
        /// </summary>
        /// <value></value>
        private AssetVersion LocalVersion;

        /// <summary>
        /// 本地的只读版本信息
        /// </summary>
        /// <value></value>
        private AssetVersion StreamingAssetsVersion;

        private Dictionary<string, BundleProvider> m_assetBundlePathForBundleProviderMap = new Dictionary<string, BundleProvider>();

        //加载BundleProvider的异步锁
        private Dictionary<string, HashSet<Action<BundleProvider>>> m_lockAssetBundlePathForBundleProvider = new Dictionary<string, HashSet<Action<BundleProvider>>>();

        private AssetBundle m_manifestAb;
        //ab包依赖
        private AssetBundleManifest m_manifest;

        private HashSet<string> m_allAssetPaths = new HashSet<string>();

        internal DynamicBundleVersion(string name,string remoteURL,string localPath,string assetVersionName)
        {
            m_name = name;
            m_remoteURL = remoteURL;
            m_localPath = localPath;

            if (!string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(m_remoteURL))
                {
                    m_remoteURL = Path.Combine(m_remoteURL, name).Replace("\\","/");
                }
                m_localPath = Path.Combine(m_localPath, name).Replace("\\", "/");
            }
            m_assetVersionName = assetVersionName;
        }


        internal void CheckUpdate(Action<bool, string, HashSet<AssetHashInfo>> updateCallback)
        {
            LocalVersion = null;
            StreamingAssetsVersion = null;
            RemoteVersion = null;

            m_allAssetPaths = new HashSet<string>();

            HashSet<UnityWebRequestAsyncOperation> asyncOperations = new HashSet<UnityWebRequestAsyncOperation>();

            //本地版本
            string localPath = Path.Combine(m_localPath, m_assetVersionName);
            if (File.Exists(localPath))
            {
                string content = File.ReadAllText(localPath);
                LocalVersion = JsonUtility.FromJson<AssetVersion>(content);
                GetAllAssetPaths(LocalVersion);
            }

            //StreamingAsset版本
            string streamingAssetPath = Path.Combine(Application.streamingAssetsPath, m_assetVersionName);
            var webRequest = UnityWebRequest.Get(streamingAssetPath);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            var localRequestHandler = webRequest.SendWebRequest();
            localRequestHandler.completed += (handle) =>
            {
                //本地可能不存在， 不抛出错误
                string downloadText = webRequest.downloadHandler.text;
                if (!string.IsNullOrEmpty(downloadText))
                {
                    StreamingAssetsVersion = JsonUtility.FromJson<AssetVersion>(downloadText);
                    GetAllAssetPaths(StreamingAssetsVersion);
                }
                asyncOperations.Add(localRequestHandler);
                OnAssetVersionRequestComplete(asyncOperations, updateCallback);
            };

            //远程版本
            string remoteURL = m_remoteURL;
            if (!string.IsNullOrEmpty(remoteURL))
            {
                string remoteAssetPath = $"{remoteURL}/{m_assetVersionName}";
                var remoteWebRequest = UnityWebRequest.Get(remoteAssetPath);
                remoteWebRequest.downloadHandler = new DownloadHandlerBuffer();
                var remoteWebRequestHandle = remoteWebRequest.SendWebRequest();
                remoteWebRequestHandle.completed += (handle) =>
                {
                    //有错误
                    if (!string.IsNullOrEmpty(remoteWebRequest.error))
                    {
                        updateCallback?.Invoke(false, remoteWebRequest.error,null);
                        return;
                    }
                    else
                    {
                        string downloadText = remoteWebRequest.downloadHandler.text;
                        if (!string.IsNullOrEmpty(downloadText))
                        {
                            RemoteVersion = JsonUtility.FromJson<AssetVersion>(downloadText);
                            GetAllAssetPaths(RemoteVersion);
                        }
                        asyncOperations.Add(remoteWebRequestHandle);
                        OnAssetVersionRequestComplete(asyncOperations, updateCallback);
                    }
                };
            }
        }

        internal void UpdateLocalVersion(AssetHashInfo assetHashInfo)
        {
            if (assetHashInfo != null && RemoteVersion != null)
            {
                string localVersionPath = Path.Combine(m_localPath, m_assetVersionName);
                if (LocalVersion == null)
                {
                    string jsonContent = JsonUtility.ToJson(RemoteVersion);
                    LocalVersion = JsonUtility.FromJson<AssetVersion>(jsonContent);
                    LocalVersion.AssetHashInfos.Clear();
                }

                var oldAssetHashInfo = LocalVersion.AssetHashInfos.Find(x => x.Name.Equals(assetHashInfo.Name));
                if (oldAssetHashInfo != null)
                {
                    LocalVersion.AssetHashInfos.Remove(oldAssetHashInfo);
                }
                LocalVersion.AssetHashInfos.Add(assetHashInfo);
                //更新本地信息
                File.WriteAllText(localVersionPath, JsonUtility.ToJson(LocalVersion));
            }
        }

        internal bool HasAssetAddress(string assetPath)
        {
            return m_allAssetPaths.Contains(assetPath);
        }

        internal BundleProvider LoadBundleProviderFromAssetPath(string assetPath)
        {
            var bundlePath = AssetPathTransformBundlePath(assetPath);
            if (string.IsNullOrEmpty(bundlePath))
            {
                Log.Error("Cannot find assetbundle path: {0}", bundlePath);
            }
            else
            {
                return LoadBundleProviderFromPath(bundlePath);
            }

            return null;
        }

        internal void LoadBundleProviderFromAssetPath(string assetPath,Action<BundleProvider> onGetBundleProviderCallback)
        {
            var bundlePath = AssetPathTransformBundlePath(assetPath);
            if (string.IsNullOrEmpty(bundlePath))
            {
                Log.Error("Cannot find assetbundle path: {0}", bundlePath);
                onGetBundleProviderCallback?.Invoke(null);
            }
            else
            {
                LoadBundleProviderFromPath(bundlePath, onGetBundleProviderCallback);
            }
        }

        internal void UnloadBundleProvider(BundleProvider bundleProvider)
        {
            HashSet<string> keys = new HashSet<string>();
            foreach (var item in m_assetBundlePathForBundleProviderMap)
            {
                if (bundleProvider == item.Value)
                {
                    keys.Add(item.Key);
                }
            }

            foreach (var item in keys)
            {
                m_assetBundlePathForBundleProviderMap.Remove(item);
            }

            bundleProvider.Dispose();
        }

        #region 加载AssetBunlde，以及处理版本、路径、引用关系

        private string AssetPathTransformBundlePath(string assetPath)
        {
            AssetHashInfo remoteAssetHash = null;
            AssetHashInfo localAssetHash = null;
            string assetBundlePath = null;
            if (RemoteVersion != null)
            {
                remoteAssetHash = RemoteVersion.AssetHashInfos.Find(x => x.Addressables.Contains(assetPath));
            }

            if (remoteAssetHash != null)
            {
                //找到本地对应的assetBundle
                if (StreamingAssetsVersion != null)
                {
                    localAssetHash = StreamingAssetsVersion.AssetHashInfos.Find(x => x.Equals(remoteAssetHash));
                    if (localAssetHash != null)
                    {
                        assetBundlePath = Path.Combine(Application.streamingAssetsPath, m_name, localAssetHash.Name);
                    }
                }

                if (localAssetHash == null)
                {
                    if (LocalVersion != null)
                    {
                        localAssetHash = LocalVersion.AssetHashInfos.Find(x => x.Equals(remoteAssetHash));
                        if (localAssetHash != null)
                        {
                            assetBundlePath = Path.Combine(m_localPath,  localAssetHash.Name);
                        }
                    }
                }

                //本地没有下载返回整个url
                if (localAssetHash == null)
                {
                    assetBundlePath = Path.Combine(m_remoteURL, m_name, remoteAssetHash.Name);
                }
            }
            else
            {
                if (StreamingAssetsVersion != null)
                {
                    localAssetHash = StreamingAssetsVersion.AssetHashInfos.Find(x => x.Addressables.Contains(assetPath));
                    if (localAssetHash == null)
                    {
                        if (LocalVersion != null)
                        {
                            localAssetHash = LocalVersion.AssetHashInfos.Find(x => x.Addressables.Contains(assetPath));
                        }
                        else
                        {
                            assetBundlePath = Path.Combine(m_localPath, localAssetHash.Name);
                        }
                    }
                    else
                    {
                        assetBundlePath = Path.Combine(Application.streamingAssetsPath, m_name, localAssetHash.Name);
                    }
                }
            }

            if (!string.IsNullOrEmpty(assetBundlePath))
            {
                assetBundlePath = assetBundlePath.Replace("\\", "/");
            }

            return assetBundlePath;
        }


        private async void LoadBundleProviderFromPath(string abPath, Action<BundleProvider> getAssetBundleACallback)
        {
            if (string.IsNullOrEmpty(abPath))
            {
                Log.Error("Cannot find assetbundle path: {0}", abPath);
                getAssetBundleACallback?.Invoke(null);
                return;
            }

            Log.Info("LoadBundleProviderFromPath. {0}", abPath);

            BundleProvider bundleProvider = null;
            if (!m_assetBundlePathForBundleProviderMap.TryGetValue(abPath, out bundleProvider))
            {
                //已有ab在加载中
                if (m_lockAssetBundlePathForBundleProvider.TryGetValue(abPath, out HashSet<Action<BundleProvider>> lockBundleProviderCallbacks))
                {
                    lockBundleProviderCallbacks.Add(getAssetBundleACallback);
                    return;
                }

                bundleProvider = new BundleProvider(abPath);
                m_lockAssetBundlePathForBundleProvider.Add(abPath, new HashSet<Action<BundleProvider>>());
                //处理引用
                var allDependencies = GetManifest().GetAllDependencies(bundleProvider.BundleName);
                if (allDependencies != null && allDependencies.Length > 0)
                {
                    foreach (var bundleName in allDependencies)
                    {
                        var getBundleTask = new TaskCompletionSource<BundleProvider>();
                        LoadBundleProviderFromPath(Path.Combine(m_localPath, bundleName), (bp) =>
                        {
                            getBundleTask.SetResult(bp);
                        });
                        await getBundleTask.Task;
                    }
                }

                //加载自己的ab
                bundleProvider.CheckLoadStatus((bp) => {
                    //资源路径对应Bundle处理
                    m_assetBundlePathForBundleProviderMap.Add(abPath, bundleProvider);
                    getAssetBundleACallback?.Invoke(bundleProvider);
                    if (m_lockAssetBundlePathForBundleProvider.TryGetValue(abPath, out HashSet<Action<BundleProvider>> onGetBPCallback))
                    {
                        foreach (var item in onGetBPCallback)
                        {
                            item?.Invoke(bp);
                        }
                        m_lockAssetBundlePathForBundleProvider.Remove(abPath);
                    }
                });
            }
            else
            {
                getAssetBundleACallback?.Invoke(bundleProvider);
            }
        }

        private BundleProvider LoadBundleProviderFromPath(string abPath)
        {
            if (string.IsNullOrEmpty(abPath))
            {
                Log.Error("Cannot find assetbundle path: {0}", abPath);
                return null;
            }

            BundleProvider bundleProvider = null;
            if (!m_assetBundlePathForBundleProviderMap.TryGetValue(abPath, out bundleProvider))
            {
                bundleProvider = new BundleProvider(abPath);
                //处理引用
                var allDependencies = GetManifest().GetAllDependencies(bundleProvider.BundleName);

                if (allDependencies != null && allDependencies.Length > 0)
                {
                    foreach (var bundleName in allDependencies)
                    {
                        var depAb = LoadBundleProviderFromPath(Path.Combine(m_localPath,bundleName));
                        if (depAb == null)
                        {
                            Log.Error("同步加载错误");
                        }
                    }
                }

                //ab包还在加载中
                if (bundleProvider.CheckLoadStatus(null))
                {
                    Log.Error("同步加载错误"+ abPath);
                    return null;
                }

                //资源路径对应Bundle处理
                m_assetBundlePathForBundleProviderMap.Add(abPath, bundleProvider);
            }

            return bundleProvider;
        }


        #endregion

        #region 内部函数
        private void GetAllAssetPaths(AssetVersion assetVersion)
        {
            if (assetVersion != null)
            {
                foreach (var item in assetVersion.AssetHashInfos)
                {
                    foreach (var itemPath in item.Addressables)
                    {
                        m_allAssetPaths.Add(itemPath);
                    }
                }
            }
        }

        private void OnAssetVersionRequestComplete(HashSet<UnityWebRequestAsyncOperation> asyncOperations, Action<bool, string, HashSet<AssetHashInfo>> updateCallback)
        {
            if (asyncOperations != null)
            {
                if (string.IsNullOrEmpty(m_remoteURL))
                {
                    updateCallback?.Invoke(false, m_name, null);
                }
                else
                {
                    if (asyncOperations.Count == 2)
                    {

                        HashSet<AssetHashInfo> updateAssets = new HashSet<AssetHashInfo>();

                        //必须有远程版本，检查资源是否有更新
                        if (RemoteVersion != null)
                        {
                            foreach (var item in RemoteVersion.AssetHashInfos)
                            {
                                if ((StreamingAssetsVersion != null && StreamingAssetsVersion.AssetHashInfos.Contains(item))
                                    || (LocalVersion != null && LocalVersion.AssetHashInfos.Contains(item)))
                                {
                                    continue;
                                }


                                if (item.UpdateTag.Equals("default"))
                                {
                                    updateAssets.Add(item);
                                }
                            }

                            //返回更新结果
                            updateCallback?.Invoke(updateAssets.Count > 0, m_name, updateAssets);
                        }
                        else
                        {
                            updateCallback?.Invoke(false, m_name, null);
                        }

                    }
                }
            }
        }
        private AssetBundleManifest GetManifest()
        {
            if (m_manifest == null)
            {
                string strManiFest = "";
                if (RemoteVersion != null)
                {
                    strManiFest = RemoteVersion.Manifest;

                }
                else if (LocalVersion != null)
                {
                    strManiFest = LocalVersion.Manifest;
                }
                else if (StreamingAssetsVersion != null)
                {
                    strManiFest = StreamingAssetsVersion.Manifest;
                }

                //更新manifest
                var bytes = Convert.FromBase64String(strManiFest);
                m_manifestAb = AssetBundle.LoadFromMemory(bytes);
                m_manifest = m_manifestAb.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
            }
            return m_manifest;
        }
        #endregion

        public void Dispose()
        {
            StreamingAssetsVersion = null;
            LocalVersion = null;
            RemoteVersion = null;
            //ForceUpdateAssets = null;
            //DefaultUpdateAssets = null;
            //UpdateAssets = null;

            m_manifest = null;
            if (m_manifestAb != null)
            {
                m_manifestAb.Unload(true);
                m_manifestAb = null;
            }
            foreach (var item in m_lockAssetBundlePathForBundleProvider.Values)
            {
                if (item != null)
                {
                    foreach (var itemCallback in item)
                    {
                        if (itemCallback != null)
                        {
                            itemCallback?.Invoke(null);
                        }
                    }
                }
            }
            m_lockAssetBundlePathForBundleProvider.Clear();
            m_lockAssetBundlePathForBundleProvider = null;

            foreach (BundleProvider bundleProvider in m_assetBundlePathForBundleProviderMap.Values)
            {
                bundleProvider.Dispose();
            }

            m_assetBundlePathForBundleProviderMap.Clear();
            m_assetBundlePathForBundleProviderMap = null;
       
        }
    }


}
