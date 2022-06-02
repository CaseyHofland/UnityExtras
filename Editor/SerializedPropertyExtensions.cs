#nullable enable
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    public static class SerializedPropertyExtensions
	{
		public static string GetSanitizedPropertyPath(this SerializedProperty serializedProperty)
		{
			return serializedProperty.propertyPath.Replace(".Array.data[", "[");
		}

		public static System.Type? GetSerializedPropertyType(this SerializedProperty serializedProperty)
		{
			// follow reflection up to match path and return type of last node

			// fix path for arrays
			var path = GetSanitizedPropertyPath(serializedProperty);

			var currentType = serializedProperty.serializedObject.targetObject.GetType();

			string[] slices = path.Split('.', '[');
			foreach (var slice in slices)
			{
				if (currentType == null)
				{
					Debug.LogErrorFormat("GetSerializedPropertyType Couldn't extract type from {0}:{1}",
						serializedProperty.serializedObject.targetObject.name,
						serializedProperty.propertyPath);

					return null;
				}

				// array element: get array type if this is an array element
				if (slice.EndsWith("]"))
				{
					if (currentType.IsArray)
					{
						currentType = currentType.GetElementType();
					}
					else if (currentType.IsGenericType && currentType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
					{
						currentType = currentType.GetGenericArguments()[0];
					}
					else
					{
						Debug.LogErrorFormat("GetSerializedPropertyType unkown array/container type for {0}:{1}",
							serializedProperty.serializedObject.targetObject.name,
							serializedProperty.propertyPath);

						return null;
					}
				}
				else // field: find field by same name as slice and match to type
				{
					var type = currentType;
					while (type != null)
					{
						var fieldInfo = type.GetField(slice, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						if (fieldInfo == null)
						{
							type = type.BaseType;
							continue;
						}

						currentType = fieldInfo.FieldType;
						break;
					}
					// Assert.IsNotNull(type);
				}
			}

			return currentType;
		}
	}
}
