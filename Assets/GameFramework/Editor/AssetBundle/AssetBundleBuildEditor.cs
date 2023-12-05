using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;

namespace Wanderer
{

    public class AssetBundleBuildEditor : EditorWindow
    {
        //资源信息文本名称
        private const string m_assetVersionTxt = "version.json";
        private const string m_configPath = "ProjectSettings/AssetBundleBuildConfig.json";
        private static AssetBundleBuildConfigInfo m_config;
        private static List<string> m_buildTargets;
        private static string m_outPutPath = "";

        private Vector2 m_scrollViewPos;
        private Vector2 m_scrollViewPosOtherRs;
        private static string m_rootPath;

        private static readonly Dictionary<BuildTarget, bool> _allTargets = new Dictionary<BuildTarget, bool>();

        //压缩内容
        string[] _compressionOptionsContent = new string[] { "No Compression", "Standard Compression (LZMA)", "Chunk Based Compression (LZ4)" };

        [MenuItem("Tools/Asset Bundle/AssetBundle Build Options #&O")]
        public static void AssetBundilesOptions()
        {
            m_rootPath = Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));

            LoadConfig();
            GetWindowWithRect<AssetBundleBuildEditor>(new Rect(200, 300, 500, 400), true, "Options");
        }

        /// <summary>
        /// 打包AssetBundle
        /// </summary>
        public static string BuildAssetBundles(BuildTarget target,string buildPath=null)
        {
            LoadConfig();
            //打包编辑器的激活平台
            string buildTargetPath = BuildTarget(target, buildPath);
            return buildTargetPath;
        }

       

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// This function can be called multiple times per frame (one call per event).
        /// </summary>
        void OnGUI()
        {
            if (m_config == null)
            {
                return;
            }

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("App Version:");
            GUILayout.Label(Application.version.ToString());

            GUILayout.Label("Resource Version:");
            var resBundleVersion = CheckResourceBundleVersion();
            GUILayout.Label(Resources.Load<ResourceVersion>("BundleVersion").MainVersion);

            GUILayout.Label("Asset Version:");
            GUILayout.Label(m_config.Version.ToString());
            if (GUILayout.Button("CONFIG", GUILayout.Width(60)))
            {
                Selection.activeObject = resBundleVersion;
                EditorUtility.FocusProjectWindow();
            }
            if (GUILayout.Button("RESET", GUILayout.Width(60)))
            {
                m_config.Version = 0;
            }
            GUILayout.EndHorizontal();

            //支持以前的app版本
            GUILayout.BeginHorizontal("Box");
            m_config.SupportOldAppVersions = EditorGUILayout.TextField("Supported old versions:", m_config.SupportOldAppVersions);
            GUILayout.EndHorizontal();

            //老版的资源链接
            GUILayout.BeginHorizontal("Box");
            m_config.OldResourceUrl = EditorGUILayout.TextField("Old resource url:", m_config.OldResourceUrl);
            GUILayout.EndHorizontal();

            //压缩格式
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("Compression:");
            m_config.CompressOptions = EditorGUILayout.Popup(m_config.CompressOptions, _compressionOptionsContent);
            GUILayout.EndHorizontal();

            ////Encrypt------------------------------------
            // GUILayout.BeginHorizontal("Box");
            // GUILayout.FlexibleSpace();
            // _config.IsEncrypt = GUILayout.Toggle(_config.IsEncrypt, "Encrypt");
            // GUILayout.EndHorizontal();

            //BUILD PATH
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("Build Path:");
            GUILayout.TextArea(string.IsNullOrEmpty(m_config.BuildPath) ? m_rootPath : m_config.BuildPath);

            if (GUILayout.Button("BROWSE", GUILayout.Width(80)))
            {
                string path = (string.IsNullOrEmpty(m_config.BuildPath) || !Directory.Exists(m_config.BuildPath)) ? m_rootPath : m_config.BuildPath;

                path = EditorUtility.OpenFolderPanel("Build Path", path, "");
                if (!string.IsNullOrEmpty(path))
                {
                    path = Path.GetFullPath(path);
                    if (path.Contains(m_rootPath))
                    {
                        path = path.Replace(m_rootPath, "").Replace("\\", "/");
                        if (path.IndexOf("/") == 0)
                        {
                            path = path.Substring(1, path.Length - 1);
                        }
                    }
                    m_config.BuildPath = path;
                }
                return;
            }
            GUILayout.EndHorizontal();

            //build target----------------------------------------------------------------------------
            BuildTarget buildTarget = (BuildTarget)m_config.BuildTarget;
            BuildTarget newBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target:", buildTarget);
            if (newBuildTarget != buildTarget)
            {
                m_config.BuildTarget = (int)newBuildTarget;
            }
         
            //other reseources----------------------------------------------------------------------------
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Other Resources:");
            m_scrollViewPosOtherRs = GUILayout.BeginScrollView(m_scrollViewPosOtherRs, "Box");
            m_config.OtherResources = EditorGUILayout.TextArea(m_config.OtherResources, GUILayout.Height(200));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            //确认更改--------------------------------------------------------------------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.Width(60)))
            {
                //保存配置文件
                SaveConfig();
                //关闭窗口
                //  Close();
                EditorUtility.DisplayDialog("SaveConfig", "Succee", "OK");
            }
            if (GUILayout.Button("Build", GUILayout.Width(60)))
            {
                //保存配置文件
                SaveConfig();
                //资源打包
                buildTarget = (BuildTarget)m_config.BuildTarget;
                BuildAssetBundles(buildTarget);
                EditorGUIUtility.ExitGUI();
                //BuildTarget();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }


        //检查ResourceBundleVersion
        private static ResourceVersion CheckResourceBundleVersion()
        {
            var resBundleVersion = Resources.Load<ResourceVersion>("BundleVersion");
            if (resBundleVersion == null)
            {
                resBundleVersion = new ResourceVersion();
                string resBundleVersionPath = "Assets/Resources";
                if (!Directory.Exists(resBundleVersionPath))
                {
                    Directory.CreateDirectory(resBundleVersionPath);
                }
                resBundleVersionPath += "/BundleVersion.asset";
                AssetDatabase.CreateAsset(resBundleVersion, resBundleVersionPath);
                AssetDatabase.Refresh();
                resBundleVersion = AssetDatabase.LoadAssetAtPath<ResourceVersion>(resBundleVersionPath);
            }
            return resBundleVersion;
        }

        //加载配置信息
        private static void LoadConfig()
        {
            CheckResourceBundleVersion();

            if (!File.Exists(m_configPath))
            {
                File.WriteAllText(m_configPath, JsonUtility.ToJson(new AssetBundleBuildConfigInfo()));
            }

            m_config = JsonUtility.FromJson<AssetBundleBuildConfigInfo>(File.ReadAllText(m_configPath));
        }

        //保存配置信息
        private static void SaveConfig()
        {
            string json = JsonUtility.ToJson(m_config);
            File.WriteAllText(m_configPath, json);
        }

        //资源打包
        private static string BuildTarget(BuildTarget target,string buildPath = null)
        {
            try
            {
                if (string.IsNullOrEmpty(buildPath))
                {
                    buildPath = m_config.BuildPath;
                }
                //打包路径
                buildPath = Path.Combine(buildPath, BuildTargetToString(target));
                buildPath = buildPath.Replace("\\", "/");

                if (!Directory.Exists(buildPath))
                    Directory.CreateDirectory(buildPath);
                //设置打包的相关选项
                BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
                //设置压缩 默认LZMA
                if (m_config.CompressOptions == 0)
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                //LZ4
                else if (m_config.CompressOptions == 2)
                    options |= BuildAssetBundleOptions.ChunkBasedCompression;
                //打包  Build
                //BuildPipeline.BuildAssetBundles(buildPath, AssetBundleEditor.GetAssetBundleBuild(), options, target);
                //BuildPipeline.BuildAssetBundles(buildPath, options, target);
                var assetBundleBuildMaps = AssetBundleEditor.GetAssetBundleBuildFromKey();
                if (assetBundleBuildMaps != null)
                {
                    foreach (var item in assetBundleBuildMaps)
                    {
                        string buildKeyPath = Path.Combine(buildPath, item.Key);
                        if (!Directory.Exists(buildKeyPath))
                        {
                            Directory.CreateDirectory(buildKeyPath);
                        }

                        BuildPipeline.BuildAssetBundles(buildKeyPath, item.Value.ToArray(), options, target);

                        //保存资源版本信息
                        SaveAssetVersion(buildKeyPath, target);
                    }
                }

                //保存资源版本信息
                //SaveAssetVersion(buildPath, target);
                Debug.Log($"资源打包成功: {buildPath}");
                //    //更新资源版本号 -- 保存配置文件
                m_config.Version++;
                SaveConfig();
                return buildPath;
            }
            catch (System.Exception e)
            {
                throw new Exception($"Build assetbundle error [{target.ToString()}] :{e.ToString()}");
            }
			
        }

        //保存资源版本信息
        private static void SaveAssetVersion(string buildPath, BuildTarget target)
        {
            //string targetName = target.ToString().ToLower();
            string targetBundlePath = Path.Combine(buildPath,Path.GetFileName(buildPath));;
            if (!File.Exists(targetBundlePath))
                return;
            //删除manifest文件
            string targetManifestPath =$"{targetBundlePath}.manifest";
            if (File.Exists(targetManifestPath))
                File.Delete(targetManifestPath);
            //移动manifest文件
            byte[] manifestBytes = File.ReadAllBytes(targetBundlePath);
            //string targetName = "manifest";
            //string tempTargetBundlePath = Path.Combine(buildPath, targetName);
            //if (File.Exists(tempTargetBundlePath))
            //    File.Delete(tempTargetBundlePath);
            //File.Move(targetBundlePath, tempTargetBundlePath) ;
            //targetBundlePath = tempTargetBundlePath;

            //整理AssetBundleVersionInfo
            AssetVersion assetVersionInfo = new AssetVersion();
            assetVersionInfo.Version = m_config.Version;
            assetVersionInfo.AppVersion = Application.version;
            assetVersionInfo.OldResourceUrl = m_config.OldResourceUrl;
            assetVersionInfo.Manifest = Convert.ToBase64String(manifestBytes);
            assetVersionInfo.AssetHashInfos = new List<AssetHashInfo>();
            //整理当前资源支持的以前的AppVersion
            if (!string.IsNullOrEmpty(m_config.SupportOldAppVersions))
            {
                string[] args = m_config.SupportOldAppVersions.Split(';');
                if (args != null)
                {
					foreach (var item in args)
					{
                        if (!string.IsNullOrEmpty(item))
                        {
                            assetVersionInfo.SupportOldAppVersions.Add(item);
                        }
					}
                }
            }

            AssetBundle targetBundle = AssetBundle.LoadFromFile(targetBundlePath);
            AssetBundleManifest manifest = targetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //整理全部的资源并生成配置文件
            //string assetContent = GetAssetsFromAssetBundle(buildPath,manifest);
            //string assetsTxtPath=Path.Combine(buildPath,m_assetsTxt);
            //File.WriteAllText(Path.Combine(buildPath,"assets"),assetContent);
            //assetbundle
            List<string> assetNames = new List<string>();
            assetNames.AddRange(manifest.GetAllAssetBundles());
            targetBundle.Unload(true);
            File.Delete(targetBundlePath);

            for (int i = 0; i < assetNames.Count; i++)
            {
                AssetHashInfo assetHashInfo = new AssetHashInfo();
                assetHashInfo.Name = assetNames[i];
                assetHashInfo.UpdateTag = AssetBundleEditor.GetUpdateTag(assetHashInfo.Name);
                assetHashInfo.Preload =AssetBundleEditor.GetPreload(assetHashInfo.Name);
                assetHashInfo.Addressables = GetAssetsFromAssetBundle(Path.Combine(buildPath, assetNames[i]));
                string sourceFilePath = Path.Combine(buildPath, $"{assetNames[i]}.tmp");
                string targetFilePath = Path.Combine(buildPath, assetNames[i]);
                if (File.Exists(sourceFilePath))
                {
                    File.Delete(sourceFilePath);
                }
                File.Move(targetFilePath, sourceFilePath);
                using (var readStream = File.OpenRead(sourceFilePath))
                {
                    using (var writeStream = new EncryptFileStream(targetFilePath, FileMode.OpenOrCreate,FileAccess.Write,FileShare.None,1024*10,false))
                    {
                        var buffer = new byte[1024*10];
                        int readSize = 1;
                        while (readSize > 0)
                        {
                            readSize = readStream.Read(buffer, 0, buffer.Length);
                            if (readSize > 0)
                            {
                                writeStream.Write(buffer, 0, readSize);
                            }
                        }
                    }
                }
                File.Delete(sourceFilePath);
                //File.Move(targetFilePath, sourceFilePath);
                long dataLength = FileUtility.GetFileSize(targetFilePath);
                assetHashInfo.Size = dataLength > 1024 ? (int)(dataLength / 1024.0f) : 1;
                assetHashInfo.MD5 = FileUtility.GetFileMD5(targetFilePath);
                assetVersionInfo.AssetHashInfos.Add(assetHashInfo);
                //删除manifest文件
                string manifestPath = Path.Combine(buildPath, assetNames[i] + ".manifest");
              //  manifestPath=Path.GetFullPath(manifestPath);
                if (File.Exists(manifestPath))
                {
                    File.Delete(manifestPath);
                }
            }

            //添加其他资源的信息
            List<AssetHashInfo> otherResInfo = LoadOtherResource(buildPath);
            assetVersionInfo.AssetHashInfos.AddRange(otherResInfo);

            string json = JsonUtility.ToJson(assetVersionInfo);
            //VersionAsset 加密
            //json = json.ToEncrypt();
            string buildAssetVersionPath = Path.Combine(buildPath, m_assetVersionTxt);
            File.WriteAllText(buildAssetVersionPath, json);
        }

        //保存平台版本信息
        //private static void SavePlatformVersion(List<BuildTarget> targets)
        //{
        //    if (targets == null || targets.Count == 0)
        //        return;

        //    PlatformVersionInfo platformInfo = new PlatformVersionInfo();
        //    platformInfo.Version = _config.Version;
        //    platformInfo.Platforms = new List<string>();
        //    foreach (var item in targets)
        //    {
        //        platformInfo.Platforms.Add(item.ToString().ToLower());
        //    }
        //    string json = JsonUtility.ToJson(platformInfo);
        //    //保存平台信息
        //    File.WriteAllText(Path.Combine(_config.BuildPath, _assetVersionTxt), json);
        //    //更新资源版本号 -- 保存配置文件
        //    _config.Version++;
        //    SaveConfig();
        //    //打开文件夹
        //    // EditorUtility.OpenWithDefaultApp(_config.BuildPath);
        //}

        //添加本地资源
        private static List<AssetHashInfo> LoadOtherResource(string buildPath)
        {
            List<AssetHashInfo> assetHashInfos = new List<AssetHashInfo>();
            if (m_config == null || string.IsNullOrEmpty(m_config.OtherResources))
                return assetHashInfos;

            string[] otherRes = m_config.OtherResources.Split('\n');
            if (otherRes != null && otherRes.Length > 0)
            {
                string rootPath = Application.dataPath;
                foreach (var item in otherRes)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    string srcPath = Path.Combine(rootPath, item);
                    if (!File.Exists(srcPath))
                    {
                        Debug.LogWarning($"找不到文件:{srcPath}");
                        continue;
                    }

                    //获取文件的md5值
                    string hash = FileUtility.GetFileMD5(srcPath);
                    string fileName = Path.GetFileName(srcPath);
                    long dataLength = FileUtility.GetFileSize(srcPath);
                    int size = dataLength > 1024 ? (int)(dataLength / 1024.0f) : 1;
                    //复制文件
                    File.Copy(srcPath, Path.Combine(buildPath, fileName), true);
                    //添加信息
                    assetHashInfos.Add(new AssetHashInfo() { Name = fileName, MD5 = hash, Size = size });
                }
            }

            return assetHashInfos;
        }

        //获取所有的资源
        private static List<string> GetAssetsFromAssetBundle(string abPath)
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(abPath);
            //存储资源名称
            string[] assetNames = assetBundle.GetAllAssetNames();
            if (assetBundle.isStreamedSceneAssetBundle)
            {
                assetNames = assetBundle.GetAllScenePaths();
            }
            assetBundle.Unload(true);
            List<string> assetList =new List<string>();
            assetList.AddRange(assetNames);
            return assetList;
        }

        //复制资源
        private static void CopyResource(BuildTarget target)
        {
            //打包路径
            string buildPath = Path.GetFullPath(Path.Combine(m_config.BuildPath, target.ToString().ToLower()));
            //获取源文件夹下的所有文件  不考虑子文件夹
            string[] files = Directory.GetFiles(buildPath);
            string targetPath = Path.GetFullPath(Application.streamingAssetsPath);
            if(!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i].Replace(buildPath, targetPath);
                File.Copy(files[i], path, true);
            }
        }


        /// <summary>
        /// BuildTarget转字符串
        /// </summary>
        /// <param name="target"></param>
        public static string BuildTargetToString(BuildTarget target)
        {
            string targetName = target.ToString().ToLower();
            if (targetName.Contains("windows"))
            {
                targetName = "windows";
            }
            else if (targetName.Contains("osx"))
            {
                targetName = "osx";
            }
            else if (targetName.Contains("linux"))
            {
                targetName = "linux";
            }
            return targetName;
        }

        //ab包的配置文件信息
        [System.Serializable]
        public class AssetBundleBuildConfigInfo
        {
            /// <summary>
            /// 资源版本号
            /// </summary>
            public int Version = 0;
            /// <summary>
            /// 打包的路径
            /// </summary>
            public string BuildPath = "";
            /// <summary>
            /// 压缩格式
            /// </summary>
            public int CompressOptions = 1;
            /// <summary>
            /// 打包的目标平台
            /// </summary>
            public int BuildTarget = 13;
         //   public List<int> BuildTargets = new List<int>();
            /// <summary>
            /// 其他的资源路径
            /// </summary>
            public string OtherResources = "";
            /// <summary>
            /// 支持旧版的App版本号
            /// </summary>
            public string SupportOldAppVersions = "";
            /// <summary>
            /// 上一版的资源链接
            /// </summary>
            public string OldResourceUrl = "";
        }
    }
}