using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RuntimeDebuggerInformation : RuntimeDebuggerBase
{
	public RuntimeDebuggerInformation()
	{
		
	}

	public override void OnMessage(string message)
	{
		Debug.Log($"OnMessage: {message}");
		//收到消息，直接回包
		var information = BuildInformation();
		string informationDump = JsonConvert.SerializeObject(information);
		Debug.Log(informationDump);
		Send(informationDump);
	}

	private Dictionary<string, Dictionary<string, string>> BuildInformation()
	{
		var information = new Dictionary<string, Dictionary<string,string>>();
		information.Add("System", BuildSystemInformation());
		information.Add("Environment", BuildEnvironmentInformation());
		information.Add("Screen", BuildScreenInformation());
		information.Add("Graphics", BuildGraphicsInformation());
		information.Add("Path", BuildPathInformation());
		information.Add("Quality", BuildQualityInformation());
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

	private Dictionary<string, string> BuildScreenInformation()
	{
		var dictionaryBuilder = new Dictionary<string, string>();
		AppendDictionaryBuilder(dictionaryBuilder, "Current Resolution", GetResolutionString(Screen.currentResolution));
		AppendDictionaryBuilder(dictionaryBuilder, "Screen Width", string.Format("{0} px / {1} in / {2} cm", Screen.width.ToString(), GetInchesFromPixels(Screen.width).ToString("F2"), GetCentimetersFromPixels(Screen.width).ToString("F2")));
		AppendDictionaryBuilder(dictionaryBuilder, "Screen Height", string.Format("{0} px / {1} in / {2} cm", Screen.height.ToString(), GetInchesFromPixels(Screen.height).ToString("F2"), GetCentimetersFromPixels(Screen.height).ToString("F2")));
		AppendDictionaryBuilder(dictionaryBuilder, "Screen DPI", Screen.dpi.ToString("F2"));
		AppendDictionaryBuilder(dictionaryBuilder, "Screen Orientation", Screen.orientation.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Is Full Screen", Screen.fullScreen.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Full Screen Mode", Screen.fullScreenMode.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Sleep Timeout", GetSleepTimeoutDescription(Screen.sleepTimeout));
		AppendDictionaryBuilder(dictionaryBuilder, "Brightness", Screen.brightness.ToString("F2"));
		AppendDictionaryBuilder(dictionaryBuilder, "Cursor Visible", Cursor.visible.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Cursor Lock State", Cursor.lockState.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Auto Landscape Left", Screen.autorotateToLandscapeLeft.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Auto Landscape Right", Screen.autorotateToLandscapeRight.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Auto Portrait", Screen.autorotateToPortrait.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Auto Portrait Upside Down", Screen.autorotateToPortraitUpsideDown.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Safe Area", Screen.safeArea.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Cutouts", GetCutoutsString(Screen.cutouts));
		AppendDictionaryBuilder(dictionaryBuilder, "Support Resolutions", GetResolutionsString(Screen.resolutions));
		return dictionaryBuilder;
	}
	private Dictionary<string, string> BuildGraphicsInformation()
	{
		var dictionaryBuilder = new Dictionary<string, string>();
		AppendDictionaryBuilder(dictionaryBuilder, "Device ID", SystemInfo.graphicsDeviceID.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Device Name", SystemInfo.graphicsDeviceName);
		AppendDictionaryBuilder(dictionaryBuilder, "Device Vendor ID", SystemInfo.graphicsDeviceVendorID.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Device Vendor", SystemInfo.graphicsDeviceVendor);
		AppendDictionaryBuilder(dictionaryBuilder, "Device Type", SystemInfo.graphicsDeviceType.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Device Version", SystemInfo.graphicsDeviceVersion);
		AppendDictionaryBuilder(dictionaryBuilder, "Memory Size", string.Format("{0} MB", SystemInfo.graphicsMemorySize.ToString()));
		AppendDictionaryBuilder(dictionaryBuilder, "Multi Threaded", SystemInfo.graphicsMultiThreaded.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Shader Level", SystemInfo.graphicsShaderLevel.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Global Maximum LOD", Shader.globalMaximumLOD.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Global Render Pipeline", Shader.globalRenderPipeline);
		AppendDictionaryBuilder(dictionaryBuilder, "Active Tier", Graphics.activeTier.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Active Color Gamut", Graphics.activeColorGamut.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Preserve Frame Buffer Alpha", Graphics.preserveFramebufferAlpha.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "NPOT Support", SystemInfo.npotSupport.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Max Texture Size", SystemInfo.maxTextureSize.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supported Render Target Count", SystemInfo.supportedRenderTargetCount.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Copy Texture Support", SystemInfo.copyTextureSupport.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Uses Reversed ZBuffer", SystemInfo.usesReversedZBuffer.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Max Cubemap Size", SystemInfo.maxCubemapSize.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Graphics UV Starts At Top", SystemInfo.graphicsUVStartsAtTop.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Min Constant Buffer Offset Alignment", SystemInfo.constantBufferOffsetAlignment.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Has Hidden Surface Removal On GPU", SystemInfo.hasHiddenSurfaceRemovalOnGPU.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Has Dynamic Uniform Array Indexing In Fragment Shaders", SystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Has Mip Max Level", SystemInfo.hasMipMaxLevel.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Sparse Textures", SystemInfo.supportsSparseTextures.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports 3D Textures", SystemInfo.supports3DTextures.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Shadows", SystemInfo.supportsShadows.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Raw Shadow Depth Sampling", SystemInfo.supportsRawShadowDepthSampling.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Compute Shader", SystemInfo.supportsComputeShaders.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Instancing", SystemInfo.supportsInstancing.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports 2D Array Textures", SystemInfo.supports2DArrayTextures.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Motion Vectors", SystemInfo.supportsMotionVectors.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Cubemap Array Textures", SystemInfo.supportsCubemapArrayTextures.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports 3D Render Textures", SystemInfo.supports3DRenderTextures.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Texture Wrap Mirror Once", SystemInfo.supportsTextureWrapMirrorOnce.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Graphics Fence", SystemInfo.supportsGraphicsFence.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Async Compute", SystemInfo.supportsAsyncCompute.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Multisampled Textures", SystemInfo.supportsMultisampledTextures.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Async GPU Readback", SystemInfo.supportsAsyncGPUReadback.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports 32bits Index Buffer", SystemInfo.supports32bitsIndexBuffer.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Hardware Quad Topology", SystemInfo.supportsHardwareQuadTopology.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Mip Streaming", SystemInfo.supportsMipStreaming.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Multisample Auto Resolve", SystemInfo.supportsMultisampleAutoResolve.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Separated Render Targets Blend", SystemInfo.supportsSeparatedRenderTargetsBlend.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Supports Set Constant Buffer", SystemInfo.supportsSetConstantBuffer.ToString());
		return dictionaryBuilder;
	}

	private Dictionary<string, string> BuildPathInformation()
	{
		var dictionaryBuilder = new Dictionary<string, string>();
		AppendDictionaryBuilder(dictionaryBuilder, "Data Path", Application.dataPath);
		AppendDictionaryBuilder(dictionaryBuilder, "Persistent Data Path", Application.persistentDataPath);
		AppendDictionaryBuilder(dictionaryBuilder, "Streaming Assets Path", Application.streamingAssetsPath);
		AppendDictionaryBuilder(dictionaryBuilder, "Temporary Cache Path", Application.temporaryCachePath);
		AppendDictionaryBuilder(dictionaryBuilder, "Console Log Path", Application.consoleLogPath);
		return dictionaryBuilder;
	}

	private Dictionary<string, string> BuildQualityInformation()
	{
		var dictionaryBuilder = new Dictionary<string, string>();
		//AppendDictionaryBuilder(dictionaryBuilder, "Quality Level", "------");
		AppendDictionaryBuilder(dictionaryBuilder, "Current Quality Level", QualitySettings.names[QualitySettings.GetQualityLevel()]);
		//AppendDictionaryBuilder(dictionaryBuilder, "Rendering Information", "------");
		AppendDictionaryBuilder(dictionaryBuilder, "Active Color Space", QualitySettings.activeColorSpace.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Desired Color Space", QualitySettings.desiredColorSpace.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Max Queued Frames", QualitySettings.maxQueuedFrames.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Pixel Light Count", QualitySettings.pixelLightCount.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Master Texture Limit", QualitySettings.masterTextureLimit.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Anisotropic Filtering", QualitySettings.anisotropicFiltering.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Anti Aliasing", QualitySettings.antiAliasing.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Soft Particles", QualitySettings.softParticles.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Soft Vegetation", QualitySettings.softVegetation.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Realtime Reflection Probes", QualitySettings.realtimeReflectionProbes.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Billboards Face Camera Position", QualitySettings.billboardsFaceCameraPosition.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Resolution Scaling Fixed DPI Factor", QualitySettings.resolutionScalingFixedDPIFactor.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Texture Streaming Enabled", QualitySettings.streamingMipmapsActive.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Texture Streaming Add All Cameras", QualitySettings.streamingMipmapsAddAllCameras.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Texture Streaming Memory Budget", QualitySettings.streamingMipmapsMemoryBudget.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Texture Streaming Renderers Per Frame", QualitySettings.streamingMipmapsRenderersPerFrame.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Texture Streaming Max Level Reduction", QualitySettings.streamingMipmapsMaxLevelReduction.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Texture Streaming Max File IO Requests", QualitySettings.streamingMipmapsMaxFileIORequests.ToString());
		//AppendDictionaryBuilder(dictionaryBuilder, "Shadows Information", "------");
		AppendDictionaryBuilder(dictionaryBuilder, "Shadowmask Mode", QualitySettings.shadowmaskMode.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Shadow Quality", QualitySettings.shadows.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Shadow Resolution", QualitySettings.shadowResolution.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Shadow Projection", QualitySettings.shadowProjection.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Shadow Distance", QualitySettings.shadowDistance.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Shadow Near Plane Offset", QualitySettings.shadowNearPlaneOffset.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Shadow Cascades", QualitySettings.shadowCascades.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Shadow Cascade 2 Split", QualitySettings.shadowCascade2Split.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Shadow Cascade 4 Split", QualitySettings.shadowCascade4Split.ToString());
		//AppendDictionaryBuilder(dictionaryBuilder, "Other Information", "------");
		AppendDictionaryBuilder(dictionaryBuilder, "Skin Weights", QualitySettings.skinWeights.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "VSync Count", QualitySettings.vSyncCount.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "LOD Bias", QualitySettings.lodBias.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Maximum LOD Level", QualitySettings.maximumLODLevel.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Particle Raycast Budget", QualitySettings.particleRaycastBudget.ToString());
		AppendDictionaryBuilder(dictionaryBuilder, "Async Upload Time Slice", string.Format("{0} ms", QualitySettings.asyncUploadTimeSlice.ToString()));
		AppendDictionaryBuilder(dictionaryBuilder, "Async Upload Buffer Size", string.Format("{0} MB", QualitySettings.asyncUploadBufferSize.ToString()));
		AppendDictionaryBuilder(dictionaryBuilder, "Async Upload Persistent Buffer", QualitySettings.asyncUploadPersistentBuffer.ToString());
		return dictionaryBuilder;
	}

	private void AppendDictionaryBuilder(Dictionary<string,string> dictionaryBuilder, string key, object value)
	{
		dictionaryBuilder.Add(key,value.ToString());
	}

	#region screen
	private string GetSleepTimeoutDescription(int sleepTimeout)
	{
		if (sleepTimeout == SleepTimeout.NeverSleep)
		{
			return "Never Sleep";
		}

		if (sleepTimeout == SleepTimeout.SystemSetting)
		{
			return "System Setting";
		}

		return sleepTimeout.ToString();
	}

	private string GetResolutionString(Resolution resolution)
	{
		return string.Format("{0} x {1} @ {2}Hz", resolution.width.ToString(), resolution.height.ToString(), resolution.refreshRate.ToString());
	}

	private string GetCutoutsString(Rect[] cutouts)
	{
		string[] cutoutStrings = new string[cutouts.Length];
		for (int i = 0; i < cutouts.Length; i++)
		{
			cutoutStrings[i] = cutouts[i].ToString();
		}

		return string.Join("; ", cutoutStrings);
	}

	private string GetResolutionsString(Resolution[] resolutions)
	{
		string[] resolutionStrings = new string[resolutions.Length];
		for (int i = 0; i < resolutions.Length; i++)
		{
			resolutionStrings[i] = GetResolutionString(resolutions[i]);
		}

		return string.Join("; ", resolutionStrings);
	}

	/// <summary>
	/// 将像素转换为英寸。
	/// </summary>
	/// <param name="pixels">像素。</param>
	/// <returns>英寸。</returns>
	public float GetInchesFromPixels(float pixels)
	{
		return pixels / Screen.dpi;
	}

	/// <summary>
	/// 将像素转换为厘米。
	/// </summary>
	/// <param name="pixels">像素。</param>
	/// <returns>厘米。</returns>
	public static float GetCentimetersFromPixels(float pixels)
	{
		float inchesToCentimeters = 2.54f;  // 1 inch = 2.54 cm
		return inchesToCentimeters * pixels / Screen.dpi;
	}
	#endregion

}
