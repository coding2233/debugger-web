using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;

public class DebuggerLog: RuntimeDebuggerBase
{
	LogNode m_logNode;
	public override void OnOpen(RuntimeDebugger runtimeDebugger, IntPtr channel)
	{
		base.OnOpen(runtimeDebugger, channel);
		
		Application.logMessageReceived += OnLogMessageReceived;
	}

	public override void OnClose()
	{
		Application.logMessageReceived -= OnLogMessageReceived;
		base.OnClose();
	}

	private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
	{
		if (m_logNode == null)
		{
			m_logNode = new LogNode();
		}
		 m_logNode.Set(condition, stackTrace, type);
		string logNodeMessage = JsonConvert.SerializeObject(m_logNode);
		Debug.Log(logNodeMessage);
		Send(logNodeMessage);
	}

	internal class LogNode
	{
		public DateTime LogTime { get; private set; }
		public int LogFrameCount { get; private set; }
		public LogType LogType { get; private set; }
		public string LogMessage { get; private set; }
		public string StackTrack { get; private set; }

		public LogNode Set(string condition, string stackTrace, LogType type)
		{
			if (type == LogType.Assert)
			{
				type = LogType.Error;
			}
			LogTime = DateTime.Now;
			LogFrameCount = Time.frameCount;
			LogType = type;
			LogMessage = condition;
			StackTrack = stackTrace;
			return this;
		}

		public override string ToString()
		{
			Color32 color = GetStringColor();
			return string.Format("<color=#{0}{1}{2}{3}>[{4}][{5}] {6}</color>",
				color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"),
				LogTime.ToString("HH:mm:ss.fff"), LogFrameCount.ToString(), LogMessage);
		}

		public Color GetStringColor()
		{
			Color32 color = Color.white;
			switch (LogType)
			{
				case LogType.Log:
					color = Color.white;
					break;

				case LogType.Warning:
					color = Color.yellow;
					break;

				case LogType.Error:
					color = Color.red;
					break;

				case LogType.Exception:
					color = Color.red;
					break;
			}

			return color;
		}

	}

	internal static class LogNodePool
	{
		private static readonly ObjectPool<LogNode> _objectPool = new ObjectPool<LogNode>(null, null);

		public static LogNode Get(string condition, string stackTrace, LogType type)
		{
			return _objectPool.Get().Set(condition, stackTrace, type);
		}

		public static void Release(LogNode logNode)
		{
			_objectPool.Release(logNode);
		}

	}
}
