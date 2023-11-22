using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeDebuggerInspector : RuntimeDebuggerBase
{
	private Dictionary<GameObject, HierarchyNode> m_findAllGameObjects = new Dictionary<GameObject, HierarchyNode>();
	private Dictionary<int, GameObject> m_idFindAllGameObjects = new Dictionary<int, GameObject>();

	public RuntimeDebuggerInspector()
	{
	}


	public override void OnMessage(string message)
	{
		try
		{
			GameObject findGameObject;
			ReqInspector req = JsonConvert.DeserializeObject<ReqInspector>(message);
			switch (req.Cmd)
			{
				case ReqInspectorCmd.FindGameObjects:
					if (m_idFindAllGameObjects.TryGetValue(req.InstanceID, out findGameObject))
					{
						FindGameObjectSend(findGameObject.transform);
					}
					else
					{
						FindGameObjectSend(null);
					}
					break;
				case ReqInspectorCmd.FindComponent:
					if (m_idFindAllGameObjects.TryGetValue(req.InstanceID, out findGameObject))
					{
						FindComponentsSend(findGameObject);
					}
					break;
				case ReqInspectorCmd.EditReflectionValue:
					if (m_idFindAllGameObjects.TryGetValue(req.InstanceID, out findGameObject))
					{
						EditComponent(findGameObject, req.ComponentInstanceID, req.ReflectionValue);
					}
					break;
				case ReqInspectorCmd.EditGameObject:
					if (m_idFindAllGameObjects.TryGetValue(req.InstanceID, out findGameObject))
					{
						EditGameObject(findGameObject, req.HierarchyNode);
					}
					break;
				case ReqInspectorCmd.EditComponentEnable:
					if (m_idFindAllGameObjects.TryGetValue(req.InstanceID, out findGameObject))
					{
						EditComponentEnable(findGameObject, req.ComponentInstanceID);
					}
					break;
				default:
					break;
			}
		}
		catch (System.Exception e)
		{
			Debug.LogWarning(e.ToString());
		}
	}

	//public override void OnClose()
	//{
	//	base.OnClose();
	//}

	

	private void FindGameObjectSend(Transform parent)
	{
		var rsp = FindGameObject(parent);
		Send(rsp);
	}


	private List<GameObject> FindGameObjects(Transform parent)
	{
		List<GameObject> findGameObjects = new List<GameObject>();
		if (parent == null)
		{
			var gos = SceneManager.GetActiveScene().GetRootGameObjects();
			if (gos != null)
			{
				findGameObjects.AddRange(gos);
			}
		}
		else
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				findGameObjects.Add(parent.GetChild(i).gameObject);
			}
		}

		List<GameObject> findAllChild = new List<GameObject>();
        foreach (var item in findGameObjects)
        {
			var itemChild = FindGameObjects(item.transform);
			findAllChild.AddRange(itemChild);
		}

		findGameObjects.AddRange(findAllChild);

		return findGameObjects;
	}

	private RspInspector FindGameObject(Transform parent)
	{
		List<GameObject> findGameObjects = FindGameObjects(parent);

		List<HierarchyNode> nodes = new List<HierarchyNode>();
        foreach (var item in findGameObjects)
        {
			nodes.Add(GetHierarchyNode(item));
		}

		RspInspector rsp = new RspInspector();
		rsp.Cmd = ReqInspectorCmd.FindGameObjects;
		rsp.InstanceID = parent == null?0 : parent.gameObject.GetInstanceID();
		rsp.FindNodes = nodes.ToArray();

		return rsp;
	}

	private void FindComponentsSend(GameObject target)
	{
		var rsp = FindComponents(target);
		Send(rsp);
	}
	private RspInspector FindComponents(GameObject target)
	{
		List<ComponentInspector> componentInspectors = new List<ComponentInspector>();
		if (target != null)
		{
			var components = target.GetComponents<Component>();
			if (components != null && components.Length > 0)
			{
				for (int i = 0; i < components.Length; i++)
				{
					//组件可能丢失
					if(components[i] == null)
					{
						continue;
					}
					var ci = new ComponentInspector(components[i]);
					componentInspectors.Add(ci);
				}
			}
		}
		RspInspector rsp = new RspInspector();
		rsp.Cmd = ReqInspectorCmd.FindComponent;
		rsp.InstanceID = target.GetInstanceID();
		rsp.Components = componentInspectors.ToArray();
		return rsp;
	}

	private HierarchyNode GetHierarchyNode(GameObject findGameObject)
	{
		HierarchyNode node = null;
		if (!m_findAllGameObjects.TryGetValue(findGameObject,out node))
		{
			node = new HierarchyNode(findGameObject);
			m_findAllGameObjects.Add(findGameObject,node);
			m_idFindAllGameObjects.Add(node.InstanceID, findGameObject);
		}
		return node;
	}

	#region 编辑属性
	private void EditGameObject(GameObject target, HierarchyNode hierarchyNode)
	{
		if (target != null)
		{
			try
			{
				target.name = hierarchyNode.Name;
				target.layer = LayerMask.NameToLayer(hierarchyNode.Layer);
				target.tag = hierarchyNode.Tag;
				target.SetActive(hierarchyNode.Active);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}
	}
	private void EditComponent(GameObject target, int componentInstanceId, ReflectionInspector reflectionValue)
	{
		if (target != null)
		{
			var components = target.GetComponents<Component>();
			if (components != null && components.Length > 0)
			{
				for (int i = 0; i < components.Length; i++)
				{
					if (componentInstanceId == components[i].GetInstanceID())
					{
						if (reflectionValue != null)
						{
							reflectionValue.SetValue(components[i]);
						}
						break;
					}
				}
			}
		}
	}
	private void EditComponentEnable(GameObject target, int componentInstanceId)
	{
		if (target != null)
		{
			var components = target.GetComponents<Component>();
			if (components != null && components.Length > 0)
			{
				for (int i = 0; i < components.Length; i++)
				{
					if (componentInstanceId == components[i].GetInstanceID())
					{
						var monoBehaivour = components[i] as MonoBehaviour;
						if (monoBehaivour != null)
						{
							monoBehaivour.enabled = !monoBehaivour.enabled;
						}
						break;
					}
				}
			}
		}
	}
	#endregion
}

public class ReflectionInspector
{
	public string Name { get; set; }
	public string FullName { get; set; }
	public string ValueType { get; set; }
	public object Value { get; set; }
	public string ReflectionType { get; set; }
	public bool CanWrite { get; set; }
	public ReflectionInspector()
	{ }

	public ReflectionInspector(UnityEngine.Object component, PropertyInfo propertyInfo,string fullName = null)
	{
		Name = propertyInfo.Name;
		FullName = string.IsNullOrEmpty(fullName) ? propertyInfo.Name : fullName;
		ValueType = propertyInfo.PropertyType.IsEnum ? typeof(int).Name : propertyInfo.PropertyType.Name;
		Value = propertyInfo.GetValue(component);
		CanWrite = propertyInfo.CanWrite;
		ReflectionType = typeof(PropertyInfo).Name;

	}

	public ReflectionInspector(UnityEngine.Object component, FieldInfo fieldInfo, string fullName = null)
	{
		Name = fieldInfo.Name;
		FullName = string.IsNullOrEmpty(fullName) ? fieldInfo.Name: fullName;
		ValueType = fieldInfo.FieldType.IsEnum ? typeof(int).Name : fieldInfo.FieldType.Name;
		Value = fieldInfo.GetValue(component);
		CanWrite = true;
		ReflectionType = typeof(FieldInfo).Name;
	}

	public ReflectionInspector(Material material, string name, UnityEngine.Rendering.ShaderPropertyType propertyType, string fullName)
	{
		Name = name;
		FullName = fullName;
		ValueType = propertyType.ToString();
		CanWrite = true;
		ReflectionType = typeof(Material).Name;
		switch (propertyType)
		{
			case UnityEngine.Rendering.ShaderPropertyType.Color:
				Value = material.GetColor(name);
				break;
			case UnityEngine.Rendering.ShaderPropertyType.Vector:
				Value = material.GetVector(name);
				break;
			case UnityEngine.Rendering.ShaderPropertyType.Float:
			case UnityEngine.Rendering.ShaderPropertyType.Range:
				Value = material.GetFloat(name);
				break;
			case UnityEngine.Rendering.ShaderPropertyType.Texture:
				CanWrite = false;
				var texture = material.GetTexture(name);
				Value = texture == null? "": texture.name;
				break;
			case UnityEngine.Rendering.ShaderPropertyType.Int:
				Value = material.GetInt(name);
				break;
			default:
				break;
		}
	}

	public void SetValue(Component target)
	{
		try
		{
			if (target == null)
			{
				return;
			}
			
			if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(FullName))
			{
				return;
			}

			//普通属性修改
			if (Name.Equals(FullName))
			{
				SetNormalValue(target);
			}
			else
			{
				//材质需要再处理
				SetMaterialValue(target);
			}
		}
		catch (System.Exception e)
		{
			Debug.LogWarning($"SetValue Exception e:{e}");
		}
	}

	private void SetMaterialValue(Component target)
	{ 
		try
		{
			var nameArgs = FullName.Split('/');
			string targetMaterilName = nameArgs[0];
			string materialName = nameArgs[1];
			string shaderName = nameArgs[2];
			int materialIndex = int.Parse(nameArgs[3]);
			string materialTypeName = nameArgs[4];
			string materialRefName = nameArgs[5];

			var targetType = target.GetType();

			var property = targetType.GetProperty(nameArgs[0]);
			if (property != null)
			{
				if (property.PropertyType == typeof(Material))
				{
					var materialObject = property.GetValue(target);
					SetMaterialValue((Material)materialObject);
					return;
				}
				else if (property.PropertyType == typeof(Material[]))
				{
					var materialObjects = property.GetValue(target) as Material[];
					SetMaterialValue((Material)materialObjects[materialIndex]);
					return;
				}
			}

			var field = targetType.GetField(nameArgs[0]);
			if (field != null)
			{
				if (field.FieldType == typeof(Material))
				{
					var materialObject = field.GetValue(target);
					SetMaterialValue((Material)materialObject);
					return;
				}
				else if (field.FieldType == typeof(Material[]))
				{
					var materialObjects = field.GetValue(target) as Material[];
					SetMaterialValue((Material)materialObjects[materialIndex]);
					return;
				}
			}

		}
		catch (System.Exception e)
		{
			Debug.LogWarning($"SetValue Exception e:{e}");
		}
	}

	private void SetMaterialValue(Material material)
	{
		try
		{
			if (material == null)
			{
				return;
			}
			switch (ValueType)
			{
				case "Color":
					material.SetColor(Name,(Color)ConverterTypes.GetConverterShaderValue(ValueType,Value));
					break;
				case "Vector":
					material.SetVector(Name, (Vector4)ConverterTypes.GetConverterShaderValue(ValueType, Value));
					break;
				case "Float":
				case "Range":
					material.SetFloat(Name, (float)ConverterTypes.GetConverterShaderValue(ValueType, Value));
					break;
				case "Int":
					material.SetInt(Name, (int)ConverterTypes.GetConverterShaderValue(ValueType, Value));
					break;
				case "Texture":
					break;
				default:
					SetNormalValue(material);
					break;
			}
		}
		catch (System.Exception e)
		{
			Debug.LogWarning($"SetMaterialValue Exception e:{e}");
		}
	}

	private void SetNormalValue(object target)
	{
		try
		{
			var targetType = target.GetType();
			switch (ReflectionType)
			{
				case "PropertyInfo":
					var property = targetType.GetProperty(Name);
					if (property != null)
					{
						property.SetValue(target, ConverterTypes.GetConverterValue(ValueType, Value));
					}
					break;
				case "FieldInfo":
					var field = targetType.GetField(Name);
					if (field != null)
					{
						field.SetValue(target, ConverterTypes.GetConverterValue(ValueType, Value));
					}
					break;
			}
		}
		catch (System.Exception e)
		{
			Debug.LogWarning($"SetValue Exception e:{e}");
		}
	}
}

public class ComponentInspector
{
	public int InstanceID { get; set; }
	public string Name { get; set; }
	public List<ReflectionInspector> ReflectionValues { get; set; }
	public Dictionary<string, List<List<string>>> MapMaterialValues { get; set; }

	public ComponentInspector()
	{

	}

	public ComponentInspector SetMissing()
	{
		InstanceID = -1;
		Name = "Missing (Mono Script)";
		return this;
	}

	public ComponentInspector(Component component)
	{
		InstanceID = component.GetInstanceID();
		var type = component.GetType();
		Name = type.Name;
		ReflectionValues = new List<ReflectionInspector>();
		MapMaterialValues = new Dictionary<string, List<List<string>>>();
		var properties = type.GetProperties();
        foreach (var item in properties)
        {
			if (item.CanRead && ConverterTypes.CheckType(item.PropertyType))
			{
				ReflectionValues.Add(new ReflectionInspector(component, item));
			}
			else if (item.PropertyType == typeof(Material))
			{
				var material = item.GetValue(component) as Material;
				var materialName = item.Name;
				SetMaterialValues(new Material[] { material }, materialName);
			}
			else if (item.PropertyType == typeof(Material[]))
			{
				var materials = item.GetValue(component) as Material[];
				var materialName = item.Name;
				SetMaterialValues(materials, materialName);
			}
			else
			{
				//Debug.Log($"{typeof(PropertyInfo).Name} {item.Name} {item.PropertyType} {item.ReflectedType}");
			}
		}
        var fieldInfos = type.GetFields();
		foreach (var item in fieldInfos)
		{
			if (ConverterTypes.CheckType(item.FieldType))
			{
				ReflectionValues.Add(new ReflectionInspector(component, item));
			}
			else if (item.FieldType == typeof(Material))
			{
				var material = item.GetValue(component) as Material;
				var materialName = item.Name;
				SetMaterialValues(new Material[] { material }, materialName);
			}
			else if (item.FieldType == typeof(Material[]))
			{
				var materials = item.GetValue(component) as Material[];
				var materialName = item.Name;
				SetMaterialValues(materials, materialName);
			}
			else
			{
				//Debug.Log($"{typeof(FileInfo).Name} {item.Name} {item.FieldType} {item.ReflectedType}");
			}
		}
	}

	private void SetMaterialValues(Material[] materials,string name)
	{
		try
		{
			if (materials == null || materials.Length == 0)
			{
				return;
			}

			HashSet<string> ignoreProperties = new HashSet<string>() { "color", "mainTexture", "mainTextureOffset", "mainTextureScale" };

			List<List<string>> materialsNames = new List<List<string>>();
			MapMaterialValues.Add(name, materialsNames);

			int materialIndex = 0;
			foreach (var material in materials)
			{
				if (material != null)
				{
					var matShader = material.shader;
					var shaderName = matShader == null ? "no shader" : material.shader.name;

					List<string> materialNames = new List<string>();
					var t = material.GetType();

					var properties = t.GetProperties();
					if (properties != null)
					{
						foreach (var item in properties)
						{
							if (ignoreProperties.Contains(item.Name))
							{
								continue;
							}

							if (item.CanRead && ConverterTypes.CheckType(item.PropertyType))
							{
								string refName = $"{name}/{material.name}/{shaderName}/{materialIndex}/{typeof(PropertyInfo).Name}/{item.Name}";
								ReflectionValues.Add(new ReflectionInspector(material, item, refName));
								materialNames.Add(refName);
							}
						}
					}

					if (matShader != null)
					{
						for (int i = 0; i < matShader.GetPropertyCount(); i++)
						{
							var propertyName = matShader.GetPropertyName(i);
							var propertyType = matShader.GetPropertyType(i);
							string refName = $"{name}/{material.name}/{matShader.name}/{materialIndex}/{propertyType}/{propertyName}";
							ReflectionValues.Add(new ReflectionInspector(material, propertyName, propertyType, refName));
							materialNames.Add(refName);
						}
					}

					materialsNames.Add(materialNames);
				}
				materialIndex++;
			}
		}
		catch (Exception e)
		{
			Debug.Log("e:" + e);
		}
	}

}

public class HierarchyNode
{
	public int InstanceID { get; set; }
	public int ParentInstanceID { get; set; }
	public string Name { get; set; }
	public string Tag { get; set; }
	public string Layer { get; set; }
	public bool Active { get; set; }
	//public ComponentInspector[] Components { get; set; }

	public HierarchyNode()
	{ }

	public HierarchyNode(GameObject gameObject)
	{
		InstanceID = gameObject.GetInstanceID();
		Name = gameObject.name;
		Tag = gameObject.tag;
		Layer = LayerMask.LayerToName(gameObject.layer);
		Active = gameObject.activeSelf;
		if (gameObject.transform.parent != null)
		{
			ParentInstanceID = gameObject.transform.parent.gameObject.GetInstanceID(); 
		}
    }
}


public enum ReqInspectorCmd
{
	FindGameObjects,
	FindComponent,
	EditReflectionValue,
	EditGameObject,
	EditComponentEnable,
	EditorMaterial,
}

public class ReqInspector
{
	public ReqInspectorCmd Cmd { get; set; }
	public int InstanceID { get; set; }
	public int ComponentInstanceID { get; set; }
	public ReflectionInspector ReflectionValue { get; set; }
	public HierarchyNode HierarchyNode { get; set; }
}

public class RspInspector
{
	public ReqInspectorCmd Cmd { get; set; }
	public int InstanceID { get; set; }
	public HierarchyNode[] FindNodes { get; set; }
	public ComponentInspector[] Components { get; set; }
}
