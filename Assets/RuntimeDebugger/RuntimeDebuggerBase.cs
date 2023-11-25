using System;
using System.Collections;
using System.Collections.Generic;
namespace RuntimeDebugger
{
	public abstract class RuntimeDebuggerBase : IDisposable
	{
		private byte m_sendKey;
		private Action<byte, object> m_sendAction;

		public void BindSend(Action<byte, object> sendAction, byte key)
		{
			m_sendKey = key;
			m_sendAction = sendAction;
		}



		public virtual void OnMessage(string message)
		{

		}

		public virtual void Dispose()
		{
			m_sendAction = null;
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