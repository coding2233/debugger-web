using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Wanderer
{

    public class StrangIocTemplateTool :EditorWindow
    {

        private string m_nameSpace="Wanderer";
        private string m_nameTemplate = "";

        private string m_templateContextView;
        private string m_templateContext;
        private string m_templateDef;

        private string m_showTemplateContext="";

        private int m_selectTemplate;
        private bool m_createTemplateDir=true;

        [MenuItem("Tools/StangeIoc Template")]
        static void CreateTemplate()
        {
            GetWindow<StrangIocTemplateTool>("StangeIoc Template");
        }


        private void OnEnable()
        {
            m_templateContextView = File.ReadAllText("Assets/GameFramwork/Editor/Template/TemplateContextView.cs.txt");
            m_templateContext = File.ReadAllText("Assets/GameFramwork/Editor/Template/TemplateContext.cs.txt");
            m_templateDef = File.ReadAllText("Assets/GameFramwork/Editor/Template/TemplateDef.cs.txt");

            m_showTemplateContext = ReplateTempalte(m_templateContextView);
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(350));
            OnDrawMenuItem();
            GUILayout.EndVertical();
            OnDrawTemplate();
            GUILayout.EndHorizontal();
        }

        private void OnDrawMenuItem()
        {
            m_nameSpace=EditorGUILayout.TextField("命名空间",m_nameSpace);
            m_nameTemplate = EditorGUILayout.TextField("模板名称", m_nameTemplate);
            m_createTemplateDir = GUILayout.Toggle(m_createTemplateDir, "是否创建文件夹");
            if (GUILayout.Button("Create"))
            {
                if (string.IsNullOrEmpty(m_nameTemplate)
                    || string.IsNullOrEmpty(m_nameSpace))
                {
                    return;
                }

                string dir = EditorUtility.OpenFolderPanel("请选择需要创建的文件夹路径","Assets","");
                if (!string.IsNullOrEmpty(dir))
                {
                    if (m_createTemplateDir)
                    {
                        dir = $"{dir}/{m_nameTemplate}";
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }

                    CreateTempalte($"{dir}/{m_nameTemplate}ContextView.cs",m_templateContextView);
                    CreateTempalte($"{dir}/{m_nameTemplate}Context.cs",m_templateContext);
                    CreateTempalte($"{dir}/{m_nameTemplate}Def.cs",m_templateDef);

                    AssetDatabase.Refresh();
                }
            }
        }


        private void OnDrawTemplate()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            int selectTemplate = GUILayout.Toolbar(m_selectTemplate, new string[] { "ContextView", "Context", "Def" });
            if (selectTemplate != m_selectTemplate)
            {
                switch (selectTemplate)
                {
                    case 0:
                        m_showTemplateContext = ReplateTempalte(m_templateContextView);
                        break;
                    case 1:
                        m_showTemplateContext = ReplateTempalte(m_templateContext);
                        break;
                    case 2:
                        m_showTemplateContext = ReplateTempalte(m_templateDef);
                        break;
                    default:
                        break;
                }
                
                m_selectTemplate = selectTemplate;
            }

            GUILayout.EndHorizontal();

            GUILayout.TextField(m_showTemplateContext);

            GUILayout.EndVertical();
        }


        private string ReplateTempalte(string template)
        {
            if (!string.IsNullOrEmpty(m_nameSpace))
            {
                template = template.Replace("$TemplateNameSpace", m_nameSpace);
            }
            if (!string.IsNullOrEmpty(m_nameTemplate))
            {
                template = template.Replace("$TemplateName", m_nameTemplate);
            }
            return template;
        }

        private void CreateTempalte(string path, string template)
        {
            template = ReplateTempalte(template);
            File.WriteAllText(path, template);
        }

    }

}