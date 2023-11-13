using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public unsafe class RuntimeDebugger : MonoBehaviour
{
	private Dictionary<string, RuntimeDebuggerBase> m_registerRuntimeDebugger;
	private Dictionary<IntPtr, RuntimeDebuggerBase> m_runtimeDebugger;
	private SynchronizationContext m_mainSynchronizationContext;

	void OnOpen(IntPtr channel, string req_path)
	{
		Console.WriteLine(string.Format("RuntimeDebugger.OnOpen {0} {1}", channel, channel));
		if (m_registerRuntimeDebugger.TryGetValue(req_path, out RuntimeDebuggerBase runtimeDebugger))
		{
			m_runtimeDebugger.Add(channel, runtimeDebugger);
			m_mainSynchronizationContext.Post((state) => {
				runtimeDebugger.OnOpen(this, channel);
			}, null);
		}
	}
	void OnMessage(IntPtr channel, byte* data, int size)
	{
		var message = System.Text.Encoding.UTF8.GetString(data, size);
		//Console.WriteLine("on message " + message);
		if (m_runtimeDebugger.TryGetValue(channel, out RuntimeDebuggerBase runtimeDebugger))
		{
			m_mainSynchronizationContext.Post((state) => {
				runtimeDebugger.OnMessage(message);
			}, null);
		}

		var bytes = System.Text.Encoding.UTF8.GetBytes(message);
		fixed (byte* aaa = bytes)
		{
			WebSocketSendBinary(channel, aaa, bytes.Length);
		}
	}

	void OnClose(IntPtr channel)
	{
		//Console.WriteLine("on close");
		if (m_runtimeDebugger.TryGetValue(channel, out RuntimeDebuggerBase runtimeDebugger))
		{
			m_mainSynchronizationContext.Post((state) => {
				runtimeDebugger.OnClose();
			}, null);
			m_runtimeDebugger.Remove(channel);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		m_mainSynchronizationContext = SynchronizationContext.Current;
		if (m_mainSynchronizationContext == null)
		{
			m_mainSynchronizationContext = new SynchronizationContext();
		}

		m_runtimeDebugger = new Dictionary<IntPtr, RuntimeDebuggerBase>();
		m_registerRuntimeDebugger = new Dictionary<string, RuntimeDebuggerBase>();
		m_registerRuntimeDebugger.Add("/",new RuntimeDebuggerInformation());

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
					Console.WriteLine(ex);
				}
				});
			Debug.Log("RunHttpService.");
		}
		catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

	private void OnDestroy()
	{
        StopHttpService();
		Debug.Log("OnDestroy.");
	}


	public void Send(IntPtr channel,string message)
	{
		if (channel == IntPtr.Zero)
		{
			return;
		}

		if (string.IsNullOrEmpty(message))
		{
			return;
		}

		var bytes = System.Text.Encoding.UTF8.GetBytes(message);
		fixed (byte* aaa = bytes)
		{
			WebSocketSendBinary(channel, aaa, bytes.Length);
		}
	}

	delegate void OnWebSocketOpenCallback(IntPtr channel, string req_path);
	delegate void OnWebSocketMessageCallback(IntPtr channel, byte* data, int size);
	delegate void OnWebSocketCloseCallback(IntPtr channel);

	[DllImport("xhv.dll")]
    extern static int CreateHttpService(int port);

	[DllImport("xhv.dll")]
	extern static void RunHttpService(bool wait);
	[DllImport("xhv.dll")]
	extern static void StopHttpService();

	[DllImport("xhv.dll")]
	extern static void BindWebSocketService(OnWebSocketOpenCallback onOpen, OnWebSocketMessageCallback onMessage, OnWebSocketCloseCallback onClose);

	[DllImport("xhv.dll")]
	extern static void WebSocketSendBinary(IntPtr channel, byte* data, int size);
	[DllImport("xhv.dll")]
	extern static void WebSocketSend(IntPtr channel,string message);
}



