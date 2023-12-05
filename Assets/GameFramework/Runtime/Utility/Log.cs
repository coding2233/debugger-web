//using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanderer
{
    public enum LogLevel { LOG_TRACE, LOG_DEBUG, LOG_INFO, LOG_WARN, LOG_ERROR, LOG_FATAL };

    public interface ILogger:IDisposable
    {
        void Log(int level, string file, int line, string log);
    }

    public static class Log
    {
        public static ILogger Logger { get; set; }
        private static StringBuilder _logStringBuilder = new StringBuilder();
        private static void Full(string log, LogLevel logLevel)
        {
            StackFrame stackFrame = null;

            if (logLevel == LogLevel.LOG_TRACE)
            {
                stackFrame = GetTraceLog(ref log);
            }
            else if (logLevel == LogLevel.LOG_DEBUG)
            {
#if DEBUG
                stackFrame = GetTraceLog(ref log);

#else
                stackFrame = GetStackFrame();
#endif
            }
            else
            {
                stackFrame = GetStackFrame();

            }

            if (Logger != null)
            {
                Logger.Log((int)logLevel, stackFrame == null ? "NULL" : stackFrame.GetFileName(), stackFrame == null ? -1 : stackFrame.GetFileLineNumber(), log);
            }
        }

        public static void Trace(string log)
        {
            Full(log, LogLevel.LOG_TRACE);
        }
        public static void Trace(string format, params object[] args)
        {
            Trace(string.Format(format, args));
        }

        public static void Debug(string log)
        {
            Full(log, LogLevel.LOG_DEBUG);
        }

        public static void Debug(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }

        public static void Info(string log)
        {
            Full(log, LogLevel.LOG_INFO);
        }

        public static void Info(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        public static void Warn(string log)
        {
            Full(log, LogLevel.LOG_WARN);
        }

        public static void Warn(string format, params object[] args)
        {
            Warn(string.Format(format, args));
        }

        public static void Error(string log)
        {
            Full(log, LogLevel.LOG_ERROR);
        }

        public static void Error(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public static void Fatal(string log)
        {
            Full(log, LogLevel.LOG_FATAL);
        }

        public static void Fatal(string format, params object[] args)
        {
            Fatal(string.Format(format, args));
        }

        private static StackFrame GetTraceLog(ref string log)
        {
            StackTrace stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            if (frames != null)
            {
                if (_logStringBuilder == null)
                {
                    _logStringBuilder = new StringBuilder();
                }
                _logStringBuilder.Clear();
                _logStringBuilder.AppendLine(log);
                for (int i = 0; i < frames.Length; i++)
                {
                    var item = frames[i];
                    _logStringBuilder.Append($"{Path.GetFileName(item.GetFileName())}");
                    _logStringBuilder.Append("\t");
                    _logStringBuilder.Append($"{item.GetFileLineNumber()}");
                    _logStringBuilder.Append("\t");
                    _logStringBuilder.Append(item.GetMethod());

                    if (i < frames.Length - 1)
                    {
                        _logStringBuilder.Append("\n");
                    }
                }
                log = _logStringBuilder.ToString();
            }

            return GetStackFrame(stackTrace);
        }

        private static StackFrame GetStackFrame(StackTrace stackTrace = null)
        {
            if (stackTrace == null)
            {
                stackTrace = new StackTrace(true);
            }
            StackFrame stackFrame = null;
            var frames = stackTrace.GetFrames();
            if (frames != null)
            {
                foreach (var item in frames)
                {
                    if (item == null)
                        continue;
                    string fileName = item.GetFileName();
                    if (!string.IsNullOrEmpty(fileName) && !fileName.EndsWith("Log.cs"))
                    {
                        stackFrame = item;
                        break;
                    }
                }
            }
            return stackFrame;
        }

        public static void Dispose()
        {
            if (Logger != null)
            {
                Logger.Dispose();
            }
        }
    }

    public class DefaultLog : ILogger
    {
        private string m_logRootPath;
        private string m_logFilePath;
        private FileStream m_logFileStream;
        private StringBuilder m_logStringBuilder;

        public DefaultLog(string rootPath)
        {
            m_logStringBuilder = new StringBuilder();

            m_logRootPath = rootPath;
            if (!Directory.Exists(m_logRootPath))
            {
                Directory.CreateDirectory(m_logRootPath);
            }

            string logFileName = DateTime.Now.ToString("yyyy_MM_dd");
            m_logFilePath = Path.Combine(m_logRootPath, $"log_{logFileName}.txt");

            m_logFileStream = new FileStream(m_logFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public void Dispose()
        {
            if (m_logFileStream != null)
            {
                m_logFileStream.Close();
                m_logFileStream?.Dispose();
                m_logFileStream = null;
            }

            if (m_logStringBuilder != null)
            {
                m_logStringBuilder.Clear();
                m_logStringBuilder = null;
            }
        }

        public void Log(int level, string file, int line, string log)
        {
            if (string.IsNullOrEmpty(log))
                return;

            LogLevel logLevel = (LogLevel)level;
            switch (logLevel)
            {
                case LogLevel.LOG_TRACE:
                case LogLevel.LOG_DEBUG:
                case LogLevel.LOG_INFO:
                    UnityEngine.Debug.Log(log);
                    break;
                case LogLevel.LOG_WARN:
                    UnityEngine.Debug.LogWarning(log);
                    break;
                case LogLevel.LOG_ERROR:
                case LogLevel.LOG_FATAL:
                    UnityEngine.Debug.LogError(log);
                    break;
                default:
                    break;
            }

            if (m_logFileStream != null)
            {
                m_logStringBuilder.Clear();
                m_logStringBuilder.Append(DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss"));
                m_logStringBuilder.Append(" ");
                m_logStringBuilder.Append(Path.GetFileName(file));
                m_logStringBuilder.Append($"({line}):");
                m_logStringBuilder.Append(logLevel.ToString().Replace("LOG_", ""));
                m_logStringBuilder.Append(": ");
                m_logStringBuilder.Append(log);
                m_logStringBuilder.Append("\n");

                var logData = System.Text.Encoding.UTF8.GetBytes(m_logStringBuilder.ToString());
                m_logFileStream.Write(logData, 0, logData.Length);
            }
        }
    }

}
