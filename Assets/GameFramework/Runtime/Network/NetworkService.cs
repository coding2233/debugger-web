//using strange.extensions.context.api;
//using strange.extensions.context.impl;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

//namespace Wanderer
//{
//    public interface INetwork
//    {
//        void AddListener();
//    }


//    public class NetworkService : MonoService,  IMessageDispatcher, INetwork
//    {
//        //[Inject(ContextKeys.CONTEXT_VIEW)]
//        //public GameObject contextView { get; set; }

//        KitNetwork m_kitNetwork;
//        // Start is called before the first frame update

//        public NetworkService()
//        {
//            try
//            {
//                //消息
//                m_kitNetwork = new KitNetwork(this);
//                //m_kitNetwork.SetLog(null, OnLogCallback);
//                m_kitNetwork.SetLog(Path.Combine(Application.persistentDataPath, "logs"), OnLogCallback);
//                //.ListenHttp(62233, Application.persistentDataPath);
//                Log.Info("NetworkService");
//            }
//            catch (Exception e)
//            {
//                m_kitNetwork.ShutDown();
//                m_kitNetwork = null;
//                Debug.Log(e);
//            }
//        }

//        private void OnLogCallback(int level, string file, int line, string log)
//        {
//            LogLevel logLevel = (LogLevel)level;
//            log = $"{logLevel.ToString().Replace("LOG_", "")} {log}";
//            switch (logLevel)
//            {
//                case LogLevel.LOG_TRACE:
//                case LogLevel.LOG_DEBUG:
//                case LogLevel.LOG_INFO:
//                    Debug.Log(log);
//                    break;
//                case LogLevel.LOG_WARN:
//                    Debug.LogWarning(log);
//                    break;
//                case LogLevel.LOG_ERROR:
//                case LogLevel.LOG_FATAL:
//                    Debug.LogError(log);
//                    break;
//                default:
//                    break;
//            }
//        }

//        public override void OnUpdate()
//        {
//            if (m_kitNetwork != null)
//            {
//                m_kitNetwork.Run(RunMode.UV_RUN_NOWAIT);
//            }
//        }

//        public override void OnMonoDestroy()
//        {
//            if (m_kitNetwork != null)
//            {
//                m_kitNetwork.ShutDown();
//            }
//            m_kitNetwork = null;

//            base.OnMonoDestroy();
//        }

//        public void OnInitialized()
//        {
//        }

//        public void OnConnected(Session session)
//        {
//        }

//        public void OnDisconnected(Session session)
//        {
//        }

//        public string OnHttpRequest(string path)
//        {
//            return "";
//        }

//        public void AddListener(string name, IMessageHandler messageHandler)
//        {
//        }

//        public void AddListener()
//        {
//        }
//    }
//}

