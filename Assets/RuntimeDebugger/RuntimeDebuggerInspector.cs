using Newtonsoft.Json;
using strange.extensions.reflector.api;
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

	private void Send(RspInspector rsp)
	{
		string message = JsonConvert.SerializeObject(rsp,new VectorConverter());
		Debug.Log(message);
		Send(message);
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


	public void SetValue(Component target)
	{
		try
		{
			if (target == null)
			{
				return;
			}
			
			if (string.IsNullOrEmpty(Name))
			{
				return;
			}

			if (Name.Contains("/"))
			{
				SetMaterialValue(target);
			}
			else
			{
				SetNormalValue(target);
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
			if (target == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(Name))
			{
				return;
			}
			var nameArgs = Name.Split('/');
			if (nameArgs == null || nameArgs.Length != 3)
			{
				return;
			}
			string targetMaterilName = nameArgs[0];
			int materialIndex = int.Parse(nameArgs[1]);
			Name = nameArgs[2];
			var targetType = target.GetType();
			var property = targetType.GetProperty(nameArgs[0]);
			if (property != null)
			{
				if (property.PropertyType == typeof(Material))
				{
					var materialObject = property.GetValue(target);
					SetNormalValue(materialObject);
					return;
				}
				else if (property.PropertyType == typeof(Material[]))
				{
					var materialObjects = property.GetValue(target) as Material[];
					SetNormalValue(materialObjects[materialIndex]);
					return;
				}
			}
			var field = targetType.GetField(nameArgs[0]);
			if (field != null)
			{
				if (field.FieldType == typeof(Material))
				{
					var materialObject = field.GetValue(target);
					SetNormalValue(materialObject);
					return;
				}
				else if (field.FieldType == typeof(Material[]))
				{
					var materialObjects = field.GetValue(target) as Material[];
					SetNormalValue(materialObjects[materialIndex]);
					return;
				}
			}
			
		}
		catch (System.Exception e)
		{
			Debug.LogWarning($"SetValue Exception e:{e}");
		}
	}

	private void SetNormalValue(object target)
	{
		try
		{
			if (target == null)
			{
				return;
			}
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
	public bool Enable { get; set; } = true;
	public bool IsMonoBehaviour { get; set; }
	public List<ReflectionInspector> ReflectionValues { get; set; }
	public Dictionary<string, List<List<string>>> MapMaterialValues { get; set; }

	public ComponentInspector()
	{

	}

	public ComponentInspector(Component component)
	{
		InstanceID = component.GetInstanceID();
		IsMonoBehaviour = component is MonoBehaviour;
		if (IsMonoBehaviour)
		{
			Enable = (component as MonoBehaviour).enabled;
		}
		var type = component.GetType();
		Name = type.Name;
		ReflectionValues = new List<ReflectionInspector>();
		MapMaterialValues = new Dictionary<string, List<List<string>>>();
		var properties = type.GetProperties();
        foreach (var item in properties)
        {
			if (ConverterTypes.CheckType(item.PropertyType))
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
		}
	}

	private void SetMaterialValues(Material[] materials,string name)
	{
		if (materials == null || materials.Length ==0)
		{
			return;
		}

		List<List<string>> materialsNames = new List<List<string>>();
		MapMaterialValues.Add(name, materialsNames);

		int materialIndex = 0;
		foreach (var material in materials)
        {
			List<string> materialNames = new List<string>(0);
			var type = material.GetType();
			var properties = type.GetProperties();
			foreach (var item in properties)
			{
				if (ConverterTypes.CheckType(item.PropertyType))
				{
					string refName = $"{name}/{material.name}/{typeof(PropertyInfo).Name}/{materialIndex}/{item.PropertyType.Name}";
					ReflectionValues.Add(new ReflectionInspector(material, item, refName));
					materialNames.Add(refName);
				}
			}
			var fieldInfos = type.GetFields();
			foreach (var item in fieldInfos)
			{
				if (ConverterTypes.CheckType(item.FieldType))
				{
					string refName = $"{name}/{material.name}/{typeof(FieldInfo).Name}/{materialIndex}/{item.FieldType.Name}";
					ReflectionValues.Add(new ReflectionInspector(material, item, refName));
					materialNames.Add(refName);
				}
			}
			materialIndex++;
			materialsNames.Add(materialNames);
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
