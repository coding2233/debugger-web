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

		//FindGameObjectSend(null);
		//FindComponentsSend(GameObject.Find("Main Camera").gameObject);
	}

	//public override void OnOpen(RuntimeDebugger runtimeDebugger, IntPtr channel)
	//{
	//	base.OnOpen(runtimeDebugger, channel);

	//	FindGameObjectSend(null);
	//}

	public override void OnMessage(string message)
	{
		try
		{
			ReqInspector req = JsonConvert.DeserializeObject<ReqInspector>(message, new VectorConverter());
			switch (req.Cmd)
			{
				case ReqInspectorCmd.FindGameObjects:
					if (m_idFindAllGameObjects.TryGetValue(req.InstanceID, out GameObject findParent))
					{
						FindGameObjectSend(findParent.transform);
					}
					else
					{
						FindGameObjectSend(null);
					}
					break;
				case ReqInspectorCmd.FindComponent:
					if (m_idFindAllGameObjects.TryGetValue(req.InstanceID, out GameObject findGameObject))
					{
						FindComponentsSend(findGameObject);
					}
					break;
				case ReqInspectorCmd.EditReflectionValue:
					if (m_idFindAllGameObjects.TryGetValue(req.InstanceID, out GameObject findEditGameObject))
					{
						EditorComponent(findEditGameObject, req.ComponentInstanceID, req.ReflectionValue);
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
	private void EditorComponent(GameObject target, int componentInstanceId, ReflectionInspector reflectionValue)
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
	#endregion
}

public class ReflectionInspector
{
	public string Name { get; set; }
	public string ValueType { get; set; }
	public object Value { get; set; }
	public string ReflectionType { get; set; }
	public bool CanWrite { get; set; }
	public ReflectionInspector()
	{ }

	public ReflectionInspector(Component component, PropertyInfo propertyInfo)
	{
		Name = propertyInfo.Name;
		ValueType = propertyInfo.PropertyType.Name;
		Value = propertyInfo.GetValue(component);
		CanWrite = propertyInfo.CanWrite;
		ReflectionType = typeof(PropertyInfo).Name;
	}

	public ReflectionInspector(Component component, FieldInfo fieldInfo)
	{
		Name = fieldInfo.Name;
		ValueType = fieldInfo.FieldType.Name;
		Value = fieldInfo.GetValue(component);
		CanWrite = true;
		ReflectionType = typeof(FieldInfo).Name;
	}

	public void SetValue(Component target)
	{
		if (target == null || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(ReflectionType))
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
					property.SetValue(target, Value);
				}
				break;
			case "FieldInfo":
				var field = targetType.GetField(Name);
				if (field != null)
				{
					field.SetValue(target, Value);
				}
				break;
		}
	}



}

public class ComponentInspector
{
	public int InstanceID { get; set; }
	public string Name { get; set; }
	public bool Enable { get; set; } = true;

	public List<ReflectionInspector> ReflectionValues { get; private set; }

	private static HashSet<Type> s_checkTypes;

	public ComponentInspector()
	{

	}

	public ComponentInspector(Component component)
	{
		InstanceID = component.GetInstanceID();
		if (component is MonoBehaviour)
		{
			Enable = (component as MonoBehaviour).enabled;
		}
		var type = component.GetType();
		Name = type.Name;
		ReflectionValues = new List<ReflectionInspector>();
		var properties = type.GetProperties();
        foreach (var item in properties)
        {
			if ( item.CanRead && CheckType(item.PropertyType))
			{
				ReflectionValues.Add(new ReflectionInspector(component,item));
			}
        }
        var fieldInfos = type.GetFields();
		foreach (var item in fieldInfos)
		{
			if (CheckType(item.FieldType))
			{
				ReflectionValues.Add(new ReflectionInspector(component, item));
			}
		}
		//MemberInfos = type.GetMembers();
	}

	private static bool CheckType(Type type)
	{
		if (s_checkTypes == null || s_checkTypes.Count == 0)
		{
			s_checkTypes = new HashSet<Type>() {typeof(string), typeof(int), typeof(float), typeof(double),
				typeof(long), typeof(uint),typeof(ulong),typeof(Color),typeof(Color32),typeof(Quaternion),
				typeof(Vector2), typeof(Vector3), typeof(Vector4), typeof(Vector2Int), typeof(Vector3Int)};
		}

		return s_checkTypes.Contains(type);
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
}

public class ReqInspector
{
	public ReqInspectorCmd Cmd { get; set; }
	public int InstanceID { get; set; }
	public int ComponentInstanceID { get; set; }
	public ReflectionInspector ReflectionValue { get; set; }
}

public class RspInspector
{
	public ReqInspectorCmd Cmd { get; set; }
	public int InstanceID { get; set; }
	public HierarchyNode[] FindNodes { get; set; }
	public ComponentInspector[] Components { get; set; }
}
