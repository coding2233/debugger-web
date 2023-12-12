using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Wanderer
{
	[CustomEditor(typeof(FrameworkContextView),true)]
	public class FrameworkContextViewEditor : Editor
	{
		private List<string> m_defines;
		private const string EDITOR_BUNDLE = "EDITOR_BUNDLE";
		private bool m_editorBundle;

		private void OnEnable()
		{
			m_defines = new List<string>();
			var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
			string[] defines;
			PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget,out defines);
			if (defines != null)
			{
				m_defines.AddRange(defines);
			}
			m_editorBundle = m_defines.Find(x=> EDITOR_BUNDLE.Equals(x)) != null;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			bool editorBundle = GUILayout.Toggle(m_editorBundle,"Use Asset Bundle In Editor");
			if (m_editorBundle != editorBundle)
			{
				if (editorBundle)
				{
					m_defines.Add(EDITOR_BUNDLE);
				}
				else
				{
					m_defines.Remove(EDITOR_BUNDLE);
				}
				var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
				PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, m_defines.ToArray());
				m_editorBundle = editorBundle;
			}
		}
	}
}
