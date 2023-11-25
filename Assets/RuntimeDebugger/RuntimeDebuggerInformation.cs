using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuntimeDebugger
{
	public class RuntimeDebuggerInformation : RuntimeDebuggerBase
	{
		bool m_updateInformation;
		public RuntimeDebuggerInformation()
		{

		}

		public override void OnMessage(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}

			if ("show" == message)
			{
				//收到消息，直接回包
				var information = BuildInformation();
				Send(information);
				m_updateInformation = true;
			}
			else if("hide" == message) 
			{
				m_updateInformation = false;
			}
		}
		public override void OnUpdate()
		{
			if (m_updateInformation)
			{
				var information = new Dictionary<string, Dictionary<string, string>>();
				information.Add("Time", BuildTimeInformation());
				information.Add("Input Summary", BuildSummaryInputInformation());
				information.Add("Input Touch", BuildTouchInputInformation());
				information.Add("Input Location", BuildLocationInputInformation());
				information.Add("Input Acceleration", BuildAccelerationInputInformation());
				information.Add("Input Gyroscope", BuildGyroscopeInputInformation());
				information.Add("Input Compass", BuildCompassInputInformation());
				Send(information);
			}
		}

		private Dictionary<string, Dictionary<string, string>> BuildInformation()
		{
			var information = new Dictionary<string, Dictionary<string, string>>();
			information.Add("System", BuildSystemInformation());
			information.Add("Environment", BuildEnvironmentInformation());
			information.Add("Screen", BuildScreenInformation());
			information.Add("Graphics", BuildGraphicsInformation());
			information.Add("Path", BuildPathInformation());
			information.Add("Quality", BuildQualityInformation());
			information.Add("Scene", BuildSceneInformation());
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

		private Dictionary<string, string> BuildEnvironmentInformation()
		{
			var dictionaryBuilder = new Dictionary<string, string>();
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

		private Dictionary<string, string> BuildSceneInformation()
		{
			var dictionaryBuilder = new Dictionary<string, string>();

			AppendDictionaryBuilder(dictionaryBuilder, "Scene Count", SceneManager.sceneCount.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Scene Count In Build Settings", SceneManager.sceneCountInBuildSettings.ToString());
			Scene activeScene = SceneManager.GetActiveScene();
			AppendDictionaryBuilder(dictionaryBuilder, "Active Scene Name", activeScene.name);
			AppendDictionaryBuilder(dictionaryBuilder, "Active Scene Path", activeScene.path);
			AppendDictionaryBuilder(dictionaryBuilder, "Active Scene Build Index", activeScene.buildIndex.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Active Scene Is Dirty", activeScene.isDirty.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Active Scene Is Loaded", activeScene.isLoaded.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Active Scene Is Valid", activeScene.IsValid().ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Active Scene Root Count", activeScene.rootCount.ToString());

			return dictionaryBuilder;
		}

		private Dictionary<string, string> BuildTimeInformation()
		{
			var dictionaryBuilder = new Dictionary<string, string>();

			AppendDictionaryBuilder(dictionaryBuilder, "Time Scale", string.Format("{0} [{1}]", Time.timeScale.ToString(), GetTimeScaleDescription(Time.timeScale)));
			AppendDictionaryBuilder(dictionaryBuilder, "Realtime Since Startup", Time.realtimeSinceStartup.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Time Since Level Load", Time.timeSinceLevelLoad.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Time", Time.time.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Fixed Time", Time.fixedTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Unscaled Time", Time.unscaledTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Fixed Unscaled Time", Time.fixedUnscaledTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Delta Time", Time.deltaTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Fixed Delta Time", Time.fixedDeltaTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Unscaled Delta Time", Time.unscaledDeltaTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Fixed Unscaled Delta Time", Time.fixedUnscaledDeltaTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Smooth Delta Time", Time.smoothDeltaTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Maximum Delta Time", Time.maximumDeltaTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Maximum Particle Delta Time", Time.maximumParticleDeltaTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Frame Count", Time.frameCount.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Rendered Frame Count", Time.renderedFrameCount.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Capture Framerate", Time.captureFramerate.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Capture Delta Time", Time.captureDeltaTime.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "In Fixed Time Step", Time.inFixedTimeStep.ToString());

			return dictionaryBuilder;
		}

		#region input
		private Dictionary<string, string> BuildSummaryInputInformation()
		{
			var dictionaryBuilder = new Dictionary<string, string>();

			AppendDictionaryBuilder(dictionaryBuilder, "Back Button Leaves App", Input.backButtonLeavesApp.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Device Orientation", Input.deviceOrientation.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Mouse Present", Input.mousePresent.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Mouse Position", Input.mousePosition.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Mouse Scroll Delta", Input.mouseScrollDelta.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Any Key", Input.anyKey.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Any Key Down", Input.anyKeyDown.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Input String", Input.inputString);
			AppendDictionaryBuilder(dictionaryBuilder, "IME Is Selected", Input.imeIsSelected.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "IME Composition Mode", Input.imeCompositionMode.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Compensate Sensors", Input.compensateSensors.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Composition Cursor Position", Input.compositionCursorPos.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Composition String", Input.compositionString);

			return dictionaryBuilder;
		}

		private Dictionary<string, string> BuildTouchInputInformation()
		{
			var dictionaryBuilder = new Dictionary<string, string>();

			AppendDictionaryBuilder(dictionaryBuilder, "Touch Supported", Input.touchSupported.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Touch Pressure Supported", Input.touchPressureSupported.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Stylus Touch Supported", Input.stylusTouchSupported.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Simulate Mouse With Touches", Input.simulateMouseWithTouches.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Multi Touch Enabled", Input.multiTouchEnabled.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Touch Count", Input.touchCount.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Touches", GetTouchesString(Input.touches));

			return dictionaryBuilder;
		}

		private Dictionary<string, string> BuildLocationInputInformation()
		{
			var dictionaryBuilder = new Dictionary<string, string>();
			AppendDictionaryBuilder(dictionaryBuilder, "*", "Need to manually enable Input.localtion.");
			AppendDictionaryBuilder(dictionaryBuilder, "Is Enabled By User", Input.location.isEnabledByUser.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Status", Input.location.status.ToString());
			if (Input.location.status == LocationServiceStatus.Running)
			{
				AppendDictionaryBuilder(dictionaryBuilder, "Horizontal Accuracy", Input.location.lastData.horizontalAccuracy.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Vertical Accuracy", Input.location.lastData.verticalAccuracy.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Longitude", Input.location.lastData.longitude.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Latitude", Input.location.lastData.latitude.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Altitude", Input.location.lastData.altitude.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Timestamp", Input.location.lastData.timestamp.ToString());
			}

			return dictionaryBuilder;
		}

		private Dictionary<string, string> BuildAccelerationInputInformation()
		{
			var dictionaryBuilder = new Dictionary<string, string>();
			AppendDictionaryBuilder(dictionaryBuilder, "Acceleration", Input.acceleration.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Acceleration Event Count", Input.accelerationEventCount.ToString());
			AppendDictionaryBuilder(dictionaryBuilder, "Acceleration Events", GetAccelerationEventsString(Input.accelerationEvents));

			return dictionaryBuilder;
		}

		private Dictionary<string, string> BuildGyroscopeInputInformation()
		{
			var dictionaryBuilder = new Dictionary<string, string>();
			AppendDictionaryBuilder(dictionaryBuilder, "*", "Need to manually enable Input.gyro.");
			AppendDictionaryBuilder(dictionaryBuilder, "Enabled", Input.gyro.enabled.ToString());
			if (Input.gyro.enabled)
			{
				AppendDictionaryBuilder(dictionaryBuilder, "Update Interval", Input.gyro.updateInterval.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Attitude", Input.gyro.attitude.eulerAngles.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Gravity", Input.gyro.gravity.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Rotation Rate", Input.gyro.rotationRate.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Rotation Rate Unbiased", Input.gyro.rotationRateUnbiased.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "User Acceleration", Input.gyro.userAcceleration.ToString());
			}
			return dictionaryBuilder;
		}

		private Dictionary<string, string> BuildCompassInputInformation()
		{
			var dictionaryBuilder = new Dictionary<string, string>();
			AppendDictionaryBuilder(dictionaryBuilder, "*", "Need to manually enable Input.compass.");
			AppendDictionaryBuilder(dictionaryBuilder, "Enabled", Input.compass.enabled.ToString());
			if (Input.compass.enabled)
			{
				AppendDictionaryBuilder(dictionaryBuilder, "Heading Accuracy", Input.compass.headingAccuracy.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Magnetic Heading", Input.compass.magneticHeading.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Raw Vector", Input.compass.rawVector.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "Timestamp", Input.compass.timestamp.ToString());
				AppendDictionaryBuilder(dictionaryBuilder, "True Heading", Input.compass.trueHeading.ToString());
			}
			return dictionaryBuilder;
		}

		private void AppendDictionaryBuilder(Dictionary<string, string> dictionaryBuilder, string key, object value)
		{
			dictionaryBuilder.Add(key, value.ToString());
		}

		private string GetTimeScaleDescription(float timeScale)
		{
			if (timeScale <= 0f)
			{
				return "Pause";
			}

			if (timeScale < 1f)
			{
				return "Slower";
			}

			if (timeScale > 1f)
			{
				return "Faster";
			}

			return "Normal";
		}

		private string GetTouchesString(Touch[] touches)
		{
			string[] touchStrings = new string[touches.Length];
			for (int i = 0; i < touches.Length; i++)
			{
				touchStrings[i] = GetTouchString(touches[i]);
			}

			return string.Join("; ", touchStrings);
		}
		private string GetTouchString(Touch touch)
		{
			return string.Format("{0}, {1}, {2}, {3}, {4}", touch.position.ToString(), touch.deltaPosition.ToString(), touch.rawPosition.ToString(), touch.pressure.ToString(), touch.phase.ToString());
		}
		private string GetAccelerationEventsString(AccelerationEvent[] accelerationEvents)
		{
			string[] accelerationEventStrings = new string[accelerationEvents.Length];
			for (int i = 0; i < accelerationEvents.Length; i++)
			{
				accelerationEventStrings[i] = GetAccelerationEventString(accelerationEvents[i]);
			}

			return string.Join("; ", accelerationEventStrings);
		}
		private string GetAccelerationEventString(AccelerationEvent accelerationEvent)
		{
			return string.Format("{0}, {1}", accelerationEvent.acceleration.ToString(), accelerationEvent.deltaTime.ToString());
		}
		#endregion

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

	public class InformationOperation
	{
		
	}
}