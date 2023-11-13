using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RuntimeDebuggerInformation : RuntimeDebuggerBase
{
	public override void OnOpen(RuntimeDebugger runtimeDebugger, IntPtr channel)
	{
		base.OnOpen(runtimeDebugger, channel);

		var information = BuildInformation();
		string informationDump = JsonConvert.SerializeObject(information);
		//Debug.Log(informationDump);
		Send(informationDump);
	}

	private Dictionary<string, string> BuildInformation()
	{
		var information = new Dictionary<string, string>();
		var stringBuilder = new StringBuilder();
		information.Add("System Information", BuildSystemInformation(stringBuilder));
		information.Add("Environment Information", BuildEnvironmentInformation(stringBuilder));
		return information;
	}

	private string BuildSystemInformation(StringBuilder stringBuilder)
	{
		stringBuilder.Clear();
		AppendStringBuilder(stringBuilder, "Device Unique ID", SystemInfo.deviceUniqueIdentifier);
		AppendStringBuilder(stringBuilder, "Device Name", SystemInfo.deviceName);
		AppendStringBuilder(stringBuilder, "Device Type", SystemInfo.deviceType);
		AppendStringBuilder(stringBuilder, "Device Model", SystemInfo.deviceModel);
		AppendStringBuilder(stringBuilder, "Processor Type", SystemInfo.processorType);
		AppendStringBuilder(stringBuilder, "Processor Count", SystemInfo.processorCount);
		AppendStringBuilder(stringBuilder, "Processor Frequency", string.Format("{0} MHz", SystemInfo.processorFrequency.ToString()));
		AppendStringBuilder(stringBuilder, "System Memory Size", string.Format("{0} MB", SystemInfo.systemMemorySize.ToString()));
		AppendStringBuilder(stringBuilder, "Operating System Family", SystemInfo.operatingSystemFamily);
		AppendStringBuilder(stringBuilder, "Operating System", SystemInfo.operatingSystem);
		AppendStringBuilder(stringBuilder, "Battery Status", SystemInfo.batteryStatus);
		AppendStringBuilder(stringBuilder, "Battery Level", SystemInfo.batteryLevel.ToString("f12"));
		AppendStringBuilder(stringBuilder, "Supports Audio", SystemInfo.supportsAudio);
		AppendStringBuilder(stringBuilder, "Supports Location Service", SystemInfo.supportsLocationService);
		AppendStringBuilder(stringBuilder, "Supports Accelerometer", SystemInfo.supportsAccelerometer);
		AppendStringBuilder(stringBuilder, "Supports Gyroscope", SystemInfo.supportsGyroscope);
		AppendStringBuilder(stringBuilder, "Supports Vibration", SystemInfo.supportsVibration);
		AppendStringBuilder(stringBuilder, "Genuine", Application.genuine);
		AppendStringBuilder(stringBuilder, "Genuine Check Available", Application.genuineCheckAvailable);

		return stringBuilder.ToString();
	}

	private string BuildEnvironmentInformation(StringBuilder stringBuilder)
	{
		stringBuilder.Clear();
		AppendStringBuilder(stringBuilder, "Product Name", Application.productName);
		AppendStringBuilder(stringBuilder, "Company Name", Application.companyName);
		AppendStringBuilder(stringBuilder, "Game Identifier", Application.identifier);
		AppendStringBuilder(stringBuilder, "Application Version", Application.version);
		AppendStringBuilder(stringBuilder, "Unity Version", Application.unityVersion);
		AppendStringBuilder(stringBuilder, "Platform", Application.platform);
		AppendStringBuilder(stringBuilder, "System Language", Application.systemLanguage);
		AppendStringBuilder(stringBuilder, "Cloud Project Id", Application.cloudProjectId);
		AppendStringBuilder(stringBuilder, "Build Guid", Application.buildGUID);
		AppendStringBuilder(stringBuilder, "Target Frame Rate", Application.targetFrameRate);
		AppendStringBuilder(stringBuilder, "Internet Reachability", Application.internetReachability);
		AppendStringBuilder(stringBuilder, "Background Loading Priority", Application.backgroundLoadingPriority);
		AppendStringBuilder(stringBuilder, "Is Playing", Application.isPlaying);
		AppendStringBuilder(stringBuilder, "Run In Background", Application.runInBackground);
		AppendStringBuilder(stringBuilder, "Install Name", Application.installerName);
		AppendStringBuilder(stringBuilder, "Install Mode", Application.installMode);
		AppendStringBuilder(stringBuilder, "Sandbox Type", Application.sandboxType);
		AppendStringBuilder(stringBuilder, "Is Mobile Platform", Application.isMobilePlatform);
		AppendStringBuilder(stringBuilder, "Is Console Platform", Application.isConsolePlatform);
		AppendStringBuilder(stringBuilder, "Is Editor", Application.isEditor);
		AppendStringBuilder(stringBuilder, "Is Focused", Application.isFocused);
		AppendStringBuilder(stringBuilder, "Is Batch Mode", Application.isBatchMode);

		return stringBuilder.ToString();
	}

	private void AppendStringBuilder(StringBuilder stringBuilder,string key,object value)
	{
		stringBuilder.Append(key);
		stringBuilder.Append(":");
		stringBuilder.AppendLine(value.ToString());
	}
}
