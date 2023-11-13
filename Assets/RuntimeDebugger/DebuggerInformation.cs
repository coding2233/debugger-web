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
		Debug.Log(informationDump);
		Send(informationDump);
	}

	private Dictionary<string, Dictionary<string, string>> BuildInformation()
	{
		var information = new Dictionary<string, Dictionary<string,string>>();
		information.Add("System Information", BuildSystemInformation());
		information.Add("Environment Information", BuildEnvironmentInformation());
		return information;
	}

	private Dictionary<string, string> BuildSystemInformation()
	{
		var dictionaryBuilder = new Dictionary<string, string>();
		AppendDictionaryBuilder(dictionaryBuilder, "Device Unique ID", SystemInfo.deviceUniqueIdentifier);
		AppendDictionaryBuilder(dictionaryBuilder, "Device Name", SystemInfo.deviceName);
		AppendDictionaryBuilder(dictionaryBuilder, "Device Type", SystemInfo.deviceType);
		AppendDictionaryBuilder(dictionaryBuilder, "Device Model", SystemInfo.deviceModel);
		AppendDictionaryBuilder(dictionaryBuilder, "Processor Type", SystemInfo.processorType);
		AppendDictionaryBuilder(dictionaryBuilder, "Processor Count", SystemInfo.processorCount);
		AppendDictionaryBuilder(dictionaryBuilder, "Processor Frequency", string.Format("{0} MHz", SystemInfo.processorFrequency.ToString()));
		AppendDictionaryBuilder(dictionaryBuilder, "System Memory Size", string.Format("{0} MB", SystemInfo.systemMemorySize.ToString()));
		AppendDictionaryBuilder(dictionaryBuilder, "Operating System Family", SystemInfo.operatingSystemFamily);
		AppendDictionaryBuilder(dictionaryBuilder, "Operating System", SystemInfo.operatingSystem);
		AppendDictionaryBuilder(dictionaryBuilder, "Battery Status", SystemInfo.batteryStatus);
		AppendDictionaryBuilder(dictionaryBuilder, "Battery Level", SystemInfo.batteryLevel.ToString("f12"));
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Audio", SystemInfo.supportsAudio);
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Location Service", SystemInfo.supportsLocationService);
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Accelerometer", SystemInfo.supportsAccelerometer);
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Gyroscope", SystemInfo.supportsGyroscope);
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Vibration", SystemInfo.supportsVibration);
		AppendDictionaryBuilder(dictionaryBuilder, "Genuine", Application.genuine);
		AppendDictionaryBuilder(dictionaryBuilder, "Genuine Check Available", Application.genuineCheckAvailable);

		return dictionaryBuilder;
	}

	private Dictionary<string,string> BuildEnvironmentInformation()
	{
		var dictionaryBuilder = new Dictionary<string,string>();
		AppendDictionaryBuilder(dictionaryBuilder, "Product Name", Application.productName);
		AppendDictionaryBuilder(dictionaryBuilder, "Company Name", Application.companyName);
		AppendDictionaryBuilder(dictionaryBuilder, "Game Identifier", Application.identifier);
		AppendDictionaryBuilder(dictionaryBuilder, "Application Version", Application.version);
		AppendDictionaryBuilder(dictionaryBuilder, "Unity Version", Application.unityVersion);
		AppendDictionaryBuilder(dictionaryBuilder, "Platform", Application.platform);
		AppendDictionaryBuilder(dictionaryBuilder, "System Language", Application.systemLanguage);
		AppendDictionaryBuilder(dictionaryBuilder, "Cloud Project Id", Application.cloudProjectId);
		AppendDictionaryBuilder(dictionaryBuilder, "Build Guid", Application.buildGUID);
		AppendDictionaryBuilder(dictionaryBuilder, "Target Frame Rate", Application.targetFrameRate);
		AppendDictionaryBuilder(dictionaryBuilder, "Internet Reachability", Application.internetReachability);
		AppendDictionaryBuilder(dictionaryBuilder, "Background Loading Priority", Application.backgroundLoadingPriority);
		AppendDictionaryBuilder(dictionaryBuilder, "Is Playing", Application.isPlaying);
		AppendDictionaryBuilder(dictionaryBuilder, "Run In Background", Application.runInBackground);
		AppendDictionaryBuilder(dictionaryBuilder, "Install Name", Application.installerName);
		AppendDictionaryBuilder(dictionaryBuilder, "Install Mode", Application.installMode);
		AppendDictionaryBuilder(dictionaryBuilder, "Sandbox Type", Application.sandboxType);
		AppendDictionaryBuilder(dictionaryBuilder, "Is Mobile Platform", Application.isMobilePlatform);
		AppendDictionaryBuilder(dictionaryBuilder, "Is Console Platform", Application.isConsolePlatform);
		AppendDictionaryBuilder(dictionaryBuilder, "Is Editor", Application.isEditor);
		AppendDictionaryBuilder(dictionaryBuilder, "Is Focused", Application.isFocused);
		AppendDictionaryBuilder(dictionaryBuilder, "Is Batch Mode", Application.isBatchMode);

		return dictionaryBuilder;
	}

	private void AppendDictionaryBuilder(Dictionary<string,string> dictionaryBuilder, string key, object value)
	{
		dictionaryBuilder.Add(key,value.ToString());
	}

	//private void AppenddictionaryBuilder(dictionaryBuilder dictionaryBuilder,string key,object value)
	//{
	//	dictionaryBuilder.Append(key);
	//	dictionaryBuilder.Append(":");
	//	dictionaryBuilder.Append(value.ToString());
	//	dictionaryBuilder.Append("\n");
	//}
}
