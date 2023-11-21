using AOT;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public unsafe class RuntimeDebugger : MonoBehaviour
{
	private static Dictionary<byte, RuntimeDebuggerBase> m_runtimeDebugger;
	private static SynchronizationContext m_mainSynchronizationContext;
	private static IntPtr m_channel;

	[MonoPInvokeCallback(typeof(OnWebSocketOpenCallback))]
	private static void OnOpen(IntPtr channel, string req_path)
	{
		Console.WriteLine(string.Format("RuntimeDebugger.OnOpen {0} {1}", channel, channel));
		if (m_channel != IntPtr.Zero)
		{
			WebSocketClose(m_channel);
		}
		m_channel = channel;
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

	// Start is called before the first frame update
	void Start()
    {
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

		//StartCoroutine(TestLog());
		try
        {
           
			Task.Run(() => {
				try
				{
					CreateHttpService(2233);
					BindWebSocketService(OnOpen, OnMessage, OnClose);
					RunHttpService(true);
				}
				catch (Exception ex) 
				{
					Debug.LogWarning(ex);
				}
				});
			Debug.Log("RunHttpService.");
		}
		catch (Exception e)
        {
			Debug.LogWarning(e);
        }
    }

	private void OnDestroy()
	{
        StopHttpService();
		Debug.Log("OnDestroy.");
	}

	IEnumerator TestLog()
	{
		var wfs = new WaitForSeconds(1);
		while (true)
		{
			yield return wfs;
			Debug.Log($"TestLog {UnityEngine.Random.Range(0,99999999).ToString("D8")} {DateTime.Now} #");
			yield return wfs;
			Debug.LogWarning($"TestLog {UnityEngine.Random.Range(0, 99999999).ToString("D8")} {DateTime.Now} #");
			//yield return wfs;
			//Debug.LogError($"TestLog {UnityEngine.Random.Range(0, 99999999).ToString("D8")} {DateTime.Now} #");

		}
	}

	public void WebSocketSend(byte key, object messageObject)
	{
		if (messageObject == null)
		{
			return;
		}
		var message =  JsonConvert.SerializeObject(messageObject,new VectorConverter());
		var bytes = System.Text.Encoding.UTF8.GetBytes(message);

		int maxSize = 4096; 

		//using (MemoryStream ms = new MemoryStream())
		//{
		//	using (BsonWriter writer = new BsonWriter(ms))
		//	{
		//		JsonSerializer serializer = new JsonSerializer();
		//		serializer.Serialize(writer, messageObject);
		//	}
		//	int oldSize = bytes.Length;
		//	bytes = ms.ToArray();
		//}

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


#if UNITY_IOS && !UNITY_EDITOR
	const string DLLXHV = "__Internal";
#else
	const string DLLXHV = "xhv";
#endif

	delegate void OnWebSocketOpenCallback(IntPtr channel, string req_path);
	delegate void OnWebSocketMessageCallback(IntPtr channel, byte* data, int size);
	delegate void OnWebSocketCloseCallback(IntPtr channel);

	[DllImport(DLLXHV)]
    extern static int CreateHttpService(int port);

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
}



