using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer
{
	public interface IFileDownloader
	{
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        /// <param name="callback">本地文件的路径，是否下载完成，下载的文件大小，下载的进度</param>
        /// <param name="errorCallback">错误回调</param>
        /// <returns></returns>
        void Download(string remoteUrl, string localPath, Action<string, bool, ulong, float> callback, Action<string, string> errorCallback);
    }

	public class FileDownloader:System.IDisposable
	{
		public enum FileDownloadState
		{
			Ready,
			Downloading,
			Stopped,
			Done,
			Error,
		}

		private IFileDownloader m_downloader;

		/// <summary>
		/// 下载速度 KB/s
		/// </summary>
		public double Speed { get; private set; }

		///// <summary>
		///// 剩余时间 s
		///// </summary>
		//public float RemainingTime { get; private set; }
		/// <summary>
		/// 正在下载
		/// </summary>
		public bool Downloading { get; private set; }

		/// <summary>
		/// 需要下载的文件总数
		/// </summary>
		public int NeedDownloadFileCount { get; private set; }

		/// <summary>
		/// 剩余下载的文件总数
		/// </summary>
		public int RemainingFileCount { 
			get {
				return m_remainingFiles.Count;
			} 
		}

		//下载状态
		public FileDownloadState State { get; private set; }

		//需要下载的文件
		private Dictionary<string, string> m_needDownloadFiles = new Dictionary<string, string>();
		//剩余下载的文件
		private List<string> m_remainingFiles = new List<string>();
		//正在下载的文件
		private Dictionary<string,ulong> m_downloadingFiles = new Dictionary<string, ulong>();
		//下载器的个数
		private int m_downloaderCount = 3;
		//下载开始时间
		private float m_downloadStartTime = 0;
		//下载的文件大小
		private ulong m_downloadSize = 0;
		//下载进度 <当前正在下载的文件名称,下载的文件大小(KB),下载的时间(s),下载的速度(KB/s)>
		private Action<string, double, float, double> m_downloadCallback;
		//下载完成的回调
		private Action<string> m_downloadCompleteCallback;
		//下载失败的回调 <下载到本地的文件路径，下载错误>
		private Action<string, string> m_downloadErrorCallback;

		internal FileDownloader(IFileDownloader downloader)
		{
			m_downloader = downloader;
			State = FileDownloadState.Ready;
		}

		public void OnUpdate()
		{
			if (Downloading)
			{
				if (m_remainingFiles.Count > 0)
				{
					if (m_downloadingFiles.Count < m_downloaderCount)
					{
						string localPath = m_remainingFiles[0];
						m_remainingFiles.RemoveAt(0);
						//下载文件
						DownloadFile(localPath);
					}
				}
				else
				{
					if (m_downloadingFiles.Count == 0)
					{
						Reset();
					}
				}
			}
		}

		#region 外部接口
		/// <param name="remotePath"></param>
		/// <param name="localPath"></param>
		public void AddDownloadFile(string remotePath,string localPath)
		{
			if (Downloading)
			{
				Log.Warn("资源正在下载中，remotePath:{0} localPath:{1}", remotePath, localPath);
				return;
			}
			if (!m_needDownloadFiles.ContainsKey(localPath))
			{
				m_needDownloadFiles.Add(localPath, remotePath);
				m_remainingFiles.Add(localPath);
			}
		}

		/// <summary>
		/// 开始下载
		/// </summary>
		public bool StartDownload(Action<string, double, float, double> downloadCallback,Action<string> downloadCompleteCallback, Action<string, string> downloadErrorCallback)
		{
			if (m_needDownloadFiles.Count <= 0 || Downloading)
			{
                Log.Warn("资源正在下载中!");
                return false;
			}
			State = FileDownloadState.Downloading;
			//下载回调
			m_downloadCallback = downloadCallback;
			m_downloadCompleteCallback = downloadCompleteCallback;
			m_downloadErrorCallback = downloadErrorCallback;
			//开始下载
			Downloading = true;
			m_downloadStartTime = Time.realtimeSinceStartup;
			NeedDownloadFileCount = m_needDownloadFiles.Count;
			m_downloadSize = 0;
			return true;
		}


		/// <summary>
		/// 停止下载
		/// </summary>
		public void StopDownload()
		{
			State = FileDownloadState.Stopped;
		}

		#endregion

		#region 事件回调

		//下载回调
		private void OnDownloadCallback(string localPath, bool isDone, ulong downloadedBytes, float downloadProgress)
		{
			m_downloadingFiles[localPath] = downloadedBytes;

			float downloadTime = Time.realtimeSinceStartup - m_downloadStartTime;
			ulong downloadSize = m_downloadSize;
			foreach (var item in m_downloadingFiles)
			{
				downloadSize += item.Value;
			}
			double downloadKBSize = downloadSize / 1024.0f;
			Speed = downloadKBSize / downloadTime;
			m_downloadCallback?.Invoke(localPath, downloadKBSize, downloadTime, Speed);
			if (isDone)
			{
				m_downloadSize += downloadedBytes;
				//清理下载完成的文件
				RemoveDownloadFile(localPath);
				//下载完成回调
				m_downloadCompleteCallback?.Invoke(localPath);

				State = FileDownloadState.Done;
			}
		}

		//下载错误
		private void OnDownloadError(string localPath, string error)
		{
			m_downloadErrorCallback?.Invoke(localPath, error);
			//清理下载完成的文件
			RemoveDownloadFile(localPath);

			State = FileDownloadState.Error;
		}

		#endregion

		#region 内部函数
		//下载文件
		private void DownloadFile(string localPath)
		{
			m_downloadingFiles.Add(localPath, 0);
			string remotePath = m_needDownloadFiles[localPath];
			m_downloader.Download(remotePath, localPath, OnDownloadCallback, OnDownloadError);
		}

		//移除下载文件
		private void RemoveDownloadFile(string localPath)
		{
			//清理下载完成的文件
			m_downloadingFiles.Remove(localPath);
			m_needDownloadFiles.Remove(localPath);
		}

		private void Reset()
		{
			//下载回调
			m_downloadCallback = null;
			m_downloadCompleteCallback = null;
			m_downloadErrorCallback = null;
			m_downloadingFiles.Clear();
			m_remainingFiles.Clear();
			m_needDownloadFiles.Clear();
			if (Downloading)
			{
				Downloading = false;
			}
		}

		public void Dispose()
		{
			Reset();
		}
		#endregion

	}
}
