using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class RuntimeDebugger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        File.CreateText("a.txt");
        try
        {
			int ret = CreateHttpService(12233, "a.txt");
            Debug.Log(ret);
			//RunHttpService();
			Debug.Log("RunHttpService error");
			Task.Run(
                () =>
                {
                   
                }
                );
        }
        catch(Exception e)
        {

            Debug.LogException(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    delegate void OnWebSocketOpenCallback(IntPtr channel,string req_path);
    delegate void OnWebSocketMessageCallback(IntPtr channel,string message);
    delegate void OnWebSocketCloseCallback(IntPtr channel);

	[DllImport("xhv.dll")]
    extern static int CreateHttpService(int port, string config);

	[DllImport("xhv.dll")]
	extern static void RunHttpService();

	[DllImport("xhv.dll")]
	extern static void BindWebSocketService(OnWebSocketOpenCallback onOpen, OnWebSocketMessageCallback onMessage, OnWebSocketCloseCallback onClose);

	[DllImport("xhv.dll")]
	extern static void WebSocketSend(IntPtr channel,string message);
}
