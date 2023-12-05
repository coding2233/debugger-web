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
        FileDownloader GetFileDownloader(string key);
    }

    public class WebRequestService : MonoService, IWebRequest, IFileDownloader
    {
        private Dictionary<string, FileDownloader> m_fileDwonloader;

        public WebRequestService()
        {
        }

        public FileDownloader GetFileDownloader(string key)
        {
            if (m_fileDwonloader == null)
            {
                m_fileDwonloader = new Dictionary<string, FileDownloader>();
			}
            FileDownloader fileDownloader = null;
            if (!m_fileDwonloader.TryGetValue(key, out fileDownloader))
            {
                fileDownloader = new FileDownloader(this);
                m_fileDwonloader.Add(key,fileDownloader);
			}
            return fileDownloader;
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

            if (m_fileDwonloader != null && m_fileDwonloader.Count > 0)
            {
                foreach (var item in m_fileDwonloader)
                {
                    FileDownloader fileDownloader = item.Value;
                    if (fileDownloader != null)
                    {
                        if (fileDownloader.State == FileDownloader.FileDownloadState.Ready)
                            continue;
                        fileDownloader.OnUpdate();
                        if (fileDownloader.State == FileDownloader.FileDownloadState.Stopped
                            || fileDownloader.State == FileDownloader.FileDownloadState.Error
                            || fileDownloader.State == FileDownloader.FileDownloadState.Done)
                        {
							m_fileDwonloader.Remove(item.Key);
                            break;
						}
                    }
                    else
                    {
						m_fileDwonloader.Remove(item.Key);
						break;
					}
                }
            }
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