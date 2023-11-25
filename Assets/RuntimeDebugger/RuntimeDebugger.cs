using AOT;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public unsafe class RuntimeDebugger:IDisposable
{
	private static Dictionary<byte, RuntimeDebuggerBase> m_runtimeDebugger;
	private static SynchronizationContext m_mainSynchronizationContext;
	private static IntPtr m_channel;

	public static int State { get; private set; } = 0;

	public IEnumerator Start(int port)
	{
		State = 0;
		yield return new WaitForEndOfFrame();

		m_mainSynchronizationContext = SynchronizationContext.Current;
		if (m_mainSynchronizationContext == null)
		{
			m_mainSynchronizationContext = new SynchronizationContext();
		}

		m_runtimeDebugger = new Dictionary<byte, RuntimeDebuggerBase>();
		m_runtimeDebugger.Add(1, new RuntimeDebuggerInformation());
		m_runtimeDebugger.Add(2, new RuntimeDebuggerLog());
		m_runtimeDebugger.Add(3, new RuntimeDebuggerInspector());

		foreach (var item in m_runtimeDebugger)
		{
			item.Value.BindSend(WebSocketSend, item.Key);
		}

		string documentPath = Application.persistentDataPath;
		RunHttpd(port, documentPath);
	}

	public void Stop()
	{
		Dispose();
	}


	public void BindWebData(byte[] data)
	{
		if (data != null && data.Length > 0)
		{
			BindRuntimeDebuggerWebData(data, data.Length);
		}
	}

	public void Dispose()
	{
		StopHttpService();
		State = -1;
		Debug.Log("RuntimeDebugger Dispose.");
	}

	
	private void RunHttpd(int port, string documentPath)
	{
		try
		{
			Task.Run(() => {
				try
				{
					CreateHttpService(port, documentPath);
					BindWebSocketService(OnOpen, OnMessage, OnClose);
					RunHttpService(true);
					State = 1;
				}
				catch (Exception ex)
				{
					State = -1;
					Debug.LogWarning(ex);
				}
			});
		}
		catch (Exception e)
		{
			State = -1;
			Debug.LogWarning(e);
		}
	}


	private  static void WebSocketSend(byte key, object messageObject)
	{
		if (messageObject == null)
		{
			return;
		}
		var message =  JsonConvert.SerializeObject(messageObject,new VectorConverter());
		var bytes = System.Text.Encoding.UTF8.GetBytes(message);

		int size = bytes.Length + 4 + 1;
		List<byte> datas = new List<byte>();
		datas.AddRange(BitConverter.GetBytes(size));
		datas.Add(key);
		datas.AddRange(bytes);

		var dataArrary = datas.ToArray();
		fixed (byte* aaa = dataArrary)
		{
			if (m_channel == IntPtr.Zero)
			{
				return;
			}
			//int sendCount = dataArrary.Length > maxSize ? maxSize : dataArrary.Length;
			WebSocketSendBinary(m_channel, aaa, dataArrary.Length);
		}
	}

	#region  websocket回调
	[MonoPInvokeCallback(typeof(OnWebSocketOpenCallback))]
	private static void OnOpen(IntPtr channel, string req_path)
	{
		Console.WriteLine(string.Format("RuntimeDebugger.OnOpen {0} {1}", channel, channel));
		if (m_channel != IntPtr.Zero)
		{
			WebSocketClose(m_channel);
		}
		m_channel = channel;
		m_mainSynchronizationContext.Post((state) =>
		{
			//发送版本信息
			WebSocketSend(0, new RuntimeDebuggerVersion());
		}, null);
	}

	[MonoPInvokeCallback(typeof(OnWebSocketMessageCallback))]
	private static void OnMessage(IntPtr channel, byte* data, int size)
	{
		int* dataSizePtr = (int*)data;
		int dataSize = *dataSizePtr;
		byte type = data[4];
		string message = System.Text.Encoding.UTF8.GetString(data + 5, size - 5);

		if (m_runtimeDebugger.TryGetValue(type, out RuntimeDebuggerBase runtimeDebugger))
		{
			m_mainSynchronizationContext.Post((state) =>
			{
				runtimeDebugger.OnMessage(message);
			}, null);
		}

	}

	[MonoPInvokeCallback(typeof(OnWebSocketCloseCallback))]
	private static void OnClose(IntPtr channel)
	{
		m_channel = IntPtr.Zero;
	}

	#endregion


#if UNITY_IOS && !UNITY_EDITOR
	const string DLLXHV = "__Internal";
#else
	const string DLLXHV = "xhv";
#endif

	delegate void OnWebSocketOpenCallback(IntPtr channel, string req_path);
	delegate void OnWebSocketMessageCallback(IntPtr channel, byte* data, int size);
	delegate void OnWebSocketCloseCallback(IntPtr channel);

	[DllImport(DLLXHV)]
    extern static int CreateHttpService(int port,string document_root);

	[DllImport(DLLXHV)]
	extern static void RunHttpService(bool wait);
	[DllImport(DLLXHV)]
	extern static void StopHttpService();

	[DllImport(DLLXHV)]
	extern static void BindWebSocketService(OnWebSocketOpenCallback onOpen, OnWebSocketMessageCallback onMessage, OnWebSocketCloseCallback onClose);

	[DllImport(DLLXHV)]
	extern static void WebSocketSendBinary(IntPtr channel, byte* data, int size);
	[DllImport(DLLXHV)]
	extern static void WebSocketSend(IntPtr channel,string message);
	[DllImport(DLLXHV)]
	extern static void WebSocketClose(IntPtr channel);

	[DllImport(DLLXHV)]
	extern static void BindRuntimeDebuggerWebData(byte[] data, int size);
}



public class RuntimeDebuggerVersion
{
	public int Major { get; set; } = 0;
	public int Minor { get; set; } = 1;
	public int Patch { get; set; }= 0;
}
