using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public unsafe class XHVRuntime
{

	void OnOpen(IntPtr channel, string req_path)
	{
		Console.WriteLine("on open " + req_path + "  "+ channel);
	}
	void OnMessage(IntPtr channel, byte* data, int size)
	{
		var message = System.Text.Encoding.UTF8.GetString(data, size);
		Console.WriteLine("on message " + message);

		string replay_message = "啊书法大赛发是是豆腐干山豆根地方" + message;
		//WebSocketSend(channel, replay_message);
		var bytes = System.Text.Encoding.UTF8.GetBytes(replay_message);
		fixed (byte* aaa = bytes)
		{
			WebSocketSendBinary(channel, aaa, bytes.Length);
		}
	}

	void OnClose(IntPtr channel)
	{
		Console.WriteLine("on close ");
	}

	public void Run()
    {
		try
		{
			CreateHttpService(22333);
			BindWebSocketService(OnOpen,OnMessage,OnClose);
			RunHttpService(true);
			Console.WriteLine("down");
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
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
	extern static void WebSocketSend(IntPtr channel, string message);
}
