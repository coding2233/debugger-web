using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace RuntimeDebugger
{

	public class RuntimeDebuggerBehaviour : MonoBehaviour
	{
		private float m_windowScale;
		private bool m_showFullWindow;
		private Rect m_fullRect;
		private Rect m_smallRect;
		private Rect m_dragRect;
		private int m_selectIndex;
		private string[] m_toolBarNames;

		[SerializeField]
		private int m_serverPort = 2233;
		[SerializeField]
		private bool m_autoStart = true;
		[SerializeField]
		private bool m_runInBackground = true;
		private RuntimeDebugger m_runtimeDebugger;
		private List<string> m_showIPAddressList;

		private Vector2 m_ipAddressRect;

		private void Awake()
		{
			float widthScale = Screen.width / 1344.0f;
			float heightScale = Screen.height / 750f;
			m_windowScale = Mathf.Max(widthScale, heightScale);

			m_smallRect = new Rect(10, 10, 100, 60);
			m_fullRect = new Rect(10, 10, 700, 500);
			m_dragRect = new Rect(0f, 0f, float.MaxValue, 25f);

			Application.runInBackground = m_runInBackground;
		}

		private void Start()
		{
			if (m_autoStart)
			{
				StartServer();
			}
		}

		private void OnGUI()
		{
			Matrix4x4 lastMatrix = GUI.matrix;

			GUI.matrix = Matrix4x4.Scale(new Vector3(m_windowScale, m_windowScale, 1f));
			if (m_showFullWindow)
			{
				m_fullRect = GUILayout.Window(0, m_fullRect, DrawDebuggerFullWindow, "<b>RUNTIME DEBUGGER</b>");
			}
			else
			{
				m_smallRect = GUILayout.Window(0, m_smallRect, DrawDebuggerSmallWindow, "<b>DEBUGGER</b>");
			}
			GUI.matrix = lastMatrix;
		}

		private void Update()
		{
			if (m_runtimeDebugger != null)
			{
				m_runtimeDebugger.Update();
			}
		}

		private void OnDestroy()
		{
			if (m_runtimeDebugger != null)
			{
				m_runtimeDebugger.Stop();
				m_runtimeDebugger = null;
			}
		}

		private void DrawDebuggerFullWindow(int windowId)
		{
			GUI.DragWindow(m_dragRect);

			GUILayout.BeginHorizontal();
			if (m_runtimeDebugger == null)
			{
				if (GUILayout.Button("Start", GUILayout.Width(100), GUILayout.Height(30)))
				{
					StartServer();
				}
			}
			else
			{
				if (GUILayout.Button("Stop", GUILayout.Width(100), GUILayout.Height(30)))
				{
					m_runtimeDebugger.Stop();
					m_runtimeDebugger = null;
				}
			}
			if (GUILayout.Button("Close", GUILayout.Width(100), GUILayout.Height(30)))
			{
				m_showFullWindow = false;
			}
			//int selectIndex = GUILayout.Toolbar(m_selectIndex, m_toolBarNames, GUILayout.Height(30f), GUILayout.MaxWidth(Screen.width));
			GUILayout.EndHorizontal();

			bool serverRuning = m_runtimeDebugger != null;
			if (serverRuning)
			{
				if (m_showIPAddressList != null && m_showIPAddressList.Count > 0)
				{
					m_ipAddressRect = GUILayout.BeginScrollView(m_ipAddressRect, "box");
					foreach (var item in m_showIPAddressList)
					{
						if (string.IsNullOrEmpty(item))
						{ continue; }

						GUILayout.BeginHorizontal();
						if (GUILayout.Button("Open", GUILayout.Width(80), GUILayout.Height(30)))
						{
							Application.OpenURL(item);
						}
						if (GUILayout.Button("Copy", GUILayout.Width(80), GUILayout.Height(30)))
						{
							UnityEngine.GUIUtility.systemCopyBuffer = item;
						}
						GUILayout.Label(item);
						GUILayout.EndHorizontal();
					}
					GUILayout.EndScrollView();
				}
			}
		}
		//绘制小窗口
		private void DrawDebuggerSmallWindow(int windowId)
		{
			GUI.DragWindow(m_dragRect);

			Color defaultColor = GUI.contentColor;
			string title = "stopped";
			if (m_runtimeDebugger != null)
			{
				GUI.contentColor = Color.green;
				title = "running";
			}
			if (GUILayout.Button(title, GUILayout.Width(100f), GUILayout.Height(40f)))
			{
				m_showFullWindow = true;
			}
			GUI.contentColor = defaultColor;
		}


		private void StartServer()
		{
			if (m_runtimeDebugger != null)
			{
				m_runtimeDebugger.Dispose();
				m_runtimeDebugger = null;
			}
			m_runtimeDebugger = new RuntimeDebugger();
			TextAsset webTextAsset = Resources.Load<TextAsset>("debugger_web");
			m_runtimeDebugger.BindWebData(webTextAsset.bytes);
			m_showIPAddressList = new List<string>();
			StringBuilder showIPAddressBuilder = new StringBuilder();
			foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
			{
				foreach (var ip in item.GetIPProperties().UnicastAddresses)
				{
					showIPAddressBuilder.Clear();
					showIPAddressBuilder.Append("http://");
					string ipAddress = ip.Address.ToString();
					if (ipAddress.Contains(":"))
					{
						showIPAddressBuilder.Append("[");
						//暂时忽略ipv6
						continue;
					}
					showIPAddressBuilder.Append(ipAddress);
					if (ipAddress.Contains(":"))
					{
						showIPAddressBuilder.Append("]");
					}
					showIPAddressBuilder.Append(":");
					showIPAddressBuilder.Append(m_serverPort);
					showIPAddressBuilder.Append("/");
					showIPAddressBuilder.Append("debugger/");

					m_showIPAddressList.Add(showIPAddressBuilder.ToString());

				}
			}
			Debug.Log("RunHttpService.");
			m_runtimeDebugger.Start(m_serverPort);
		}

	}


	
}