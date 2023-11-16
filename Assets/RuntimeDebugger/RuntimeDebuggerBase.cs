using System;
using System.Collections;
using System.Collections.Generic;

public abstract class RuntimeDebuggerBase:IDisposable
{
	private byte m_sendKey;
	private Action<byte, string> m_sendAction;

	public void BindSend(Action<byte,string> sendAction, byte key)
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

	protected virtual void Send(string message)
	{
		if (string.IsNullOrEmpty(message))
		{
			return;
		}

		if (m_sendAction != null)
		{
			m_sendAction(m_sendKey, message);
		}
	}
}