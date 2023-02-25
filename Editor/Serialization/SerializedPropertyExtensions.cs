#nullable enable
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace UnityExtras.Editor
{
	public static class SerializedPropertyExtensions
	{
		const string backingFieldPrefix = "<";
		const string backingFieldSuffix = ">k__BackingField";

		public static SerializedProperty FindAutoPropertyRelative(this SerializedProperty serializedProperty, string relativePropertyPath) => serializedProperty.FindPropertyRelative($"{backingFieldPrefix}{relativePropertyPath}{backingFieldSuffix}");

		public static PropertyMember GetPropertyMember(this SerializedProperty property)
		{
            const BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var path = property.propertyPath.AsSpan();
			var target = property.serializedObject.targetObject;

			return GetPropertyMember(path, target, null);

			static PropertyMember GetPropertyMember(ReadOnlySpan<char> path, object target, PropertyMember? parent)
			{
                if (target is IList list
					&& path.StartsWith("Array.data["))
				{
                    //var digitStart = path.IndexOf('[') + 1;

					const int digitStart = 11; // Taken from "Array.data[".Length;
                    var digitEnd = digitStart + path[digitStart..].IndexOf(']');

                    var index = Convert.ToInt32(path[digitStart..digitEnd].ToString());
					parent = new PropertyMember(parent, target.GetType().GetProperty("Item"), target, index);
                    return digitEnd + 2 >= path.Length ? parent : GetPropertyMember(path[(digitEnd + 2)..], list[index], parent);
                }
                else
				{
                    var index = path.IndexOf('.');
                    var memberPath = index == -1
                        ? path
                        : path[0..index];

                    var targetType = target.GetType();
                    if (memberPath.StartsWith(backingFieldPrefix) && memberPath.EndsWith(backingFieldSuffix))
					{
						PropertyInfo propertyInfo;
						string propertyName = memberPath[1..memberPath.IndexOf(backingFieldSuffix)].ToString();
						while ((propertyInfo = targetType.GetProperty(propertyName, bindingAttr)) == null && (targetType = targetType.BaseType) != null) ;
                        parent = new PropertyMember(parent, propertyInfo!, target);
					}
					else
					{
						FieldInfo fieldInfo;
						string fieldName = memberPath.ToString();
						while ((fieldInfo = targetType.GetField(fieldName, bindingAttr)) == null && (targetType = targetType.BaseType) != null) ;
                        parent = new PropertyMember(parent, fieldInfo!, target);
                    }

                    return index == -1 ? parent : GetPropertyMember(path[(index + 1)..], parent.GetValue<object>(), parent);
                }
            }
        }
	}
}
