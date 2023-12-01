using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace RuntimeDebugger
{
	public class VectorConverter : JsonConverter
	{
		public override bool CanRead => true;
		public override bool CanWrite => true;
		public override bool CanConvert(Type objectType)
		{
			return typeof(Vector2) == objectType ||
			typeof(Vector2Int) == objectType ||
			typeof(Vector3) == objectType ||
			typeof(Vector3Int) == objectType ||
			typeof(Vector4) == objectType ||
			typeof(Color) == objectType ||
			typeof(Color32) == objectType ||
			typeof(Quaternion) == objectType;
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return objectType switch
			{
				var t when t == typeof(Vector2) => JsonConvert.DeserializeObject<Vector2>(serializer.Deserialize(reader).ToString()),
				var t when t == typeof(Vector2Int) => JsonConvert.DeserializeObject<Vector2Int>(serializer.Deserialize(reader).ToString()),
				var t when t == typeof(Vector3) => JsonConvert.DeserializeObject<Vector3>(serializer.Deserialize(reader).ToString()),
				var t when t == typeof(Vector3Int) => JsonConvert.DeserializeObject<Vector3Int>(serializer.Deserialize(reader).ToString()),
				var t when t == typeof(Vector4) => JsonConvert.DeserializeObject<Vector4>(serializer.Deserialize(reader).ToString()),
				var t when t == typeof(Color) => JsonConvert.DeserializeObject<Color>(serializer.Deserialize(reader).ToString()),
				var t when t == typeof(Color32) => JsonConvert.DeserializeObject<Color32>(serializer.Deserialize(reader).ToString()),
				var t when t == typeof(Quaternion) => JsonConvert.DeserializeObject<Quaternion>(serializer.Deserialize(reader).ToString()),
				_ => throw new Exception("Unexpected Error Occurred"),
			};
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			switch (value)
			{
				case Vector2 v:
					writer.WritePropertyName("x");
					writer.WriteValue(v.x);
					writer.WritePropertyName("y");
					writer.WriteValue(v.y);
					break;
				case Vector2Int v:
					writer.WritePropertyName("x");
					writer.WriteValue(v.x);
					writer.WritePropertyName("y");
					writer.WriteValue(v.y);
					break;
				case Vector3 v:
					writer.WritePropertyName("x");
					writer.WriteValue(v.x);
					writer.WritePropertyName("y");
					writer.WriteValue(v.y);
					writer.WritePropertyName("z");
					writer.WriteValue(v.z);
					break;
				case Vector3Int v:
					writer.WritePropertyName("x");
					writer.WriteValue(v.x);
					writer.WritePropertyName("y");
					writer.WriteValue(v.y);
					writer.WritePropertyName("z");
					writer.WriteValue(v.z);
					break;
				case Vector4 v:
					writer.WritePropertyName("x");
					writer.WriteValue(v.x);
					writer.WritePropertyName("y");
					writer.WriteValue(v.y);
					writer.WritePropertyName("z");
					writer.WriteValue(v.z);
					writer.WritePropertyName("w");
					writer.WriteValue(v.w);
					break;
				case Quaternion v:
					writer.WritePropertyName("x");
					writer.WriteValue(v.x);
					writer.WritePropertyName("y");
					writer.WriteValue(v.y);
					writer.WritePropertyName("z");
					writer.WriteValue(v.z);
					writer.WritePropertyName("w");
					writer.WriteValue(v.w);
					break;
				case Color v:
					writer.WritePropertyName("r");
					writer.WriteValue(v.r);
					writer.WritePropertyName("g");
					writer.WriteValue(v.g);
					writer.WritePropertyName("b");
					writer.WriteValue(v.b);
					writer.WritePropertyName("a");
					writer.WriteValue(v.a);
					break;
				case Color32 v:
					writer.WritePropertyName("r");
					writer.WriteValue(v.r);
					writer.WritePropertyName("g");
					writer.WriteValue(v.g);
					writer.WritePropertyName("b");
					writer.WriteValue(v.b);
					writer.WritePropertyName("a");
					writer.WriteValue(v.a);
					break;
				default:
					throw new Exception("Unexpected Error Occurred");
			}
			writer.WriteEndObject();
		}
	}


	public class ConverterTypes
	{
		private static HashSet<Type> s_checkVectorTypes;
		private static HashSet<Type> s_checkTypes;

		private static Dictionary<string, Type> s_mapCheckTypes;
		private static Dictionary<string, Type> s_mapCheckVectorTypes;

		static ConverterTypes()
		{
			s_checkVectorTypes = new HashSet<Type>() { typeof(Vector2), typeof(Vector2Int), typeof(Vector3), typeof(Vector3Int), typeof(Vector4), typeof(Color), typeof(Color32), typeof(Quaternion) };
			s_checkTypes = new HashSet<Type>() { typeof(bool), typeof(string), typeof(int), typeof(float), typeof(double), typeof(long), typeof(uint), typeof(ulong), };
			s_mapCheckVectorTypes = new Dictionary<string, Type>();
			s_mapCheckTypes = new Dictionary<string, Type>();
			foreach (var item in s_checkVectorTypes)
			{
				s_mapCheckVectorTypes.Add(item.Name, item);
				s_checkTypes.Add(item);
			}
			foreach (var item in s_checkTypes)
			{
				s_mapCheckTypes.Add(item.Name, item);
			}
		}

		public static bool CheckType(Type t)
		{
			bool result = s_checkTypes.Contains(t);
			result = result || t.IsEnum;
			return result;
		}

		public static object GetConverterValue(string typeName, object value)
		{
			if (s_mapCheckTypes.TryGetValue(typeName, out Type t))
			{
				if (s_checkVectorTypes.Contains(t))
				{
					var conObject = JsonConvert.DeserializeObject(value.ToString(), t, new VectorConverter());
					return conObject;
				}
				else
				{
					var changeObject = Convert.ChangeType(value, t);
					return changeObject;
				}
			}
			return value;
		}

		public static object GetConverterShaderValue(string typeName, object value)
		{

			object convertObjectValue = value;
			Type convertObjectType = null;

			switch (typeName)
			{
				case "Color":
					convertObjectType = typeof(Color);
					convertObjectValue = JsonConvert.DeserializeObject(value.ToString(), convertObjectType, new VectorConverter());
					break;
				case "Vector":
					convertObjectType = typeof(Vector4);
					convertObjectValue = JsonConvert.DeserializeObject(value.ToString(), convertObjectType, new VectorConverter());
					break;
				case "Float":
					convertObjectType = typeof(float);
					convertObjectValue = Convert.ChangeType(value, convertObjectType);
					break;
				case "Range":
					convertObjectType = typeof(float);
					convertObjectValue = Convert.ChangeType(value, convertObjectType);
					break;
				case "Int":
					convertObjectType = typeof(int);
					convertObjectValue = Convert.ChangeType(value, convertObjectType);
					break;
			}

			return convertObjectValue;
		}

	}
}