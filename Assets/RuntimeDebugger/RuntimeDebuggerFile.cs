using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RuntimeDebugger
{
    public class RuntimeDebuggerFile : RuntimeDebuggerBase
	{
        private Dictionary<string,string> m_filePaths;

        public RuntimeDebuggerFile()
        {
            m_filePaths = new Dictionary<string, string>();
            m_filePaths.Add("Application.dataPath", Application.dataPath);
            m_filePaths.Add("Application.persistentDataPath", Application.persistentDataPath);
            m_filePaths.Add("Application.streamingAssetsPath", Application.streamingAssetsPath);
            m_filePaths.Add("Application.temporaryCachePath", Application.temporaryCachePath);
            m_filePaths.Add("Application.consoleLogPath", Application.consoleLogPath);
		}

		public override void OnMessage(string message)
		{
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if ("show" == message)
            {
                if (m_filePaths != null)
                {
                    var allFileNodes = new Dictionary<string, FileNode>();
                    foreach (var item in m_filePaths)
                    {
                        var fileNode = GetFileNode(item.Value);
                        allFileNodes.Add(item.Key, fileNode);
                    }
                    if (allFileNodes.Count > 0)
                    {
                        Send(allFileNodes);
                    }
                }
            }
            else
            {
                if (File.Exists(message))
                {
                    try
                    {
                        string filePath = message;
                        var fileAllText = File.ReadAllText(filePath,System.Text.Encoding.UTF8);
                        Dictionary<string, string> mapFileAllBytes = new Dictionary<string, string>();
                        mapFileAllBytes.Add(filePath, fileAllText);
                        Send(mapFileAllBytes);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"Read file exception. {message} {e}");
                    }
                }
            }
		}

        private FileNode GetFileNode(string path)
        {
            FileNode fileNode = new FileNode();
            fileNode.Children = new List<FileNode>();
			fileNode.FullName = path;
			fileNode.Name = Path.GetFileName(path);
            bool isFile = File.Exists(path); 
			fileNode.IsFile = isFile;
            if (isFile)
            {
                FileInfo fileInfo = new FileInfo(path);
                fileNode.CreationTime = fileInfo.CreationTime.ToString();
                fileNode.LastAccessTime = fileInfo.LastAccessTime.ToString();
                fileNode.LastWriteTime = fileInfo.LastWriteTime.ToString();
                //Extension
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
				fileNode.CreationTime = directoryInfo.CreationTime.ToString();
				fileNode.LastAccessTime = directoryInfo.LastAccessTime.ToString();
				fileNode.LastWriteTime = directoryInfo.LastWriteTime.ToString();
                if (Directory.Exists(path))
                {
                    var dirs = Directory.GetDirectories(path);
                    foreach (var item in dirs)
                    {
                        fileNode.Children.Add(GetFileNode(item));
					}
                    var files = Directory.GetFiles(path);
                    foreach (var item in files)
                    {
						fileNode.Children.Add(GetFileNode(item));
					}
                }
			}

			return fileNode;
		}
	}

    public class FileNode
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool IsFile { get; set; }
        public string CreationTime { get; set; }
        public string LastAccessTime { get; set; }
        public string LastWriteTime { get; set; }
        public List<FileNode> Children { get; set; }
	}

}