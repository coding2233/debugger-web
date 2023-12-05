//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//namespace Wanderer
//{
//    public class AssetBundleBrowserEditor : EditorWindow
//    {
//        List<AssetBundle> m_bundles =new List<AssetBundle>();

//        [MenuItem("Tools/Asset Bundle/Asset Bundle Browser")]
//        private static void MainWindow()
//        {
//            GetWindow<AssetBundleBrowserEditor>("Asset Bundle Browser");

//            //EditorUtility.OpenFilePanel("x","")
//        }

//        private void OnEnable()
//        {
//            var files =  Directory.GetFiles("ServerData/android");
//            foreach (var file in files)
//            {
//                var ext = Path.GetExtension(file);
//                if (string.IsNullOrEmpty(ext))
//                {
//                    //var ab = AssetBundle.LoadFromStream(new FileStream(file, FileMode.Open));
//                    var ab = AssetBundle.LoadFromStream(new EncryptFileStream(file, FileMode.Open));
//                    if (ab == null)
//                    {
//                        Debug.LogError("ab包解析失败");
//                    }
//                    else
//                    {
//                        Debug.Log("ab包解析成功");
//                        m_bundles.Add(ab);
//                    }
//                }
//                else
//                {
//                    //string sourceFilePath = file;
//                    //string targetFilePath = $"{sourceFilePath}.temp";
//                    //Debug.Log($"source file:{FileUtility.GetFileMD5(sourceFilePath)}");
//                    //using (var readStream = File.OpenRead(sourceFilePath))
//                    //{
//                    //    using (var writeStream = new EncryptFileStream(targetFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 1024, false))
//                    //    {
//                    //        var buffer = new byte[1024];
//                    //        int readSize = 1;
//                    //        while (readSize > 0)
//                    //        {
//                    //            readSize = readStream.Read(buffer, 0, buffer.Length);
//                    //            if (readSize > 0)
//                    //            {
//                    //                writeStream.Write(buffer, 0, readSize);
//                    //            }
//                    //        }
//                    //    }
//                    //}
//                    //Debug.Log($"targetFilePath file 01:{FileUtility.GetFileMD5(targetFilePath)}");

//                    //sourceFilePath = targetFilePath;
//                    //targetFilePath = $"{sourceFilePath}.temp";

//                    //using (var readStream = File.OpenRead(sourceFilePath))
//                    //{
//                    //    using (var writeStream = new EncryptFileStream(targetFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 1024, false))
//                    //    {
//                    //        var buffer = new byte[1024];
//                    //        int readSize = 1;
//                    //        while (readSize > 0)
//                    //        {
//                    //            readSize = readStream.Read(buffer, 0, buffer.Length);
//                    //            if (readSize > 0)
//                    //            {
//                    //                writeStream.Write(buffer, 0, readSize);
//                    //            }
//                    //        }
//                    //    }
//                    //}

//                    //Debug.Log($"targetFilePath file 02:{FileUtility.GetFileMD5(targetFilePath)}");

//                }
//            }
//        }

//    }
//}
