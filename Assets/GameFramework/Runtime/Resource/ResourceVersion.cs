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
        /// 资源版本信息的名称
        /// </summary>
        public string AssetVersionName => m_assetVersionName;
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
