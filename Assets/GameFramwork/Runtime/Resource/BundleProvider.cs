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
    class BundleProvider : System.IDisposable
    {
        private string m_abPath;
        private AssetBundle m_bundle;
        private Stream m_fileStream;
        private AsyncOperation m_asyncHandle;
        private Action<BundleProvider> m_onLoadCompleteCallback;
        private int m_refCount;
        internal string BundleName { get; private set; }
        private bool m_isLoading;

        internal BundleProvider(string abPath)
        {
            m_abPath = abPath;
            BundleName = Path.GetFileName(abPath);

            LoadAssetBundle(abPath);
        }

        //internal void GetBundle(Action<AssetBundle> onABCallback)
        //{
        //    if (m_bundle != null)
        //    {
        //        onABCallback?.Invoke(m_bundle);
        //    }
        //    else
        //    {
        //        m_getABCallback += onABCallback;
        //    }
        //}
        internal AssetBundle GetBundle()
        {
            return m_bundle;
        }

        internal bool CheckLoadStatus(Action<BundleProvider> onLoadComplete)
        {
            if (m_isLoading)
            {
                m_onLoadCompleteCallback += onLoadComplete;
            }
            else
            {
                onLoadComplete?.Invoke(this);
            }
            return m_isLoading;
        }

        internal void SetRefCount(int refCount)
        {
            if (m_bundle == null)
                return;

            m_refCount += refCount;
            if (m_refCount <= 0)
            {
                m_bundle.Unload(false);
            }
        }

        private void LoadAssetBundle(string abPath)
        {
            m_isLoading = true;
            if (Application.platform == RuntimePlatform.Android && abPath.StartsWith("jar:"))
            {
                var unityRequest = UnityWebRequest.Get(abPath);
                unityRequest.downloadHandler = new DownloadHandlerBuffer();
                m_asyncHandle = unityRequest.SendWebRequest();
            }
            else
            {
                Debug.Log($"BundleProvider.EncryptFileStream: {abPath}");
                m_fileStream = new EncryptFileStream(abPath, FileMode.Open,FileAccess.Read,FileShare.Read,1024 * 10,false);
                SetBundleComplete(AssetBundle.LoadFromStream(m_fileStream));
            }

            if (m_asyncHandle != null)
            {
                m_asyncHandle.completed += OnAssetBundleReadComplete;
            }
        }

        private void OnAssetBundleReadComplete(AsyncOperation asyncHandle)
        {
            asyncHandle.completed -= OnAssetBundleReadComplete;
            if (asyncHandle is AssetBundleCreateRequest abCreateRequest)
            {
                SetBundleComplete(abCreateRequest.assetBundle);
            }
            else if (asyncHandle is UnityWebRequestAsyncOperation unityWebRequest)
            {
                //android从StreamingAssets中读取ab包
                if (Application.platform == RuntimePlatform.Android && m_abPath.StartsWith("jar:"))
                {
                    m_fileStream = new EncryptMemoryStream(unityWebRequest.webRequest.downloadHandler.data, m_abPath);
                    SetBundleComplete(AssetBundle.LoadFromStream(m_fileStream));
                }
            }
        }

        //设置回调
        private void SetBundleComplete(AssetBundle assetBundle)
        {
            m_bundle = assetBundle;
            m_isLoading = false;
            m_onLoadCompleteCallback?.Invoke(this);
            m_onLoadCompleteCallback = null;
        }

        public void Dispose()
        {
            m_abPath = null;
            m_asyncHandle = null;
            if (m_bundle != null)
            {
                m_bundle.Unload(true);
                m_bundle = null;
            }
            if (m_fileStream != null)
            {
                m_fileStream.Dispose();
                m_fileStream = null;
            }
            m_onLoadCompleteCallback = null;

        }
    }
}
