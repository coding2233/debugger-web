//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEditor.Build.Player;
//using UnityEngine;

//namespace Wanderer
//{
//    public class BuildConfig
//    {
//        //[InitializeOnLoadMethod]
//        //private static void Setup()
//        //{
//        //    //1. 需要复制 Unity/2021.3.1f1/Editor/Data/il2cpp  hybrid_clr_data/il2cpp
//        //    //2. 需要复制 Unity/2021.3.1f1/Editor/Data/MonoBleedingEdge  hybrid_clr_data/MonoBleedingEdge
//        //    //3. 复制https://gitee.com/focus-creative-games/il2cpp_plus hybrid_clr_data/il2cpp/libil2cpp
//        //    //4. 复制https://github.com/focus-creative-games/hybridclr hybrid_clr_data/il2cpp/libil2cpp/hybridclr
//        //    //5. 更新了HybridCLR的源码后  需要清除 Library\Il2cppBuildCache

//        //    /// unity允许使用UNITY_IL2CPP_PATH环境变量指定il2cpp的位置，因此我们不再直接修改安装位置的il2cpp，
//        //    var localIl2cppDir = Path.Combine(Path.GetDirectoryName(Application.dataPath), "hybrid_clr_data/il2cpp");
//        //    Environment.SetEnvironmentVariable("UNITY_IL2CPP_PATH", localIl2cppDir);
//        //}
//    }

//    internal class CompileDllHelper
//    {
//        public static void CompileDll(string buildDir, BuildTarget target)
//        {
//            var group = BuildPipeline.GetBuildTargetGroup(target);

//            ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings();
//            scriptCompilationSettings.group = group;
//            scriptCompilationSettings.target = target;
//            Directory.CreateDirectory(buildDir);
//            ScriptCompilationResult scriptCompilationResult = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, buildDir);
//            //foreach (var ass in scriptCompilationResult.assemblies)
//            //{
//            //    Debug.LogFormat("compile assemblies:{1}/{0}", ass, buildDir);
//            //}

//            string sourcePath = Path.Combine(buildDir, "Game.HotUpdate.dll");
//            string targetPath = $"Assets/Game/HotUpdate/Code/Game.HotUpdate.dll.bytes";
//            File.Copy(sourcePath, targetPath, true);
//            Debug.Log($"Copy hotupdate dll. {sourcePath} -> {targetPath}");

//            sourcePath = Path.Combine(buildDir, "Resouce.HotUpdate.dll");
//            targetPath = $"Assets/Game/ResourceUpdate/Resouce.HotUpdate.dll.bytes";
//            File.Copy(sourcePath, targetPath, true);
//            Debug.Log($"Copy hotupdate dll. {sourcePath} -> {targetPath}");

//            AssetDatabase.Refresh();
//        }

//        public static void CompileDll(BuildTarget target)
//        {
//            CompileDll($"Library/hybridclr_dlls/{target}", target);
//        }

//        [MenuItem("Tools/CompileDll/ActiveBuildTarget")]
//        public static void CompileDllActiveBuildTarget()
//        {
//            CompileDll(EditorUserBuildSettings.activeBuildTarget);
//        }

//        [MenuItem("Tools/CompileDll/Win32")]
//        public static void CompileDllWin32()
//        {
//            CompileDll(BuildTarget.StandaloneWindows);
//        }

//        [MenuItem("Tools/CompileDll/Win64")]
//        public static void CompileDllWin64()
//        {
//            CompileDll(BuildTarget.StandaloneWindows64);
//        }

//        [MenuItem("Tools/CompileDll/Android")]
//        public static void CompileDllAndroid()
//        {
//            CompileDll(BuildTarget.Android);
//        }

//        [MenuItem("Tools/CompileDll/IOS")]
//        public static void CompileDllIOS()
//        {
//            CompileDll(BuildTarget.iOS);
//        }
//    }
//}