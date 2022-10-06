#nullable enable
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(ValueRef<>), true)]
    public class ValueRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            using CSharpCodeProvider cSharpCodeProvider = new();
            string GetTypeOutput(Type type)
            {
                var typeOutput = cSharpCodeProvider.GetTypeOutput(new CodeTypeReference(type));
                return Regex.Replace(typeOutput, @"[^<]*?[?=\.]", string.Empty);
            }

            var typeArgument = fieldInfo.FieldType.GenericTypeArguments[0];
            if (fieldInfo.FieldType.GetInterface(nameof(IList)) != null)
            {
                typeArgument = typeArgument.GenericTypeArguments[0];
            }
            label.text += $" ({GetTypeOutput(typeArgument)})";
            var member = property.FindAutoPropertyRelative(nameof(ValueRef<object>.member));
            EditorGUI.PropertyField(position, member, label, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property.FindAutoPropertyRelative(nameof(ValueRef<object>.member)), label);
    }
}
