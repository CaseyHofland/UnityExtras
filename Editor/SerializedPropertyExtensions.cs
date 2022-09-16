#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
	public static class SerializedPropertyExtensions
	{
		public static SerializedProperty FindAutoPropertyRelative(this SerializedProperty serializedProperty, string relativePropertyPath) => serializedProperty.FindPropertyRelative($"<{relativePropertyPath}>k__BackingField");

		[Obsolete("This method does not take into account properties nested in arrays and should be used with caution!", false)]
		public static FieldInfo GetField(this SerializedProperty property, out object target)
		{
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var paths = property.propertyPath.Split('.');

			target = property.serializedObject.targetObject;
			FieldInfo field = target.GetType().GetField(paths[0], flags);
			for (int i = 1; i < paths.Length; i++)
			{
                var child = field.GetValue(target);
                if (child is IList)
                {
                    return field;
                }

				target = child;
                field = field.FieldType.GetField(paths[i], flags);
			}

			return field;
		}

		[Obsolete("This method does not take into account properties nested in arrays and should be used with caution!", false)]
		public static T GetValue<T>(this SerializedProperty property) => (T)property.GetField(out var target).GetValue(target);

		//public static void SetValue(this SerializedProperty property, object value)
		//{
		//	const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		//	var paths = property.propertyPath.Split('.');
		//	int index = 0;

		//	SetValueRecursive(property.serializedObject.targetObject);

		//	void SetValueRecursive(object target)
		//	{
		//		var field = target.GetType().GetField(paths[index++], flags);
		//		if (index >= paths.Length)
		//		{
		//			field.SetValue(target, value);
		//			return;
		//		}

		//              var child = field.GetValue(target);
		//              SetValueRecursive(child);
		//              field.SetValue(target, child);
		//	}
		//}

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
