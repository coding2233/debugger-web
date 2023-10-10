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
    [CreateAssetMenu(fileName = "BundleVersion.asset", menuName = "AssetBundle/BundleVersion")]
    public class ResourceVersion : ScriptableObject
    {
        //资源信息文本名称
        [SerializeField]
        private string m_assetVersionName = "version.json";
        //主资源的版本号
        [SerializeField]
        private string m_mainVersion = "1.0";

        //远程更新的路径
        [SerializeField]
        private List<string> m_remoteUpdatePath;
        [SerializeField]
        private int m_remoteUpdatePathActive;
        /// <summary>
        /// 资源主版本
        /// </summary>
        public string MainVersion => m_mainVersion;
        /// <summary>
        /// 当前激活的远程url
        /// </summary>
        private string m_remoteURL;

        //在编辑器中使用ab包
        [SerializeField]
        internal bool UseAssetBundleInEditor;


        private Dictionary<string, DynamicBundleVersion> m_assetVersions = new Dictionary<string, DynamicBundleVersion>();

        internal void CheckUpdate(Action<bool, string, HashSet<AssetHashInfo>> onUpdateCallback, string name)
        {
            DynamicBundleVersion assetVersion;
            if (!m_assetVersions.TryGetValue(name, out assetVersion))
            {
                assetVersion = new DynamicBundleVersion(name, GetRemoteURL(), GetLocalPath(), m_assetVersionName);
                m_assetVersions.Add(name, assetVersion);
            }

            if (assetVersion != null)
            {
                assetVersion.CheckUpdate(onUpdateCallback);
            }
        }

        internal AssetBundle LoadAssetBundle(string assetPath)
        {
            foreach (var item in m_assetVersions.Values)
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

       
        internal void LoadAssetBundle(string assetPath,Action<AssetBundle> onGetBundleCallback)
        {
            bool findAsset = false;
            foreach (var item in m_assetVersions.Values)
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

        internal void UnloadAsset(string assetPath)
        {
            foreach (var item in m_assetVersions.Values)
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

        internal void UnloadScene(string scenePath)
        {
            foreach (var item in m_assetVersions.Values)
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
        }


        internal void UnloadDynamicBundle(string name)
        {
            if (m_assetVersions.TryGetValue(name, out DynamicBundleVersion dynamicBundle))
            {
                dynamicBundle.Dispose();
                m_assetVersions.Remove(name);
            }
        }


        internal void UpdateLocalVersion(string name,AssetHashInfo assetHashInfo)
        {
            if (m_assetVersions.TryGetValue(name, out DynamicBundleVersion assetVersion))
            {
                assetVersion.UpdateLocalVersion(assetHashInfo);
            }
        }


        internal void Clear()
        {
            foreach (var item in m_assetVersions.Values)
            {
                item.Dispose();
            }
            m_assetVersions.Clear();
        }

        //获取远程激活的URL
        internal string GetRemoteURL()
        {
            if (string.IsNullOrEmpty(m_remoteURL))
            {
                if (m_remoteUpdatePath != null && m_remoteUpdatePath.Count > m_remoteUpdatePathActive)
                {
                    m_remoteURL = $"{m_remoteUpdatePath[m_remoteUpdatePathActive]}/{GetPlatform()}";
                }
            }
            return m_remoteURL;
        }

        internal string GetLocalPath()
        {
            return Application.persistentDataPath;
        }

        private string GetPlatform()
        {
            string platform = Application.platform.ToString().ToLower();
            if (platform.Contains("windows"))
            {
                platform = "windows";
            }
            else if (platform.Contains("osx"))
            {
                platform = "osx";
            }
            else if (platform.Contains("linux"))
            {
                platform = "linux";

            }
            else if (platform.Contains("iphoneplayer"))
            {
                platform = "ios";
            }
            return platform;
        }
    }

}
