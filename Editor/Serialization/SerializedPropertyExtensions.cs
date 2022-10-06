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
		const string backingFieldPrefix = "<";
		const string backingFieldSuffix = ">k__BackingField";

		public static SerializedProperty FindAutoPropertyRelative(this SerializedProperty serializedProperty, string relativePropertyPath) => serializedProperty.FindPropertyRelative($"{backingFieldPrefix}{relativePropertyPath}{backingFieldSuffix}");

        private static MemberInfo GetMember(this SerializedProperty property, out object target, out int index)
        {
			const BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            target = property.serializedObject.targetObject;
            var path = property.propertyPath.AsSpan();

			return GetMember(path, ref target, out index);

            static MemberInfo GetMember(ReadOnlySpan<char> path, ref object target, out int index)
			{
				if (target is IList list)
				{
                    var digitStart = path.IndexOf('[') + 1;
                    var digitEnd = digitStart + path[digitStart..].IndexOf(']');

                    index = Convert.ToInt32(path[digitStart..digitEnd].ToString());
                    if (digitEnd + 2 >= path.Length)
					{
                        return target.GetType().GetProperty("Item");
					}

					target = list[index];
					return GetMember(path[(digitEnd + 2)..], ref target, out index);
                }

                index = path.IndexOf('.');
				var memberPath = index == -1
					? path
					: path[0..index];

                if (memberPath.StartsWith(backingFieldPrefix) && memberPath.EndsWith(backingFieldSuffix))
				{
					var propertyInfo = target.GetType().GetProperty(memberPath[1..memberPath.IndexOf(backingFieldSuffix)].ToString(), bindingAttr);
					if (index == -1)
					{
						return propertyInfo;
					}
					
					target = propertyInfo.GetValue(target);
                }
				else
				{
					var fieldInfo = target.GetType().GetField(memberPath.ToString(), bindingAttr);
					if (index == -1)
					{
						return fieldInfo;
					}
			
					target = fieldInfo.GetValue(target);
                }

				return GetMember(path[(index + 1)..], ref target, out index);
            }
        }

		public static PropertyInfo GetProperty(this SerializedProperty property, out object target, out int index) => (PropertyInfo)property.GetMember(out target, out index);
		public static FieldInfo GetField(this SerializedProperty property, out object target) => (FieldInfo)property.GetMember(out target, out _);

		public static T GetValue<T>(this SerializedProperty property) => property.GetMember(out var target, out int index) switch
		{
			PropertyInfo propertyInfo => index == -1 ? (T)propertyInfo.GetValue(target) : (T)propertyInfo.GetValue(target, new object[] { index }),
			FieldInfo fieldInfo => (T)fieldInfo.GetValue(target),
			_ => throw new ArgumentException($"MemberInfo was invalid. Should be of type PropertyInfo or FieldInfo."),
		};

		public static void SetValue<T>(this SerializedProperty property, T value)
		{
			switch (property.GetMember(out var target, out int index))
			{
				case PropertyInfo propertyInfo:
					if (index == -1)
					{
						propertyInfo.SetValue(target, value);
					}
					else
					{
						propertyInfo.SetValue(target, value, new object[] { index });
					}
					break;
				case FieldInfo fieldInfo:
					fieldInfo.SetValue(target, value);
					break;
				default:
					throw new ArgumentException($"MemberInfo was invalid. Should be of type PropertyInfo or FieldInfo.");
            }
		}

        public static string GetSanitizedPropertyPath(this SerializedProperty serializedProperty)
		{
			return serializedProperty.propertyPath.Replace(".Array.data[", "[");
		}

		public static Type? GetSerializedPropertyType(this SerializedProperty serializedProperty)
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
