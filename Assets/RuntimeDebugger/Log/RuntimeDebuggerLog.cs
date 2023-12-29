using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RuntimeDebugger
{
	public class RuntimeDebuggerLog : RuntimeDebuggerBase
	{
		DebuggerProtocol.LogList m_logNodeList;
		DebuggerProtocol.LogList m_logNodeSendList;
		DebuggerProtocol.LogNode m_logNodeSelected;
		private DebuggerPriority m_logPriority;
		private Vector2 m_logScrollView;
		private Vector2 m_logSelectScrollView;

		public RuntimeDebuggerLog()
		{
			m_logNodeList = new DebuggerProtocol.LogList();
			m_logNodeSendList = new DebuggerProtocol.LogList();
			Application.logMessageReceived += OnLogMessageReceived;
		}

		public override void Dispose()
		{
			Application.logMessageReceived -= OnLogMessageReceived;
			base.Dispose();
		}


		private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
		{
			DebuggerProtocol.LogNode logNode = new DebuggerProtocol.LogNode();
			logNode.LogTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
			logNode.LogFrameCount = Time.frameCount;
			logNode.LogType = (DebuggerProtocol.LogType)(int)type;
			switch (type)
			{
				case LogType.Error:
					if (m_logPriority < DebuggerPriority.Error)
					{
						m_logPriority = DebuggerPriority.Error;
					}
					break;
				case LogType.Warning:
					if (m_logPriority < DebuggerPriority.Warn)
					{ 
						m_logPriority = DebuggerPriority.Warn;
					}
					break;
				case LogType.Log:
					break;
				case LogType.Assert:
					if (m_logPriority < DebuggerPriority.Error)
					{
						m_logPriority = DebuggerPriority.Error;
					}
					break;
				case LogType.Exception:
					if (m_logPriority < DebuggerPriority.Error)
					{
						m_logPriority = DebuggerPriority.Error;
					}
					break;
				default:
					break;
			}

			logNode.LogMessage = $"[{logNode.LogTime}] [{logNode.LogFrameCount}] [{type}] {condition}";
			logNode.LogStackTrack = stackTrace;

			m_logNodeList.LogNodeList.Add(logNode);
			m_logNodeSendList.LogNodeList.Add(logNode);

			SendLogList();
		}

		private void SendLogList()
		{
			if (m_logNodeSendList != null)
			{
				if (m_logNodeSendList.LogNodeList.Count > 0)
				{
					Send(m_logNodeSendList);
					m_logNodeSendList.LogNodeList.Clear();
				}
			}
		}

		public override string GetSmallGUITitle(ref DebuggerPriority priority)
		{
			if (m_logPriority > DebuggerPriority.Log)
			{
				priority = m_logPriority;
			}
			return null;
		}

		public override void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(m_logNodeList.LogNodeList.Count.ToString());
			if (GUILayout.Button("Clear",GUILayout.Width(100)))
			{
				m_logNodeSelected = null;
				m_logNodeList.LogNodeList.Clear();
				m_logNodeSendList.LogNodeList.Clear();
				m_logPriority = DebuggerPriority.None;
			}
			if (GUILayout.Button("Send", GUILayout.Width(100)))
			{
				m_logNodeSendList.LogNodeList.AddRange(m_logNodeList.LogNodeList);
				SendLogList();
			}
			GUILayout.EndHorizontal();

			m_logScrollView = GUILayout.BeginScrollView(m_logScrollView,"box");
			foreach (var logNode in m_logNodeList.LogNodeList)
			{
				Color32 color = GetStringColor(logNode.LogType);
				string logMessage= string.Format("<color=#{0}{1}{2}{3}>{6}</color>",
					color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"), logNode.LogMessage);
				bool select = m_logNodeSelected == logNode;
				select = GUILayout.Toggle(select, logMessage);
				if (select)
				{
					m_logNodeSelected = logNode;
				}
			}
			GUILayout.EndScrollView();

			if (m_logNodeSelected != null)
			{
				m_logSelectScrollView = GUILayout.BeginScrollView(m_logSelectScrollView, "box", GUILayout.Height(150));
				GUILayout.Label(m_logNodeSelected.LogStackTrack);
				GUILayout.EndScrollView();
			}
		}

		private Color GetStringColor(DebuggerProtocol.LogType logType)
		{
			Color32 color = Color.white;
			switch (logType)
			{
				case DebuggerProtocol.LogType.Log: color = Color.white; break;
				case DebuggerProtocol.LogType.Warning: color = Color.yellow; break;
				case DebuggerProtocol.LogType.Error: color = Color.red; break;
				case DebuggerProtocol.LogType.Exception: color = Color.red; break;
				case DebuggerProtocol.LogType.Assert: color = Color.red; break;
			}
			return color;
		}

	}

	//private class LogNode
	//{
	//	public DateTime LogTime { get; private set; }
	//	public int LogFrameCount { get; private set; }
	//	public LogType LogType { get; private set; }
	//	public string LogMessage { get; private set; }
	//	public string StackTrack { get; private set; }

	//	public LogNode Set(string condition, string stackTrace, LogType type)
	//	{
	//		if (type == LogType.Assert)
	//		{
	//			type = LogType.Error;
	//		}
	//		LogTime = DateTime.Now;
	//		LogFrameCount = Time.frameCount;
	//		LogType = type;
	//		LogMessage = condition;
	//		StackTrack = stackTrace;
	//		return this;
	//	}

	//	public override string ToString()
	//	{
	//		Color32 color = GetStringColor();
	//		return string.Format("<color=#{0}{1}{2}{3}>[{4}][{5}] {6}</color>",
	//			color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"),
	//			LogTime.ToString("HH:mm:ss.fff"), LogFrameCount.ToString(), LogMessage);
	//	}

	//	public Color GetStringColor()
	//	{
	//		Color32 color = Color.white;
	//		switch (LogType)
	//		{
	//			case LogType.Log:
	//				color = Color.white;
	//				break;

	//			case LogType.Warning:
	//				color = Color.yellow;
	//				break;

	//			case LogType.Error:
	//				color = Color.red;
	//				break;

	//			case LogType.Exception:
	//				color = Color.red;
	//				break;
	//		}

	//		return color;
	//	}

	//}


}