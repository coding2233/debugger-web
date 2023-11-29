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
		public double AVGFPS { get; private set; }
		public double AVGFPSMIN { get; private set; }
		public double AVGFPSMAX { get; private set; }

		public FPSProfiler()
		{
			m_lastAvgTime = Time.realtimeSinceStartupAsDouble;
			m_lastFrameTime = Time.realtimeSinceStartupAsDouble;
			m_avgFPSCount = 0;

			AVGFPSMIN = -1.0;
			AVGFPSMAX = -1.0;
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
			AVGFPSMIN = AVGFPSMIN < 0 ? FPS : Math.Min(AVGFPSMIN, FPS);
			AVGFPSMAX = AVGFPSMAX < 0 ? FPS : Math.Max(AVGFPSMAX, FPS);
			if (avgTime > AVGCountTime)
			{
				AVGFPS = m_avgFPSCount / avgTime;

				m_avgFPSCount = 0;
				AVGFPSMIN = -1.0;
				AVGFPSMAX = -1.0;
				m_lastAvgTime = realtimeNow;
			}
		}
	}

	public class MemoryProfiler
	{
		public int FrameCount { get; private set; }
		public double Realtime { get; private set; }
		public int MaxUsedMemory { get;private set; }
		public long MonoUsedSize { get; private set; }
		public long MonoHeapSize { get; private set; }
		public long UsedHeapSize { get; private set; }
		public long TotalAllocatedMemory { get; private set; }
		public long TotalReservedMemory { get; private set; }
		public long TotalUnusedReserved { get; private set; }
		public long AllocatedMemoryForGraphicsDriver { get; private set; }
		public uint TempAllocatorSize { get; private set; }

		public void Update()
		{
			FrameCount = Time.frameCount;
			Realtime = Time.realtimeSinceStartupAsDouble;
			MaxUsedMemory = Profiler.maxUsedMemory;
			MonoUsedSize = Profiler.GetMonoUsedSizeLong();
			MonoHeapSize = Profiler.GetMonoHeapSizeLong();
			UsedHeapSize = Profiler.usedHeapSizeLong;
			TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
			TotalReservedMemory = Profiler.GetTotalReservedMemoryLong();
			TotalUnusedReserved = Profiler.GetTotalUnusedReservedMemoryLong();
			AllocatedMemoryForGraphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver();
			TempAllocatorSize = Profiler.GetTempAllocatorSize();
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

