using System;
using System.Collections;
using System.Collections.Generic;

public abstract class RuntimeDebuggerBase
{
	protected RuntimeDebugger m_runtimeDebugger;
	protected RuntimeDebuggerType m_runtimeDebuggerType;

	public virtual void OnOpen(RuntimeDebugger runtimeDebugger, RuntimeDebuggerType runtimeDebuggerType)
	{
		m_runtimeDebugger = runtimeDebugger;
		m_runtimeDebuggerType = runtimeDebuggerType;
	}

	public virtual void OnMessage(string message)
	{

	}

	public virtual void OnClose()
	{
		m_runtimeDebugger = null;
	}

	protected virtual void Send(string message)
	{
		if (string.IsNullOrEmpty(message))
		{
			return;
		}

		if (m_runtimeDebugger != null)
		{
			m_runtimeDebugger.Send(m_runtimeDebuggerType, message);
		}
	}
}