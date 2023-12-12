using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Wanderer
{
	public class AssetBundleEditor : EditorWindow
	{
		private const string configName = "AssetBundleEditorConfig.json";
		private Vector2 m_scrollView = Vector2.zero;
		private bool m_showHelp;
		private static EditorAssetBundleConfigInfo s_editorAbConfig;
		private static EditorAssetBundleConfig s_newAbConfig;

		[MenuItem("Tools/Asset Bundle/Asset Bundle Editor")]
		private static void MainWindow()
		{
			//GetWindowWithRect<AssetBundleEditor>(new Rect(100, 100, 1200, 600), false, "Asset Bundle Editor");
			GetWindow<AssetBundleEditor>("Asset Bundle Editor");
		}

		/// <summary>
		/// 获取所有的AssetBundleBuild
		/// </summary>
		/// <returns></returns>
		public static AssetBundleBuild[] GetAssetBundleBuild()
		{
			var config = ProjectSettingsConfig.LoadJson<EditorAssetBundleConfigInfo>(configName);
			if (config == null || config.BundleConfigs==null || config.BundleConfigs.Count== 0)
			{
				if (EditorUtility.DisplayDialog("Waring!", "No data!", "OK"))
				{
					MainWindow();
				}

			}
			else
			{
				List<AssetBundleBuild> abbs = new List<AssetBundleBuild>();
				for (int i = 0; i < config.BundleConfigs.Count; i++)
				{
					var item = config.BundleConfigs[i];
					abbs.AddRange(JsonToABB(item));
				}
				return abbs.ToArray();
			}
			return null;
		}

        public static Dictionary<string, List<AssetBundleBuild>> GetAssetBundleBuildFromKey()
        {
            var config = ProjectSettingsConfig.LoadJson<EditorAssetBundleConfigInfo>(configName);
            if (config == null || config.BundleConfigs == null || config.BundleConfigs.Count == 0)
            {
                if (EditorUtility.DisplayDialog("Waring!", "No data!", "OK"))
                {
                    MainWindow();
                }
            }
            else
            {
                Dictionary<string, List<AssetBundleBuild>> abbsMap = new Dictionary<string, List<AssetBundleBuild>>();
                for (int i = 0; i < config.BundleConfigs.Count; i++)
                {
                    var item = config.BundleConfigs[i];
                    string key = item.AssetBundleName.Contains("/")?Path.GetDirectoryName(item.AssetBundleName):"";
                    List<AssetBundleBuild> abbs = null;
                    if (!abbsMap.TryGetValue(key, out abbs))
                    {
                        abbs = new List<AssetBundleBuild>();
                        abbsMap.Add(key,abbs);
                    }
                    item.AssetBundleName = Path.GetFileName(item.AssetBundleName);
                    abbs.AddRange(JsonToABB(item));
                }

                return abbsMap;
            }
            return null;
        }

        /// <summary>
        /// 设置是否强制更新
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetUpdateTag(string name)
		{
            var config = ProjectSettingsConfig.LoadJson<EditorAssetBundleConfigInfo>(configName);
            if (config == null || config.BundleConfigs == null || config.BundleConfigs.Count == 0)
            {
                if (EditorUtility.DisplayDialog("Waring!", "No data!", "OK"))
                {
                    MainWindow();
                }
            }
            else
            {
                var abConfig = config.BundleConfigs.Find(x => x.AssetBundleName.ToLower().Equals(name));
                if (abConfig != null)
                {
                    return abConfig.UpdateTag;
                }
            }
           
            return "default";
		}


        /// <summary>
        /// 设置是否预加载
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool GetPreload(string name)
        {
            var config = ProjectSettingsConfig.LoadJson<EditorAssetBundleConfigInfo>(configName);
            if (config == null || config.BundleConfigs == null || config.BundleConfigs.Count == 0)
            {
                if (EditorUtility.DisplayDialog("Waring!", "No data!", "OK"))
                {
                    MainWindow();
                }
            }
            else
            {
                var abConfig = config.BundleConfigs.Find(x => x.AssetBundleName.ToLower().Equals(name));
                if (abConfig != null)
                {
                    return abConfig.Preload;
                }
            }

            return false;
        }

      

		/// <summary>
		/// JsonData转AssetBundleBuild
		/// </summary>
		/// <param name="jsonData"></param>
		/// <returns></returns>
		private static List<AssetBundleBuild> JsonToABB(EditorAssetBundleConfig abConfig)
		{
			List<AssetBundleBuild> listABB = new List<AssetBundleBuild>();
			//AssetBundleBuild abb = new AssetBundleBuild();
			string abName = abConfig.AssetBundleName;
			string abVariant = abConfig.Variant;
			//Filter t:Prefab
			string filter = abConfig.Variant;
			//SearchInFolders
			string[] searchInFolders = null;
			if (!string.IsNullOrEmpty(abConfig.SearchInFolders))
			{
				searchInFolders = abConfig.SearchInFolders.Split(';');
			}

			//获取到所有的资源
			string[] assets = AssetDatabase.FindAssets(filter, searchInFolders);
			//Split
			int splitCount= abConfig.SplitCount;

			if (splitCount > 0 && assets.Length >= splitCount)
			{
				int splitNum = assets.Length / splitCount + 1;
				int index = 0;
				for (int i = 0; i < splitCount; i++)
				{
					string partAbName = $"{abName}_part{i}";
					List<string> assetNames = new List<string>();
					int tempNum = index + splitNum;
					tempNum = Mathf.Min(tempNum, assets.Length);

					for (int j = index; j < tempNum; j++)
					{
						string guid = assets[j];
						string assetPath = AssetDatabase.GUIDToAssetPath(guid);
						if (assetPath.EndsWith(".cs"))
							continue;
                        assetNames.Add(assetPath);
					}
					index = tempNum;

					AssetBundleBuild abb = new AssetBundleBuild();
					abb.assetBundleName = partAbName;
					abb.assetBundleVariant = abVariant;
					abb.assetNames = assetNames.ToArray();
					listABB.Add(abb);
				}
			}
			else
			{
				List<string> assetNames = new List<string>();
				for (int j = 0; j < assets.Length; j++)
				{
					string guid = assets[j];
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (assetPath.EndsWith(".cs"))
                        continue;
                    assetNames.Add(assetPath);
				}

				AssetBundleBuild abb = new AssetBundleBuild();
				abb.assetBundleName = abName;
				abb.assetBundleVariant = abVariant;
				abb.assetNames = assetNames.ToArray();
				listABB.Add(abb);
			}

			return listABB;
			//abb.assetNames = (string)jsonData["Variant"];
			//return abb;
		}

		private void OnEnable()
		{
			s_editorAbConfig = ProjectSettingsConfig.LoadJson<EditorAssetBundleConfigInfo>(configName);
            if (s_editorAbConfig == null || s_editorAbConfig.BundleConfigs==null || s_editorAbConfig.BundleConfigs.Count==0)
            {
				s_editorAbConfig = new EditorAssetBundleConfigInfo();
				s_editorAbConfig.BundleConfigs = new List<EditorAssetBundleConfig>();
				ProjectSettingsConfig.SaveJson(configName, s_editorAbConfig);

			}
			s_newAbConfig = new EditorAssetBundleConfig();
        }

		private void OnDisable()
		{
			s_editorAbConfig = null;
			s_newAbConfig = null;
            //_selectFoldersJsonData = null;
        }

		private void OnGUI()
		{
            DrawEditorWindow();
        }

        void DrawEditorWindow()
        {
            //主窗口
            m_scrollView = GUILayout.BeginScrollView(m_scrollView);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
			m_showHelp = GUILayout.Toggle(m_showHelp, "Help");
			if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                ProjectSettingsConfig.SaveJson(configName, s_editorAbConfig);
                EditorUtility.DisplayDialog("Save Config", "Data saved successfully!", "OK");
            }
            GUILayout.EndHorizontal();
			if (m_showHelp)
			{
				GUILayout.BeginVertical();
				GUILayout.Label("DefaultFilter");
				GUILayout.BeginHorizontal("box");
                foreach (var item in s_editorAbConfig.DefaultFilter)
                {
					GUILayout.TextField(item);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10);
				GUILayout.Label("DefaultUpdateTag");
				GUILayout.BeginHorizontal("box");
				foreach (var item in s_editorAbConfig.DefaultUpdateTag)
				{
					GUILayout.TextField(item);
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				GUILayout.Space(20);
			}
            GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width));//GUILayout.Width(100)
            GUILayout.Label("AssetBundleName", GUILayout.Width(150));
            GUILayout.Label("Annotation", GUILayout.Width(100));
            GUILayout.Label("Variant", GUILayout.Width(100));
            GUILayout.Label("Filter", GUILayout.Width(100));
            GUILayout.Label("SearchInFolders", GUILayout.Width(300));
            //GUILayout.Label("Split", GUILayout.Width(100));
            GUILayout.Label("SplitCount", GUILayout.Width(100));
            GUILayout.Label("UpdateTag", GUILayout.Width(100));
            GUILayout.Label("Preload");
            GUILayout.EndHorizontal();
            int configCount = s_editorAbConfig.BundleConfigs.Count;
            if (configCount > 0)
            {
                for (int i = 0; i < configCount; i++)
                {
                    GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width));
                    DrawEditorAssetBundleConfig(s_editorAbConfig.BundleConfigs[i]);
                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        s_editorAbConfig.BundleConfigs.RemoveAt(i);
                        GUIUtility.ExitGUI();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width));
            DrawEditorAssetBundleConfig(s_newAbConfig);
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                bool addResult = false;
                if (!string.IsNullOrEmpty(s_newAbConfig.AssetBundleName) && !string.IsNullOrEmpty(s_newAbConfig.SearchInFolders)
					&& s_editorAbConfig.BundleConfigs.Find(x=>x.AssetBundleName.Equals(s_newAbConfig.AssetBundleName))==null)
                {
					s_editorAbConfig.BundleConfigs.Add(s_newAbConfig);
					s_newAbConfig = new EditorAssetBundleConfig();
					addResult = true;
                }

                if (!addResult)
                {
                    EditorUtility.DisplayDialog("Tips", "The search folder cannot be empty!", "OK");
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

          
        }

        //绘制jsondata
        private void DrawEditorAssetBundleConfig(EditorAssetBundleConfig jsonData)
        {
         
            string content = jsonData.AssetBundleName;
            string newContent = EditorGUILayout.TextField(content, GUILayout.Width(150));
            if (!content.Equals(newContent))
            {
                jsonData.AssetBundleName = newContent;
            }

            content = jsonData.Desc;
            newContent = EditorGUILayout.TextField(content, GUILayout.Width(100));
            if (!content.Equals(newContent))
            {
                jsonData.Desc = newContent;
            }

            content = jsonData.Variant;
            newContent = EditorGUILayout.TextField(content, GUILayout.Width(100));
            if (!content.Equals(newContent))
            {
                jsonData.Variant = newContent;
            }

            content = jsonData.Filter;
            newContent = EditorGUILayout.TextField(content, GUILayout.Width(100));
            if (!content.Equals(newContent))
            {
                jsonData.Filter = newContent;
            }

            content = jsonData.SearchInFolders;
            newContent = EditorGUILayout.TextField(content, GUILayout.Width(300));
            if (!content.Equals(newContent))
            {
                jsonData.SearchInFolders = newContent;
            }


            int splitCount = jsonData.SplitCount;
            int newSplitCount = EditorGUILayout.IntField(splitCount, GUILayout.Width(100));
            if (splitCount != newSplitCount)
            {
                jsonData.SplitCount = newSplitCount;
            }

            content = jsonData.UpdateTag;
            newContent = EditorGUILayout.TextField(content, GUILayout.Width(100));
            if (!content.Equals(newContent))
            {
                jsonData.UpdateTag = newContent;
            }

            bool preload = jsonData.Preload;
            bool newPreload = EditorGUILayout.Toggle(preload, GUILayout.Width(100));
            if (preload != newPreload)
            {
                jsonData.Preload = newPreload;
            }

        }


        #region 属性
        [System.Serializable]
		public class EditorAssetBundleConfigInfo
		{
			public List<string> DefaultFilter = new List<string> { "AnimationClip",
			"AudioClip",
			"AudioMixer",
			"ComputeShader",
			"Font",
			"GUISkin",
			"Material",
			"Mesh",
			"Model",
			"PhysicMaterial",
			"Prefab",
			"Scene",
			"Script",
			"Shader",
			"Sprite",
			"Texture",
			"VideoClip","TextAsset","ScriptableObject","AnimatorController","SpriteAtlas"};

            //force: 不通知用户强制更新
            //default: 通知用户更新，更新完成后才能开始游戏
            //background: 在后台下载， 不影响用户游戏，可提示
            //其他都是使用到再申请下载都可以
            public List<string> DefaultUpdateTag = new List<string>() {
            "force","default","background"
            };

			public List<EditorAssetBundleConfig> BundleConfigs;
		}

		[System.Serializable]
		public class EditorAssetBundleConfig
		{
			public string AssetBundleName= "";
            public string Desc= "unknown";
			public string Variant = "";
			public string Filter = "t:Prefab";
            //多个文件夹使用;隔开
			public string SearchInFolders="Assets/";
			public int SplitCount;
			public string UpdateTag="default";
            public bool Preload;
        }
        #endregion
    }

}