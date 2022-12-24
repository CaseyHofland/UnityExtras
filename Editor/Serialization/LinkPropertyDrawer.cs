#nullable enable
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(LinkPropertyAttribute))]
    public class LinkPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var propertyScope = new EditorGUI.PropertyScope(position, label, property);

            using var changeCheckScope = new EditorGUI.ChangeCheckScope();
            EditorGUI.PropertyField(position, property);

            if (changeCheckScope.changed)
            {
                property.serializedObject.ApplyModifiedProperties();

                const BindingFlags getPropertyFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;
                var linkProperty = (LinkPropertyAttribute)attribute;

                var propertyMember = property.GetPropertyMember();
                var propertyInfo = propertyMember.memberInfo.DeclaringType.GetProperty(linkProperty.propertyName, getPropertyFlags);

                propertyInfo.SetValue(propertyMember.target, propertyMember.GetValue<object>());
                propertyMember.SetValue(propertyInfo.GetValue(propertyMember.target));
            }
        }
    }
}
