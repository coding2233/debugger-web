using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace RuntimeDebugger
{
	public class RuntimeDebuggerTerminal : RuntimeDebuggerBase
	{
		private static Dictionary<string, TerminalCommand> s_terminalCommands = new Dictionary<string, TerminalCommand>();

		public RuntimeDebuggerTerminal()
		{
			BindSystemCommands();
			BindGameObjectCommands();
			BindQualityCommands();
			BindInputCommands();
		}

		public override void OnMessage(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}

			var terminalCommand = RunCommand(message);
			Send(terminalCommand);
		}

		private TerminalMessage RunCommand(string commandMessage)
		{
			TerminalMessage terminalMessage = new TerminalMessage();
			terminalMessage.Command = commandMessage;
			int resultCode = 0;
			var args = commandMessage.Split(' ');
			string result = string.Empty;
			if (s_terminalCommands.TryGetValue(args[0], out TerminalCommand command))
			{
				string[] arguments = null;
				if (args.Length > 1)
				{
					arguments = new string[args.Length - 1];
					for (int i = 1; i < args.Length; i++)
					{
						arguments[i - 1] = args[i];
					}
				}
				result = command.Run(arguments, out resultCode);
			}
			else
			{
				resultCode = -1;
				result = "command not found.";
			}
			terminalMessage.Result = result;
			terminalMessage.ResultCode = resultCode;
			return terminalMessage;
		}

		#region commands
		private void BindSystemCommands()
		{
			Func<string> helpFunc = () => {
				StringBuilder helpText = new StringBuilder();
				foreach (var item in s_terminalCommands)
				{
					helpText.Append(item.Key);
					helpText.AppendLine(item.Value.Help);
				}
				return helpText.ToString();
			};

			BindCommand("?", helpFunc);
			BindCommand("help", helpFunc);
		}

		private void BindGameObjectCommands()
		{
			BindCommand<string>("GameObject.Find", (value) => {
				string reult = "Not found";
				var findGameObject = GameObject.Find(value);
				if (findGameObject != null)
				{
					reult = $"id:{findGameObject.GetInstanceID()} name:{findGameObject.name}";
				}
				return reult;
			});
		}

		private void BindQualityCommands()
		{
			BindCommand<int>("Quality.SetLevel", (level) => {
				QualitySettings.SetQualityLevel(level);
				string result = $"QualitySettings.GetQualityLevel {QualitySettings.GetQualityLevel()}";
				return result;
			});
		}

		private void BindInputCommands()
		{
			BindCommand<bool>("Input.Location", (value) => {
				if (value)
				{
					Input.location.Start();
				}
				else
				{
					Input.location.Stop();
				}
				string result = $"Input.location.status {Input.location.status}";
				return result;
			});

			BindCommand<bool>("Input.gyro", (value) => {
				Input.gyro.enabled = value;
				string result = $"Input.gyro.enabled {Input.gyro.enabled}";
				return result;
			});

			BindCommand<bool>("Input.compass", (value) => {
				Input.compass.enabled = value;
				string result = $"Input.compass.enabled {Input.compass.enabled}";
				return result;
			});
		}
		#endregion

		#region binds
		public static TerminalCommand BindCommand(string command, Func<string> callCommand)
		{
			TerminalCommand terminalCommand = null;
			if (!s_terminalCommands.TryGetValue(command, out terminalCommand))
			{
				terminalCommand = new FuncTerminalCommand(callCommand);
				s_terminalCommands.Add(command, terminalCommand);
			}
			return terminalCommand;
		}
		public static TerminalCommand BindCommand<TParam1>(string command,Func<TParam1, string> callCommand)
		{
			TerminalCommand terminalCommand = null;
			if (!s_terminalCommands.TryGetValue(command, out terminalCommand))
			{
				terminalCommand = new FuncTerminalCommand<TParam1>(callCommand);
				s_terminalCommands.Add(command,terminalCommand);
			}
			return terminalCommand;
		}
		public static TerminalCommand BindCommand<TParam1, TParam2>(string command, Func<TParam1, TParam2, string> callCommand)
		{
			TerminalCommand terminalCommand = null;
			if (!s_terminalCommands.TryGetValue(command, out terminalCommand))
			{
				terminalCommand = new FuncTerminalCommand<TParam1, TParam2>(callCommand);
				s_terminalCommands.Add(command, terminalCommand);
			}
			return terminalCommand;
		}
		public static TerminalCommand BindCommand<TParam1, TParam2, TParam3>(string command, Func<TParam1, TParam2, TParam3, string> callCommand)
		{
			TerminalCommand terminalCommand = null;
			if (!s_terminalCommands.TryGetValue(command, out terminalCommand))
			{
				terminalCommand = new FuncTerminalCommand<TParam1, TParam2, TParam3>(callCommand);
				s_terminalCommands.Add(command, terminalCommand);
			}
			return terminalCommand;
		}

		public static void BindCommandAlias(string originName, string newName)
		{
			if (s_terminalCommands.TryGetValue(originName, out TerminalCommand terminalCommand))
			{
				if (!s_terminalCommands.ContainsKey(newName))
				{
					s_terminalCommands.Add(newName, terminalCommand);
				}
			}
		}
		#endregion
	}

	public class TerminalMessage
	{
		public string Command { get; set; }
		public string Result { get; set; }
		public int ResultCode { get; set; }
	}

	#region TerminalCommand
	public class TerminalCommand
	{
		public string Help { get; protected set; }
		public virtual string Run(string[] arguments,out int resultCode)
		{
			resultCode = -1;
			return "command arguments error";
		}

		public bool TryParse<T>(string argument,out T value)
		{
			value = default(T);
			if (!string.IsNullOrEmpty(argument))
			{
				if (typeof(int) == typeof(T))
				{
					if (int.TryParse(argument, out int result))
					{
						value = (T)Convert.ChangeType(result, typeof(T));
						return true;
					}
				}
				else if (typeof(float) == typeof(T))
				{
					if (float.TryParse(argument, out float result))
					{
						value = (T)Convert.ChangeType(result, typeof(T));
						return true;
					}
				}
				else if (typeof(bool) == typeof(T))
				{
					if (bool.TryParse(argument, out bool result))
					{
						value = (T)Convert.ChangeType(result, typeof(T));
						return true;
					}
				}
				else if (typeof(string) == typeof(T))
				{
					value = (T)Convert.ChangeType(argument, typeof(T));
					return true;
				}
				// to do ...
			}
			return false;
		}
	}

	public class FuncTerminalCommand : TerminalCommand
	{
		private Func<string> m_call;

		public FuncTerminalCommand(Func<string> callCommand)
		{
			m_call = callCommand;
			Help = $" ";
		}

		public override string Run(string[] argument, out int resultCode)
		{
			if (m_call != null)
			{
				var result = m_call();
				resultCode = 0;
				return result;
			}
			return base.Run(argument,out resultCode);
		}
	}

	public class FuncTerminalCommand<TParam1> : TerminalCommand
	{
		private Func<TParam1, string> m_call;

		public FuncTerminalCommand(Func<TParam1, string> callCommand)
		{
			m_call = callCommand;
			Help = $" {typeof(TParam1).Name}";
		}

		public override string Run(string[] argument, out int resultCode)
		{
			if (argument!=null && argument.Length>0)
			{
				if (m_call != null)
				{
					if (TryParse<TParam1>(argument[0], out TParam1 param1))
					{
						var reuslt = m_call.Invoke(param1);
						resultCode = 0;
						return reuslt;
					}
					
				}
			}
			return base.Run(argument, out resultCode);
		}
	}

	public class FuncTerminalCommand<TParam1, TParam2> : TerminalCommand
	{
		private Func<TParam1, TParam2,string> m_call;

		public FuncTerminalCommand(Func<TParam1, TParam2, string> callCommand)
		{
			m_call = callCommand;
			Help = $" {typeof(TParam1).Name} {typeof(TParam2).Name}";
		}

		public override string Run(string[] argument, out int resultCode)
		{
			if (argument != null && argument.Length > 1)
			{
				if (m_call != null)
				{
					if (TryParse<TParam1>(argument[0], out TParam1 param1)
						&& TryParse<TParam2>(argument[1], out TParam2 param2))
					{
						var reuslt = m_call.Invoke(param1, param2);
						resultCode = 0;
						return reuslt;
					}

				}
			}
			return base.Run(argument,out resultCode);
		}
	}

	public class FuncTerminalCommand<TParam1, TParam2, TParam3> : TerminalCommand
	{
		private Func<TParam1, TParam2, TParam3,string> m_call;

		public FuncTerminalCommand(Func<TParam1, TParam2, TParam3, string> callCommand)
		{
			m_call = callCommand;
			Help = $" {typeof(TParam1).Name} {typeof(TParam2).Name} {typeof(TParam3).Name}";
		}

		public override string Run(string[] argument,out int resultCode)
		{
			if (argument != null && argument.Length > 2)
			{
				if (m_call != null)
				{
					if (TryParse<TParam1>(argument[0], out TParam1 param1)
						&& TryParse<TParam2>(argument[1], out TParam2 param2)
						&& TryParse<TParam3>(argument[2], out TParam3 param3))
					{
						var reuslt = m_call.Invoke(param1, param2, param3);
						resultCode = 0;
						return reuslt;
					}

				}
			}
			return base.Run(argument,out resultCode);
		}
	}

	#endregion

}