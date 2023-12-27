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
		LogNode m_logNode;
		private DebuggerPriority m_logPriority;

		public RuntimeDebuggerLog()
		{
			Application.logMessageReceived += OnLogMessageReceived;
		}

		public override void Dispose()
		{
			Application.logMessageReceived -= OnLogMessageReceived;
			base.Dispose();
		}


		private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
		{
			if (type != LogType.Log)
			{
				if (m_logPriority < DebuggerPriority.Error)
				{
					switch (type)
					{
						case LogType.Error:
						case LogType.Assert:
						case LogType.Exception:
							m_logPriority = DebuggerPriority.Error;
							break;
						case LogType.Warning:
							m_logPriority = DebuggerPriority.Warn;
							break;
					}
				}
			}

			if (m_logNode == null)
			{
				m_logNode = new LogNode();
			}
			m_logNode.Set(condition, stackTrace, type);
			Send(m_logNode);
		}

		public override string GetSmallGUITitle(ref DebuggerPriority priority)
		{
			if (m_logPriority > DebuggerPriority.Log)
			{
				return m_logPriority.ToString();
			}
			return null;
		}

		private class LogNode
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


	}
}