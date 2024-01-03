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
		private static Dictionary<string, string> s_aliasCommands = new Dictionary<string, string>();
		private static Dictionary<string, string> s_onlyHelpCommands = new Dictionary<string, string>();

		private string m_cmdInput;
		private Vector2 m_commandView;
		private List<string> m_commandLines;

		private static int s_maxCommandLength;
		private static int s_maxAargumentsLength;

		public RuntimeDebuggerTerminal()
		{
			m_commandLines = new List<string>();
			m_cmdInput = "help";

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

		public override void OnGUI()
		{
			m_commandView = GUILayout.BeginScrollView(m_commandView, "box");
			foreach (var item in m_commandLines)
			{
				GUILayout.Label(item);
			}
			GUILayout.EndScrollView();

			GUILayout.BeginHorizontal(GUILayout.Height(30));
			m_cmdInput = GUILayout.TextField(m_cmdInput);
			if (GUILayout.Button("run", GUILayout.Width(100)))
			{
				if (!string.IsNullOrEmpty(m_cmdInput))
				{
					if ("clear".Equals(m_cmdInput))
					{
						m_commandLines.Clear();
					}
					else
					{
						var terminalCommand = RunCommand(m_cmdInput);
						m_commandLines.Add(m_cmdInput);
						m_commandLines.Add(terminalCommand.Result);
					}
					m_cmdInput = string.Empty;
				}
			}
			GUILayout.EndHorizontal();
		}


		private TerminalMessage RunCommand(string commandMessage)
		{
			TerminalMessage terminalMessage = new TerminalMessage();
			terminalMessage.Command = commandMessage;
			int resultCode = 0;
			var args = commandMessage.Split(' ');
			string commandName = args[0];
			if (s_aliasCommands.TryGetValue(commandName, out string originCommandName))
			{
				commandName = originCommandName;
			}
			string result = string.Empty;
			if (s_terminalCommands.TryGetValue(commandName, out TerminalCommand command))
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
				int maxCommandLength = s_maxCommandLength + 4;
				int maxAargumentsLength = s_maxAargumentsLength + 2;
				StringBuilder helpText = new StringBuilder();
				Dictionary<string, List<string>> aliasCommands = new Dictionary<string, List<string>>();
				foreach (var item in s_aliasCommands)
				{
					List<string> aliasNames = null;
					if (!aliasCommands.TryGetValue(item.Value, out aliasNames))
					{
						aliasNames = new List<string>();
						aliasCommands.Add(item.Value, aliasNames);
					}
					aliasNames.Add(item.Key);
				}
				foreach (var item in s_terminalCommands)
				{
					helpText.Append(item.Key);
					int keyLength = item.Key.Length;
					if (aliasCommands.TryGetValue(item.Key, out List<string> alias))
					{
						foreach (var itemAlia in alias)
						{
							helpText.Append(",");
							helpText.Append(itemAlia);
							keyLength += itemAlia.Length + 1;
						}
					}
					helpText.Append(' ', maxCommandLength - keyLength);
					helpText.Append(item.Value.Aarguments);
					helpText.Append(' ', maxAargumentsLength - item.Value.Aarguments.Length);
					helpText.AppendLine(string.IsNullOrEmpty(item.Value.Help) ?  " " : item.Value.Help);
				}
				foreach (var item in s_onlyHelpCommands)
				{
					helpText.Append(item.Key);
					helpText.Append(' ', maxCommandLength - item.Key.Length);
					helpText.Append(' ', maxAargumentsLength);
					helpText.AppendLine(item.Value);
				}
				return helpText.ToString();
			};

			BindCommand("help", helpFunc, "Show this text.");
			BindCommandAlias("help", "?");
			BindOnlyHelpCommand("clear", "Clears your terminal screen.");
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
			}, "Find GameObject by Name.");
		}

		private void BindQualityCommands()
		{
			BindCommand<int>("Quality.SetLevel", (level) => {
				QualitySettings.SetQualityLevel(level);
				string result = $"QualitySettings.GetQualityLevel {QualitySettings.GetQualityLevel()}";
				return result;
			}, "Set the quality level.");
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
			}, "Property for accessing device location (handheld devices only).");

			BindCommand<bool>("Input.gyro", (value) => {
				Input.gyro.enabled = value;
				string result = $"Input.gyro.enabled {Input.gyro.enabled}";
				return result;
			}, "Use this to return the gyroscope details of your device. Ensure first that your device has a gyroscope. Use Input.gyro.enabled to check this.");

			BindCommand<bool>("Input.compass", (value) => {
				Input.compass.enabled = value;
				string result = $"Input.compass.enabled {Input.compass.enabled}";
				return result;
			}, "Property for accessing compass (handheld devices only).");
		}
		#endregion

		#region binds
		public static TerminalCommand BindCommand(string command, Func<string> callCommand, string help = null)
		{
			TerminalCommand terminalCommand = null;
			if (!s_terminalCommands.TryGetValue(command, out terminalCommand))
			{
				terminalCommand = new FuncTerminalCommand(callCommand);
				AddTerminalCommand(command, terminalCommand, help);
			}
			return terminalCommand;
		}
		public static TerminalCommand BindCommand<TParam1>(string command,Func<TParam1, string> callCommand, string help = null)
		{
			TerminalCommand terminalCommand = null;
			if (!s_terminalCommands.TryGetValue(command, out terminalCommand))
			{
				terminalCommand = new FuncTerminalCommand<TParam1>(callCommand);
				AddTerminalCommand(command, terminalCommand, help);
			}
			return terminalCommand;
		}
		public static TerminalCommand BindCommand<TParam1, TParam2>(string command, Func<TParam1, TParam2, string> callCommand, string help = null)
		{
			TerminalCommand terminalCommand = null;
			if (!s_terminalCommands.TryGetValue(command, out terminalCommand))
			{
				terminalCommand = new FuncTerminalCommand<TParam1, TParam2>(callCommand);
				AddTerminalCommand(command, terminalCommand, help);
			}
			return terminalCommand;
		}
		public static TerminalCommand BindCommand<TParam1, TParam2, TParam3>(string command, Func<TParam1, TParam2, TParam3, string> callCommand, string help = null)
		{
			TerminalCommand terminalCommand = null;
			if (!s_terminalCommands.TryGetValue(command, out terminalCommand))
			{
				terminalCommand = new FuncTerminalCommand<TParam1, TParam2, TParam3>(callCommand);
				AddTerminalCommand(command, terminalCommand, help);
			}
			return terminalCommand;
		}

		public static void BindCommandAlias(string originName, string newName)
		{
			if (s_terminalCommands.ContainsKey(originName) && !s_aliasCommands.ContainsKey(newName))
			{
				s_aliasCommands.Add(newName, originName);
				Dictionary<string, int> checkAliasLength = new Dictionary<string, int>();
				foreach (var item in s_aliasCommands)
				{
					if (!checkAliasLength.ContainsKey(item.Value))
					{
						int commandLength = item.Value.Length;
						checkAliasLength.Add(item.Value, commandLength);
					}
					checkAliasLength[item.Value] += item.Key.Length + 1;
				}

				foreach (var item in checkAliasLength.Values)
				{
					s_maxCommandLength = Mathf.Max(s_maxCommandLength, item);
				}
			}
		}

		public static void BindOnlyHelpCommand(string commandName, string helpDesc)
		{
			if (string.IsNullOrEmpty(commandName) || string.IsNullOrEmpty(helpDesc))
			{
				return;
			}

			if (!s_onlyHelpCommands.ContainsKey(commandName))
			{
				s_onlyHelpCommands.Add(commandName, helpDesc);
			}
		}

		private static void AddTerminalCommand(string command, TerminalCommand terminalCommand, string help)
		{
			if (string.IsNullOrEmpty(command))
			{
				return;
			}

			s_maxCommandLength = Mathf.Max(s_maxCommandLength, command.Length);
			s_maxAargumentsLength = Mathf.Max(s_maxAargumentsLength, terminalCommand.Aarguments.Length);

			terminalCommand.Help = help;
			s_terminalCommands.Add(command, terminalCommand);
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
		public string Aarguments { get; protected set; }
		public string Help { get; set; }
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
			Aarguments = $" ";
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
			Aarguments = $" {typeof(TParam1).Name}";
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
			Aarguments = $" {typeof(TParam1).Name} {typeof(TParam2).Name}";
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
			Aarguments = $" {typeof(TParam1).Name} {typeof(TParam2).Name} {typeof(TParam3).Name}";
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