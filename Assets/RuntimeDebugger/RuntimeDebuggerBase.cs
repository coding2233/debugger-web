using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeDebugger
{
	public abstract class RuntimeDebuggerBase : IDisposable
	{
		private byte m_sendKey;
		private Action<byte, object> m_sendAction;
		private string m_debuggerName;
		public string DebuggerName
		{
			get
			{
				if (string.IsNullOrEmpty(m_debuggerName))
				{
					m_debuggerName = GetType().Name.Replace("RuntimeDebugger", "");
					Debug.Log(m_debuggerName);
				}
				return m_debuggerName;
			}
		}

		public void BindSend(Action<byte, object> sendAction, byte key)
		{
			m_sendKey = key;
			m_sendAction = sendAction;
		}

		public virtual void OnReset()
		{ }

		public virtual void OnMessage(string message)
		{

		}

		public virtual void OnUpdate()
		{ 
		}

		public virtual void Dispose()
		{
			m_sendAction = null;
		}

		public virtual void OnGUI()
		{
			
		}

		public virtual string GetSmallGUITitle(ref DebuggerPriority priority)
		{
			return null;
		}

		protected virtual void Send(object messageObject)
		{
			if (messageObject == null)
			{
				return;
			}

			if (m_sendAction != null)
			{
				m_sendAction(m_sendKey, messageObject);
			}
		}
	}
}