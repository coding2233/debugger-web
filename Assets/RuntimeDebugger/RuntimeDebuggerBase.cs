using System;
using System.Collections;
using System.Collections.Generic;

public abstract class RuntimeDebuggerBase
{
	protected RuntimeDebugger m_runtimeDebugger;
	protected IntPtr m_channel;

	public virtual void OnOpen(RuntimeDebugger runtimeDebugger, IntPtr channel)
	{
		m_runtimeDebugger = runtimeDebugger;
		m_channel = channel;
	}

	public virtual void OnMessage(string message)
	{

	}

	public virtual void OnClose()
	{
		m_channel = IntPtr.Zero;
		m_runtimeDebugger = null;
	}

	protected virtual void Send(string message)
	{
		if (string.IsNullOrEmpty(message))
		{
			return;
		}

		if (m_runtimeDebugger != null && m_channel != IntPtr.Zero)
		{
			m_runtimeDebugger.Send(m_channel, message);
		}
	}
}