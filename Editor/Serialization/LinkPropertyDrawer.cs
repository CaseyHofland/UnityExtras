#nullable enable
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(LinkPropertyAttribute))]
    public class LinkPropertyDrawer : PropertyDrawer
    {
        private const BindingFlags getPropertyFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;

        // Create Property GUI has a bug where the value won't snap correctly when changing it, and instead it will flicker. Until a fix is found, use the OnGUI implementation, which doesn't have this bug.
        //public override VisualElement CreatePropertyGUI(SerializedProperty property)
        //{
        //    var root = new PropertyField(property);
        //    root.RegisterValueChangeCallback(e =>
        //    {
        //        Update(e.changedProperty);
        //    });
        //    return root;
        //}

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var propertyScope = new EditorGUI.PropertyScope(position, label, property);

            using var changeCheckScope = new EditorGUI.ChangeCheckScope();
            EditorGUI.PropertyField(position, property);

            if (changeCheckScope.changed)
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            Update(property);
        }

        private void Update(SerializedProperty property)
        {
            var linkProperty = (LinkPropertyAttribute)attribute;

            var propertyMember = property.GetPropertyMember();
            var propertyInfo = propertyMember.memberInfo.DeclaringType.GetProperty(linkProperty.propertyName, getPropertyFlags);

            propertyInfo.SetValue(propertyMember.target, propertyMember.GetValue<object>());
            propertyMember.SetValue(propertyInfo.GetValue(propertyMember.target));
        }
    }
}
