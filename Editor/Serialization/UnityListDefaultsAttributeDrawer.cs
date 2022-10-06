#nullable enable
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(UnityListDefaultsAttribute))]
    public class UnityListDefaultsAttributeDrawer : PropertyDrawer
    {
        private SerializedProperty GetChild(SerializedProperty property)
        {
            var childProperty = property.Copy();
            childProperty.NextVisible(true);
            return childProperty;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyInfo = property.GetProperty(out var target, out int index);
            if (target is not IList list)
            {
                base.OnGUI(position, property, label);
                return;
            }

            if (!property.isExpanded)
            {
                propertyInfo.SetValue(target, Activator.CreateInstance(propertyInfo.PropertyType), new object[] { index });
                property.isExpanded = true;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, GetChild(property), label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property.GetProperty(out var target, out _);
            if (target is not IList)
            {
                return base.GetPropertyHeight(property, label);
            }

            return EditorGUI.GetPropertyHeight(GetChild(property), label, true);
        }
    }
}
