using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Profiling;

namespace RuntimeDebugger
{
	public class RuntimeDebuggerProfiler : RuntimeDebuggerBase
	{
		FPSProfiler m_fpsProfiler;
		MemoryProfiler m_memoryProfiler;
		private bool m_show;
		private float m_deltaTime;
		private const float FixedDeltaTime = 0.0166667f;

		Dictionary<string, Type> m_memorySampleTypes;

		public RuntimeDebuggerProfiler()
		{
			var memorySampleTypes = new Type[] {typeof(UnityEngine.Object),typeof(Texture), typeof(Mesh), typeof(Material), typeof(Shader), typeof(AnimationClip), typeof(AudioClip), typeof(Font), typeof(TextAsset), typeof(ScriptableObject) };
			m_memorySampleTypes = new Dictionary<string, Type>();
            foreach (var item in memorySampleTypes)
            {
				m_memorySampleTypes.Add(item.Name, item);
			}

            m_fpsProfiler = new FPSProfiler();
			m_memoryProfiler = new MemoryProfiler();
		}

		public override void OnMessage(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}

			if ("show".Equals(message))
			{
				if (m_fpsProfiler != null)
				{
					m_fpsProfiler.Reset();
				}
				m_deltaTime = 0;
				m_show = true;
			}
			else if ("hide".Equals(message))
			{
				m_show = false;
			}
			else
			{
				var req = JsonConvert.DeserializeObject<ReqProfilerMessage>(message);
				RspProfilerMessage rsp = new RspProfilerMessage();
				rsp.CmdId = req.CmdId;
				if (req.CmdId == ReqProfilerCmd.GetMemorySampleTypes)
				{
					rsp.SmapleTypes = new List<string>();
					//rsp.SmapleTypes.Add("Summary");
					if (m_memorySampleTypes != null)
					{
						foreach (var item in m_memorySampleTypes.Keys)
						{
							rsp.SmapleTypes.Add(item);
						}
					}
					else
					{
						return;
					}
				}
				else
				{
					rsp.SmapleType = req.SmapleType;
					MemorySampleProfiler memorySample = new MemorySampleProfiler();
					memorySample.DateTime = DateTime.Now.ToString();
					memorySample.FrameCount = Time.frameCount;
					memorySample.Realtime = Time.realtimeSinceStartupAsDouble;
					memorySample.Nodes = new List<MemorySampleNode>();
					rsp.Sample = memorySample;
					if (m_memorySampleTypes != null)
					{
						if (m_memorySampleTypes.TryGetValue(req.SmapleType, out Type sampleType))
						{
							UnityEngine.Object[] samples = Resources.FindObjectsOfTypeAll(sampleType);
							if (samples != null)
							{
								foreach (var item in samples) 
								{
									MemorySampleNode node = new MemorySampleNode();
									node.Name = item.name;
									node.Type = item.GetType().Name;
									node.Size = Profiler.GetRuntimeMemorySizeLong(item);
									memorySample.Nodes.Add(node);
								}
							}
						}
					}
				}
				Send(rsp);
			}
		}

		public override void OnUpdate()
		{
			if (m_show)
			{
				if (m_fpsProfiler != null)
				{
					m_fpsProfiler.Update();
				}

				m_deltaTime += Time.deltaTime;
				if (m_deltaTime > FixedDeltaTime)
				{
					m_deltaTime = 0;
					if (m_memoryProfiler != null)
					{
						m_memoryProfiler.Update();
					}

					RuntimeProfilerMessage messgae = new RuntimeProfilerMessage();
					messgae.FPS = m_fpsProfiler;
					messgae.Memory = m_memoryProfiler;
					Send(messgae);
				}
			}
		}

		public override void OnReset()
		{
			m_show = false;
		}
	}

	public enum ReqProfilerCmd
	{
		GetMemorySampleTypes,
		TaskMemorySample,
	}

	public class ReqProfilerMessage
	{
		public ReqProfilerCmd CmdId { get; set; }
		public string SmapleType { get; set; }
	}

	public class RspProfilerMessage
	{
		public ReqProfilerCmd CmdId { get; set; }
		public List<string> SmapleTypes { get; set; }
		public string SmapleType { get; set; }
		public MemorySampleProfiler Sample { get; set; }
	}

	public class RuntimeProfilerMessage
	{
		public FPSProfiler FPS { get; set; }
		public MemoryProfiler Memory { get; set; }
	}

	public class FPSProfiler
	{
		private double m_lastAvgTime;
		private int m_avgFPSCount;
		private double m_lastFrameTime;

		private const double AVGCountTime = 1.0f;

		public int FrameCount { get; private set; }
		public double Realtime { get; private set; }
		public double FPS { get; private set; }
		public int FrameTime { get; private set; }
		public double AvgFPS { get; private set; }
		public double AvgFPSMin { get; private set; }
		public double AvgFPSMax { get; private set; }

		public FPSProfiler()
		{
			Reset();
		}

		public void Reset()
		{
			m_lastAvgTime = Time.realtimeSinceStartupAsDouble;
			m_lastFrameTime = Time.realtimeSinceStartupAsDouble;
			m_avgFPSCount = 0;

			AvgFPSMin = -1.0;
			AvgFPSMax = -1.0;

			FrameCount = 0;
			Realtime = 0;
			FPS = 0;
			FrameTime = 0;
			AvgFPS = 0;
		}

		public void Update()
		{
			FrameCount = Time.frameCount;
			Realtime = Time.realtimeSinceStartupAsDouble;
			double realtimeNow = Realtime;
			double frameTime = realtimeNow - m_lastFrameTime;
			if (frameTime > 0)
			{
				FrameTime = (int)(frameTime * 1000);
				FPS = 1.0f / frameTime;
				m_lastFrameTime = realtimeNow;
			}

			m_avgFPSCount++;
			double avgTime = realtimeNow - m_lastAvgTime;
			AvgFPSMin = AvgFPSMin < 0 ? FPS : Math.Min(AvgFPSMin, FPS);
			AvgFPSMax = AvgFPSMax < 0 ? FPS : Math.Max(AvgFPSMax, FPS);
			if (avgTime > AVGCountTime)
			{
				AvgFPS = m_avgFPSCount / avgTime;

				m_avgFPSCount = 0;
				AvgFPSMin = -1.0;
				AvgFPSMax = -1.0;
				m_lastAvgTime = realtimeNow;
			}
		}
	}

	public class MemoryProfiler
	{
		public int FrameCount { get; private set; }
		public double Realtime { get; private set; }
		public float MaxUsedMemory { get;private set; }
		public float MonoUsedSize { get; private set; }
		public float MonoHeapSize { get; private set; }
		public float UsedHeapSize { get; private set; }
		public float TotalAllocatedMemory { get; private set; }
		public float TotalReservedMemory { get; private set; }
		public float TotalUnusedReserved { get; private set; }
		public float AllocatedMemoryForGraphicsDriver { get; private set; }
		public float TempAllocatorSize { get; private set; }

		public void Update()
		{
			float toMBSize = 1024 * 1024;
			FrameCount = Time.frameCount;
			Realtime = Time.realtimeSinceStartupAsDouble;
			MaxUsedMemory = Profiler.maxUsedMemory / toMBSize;
			MonoUsedSize = Profiler.GetMonoUsedSizeLong() / toMBSize;
			MonoHeapSize = Profiler.GetMonoHeapSizeLong() / toMBSize;
			UsedHeapSize = Profiler.usedHeapSizeLong / toMBSize;
			TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / toMBSize;
			TotalReservedMemory = Profiler.GetTotalReservedMemoryLong() / toMBSize;
			TotalUnusedReserved = Profiler.GetTotalUnusedReservedMemoryLong() / toMBSize;
			AllocatedMemoryForGraphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver() / toMBSize;
			TempAllocatorSize = Profiler.GetTempAllocatorSize() / toMBSize;
		}
	}

	public class MemorySampleProfiler
	{
		public string DateTime { get; set; }
		public int FrameCount { get; set; }
		public double Realtime { get; set; }
		public List<MemorySampleNode> Nodes { get; set; }
	}

	public class MemorySampleNode
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public long Size { get; set; }
	}
}

