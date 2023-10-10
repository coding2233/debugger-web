using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Player;
using System.IO;

namespace Wanderer
{
    public class GameCommandBuilder
    {
        [MenuItem("Tools/Build Scripts")]
        static void Build()
        {
            BuildTarget buildTarget = GetBuildTargetForCommandLine();
            string buildPath = $"build/HotUpdateDlls/{buildTarget}";
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            ScriptCompilationSettings compilationSettings = new ScriptCompilationSettings();
            compilationSettings.target = buildTarget;
            compilationSettings.group = BuildPipeline.GetBuildTargetGroup(compilationSettings.target);
            PlayerBuildInterface.CompilePlayerScripts(compilationSettings, buildPath);

            //复制
            string copyDir = $"ConfigData/HotUpdateDlls/{buildTarget}";
            if (!Directory.Exists(copyDir))
            {
                Directory.CreateDirectory(copyDir);
            }

            string[] targetDlls = new string[] { "Game.Runtime.dll", "Resource.Update.dll" };
            for (int i = 0; i < targetDlls.Length; i++)
            {
                File.Copy($"{buildPath}/{targetDlls[i]}", $"{copyDir}/{targetDlls[i]}", true);
            }

            File.Copy($"{buildPath}/Resource.Update.dll", "Assets/ResourceUpdate/Code/Resource.Update.dll.bytes", true);
            File.Copy($"{buildPath}/Resource.Update.pdb", "Assets/ResourceUpdate/Code/Resource.Update.pdb.bytes", true);

            AssetDatabase.Refresh();

            string buildTargetPath = AssetBundleBuildEditor.BuildAssetBundles(buildTarget,"build/ServerData");
            AssetDatabase.Refresh();

            if (Directory.Exists(buildTargetPath) && Directory.GetFiles(buildTargetPath) != null)
            {
      
            }
            Debug.Log("复制完成...");
        }


        private static BuildTarget GetBuildTargetForCommandLine()
        {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-buildTarget"))
                {
                    var buildTargetArg = args[i + 1].ToLower();
                    if (buildTargetArg.Equals("android"))
                    {
                        buildTarget = BuildTarget.Android;
                    }
                    else if (buildTargetArg.Contains("ios"))
                    {
                        buildTarget = BuildTarget.iOS;
                    }
                    else if (buildTargetArg.Contains("linux"))
                    {
                        buildTarget = BuildTarget.StandaloneLinux64;
                    }
                    else if (buildTargetArg.Contains("osx"))
                    {
                        buildTarget = BuildTarget.StandaloneOSX;
                    }
                    else
                    {
                        buildTarget = BuildTarget.StandaloneWindows64;
                    }

                    break;
                }
            }

            if (buildTarget != EditorUserBuildSettings.activeBuildTarget)
            {
                BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                //切换目标平台
                EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
                //刷新平台
                AssetDatabase.Refresh();
                Debug.Log("EditorUserBuildSettings.SwitchActiveBuildTarget: " + buildTargetGroup + "  " + buildTarget);
            }

            return buildTarget;
        }

    }

}