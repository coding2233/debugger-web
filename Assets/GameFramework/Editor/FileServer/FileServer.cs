//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using Unity.EditorCoroutines.Editor;
//using System.Threading.Tasks;
//using System.IO;

//namespace Wanderer
//{
//    public class FileServer 
//    {
//        private static EditorFileServer s_editorFileServer=null;

//        [InitializeOnLoadMethod]
//        private static void RunFileServer()
//        {
//            UnitySystemConsoleRedirector.Redirect();

//            if (s_editorFileServer != null)
//            {
//                s_editorFileServer.Dispose();
//                s_editorFileServer = null;
//            }

//            int httpPort = 62233;
//            string httpPath = "./build/";
//            if (!Directory.Exists(httpPath))
//            {
//                Directory.CreateDirectory(httpPath);
//            }
//            s_editorFileServer = new EditorFileServer(httpPort, httpPath);

            
//        }


//        class EditorFileServer : IMessageDispatcher,System.IDisposable
//        {
//            private KitNetwork m_kitNetwork;
//            //private EditorCoroutine m_lastFileServerEditorCoroutine;
//            private bool m_loop;

//            internal EditorFileServer(int port, string root)
//            {
//                //消息
//                m_kitNetwork = new KitNetwork(this);
//                m_kitNetwork.ListenHttp(port, root);
//                m_loop = true;
//                Task.Run(() => {
//                    while (m_loop && m_kitNetwork!=null && !EditorApplication.isCompiling)
//                    {
//                        m_kitNetwork.Run(RunMode.UV_RUN_NOWAIT);
//                    }

//                    ShutDown();
//                });

//                //m_lastFileServerEditorCoroutine = EditorCoroutineUtility.StartCoroutine(RunFileServer(), this);
//            }

//            //private IEnumerator RunFileServer()
//            //{
//            //    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
//            //    while (m_kitNetwork != null)
//            //    {
//            //        m_kitNetwork.Run(RunMode.UV_RUN_NOWAIT);
//            //        yield return waitForEndOfFrame;
//            //    }
//            //}

//            public void AddListener(string name, IMessageHandler messageHandler)
//            {
//            }

//            public void OnConnected(Session session)
//            {
//            }

//            public void OnDisconnected(Session session)
//            {
//            }

//            public string OnHttpRequest(string path)
//            {
//                return "";
//            }

//            public void OnInitialized()
//            {
//            }

//            public void Dispose()
//            {
//                ShutDown();
//            }

//            private void ShutDown()
//            {
//                m_loop = false;
//                //if (m_lastFileServerEditorCoroutine != null)
//                //{
//                //    EditorCoroutineUtility.StopCoroutine(m_lastFileServerEditorCoroutine);
//                //    m_lastFileServerEditorCoroutine = null;
//                //}
//                if (m_kitNetwork != null)
//                {
//                    m_kitNetwork.ShutDown();
//                    m_kitNetwork = null;
//                }
//            }

//        }

//    }
//}