using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Wanderer
{
    public interface IWebRequest
    {
        FileDownloader GetFileDownloader();
    }

    public class WebRequestService : MonoService, IWebRequest, IFileDownloader
    {
        private FileDownloader m_fileDwonloader;

        public WebRequestService()
        {
            m_fileDwonloader = new FileDownloader(this);
        }

        public FileDownloader GetFileDownloader()
        {
            return m_fileDwonloader;
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        /// <param name="callback"></param>
        /// <param name="errorCallback"></param>
        public void Download(string remoteUrl, string localPath, Action<string, bool, ulong, float> callback, Action<string, string> errorCallback)
        {
            StartCoroutine(WebRequestDownloadFile(remoteUrl, localPath, callback, errorCallback));
        }


        public override void OnUpdate()
        {
            base.OnUpdate();
            m_fileDwonloader?.OnUpdate();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        /// <param name="callback">本地文件的路径，是否下载完成，下载的文件大小，下载的进度</param>
        /// <param name="errorCallback">错误回调</param>
        /// <returns></returns>
        private IEnumerator WebRequestDownloadFile(string remoteUrl, string localPath, Action<string, bool, ulong, float> callback, Action<string, string> errorCallback)
        {
            yield return null;

            //删除本地文件
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }

            using (UnityWebRequest request = UnityWebRequest.Get(remoteUrl))
            {
                request.downloadHandler = new DownloadHandlerFile(localPath, true);

                request.SendWebRequest();

                yield return null;
                while (!request.isDone)
                {
                    if (request.downloadProgress > 0)
                    {
                        callback?.Invoke(localPath, false, request.downloadedBytes, request.downloadProgress);
                    }
                    yield return null;
                }

                yield return null;

                if (request.result == UnityWebRequest.Result.ConnectionError
                    || request.result == UnityWebRequest.Result.DataProcessingError
                    || request.result == UnityWebRequest.Result.ProtocolError)
                    errorCallback?.Invoke(localPath, request.error);
                else
                {
                    callback?.Invoke(localPath, true, request.downloadedBytes, request.downloadProgress);
                }
            }

        }
    }
}